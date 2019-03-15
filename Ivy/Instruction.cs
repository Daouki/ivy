namespace Ivy
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
    }
}