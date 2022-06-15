# PrismSharp.Highlighting.HTML

A porting of [PrismJS](https://github.com/PrismJS/prism) to C# or .NET.

All supported language grammars are [here](https://github.com/tatwd/prism-sharp/tree/main/PrismSharp.Core/Languages).


## Usage

```csharp
using PrismSharp.Core;

var text = @"<p>Hello world!</p>";
var grammar = LanguageGrammars.Html; // or defined yourself
var tokens = Prism.Tokenize(text, grammar);
```