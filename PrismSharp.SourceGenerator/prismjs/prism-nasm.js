Prism.languages.nasm = {
    'address': {
        pattern: /^[0-9A-Fa-f]{4,16}\b/m,
        alias: 'number'
    },
    'hex': {
        pattern: /\b0x[0-9A-Fa-f]+\b/,
        alias: 'number'
    },

    'conventional-instructions': {
        pattern: /\b(aaa|aad|aam|aas|adc|add|and|arpl|bound|bsf|bsr|bswap|bt|btc|btr|bts|call|cbw|cdq|cdqe|clc|cld|cli|clts|cmc|cmp|cmps|cmpxchg|cmova|cmovae|cmovb|cmovbe|cmovc|cmove|cmovg|cmovge|cmovl|cmovle|cmovna|cmovnae|cmovnb|cmovnbe|cmovnc|cmovne|cmovng|cmovnge|cmovnl|cmovnle|cmovno|cmovnp|cmovns|cmovnz|cmovo|cmovp|cmovpe|cmovpo|cmovs|cmovz|cwd|cwde|daa|das|dec|div|enter|esc|hlt|idiv|imul|in|inc|ins|int|int3|into|invd|invlpg|iret|iretd|ja|jae|jb|jbe|jc|jcxz|je|jecxz|jg|jge|jl|jle|jmp|jna|jnae|jnb|jnbe|jnc|jne|jng|jnge|jnl|jnle|jno|jnp|jns|jnz|jo|jp|jpe|jpo|js|jz|lahf|lar|lds|lea|leave|les|lfs|lgdt|lidt|lgs|lldt|lmsw|lock|lods|loop|loope|loopz|loopnz|loopne|lsl|lss|ltr|mov|movabs|movs|movsxd|movsx|movzx|mul|near|neg|nop|not|or|out|outs|pop|popa|popad|popf|popfd|push|pusha|pushad|pushf|pushfd|rcl|rcr|rep|repe|repz|repne|repnz|ret|retf|rol|ror|sahf|sal|sar|sbb|scas|section|seta|setae|setb|setbe|setc|sete|setg|setge|setl|setle|setna|setnae|setnb|setnbe|setnc|setne|setng|setnge|setnl|setnle|setno|setnp|setns|setnz|seto|setp|setpe|setpo|sets|setz|sgdt|sidt|shl|shld|shr|shrd|sldt|smsw|stc|std|sti|stos|str|sub|test|verr|verw|wait|fwait|wbinvd|xchg|xlat|xlatb|xor|equ|endbr64|endbr32|syscall|sysret|ud2)\b/,
        alias: 'function'
    },
    // https://www.nasm.us/doc/nasmdoc3.html#section-3.2
    'pseudo-instructions': {
        pattern: /\b(db|dw|dd|dq|dt|do|dy|dz|resb|resw|resd|resq|rest|reso|resy|resz)\b/,
        alias: 'function'
    },
    'control-flow': {
        pattern: /\b(far|near|short)\b/,
        alias: 'keyword'
    },
    'sizes': {
        pattern: /\b(byte|word|dword|qword|tword|oword|yword|zword)\b/,
        alias: 'function'
    },
    'ptr': {
        pattern: /\bptr\b/,
        alias: 'keyword'
    },
    'registers': {
        pattern: /\b(ip|eip|rip|eax|ebx|ecx|edx|edi|esi|ebp|esp|ax|bx|cx|dx|di|si|bp|sp|ah|al|bh|bl|ch|cl|dh|dl|cs|ds|ss|es|fs|gs|cr[0-4]|dr[0-7]|tr[67]|st\d?|rax|rbx|rcx|rdx|rsp|rbp|rsi|rdi|r8|r9|r1[0-5]|r8[bwd]|r9[bwd]|r1[0-5][bwd]|[xyz]mm\d{1,2})\b/,
        alias: 'function'
    }
};
