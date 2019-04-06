namespace Ivy.Backend
{
    public enum Instruction : byte
    {
        NoOperation,
        
        Push64,
        Pop64,
        
        AddI64,
        DivI64,          
        MulI64,
        SubI64,

        CmpLessI64,
        CmpGreaterI64,
        
        ShiftLeft64,
        ShiftRight64,
        
        StoreI64,
        LoadI64,
        
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