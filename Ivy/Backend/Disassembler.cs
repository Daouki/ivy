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
                        Console.Write("PUSH64\t");
                        Console.WriteLine(BitConverter.ToInt64(GetSubarray(byteCode, pointer + 1, 8)));
                        pointer += 9;
                        break;
                    
                    case Instruction.Pop64:
                        Console.Write("POP64");
                        Console.WriteLine(BitConverter.ToInt64(GetSubarray(byteCode, pointer + 1, 8)));
                        pointer += 9;
                        break;
                    
                    case Instruction.AddI64:
                        Console.WriteLine("ADD64");
                        pointer += 1;
                        break;
                    
                    case Instruction.DivI64:
                        Console.WriteLine("DIV64");
                        pointer += 1;
                        break;
                    
                    case Instruction.MulI64:
                        Console.WriteLine("MUL64");
                        pointer += 1;
                        break;
                    
                    case Instruction.SubI64:
                        Console.WriteLine("SUB64");
                        pointer += 1;
                        break;
                    
                    case Instruction.CmpLessI64:
                        Console.WriteLine("CMPL64");
                        pointer += 1;
                        break;
                    
                    case Instruction.CmpGreaterI64:
                        Console.WriteLine("CMPG64");
                        pointer += 1;
                        break;
                    
                    case Instruction.StoreI64:
                        Console.Write("STORE64\t");
                        Console.WriteLine(BitConverter.ToInt64(GetSubarray(byteCode, pointer + 1, 8)));
                        pointer += 9;
                        break;
                    
                    case Instruction.LoadI64:
                        Console.Write("LOAD64\t");
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