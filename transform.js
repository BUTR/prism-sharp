// [WIP] Using the script to transform supported languages in prismjs to C# code.

const prism = require("./prismjs/prism");

const { languages, util } = prism;

const csharpCode = transform("markup");
console.log(csharpCode);

function transform(lang) {
  const className =
    lang
      .split("-")
      .map((s) => s[0].toUpperCase() + (s.length > 1 ? s.slice(1) : ""))
      .join("") + "GrammarDefinition";

  var outString = `
using System.Text.RegularExpressions;

namespace PrismSharp.Core.Languages;  

// From https://github.com/PrismJS/prism/blob/master/components/prism-${lang}.js

public class ${className} : IGrammarDefinition 
{
    public Grammar Define()
    {
      var grammar = new Grammar();
`;

  var currentLang = languages[lang];
  var visited = {};
  var id = util.objId(currentLang);
  visited[id] = `grammar`;

  for (const key in currentLang) {
    var val = currentLang[key];

    if (util.type(val) === "RegExp") {
      outString += `
      grammar["${key}"] = new GrammarToken[]
      {
        ${toGrammarToken(
          val,
          false,
          false,
          [],
          null,
          visited,
          `grammar["${key}"][0]`
        )}
      };`;
    } else if (util.type(val) == "Object") {
      id = util.objId(val);
      if (visited[id]) {
        outString += `grammar["${key}"] = ${visited[id]};`;
        continue;
      }
      visited[id] = `grammar["${key}"]`;
      outString += `
      grammar["${key}"] = new GrammarToken[]
      {
        ${toGrammarToken(
          val.pattern,
          val.lookbehind,
          val.greedy,
          val.alias,
          val.inside,
          visited,
          `grammar["${key}"][0]`
        )}
      };`;
    } else if (util.type(val) === "Array") {
      // TODO
    }
  }

  outString += `

      return grammar;
    }
}`;

  // console.log(visited);

  return outString;
}

/**
 * transform a regexp to a grammar token
 * @param {RegExp} regex
 */
function toGrammarToken(
  regex,
  lookbehind,
  greedy,
  alias,
  inside,
  visited,
  objVisitCode
) {
  visited = visited || {};

  var pattern = regex.source;
  var alias =
    util.type(alias) === "String"
      ? [alias]
      : util.type(alias) === "Array"
      ? alias
      : [];
  var lookbehind = !!lookbehind;
  var greedy = !!greedy;

  var flags = regex.flags;
  var csharpRegexOptions = "RegexOptions.Compiled";

  if (flags.includes("i")) {
    csharpRegexOptions += " | RegexOptions.IgnoreCase";
  }
  if (flags.includes("m")) {
    pattern = pattern.replace(/\$/g, "\\r?$");
    csharpRegexOptions += " | RegexOptions.MultiLine";
  }

  var code = `new(new Regex(@"${pattern.replace(
    /"/g,
    '""'
  )}", ${csharpRegexOptions}), lookbehind: ${lookbehind}, greedy: ${greedy}`;

  if (alias.length) {
    code += `, alias: new []{"${alias.join(" , ")}" }`;
  }

  if (inside) {
    var id = util.objId(inside);
    if (visited[id]) {
      code += `, inside: ${visited[id]}`;
    } else {
      visited[id] = `${objVisitCode}.Inside`;
      code += `, inside: ${transformInside(inside, visited, visited[id])}`;
    }
  }

  code += `)`;
  return code;
}

function transformInside(inside, visited, objVisitCode) {
  var code = `new Grammar{
    `;

  for (const key in inside) {
    var val = inside[key];

    if (util.type(val) === "RegExp") {
      code += `
        ["${key}"] = new GrammarToken[]
        {
          ${toGrammarToken(
            val,
            false,
            false,
            [],
            null,
            visited,
            `${objVisitCode}["${key}"][0]`
          )}
        },`;
    } else if (util.type(val) == "Object") {
      id = util.objId(val);
      if (visited[id]) {
        if (key === "rest") {
          code += `
          Reset = ${visited[id]},`;
        } else {
          code += `
          ["${key}"] = ${visited[id]},`;
        }
        continue;
      }

      visited[id] = `${objVisitCode}["${key}"]`;
      code += `
        ["${key}"] = new GrammarToken[]
        {
          ${toGrammarToken(
            val.pattern,
            val.lookbehind,
            val.greedy,
            val.alias,
            val.inside,
            visited,
            `${objVisitCode}["${key}"][0]`
          )}
        },`;
    } else if (util.type(val) === "Array") {
      // TODO
    }
  }

  code += `
  }`;

  return code;
}
