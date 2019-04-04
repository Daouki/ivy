using System;
using System.Collections.Generic;
using Ivy.Backend;

namespace Ivy.Runtime
{
    public class VirtualMachine
    {
        private readonly List<byte> _byteCode;
        
        private readonly VMStack _stack = new VMStack(Kibibytes(2));
        private readonly List<ulong> _locals = new List<ulong>(512);
        private int _instructionPointer = 0;
        
        public VirtualMachine(List<byte> byteCode)
        {
            _byteCode = byteCode;
        }

        public void Execute()
        {
            var codeLength = _byteCode.Count;
            while (_instructionPointer < codeLength)
            {
                var instruction = (Instruction) _byteCode[_instructionPointer++];
                switch (instruction)
                {
                    case Instruction.NoOperation:
                        break;

                    case Instruction.Push64:
                    {
                        var value = GetUInt64FromByteCode();
                        _stack.PushQWord(value);
                        break;
                    }

                    case Instruction.Pop64:
                        _stack.PopQWord();
                        break;

                    case Instruction.AddI64:
                    {
                        var left = _stack.PopQWord();
                        var right = _stack.PopQWord();
                        _stack.PushQWord(left + right);
                        break;
                    }

                    case Instruction.DivI64:
                    {
                        var left = _stack.PopQWord();
                        var right = _stack.PopQWord();
                        _stack.PushQWord(left / right);
                        break;
                    }
                    
                    case Instruction.MulI64:
                    {
                        var left = _stack.PopQWord();
                        var right = _stack.PopQWord();
                        _stack.PushQWord(left * right);
                        break;
                    }
                    
                    case Instruction.SubI64:
                    {
                        var left = _stack.PopQWord();
                        var right = _stack.PopQWord();
                        _stack.PushQWord(left - right);
                        break;
                    }
                    
                    case Instruction.CmpLessI64:
                    {
                        var left = _stack.PopQWord();
                        var right = _stack.PopQWord();
                        _stack.PushQWord(left < right ? 1ul : 0);
                        break;
                    }
                    
                    case Instruction.CmpGreaterI64:
                    {
                        var left = _stack.PopQWord();
                        var right = _stack.PopQWord();
                        _stack.PushQWord(left > right ? 1ul : 0);
                        break;
                    }
                    
                    case Instruction.StoreI64:
                    {
                        var location = GetUInt64FromByteCode();
                        var value = _stack.PopQWord();
                        _locals.Add(value);
                        break;
                    }

                    case Instruction.LoadI64:
                    {
                        var location = GetUInt64FromByteCode();
                        _stack.PushQWord(_locals[(int) location]);
                        break;
                    }

                    case Instruction.JmpFar:
                    {
                        var offset = (int) GetUInt64FromByteCode();
                        _instructionPointer = offset;
                        break;
                    }

                    case Instruction.JmpShort:
                    {
                        // If that variable isn't defined here, the code won't work.
                        // I assume it's something related to wild C#'s optimizations.
                        var offset = (int) GetInt64FromByteCode();
                        _instructionPointer += offset;
                        break;
                    }

                    case Instruction.JmpIfZero:
                    {
                        var value = (int) _stack.PopQWord();
                        if (value == 0)
                        {
                            var offset = GetInt64FromByteCode();
                            _instructionPointer += (int) offset;
                        }
                        else
                        {
                            _instructionPointer += 8;
                        }
                        break;
                    }

                    case Instruction.JmpIfNotZero:
                    {
                        var value = (int) _stack.PopQWord();
                        if (value != 0)
                        {
                            var offset = (int) GetInt64FromByteCode();
                            _instructionPointer += offset;
                        }
                        else
                        {
                            _instructionPointer += 8;
                        }
                        break;
                    }

                    case Instruction.PrintI64:
                    {
                        var value = (long) _stack.PopQWord();
                        Console.WriteLine(value);
                        break;
                    }
                    
                    default:
                        throw new Exception("Invalid instruction");
                }
            }
        }

        private long GetInt64FromByteCode()
        {
            var bytes = _byteCode.GetRange(_instructionPointer, 8).ToArray();
            var value = BitConverter.ToInt64(bytes);
            _instructionPointer += 8;
            return value;
        }

        private ulong GetUInt64FromByteCode()
        {
            var bytes = _byteCode.GetRange(_instructionPointer, 8).ToArray();
            var value = BitConverter.ToUInt64(bytes);
            _instructionPointer += 8;
            return value;
        }

        private static int Kibibytes(int kibibytes) => kibibytes * 1024;
        private static int Mebibytes(int mebibytes) => mebibytes * 1024 * 1024;
        private static int Gibibytes(int gibibytes) => gibibytes * 1024 * 1024 * 1024;
    }
}