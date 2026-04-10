// PrismJS grammar for Capstone disassembly output (x86_64 + AArch64)
// Format: "XXXX mnemonic  operands"
//
// Token order matters: Prism matches top-to-bottom, first match wins.
// The 'instruction' token is intentionally last as a catch-all for any
// unmatched word — in this format, that's always a mnemonic.
Prism.languages.nasm = {
    // Address at line start: "000A ", "0032 ".
    // PrismSharp's JS→C# pipeline strips regex flags (toJSON returns .source only),
    // so /m doesn't work. Instead, use lookbehind with (^|\n) to anchor to line start.
    'address': {
        pattern: /(^|\n)[0-9A-Fa-f]{4,16}\b/,
        lookbehind: true,
        alias: 'number'
    },
    'hex': {
        pattern: /\b0x[0-9A-Fa-f]+\b/,
        alias: 'number'
    },
    'sizes': {
        pattern: /\b(byte|word|dword|qword|tword|oword|yword|zword|xmmword|ymmword|zmmword)\b/,
        alias: 'keyword'
    },
    'ptr': {
        pattern: /\bptr\b/,
        alias: 'keyword'
    },
    'control-flow': {
        pattern: /\b(far|near|short)\b/,
        alias: 'keyword'
    },
    'registers': {
        pattern: /\b(ip|eip|rip|eax|ebx|ecx|edx|edi|esi|ebp|esp|ax|bx|cx|dx|di|si|bp|sp|ah|al|bh|bl|ch|cl|dh|dl|cs|ds|ss|es|fs|gs|cr[0-4]|dr[0-7]|tr[67]|st\d?|rax|rbx|rcx|rdx|rsp|rbp|rsi|rdi|r8|r9|r1[0-5]|r8[bwd]|r9[bwd]|r1[0-5][bwd]|[xyz]mm\d{1,2}|[bdhswxq]\d{1,2}|v\d{1,2}|wzr|xzr|wsp|p\d{1,2}|z\d{1,2})\b/,
        alias: 'function'
    },
    // ARM64 immediates: #0x10, #-3, #42
    'immediate': {
        pattern: /#-?(?:0x[0-9A-Fa-f]+|\d+)\b/,
        alias: 'number'
    },
    // Bare decimal immediates: 0 in "cmp ..., 0"
    'number': {
        pattern: /\b\d+\b/,
        alias: 'number'
    },
    'operator': /[\[\]*+\-,]/,
    // Catch-all: any remaining word is an instruction mnemonic.
    // Must be last so addresses, hex, registers, sizes, ptr match first.
    'instruction': {
        pattern: /\b[a-z]\w*\b/,
        alias: 'function'
    }
};
