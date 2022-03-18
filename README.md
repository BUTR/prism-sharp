# PrismSharp

A porting of [PrismJS](https://github.com/PrismJS/prism) to C# or .NET.

All supported language grammars are [here](https://github.com/tatwd/prism-sharp/tree/main/PrismSharp.Core/Languages).

## Getting Started

### Tokenize Code

Install `PrismSharp.Core` package:
```sh
dotnet add package PrismSharp.Core --prerelease
```

Then,
```csharp
using PrismSharp.Core;

var text = @"<p>Hello world!</p>";
var grammar = LanguageGrammars.Html; // or defined yourself
var tokens = Prism.Tokenize(text, grammar);
```

### Highlight Code

Install `PrismSharp.Highlighting.HTML` package:
```sh
dotnet add package PrismSharp.Highlighting.HTML --prerelease
```

Then,
```csharp
using PrismSharp.Core;
using PrismSharp.Highlighting.HTML;

var text = @"<p>Hello world!</p>";
var grammar = LanguageGrammars.Html; // or defined yourself
var highlighter = new HtmlHighlighter();
var html = highlighter.Highlight(text, grammar, "html");
```

_The css styles can customize yourself, or download from [PrismJS](https://prismjs.com/download.html)._
