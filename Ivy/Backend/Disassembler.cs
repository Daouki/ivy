using System;

namespace Ivy.Backend
{
    public class Disassembler
    {
        public static void Disassemble(byte[] byteCode)
        {
            var pointer = 0;
            while (pointer < byteCode.Length)
            {
                switch ((Instruction) byteCode[pointer])
                {
                    case Instruction.NoOperation:
                        Console.WriteLine("NOP");
                        pointer += 1;
                        break;

                    case Instruction.Push64:
                        Console.Write("PUSH\t");
                        Console.WriteLine(BitConverter.ToInt64(GetSubarray(byteCode, pointer + 1, 8)));
                        pointer += 9;
                        break;
                    
                    case Instruction.Pop64:
                        Console.Write("POP");
                        Console.WriteLine(BitConverter.ToInt64(GetSubarray(byteCode, pointer + 1, 8)));
                        pointer += 9;
                        break;
                    
                    case Instruction.AddI:
                        Console.WriteLine("ADDI");
                        pointer += 1;
                        break;
                    
                    case Instruction.DivI:
                        Console.WriteLine("DIVI");
                        pointer += 1;
                        break;
                    
                    case Instruction.MulI:
                        Console.WriteLine("MUL");
                        pointer += 1;
                        break;
                    
                    case Instruction.SubI:
                        Console.WriteLine("SUB");
                        pointer += 1;
                        break;

                    case Instruction.Or:
                        Console.WriteLine("OR");
                        pointer += 1;
                        break;
                    
                    case Instruction.And:
                        Console.WriteLine("AND");
                        pointer += 1;
                        break;
                    
                    case Instruction.Xor:
                        Console.WriteLine("XOR");
                        pointer += 1;
                        break;
                    
                    case Instruction.Shl:
                        Console.WriteLine("SHL");
                        pointer += 1;
                        break;
                    
                    case Instruction.Shr:
                        Console.WriteLine("SHR");
                        pointer += 1;
                        break;
                    
                    case Instruction.CmpLessI:
                        Console.WriteLine("CMPL");
                        pointer += 1;
                        break;
                    
                    case Instruction.CmpGreaterI:
                        Console.WriteLine("CMPG");
                        pointer += 1;
                        break;
                    
                    case Instruction.Store:
                        Console.Write("STORE\t");
                        Console.WriteLine(BitConverter.ToInt64(GetSubarray(byteCode, pointer + 1, 8)));
                        pointer += 9;
                        break;
                    
                    case Instruction.Load:
                        Console.Write("LOAD\t");
                        Console.WriteLine(BitConverter.ToInt64(GetSubarray(byteCode, pointer + 1, 8)));
                        pointer += 9;
                        break;
                    
                    case Instruction.JmpShort:
                        Console.Write("JMPS\t");
                        Console.WriteLine(BitConverter.ToInt64(GetSubarray(byteCode, pointer + 1, 8)));
                        pointer += 9;
                        break;
                    
                    case Instruction.JmpFar:
                        Console.Write("JMPF\t");
                        Console.WriteLine(BitConverter.ToInt64(GetSubarray(byteCode, pointer + 1, 8)));
                        pointer += 9;
                        break;
                    
                    case Instruction.JmpIfZero:
                        Console.Write("JMPIFZ\t");
                        Console.WriteLine(BitConverter.ToInt64(GetSubarray(byteCode, pointer + 1, 8)));
                        pointer += 9;
                        break;
                    
                    case Instruction.JmpIfNotZero:
                        Console.Write("JMPIFNZ\t");
                        Console.WriteLine(BitConverter.ToInt64(GetSubarray(byteCode, pointer + 1, 8)));
                        pointer += 9;
                        break;
                    
                    case Instruction.PrintI64:
                        Console.WriteLine("PRINT");
                        pointer += 1;
                        break;
                    
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private static T[] GetSubarray<T>(T[] array, int index, int count)
        {
            var subarray = new T[count];
            Array.Copy(array, index, subarray, 0, count);
            return subarray;
        }
    }
}