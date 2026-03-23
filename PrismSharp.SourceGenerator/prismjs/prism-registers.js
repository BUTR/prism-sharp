// PrismJS grammar for CPU register dumps in crash logs
// Formats:
//   "RAX    0x00007FF7C9A070D6"
//   "RAX 0x0                (size_t)"
//   "eax | 0x12345678 | 0x12345678 ==> RTTI: NiCamera"
//   "AX:       0xAAC8BFF608       (void*)"
Prism.languages.registers = {
    'register': {
        pattern: /\b(?:R?[ABCD]X|R?[SD]I|R?[BS]P|R?IP|RFLAGS|EFLAGS|Flags|R8|R9|R1[0-5]|E?[ABCD]X|E?[SD]I|E?[BS]P|E?IP|[ABCD][HL]|[ABCD]S|[DEFGS]S|CR[0-4]|DR[0-7]|XMM\d{1,2}|YMM\d{1,2})\b/i,
        alias: 'keyword'
    },
    'hex-value': {
        pattern: /\b0x[0-9A-Fa-f]+\b/,
        alias: 'number'
    },
    'type-annotation': {
        pattern: /\((?:void\*|NULL|size_t|char\*|[iu](?:8|16|32|64)|double|float|\w+\*{1,2})\)/,
        alias: 'class-name'
    },
    'game-type': {
        pattern: /\((?:[A-Z]\w+(?:::\w+)*\*?)\)/,
        alias: 'class-name'
    },
    'rtti-label': {
        pattern: /(?:RTTI|Class):\s*\w[\w:]*/,
        alias: 'function'
    },
    'form-info': {
        pattern: /FormId:\s*[0-9A-Fa-f]+/,
        alias: 'string'
    },
    'name-info': {
        pattern: /Name:\s*[`"][^`"]+[`"]/,
        alias: 'string'
    },
    'file-info': {
        pattern: /File:\s*[`"][^`"]+[`"]/,
        alias: 'string'
    },
    'string-value': {
        pattern: /"[^"]*"/,
        alias: 'string'
    },
    'arrow': {
        pattern: /->|==>/,
        alias: 'operator'
    },
    'separator': {
        pattern: /[|:]/,
        alias: 'punctuation'
    },
    'float-value': {
        pattern: /\b\d+\.\d+(?:E[+-]?\d+)?\b/i,
        alias: 'number'
    },
    'integer-value': {
        pattern: /\b\d+\b/,
        alias: 'number'
    }
};
