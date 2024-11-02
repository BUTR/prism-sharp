Prism.languages.nasm = {
    'address': {
        pattern: /\b[0-9A-F]{4,16}\b/,
        alias: 'number'
    },
    'hex': {
        pattern: /\b0x[0-9A-Fa-f]+h\b/,
        alias: 'number'
    },

    'conventional-instructions': {
        pattern: /\b(aaa|aad|aam|aas|adc|add|and|arpl|bound|bsf|bsr|bswap|bt|btc|btr|bts|call|cbw|cdq|clc|cld|cli|clts|cmc|cmp|cmps|cmpxchg|cwd|cwde|daa|das|dec|div|enter|esc|hlt|idiv|imul|in|inc|ins|int|into|invd|invlpg|iret|iretd|ja|jae|jb|jbe|jc|jcxz|je|jecxz|jg|jge|jl|jle|jmp|jna|jnae|jnb|jnbe|jnc|jne|jng|jnge|jnl|jnle|jno|jnp|jns|jnz|jo|jp|jpe|jpo|js|jz|lahf|lar|lds|lea|leave|les|lfs|lgdt|lidt|lgs|lldt|lmsw|lock|lods|loop|loope|loopz|loopnz|loopne|lsl|lss|ltr|mov|movs|movsx|movzx|mul|near|neg|nop|not|or|out|outs|pop|popa|popad|popf|popfd|push|pusha|pushad|pushf|pushfd|rcl|rcr|rep|repe|repz|repne|repnz|ret|retf|rol|ror|sahf|sal|sar|sbb|scas|section|setae|setnb|setb|setnae|setbe|setna|sete|setz|setne|setnz|setl|setnge|setge|setng|setle|setng|setg|setnle|sets|setns|setc|setnc|seto|setno|setp|setpe|setnp|setpo|sgdt|sidt|shl|shld|shr|shrd|sldt|smsw|stc|std|sti|stos|str|sub|test|verr|verw|wait|fwait|wait|fwait|wbinvd|xchg|xlat|xlatb|xor|equ)\b/,
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
    'registers': {
        pattern: /\b(ip|eip|eax|ebx|ecx|edx|edi|esi|ebp|esp|ax|bx|cx|dx|di|si|bp|sp|ah|al|bh|bl|ch|cl|dh|dl|ax|bx|cx|dx|cs|ds|ss|es|fs|gs|cr0|cr2|cr3|db0|db1|db2|db3|db6|db7|tr6|tr7|st|rax|rcx|rdx|rbs|rsp|rbp|rsi|rdi)\b/,
        alias: 'function'
    }
};