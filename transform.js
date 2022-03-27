// [WIP] Using the script to transform supported languages in prismjs to C# code.

const prism = require("./prismjs/prism");
const loadLanguages = require("./prismjs/components/");
const { writeFileSync, existsSync, rmSync, mkdirSync } = require("fs");

// loadLanguages(); // load all

const { languages, util } = prism;

const csharp = transform("javascript");
// console.log(csharp.className);
// console.log(csharp.code);

const outputDir = "codegen";

if (!existsSync(outputDir)) mkdirSync(outputDir);

const path = `${outputDir}/${csharp.className}.cs`;
if (existsSync(path)) rmSync(path);
writeFileSync(path, csharp.code);

function transform(lang) {
  const className =
    lang
      .split("-")
      .map((s) => s[0].toUpperCase() + (s.length > 1 ? s.slice(1) : ""))
      .join("") + "GrammarDefinition";

  var outString = `using System.Text.RegularExpressions;

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
  // console.log(currentLang);
  var id = util.objId(currentLang);
  visited[id] = `grammar`;

  var laterAssigned = [];

  for (const key in currentLang) {
    var val = currentLang[key];
    var valType = util.type(val);
    var curVisitPath = `grammar["${key}"]`;

    if (valType === "RegExp" || valType === "Object") {
      outString += `
      ${curVisitPath} = new GrammarToken[]
      {
          ${innerTransform([val], curVisitPath, visited, laterAssigned)}
      };`;
    } else if (valType === "Array") {
      outString += `
      ${curVisitPath} = new GrammarToken[]
      {
          ${innerTransform(val, curVisitPath, visited, laterAssigned)}
      };`;
    } else {
      throw new Error(`not suported type for '${util.type(val)}'`);
    }
  }

  for (const expr of laterAssigned) {
    outString += `
      ${expr};`;
  }

  outString += `

      return grammar;
    }
}`;

  // console.log(visited);

  return { className, code: outString };
}

function innerTransform(items, preVisitPath, visited, laterAssigned) {
  var code = "";

  for (var i = 0, len = items.length; i < len; ++i) {
    var val = items[i];
    var valType = util.type(val);
    var curVisitPath = `${preVisitPath}[${i}]`;

    if (valType === "RegExp") {
      code += `
        ${toGrammarToken(
          val,
          false,
          false,
          [],
          null,
          visited,
          curVisitPath,
          laterAssigned
        )},`;
    } else if (valType === "Object") {
      var id = util.objId(val);
      if (visited[id]) {
        code += `
        null /* see below */,`;
        laterAssigned.push(`${curVisitPath} =  ${visited[id]}`);
        continue;
      }
      visited[id] = curVisitPath;

      code += `
        ${toGrammarToken(
          val.pattern,
          val.lookbehind,
          val.greedy,
          val.alias,
          val.inside,
          visited,
          visited[id],
          laterAssigned
        )},`;
    } else {
      throw new Error(`not suported type for '${valType}'`);
    }
  }

  return code;
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
  objVisitCode,
  laterAssigned
) {
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
    // TODO: fix
    // pattern = pattern.replace(/\$/g, "\\r?$");
    csharpRegexOptions += " | RegexOptions.Multiline";
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
      code += `, inside: null /* see below */`;
      laterAssigned.push(`${objVisitCode}.Inside = ${visited[id]}`);
    } else {
      visited[id] = `${objVisitCode}.Inside!`;
      code += `, inside: ${transformInside(
        inside,
        visited,
        visited[id],
        laterAssigned
      )}`;
    }
  }

  code += `)`;
  return code;
}

function transformInside(inside, visited, preVisitPath, laterAssigned) {
  var code = `new Grammar
  {`;

  for (const key in inside) {
    var val = inside[key];
    var valType = util.type(val);

    if (valType === "RegExp") {
      code += `
      ["${key}"] = new GrammarToken[]
      {
          ${innerTransform(
            [val],
            `${preVisitPath}["${key}"]`,
            visited,
            laterAssigned
          )}
      },`;
      continue;
    }

    var id = util.objId(val);
    if (key === "rest") {
      if (visited[id]) {
        code += `
      Reset = ${visited[id]},`;
        continue;
      } else {
        code += `
      Reset = ${transformInside(
        val,
        visited,
        `${preVisitPath}.Reset!`,
        laterAssigned
      )},`;
        continue;
      }
    }

    if (valType === "Object") {
      if (visited[id]) {
        code += `
      ["${key}"] = new GrammarToken[]
      {
        null /* see below */
      },`;
        laterAssigned.push(`${preVisitPath}["${key}"][0] = ${visited[id]}`);
        continue;
      }
      code += `
      ["${key}"] = new GrammarToken[]
      {
          ${innerTransform(
            [val],
            `${preVisitPath}["${key}"]`,
            visited,
            laterAssigned
          )}
      },`;
    } else if (valType === "Array") {
      if (visited[id]) {
        code += `
      ["${key}"] = null /* see below */,`;
        laterAssigned.push(`${preVisitPath}["${key}"] = ${visited[id]}`);
        continue;
      }
      code += `
      ["${key}"] = new GrammarToken[]
      {
          ${innerTransform(
            val,
            `${preVisitPath}["${key}"]`,
            visited,
            laterAssigned
          )}
      },`;
    } else {
      throw new Error(`not suported type for '${util.type(val)}'`);
    }
  }

  code += `
  }`;

  return code;
}
