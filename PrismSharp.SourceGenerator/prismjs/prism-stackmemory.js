// PrismJS grammar for stack memory dumps in crash logs
// Formats:
//   "[RSP+0  ] 0x2572BC21280      (void*)"
//   "[RSP+90 ] 0x2560084C600      (char*) "BGS_Logo.bik""
//   "[RSP+B0 ] 0x25600A27680      (BGSMoviePlayer*)"
//   "[SP+20]   0x26A1E7D1900      (Character*) -> (FormId: FE38480E, ...)"
//   " 00 | 0x0057172B |"
//   " 0B | 0x01016840 | 0x0102A62C ==> Class: TESObjectARMO"
Prism.languages.stackmemory = {
    'stack-offset': {
        pattern: /\[(?:RSP|ESP|SP)\+[0-9A-Fa-f]+\s*\]/,
        alias: 'keyword'
    },
    'stack-index': {
        pattern: /^\s*[0-9A-Fa-f]{1,3}\s*(?=\|)/m,
        alias: 'keyword'
    },
    'hex-value': {
        pattern: /\b0x[0-9A-Fa-f]+\b/,
        alias: 'number'
    },
    'module-offset': {
        pattern: /\w+\.(?:exe|dll)\+[0-9A-Fa-f]+/,
        alias: 'function'
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
    'plugin-info': {
        pattern: /Plugin:\s*"[^"]+"/,
        alias: 'string'
    },
    'string-value': {
        pattern: /"[^"]*"/,
        alias: 'string'
    },
    'instruction': {
        pattern: /\b(?:mov|add|sub|cmp|test|jmp|je|jne|jz|jnz|jg|jge|jl|jle|ja|jae|jb|jbe|call|ret|push|pop|lea|xor|and|or|nop|int|neg|shl|shr|sar|sal|not|mul|div|inc|dec|movzx|movsx|cdq|rep|cmovne|cmove)\b/,
        alias: 'builtin'
    },
    'asm-register': {
        pattern: /\b(?:r[abcd]x|r[sd]i|r[bs]p|rip|r[89]|r1[0-5]|e[abcd]x|e[sd]i|e[bs]p|eip|[abcd][hlx]|[sd]i|[bs]p)\b/i,
        alias: 'variable'
    },
    'asm-operand': {
        pattern: /\b(?:byte|word|dword|qword)\s+ptr\b/,
        alias: 'keyword'
    },
    'arrow': {
        pattern: /->|==>/,
        alias: 'operator'
    },
    'separator': {
        pattern: /\|/,
        alias: 'punctuation'
    },
    'integer-value': {
        pattern: /\b\d+\b/,
        alias: 'number'
    }
};
