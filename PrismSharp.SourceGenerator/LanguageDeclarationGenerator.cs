using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jint;

namespace PrismSharp.SourceGenerator;

public static class LanguageDeclarationGenerator
{
    public static IEnumerable<KeyValuePair<string, string>> GetFlattedLanguageDefinitions()
    {
        var jsFileContents = new List<KeyValuePair<string, string>>();
        var jsFiles = typeof(LanguageDeclarationGenerator).Assembly.GetManifestResourceNames().Where(x => x.StartsWith("prismjs\\") && x.EndsWith(".js")).ToArray();
        foreach (var jsFile in jsFiles)
        {
            using var stream = typeof(LanguageDeclarationGenerator).Assembly.GetManifestResourceStream(jsFile)!;
            using var reader = new StreamReader(stream);

            var file = Path.GetFileNameWithoutExtension(jsFile);
            jsFileContents.Add(new(file, reader.ReadToEnd()));
        }
        jsFileContents.Sort((x, y) => x.Key.EndsWith("prism.js") ? -1 : y.Key.EndsWith("prism.js") ? 1 : 0);

        var engine = new Engine();
        engine.SetValue("self", engine.Evaluate("return {}"));

        const string flatted = """
                               self.Flatted=function(n){"use strict";function t(n){return t="function"==typeof Symbol&&"symbol"==typeof Symbol.iterator?function(n){return typeof n}:function(n){return n&&"function"==typeof Symbol&&n.constructor===Symbol&&n!==Symbol.prototype?"symbol":typeof n},t(n)}var r=JSON.parse,e=JSON.stringify,o=Object.keys,u=String,f="string",i={},c="object",a=function(n,t){return t},l=function(n){return n instanceof u?u(n):n},s=function(n,r){return t(r)===f?new u(r):r},y=function n(r,e,f,a){for(var l=[],s=o(f),y=s.length,p=0;p<y;p++){var v=s[p],S=f[v];if(S instanceof u){var b=r[S];t(b)!==c||e.has(b)?f[v]=a.call(f,v,b):(e.add(b),f[v]=i,l.push({k:v,a:[r,e,b,a]}))}else f[v]!==i&&(f[v]=a.call(f,v,S))}for(var m=l.length,g=0;g<m;g++){var h=l[g],O=h.k,d=h.a;f[O]=a.call(f,O,n.apply(null,d))}return f},p=function(n,t,r){var e=u(t.push(r)-1);return n.set(r,e),e},v=function(n,e){var o=r(n,s).map(l),u=o[0],f=e||a,i=t(u)===c&&u?y(o,new Set,u,f):u;return f.call({"":i},"",i)},S=function(n,r,o){for(var u=r&&t(r)===c?function(n,t){return""===n||-1<r.indexOf(n)?t:void 0}:r||a,i=new Map,l=[],s=[],y=+p(i,l,u.call({"":n},"",n)),v=!y;y<l.length;)v=!0,s[y]=e(l[y++],S,o);return"["+s.join(",")+"]";function S(n,r){if(v)return v=!v,r;var e=u.call(this,n,r);switch(t(e)){case c:if(null===e)return e;case f:return i.get(e)||p(i,l,e)}return e}};return n.fromJSON=function(n){return v(e(n))},n.parse=v,n.stringify=S,n.toJSON=function(n){return r(S(n))},n}({});
                               """;
        var js = $$"""
                   {{flatted}}

                   {{string.Join("\n", jsFileContents.Select(x => x.Value))}}

                   RegExp.prototype.toJSON = function() { return this.source; }

                   function getLanguageJSON(language) { return self.Flatted.stringify(Prism.languages[language]); }
                   """;

        foreach (var language in jsFiles.Select(Path.GetFileNameWithoutExtension).Where(x => x!.StartsWith("prism-")).Select(x => x!.Replace("prism-", "")))
            yield return new(language, engine.Evaluate(js + $"getLanguageJSON('{language}')").AsString());
    }
}
