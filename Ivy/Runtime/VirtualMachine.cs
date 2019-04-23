using System;
using System.Collections.Generic;
using Ivy.Backend;

namespace Ivy.Runtime
{
    public class VirtualMachine
    {
        private readonly List<byte> _byteCode;
        
        private readonly VMStack _stack = new VMStack(Kibibytes(2));
        private int _instructionPointer = 0;

        public static void Execute(List<byte> byteCode)
        {
            try
            {
                new VirtualMachine(byteCode).Execute();
            }
            catch (InvalidOperationException e)
            {
                Console.Error.WriteLine($"Runtime error: {e.Message}");
            }
        }
        
        private VirtualMachine(List<byte> byteCode)
        {
            _byteCode = byteCode;
        }

        private void Execute()
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
                        _stack.Push(value);
                        break;
                    }

                    case Instruction.Pop64:
                        _stack.Pop();
                        break;

                    case Instruction.AddI:
                    {
                        var left = _stack.Pop();
                        var right = _stack.Pop();
                        _stack.Push(left + right);
                        break;
                    }

                    case Instruction.DivI:
                    {
                        var left = _stack.Pop();
                        var right = _stack.Pop();
                        _stack.Push(left / right);
                        break;
                    }

                    case Instruction.MulI:
                    {
                        var left = _stack.Pop();
                        var right = _stack.Pop();
                        _stack.Push(left * right);
                        break;
                    }

                    case Instruction.SubI:
                    {
                        var left = _stack.Pop();
                        var right = _stack.Pop();
                        _stack.Push(left - right);
                        break;
                    }

                    case Instruction.CmpLessI:
                    {
                        var left = _stack.Pop();
                        var right = _stack.Pop();
                        _stack.Push(left < right ? 1ul : 0);
                        break;
                    }

                    case Instruction.CmpGreaterI:
                    {
                        var left = _stack.Pop();
                        var right = _stack.Pop();
                        _stack.Push(left > right ? 1ul : 0);
                        break;
                    }

                    case Instruction.ShiftLeft:
                    {
                        var left = _stack.Pop();
                        var right = (int) _stack.Pop();
                        _stack.Push(left << right);
                        break;
                    }

                    case Instruction.ShiftRight:
                    {
                        var left = _stack.Pop();
                        var right = (int) _stack.Pop();
                        _stack.Push(left >> right);
                        break;
                    }

                    case Instruction.Store:
                    {
                        var location = (int) GetUInt64FromByteCode();
                        if (location != _stack.StackPointer - 1)
                        {
                            var value = _stack.Pop();
                            _stack.Set(location, value);
                        }

                        break;
                    }

                    case Instruction.Load:
                    {
                        var location = (int) GetUInt64FromByteCode();
                        _stack.Push(_stack.Get(location));
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
                        var value = (int) _stack.Pop();
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
                        var value = (int) _stack.Pop();
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
                        var value = (long) _stack.Pop();
                        Console.WriteLine(value);
                        break;
                    }

                    default:
                        throw new InvalidOperationException(
                            $"Invalid instruction 0x{instruction:X} at position {_instructionPointer}.");
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