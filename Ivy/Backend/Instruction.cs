namespace Ivy.Backend
{
    public enum Instruction : byte
    {
        NoOperation,
        
        Push64,
        Pop64,
        
        AddI,
        DivI,          
        MulI,
        SubI,
        
        Or,
        And,
        Xor,
        Shl,
        Shr,

        CmpLessI,
        CmpGreaterI,
        
        Store,
        Load,
        
        // Unconditional jumps.
        JmpShort,
        JmpFar,
        
        // Conditional jumps. Always relative.
        JmpIfZero,
        JmpIfFalse = JmpIfZero,
        JmpIfNotZero,
        JmpIfTrue = JmpIfNotZero,
        
        PrintI64
    }
}