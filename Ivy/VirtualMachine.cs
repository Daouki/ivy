using System;
using System.Collections.Generic;

namespace Ivy
{
    public class VirtualMachine
    {
        private readonly List<byte> _byteCode;
        
        private readonly Stack<ulong> _stack = new Stack<ulong>(128);
        private readonly List<ulong> _locals = new List<ulong>(512);
        private int _instructionPointer = 0;
        
        public VirtualMachine(List<byte> byteCode)
        {
            _byteCode = byteCode;
            for (var i = 0; i < _locals.Capacity; i += 1)
                _locals.Add(0);
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
                        var value = _byteCode.GetRange(_instructionPointer, 8).ToArray();
                        _stack.Push(BitConverter.ToUInt64(value));
                        _instructionPointer += 8;
                        break;
                    }

                    case Instruction.Pop64:
                        _stack.Pop();
                        break;

                    case Instruction.AddI64:
                    {
                        var left = _stack.Pop();
                        var right = _stack.Pop();
                        _stack.Push(left + right);
                        break;
                    }

                    case Instruction.DivI64:
                    {
                        var left = _stack.Pop();
                        var right = _stack.Pop();
                        _stack.Push(left / right);
                        break;
                    }
                    
                    case Instruction.MulI64:
                    {
                        var left = _stack.Pop();
                        var right = _stack.Pop();
                        _stack.Push(left * right);
                        break;
                    }
                    
                    case Instruction.SubI64:
                    {
                        var left = _stack.Pop();
                        var right = _stack.Pop();
                        _stack.Push(left - right);
                        break;
                    }
                    
                    case Instruction.StoreI64:
                    {
                        var location = GetUInt64FromByteCode();
                        var value = _stack.Pop();
                        _locals[(int) location] = value;
                        break;
                    }

                    case Instruction.LoadI64:
                    {
                        var location = GetUInt64FromByteCode();
                        _stack.Push(_locals[(int) location]);
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
    }
}