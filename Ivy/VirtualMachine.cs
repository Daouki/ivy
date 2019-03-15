using System;
using System.Collections.Generic;

namespace Ivy
{
    public class VirtualMachine
    {
        private readonly List<byte> _byteCode;
        
        private Stack<ulong> _stack = new Stack<ulong>(128);
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
                        var value = _byteCode.GetRange(_instructionPointer, 8).ToArray();
                        _stack.Push(BitConverter.ToUInt64(value));
                        _instructionPointer += 8;
                        break;
                    
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
                    
                    default:
                        throw new Exception("Invalid instruction");
                }
            }
        }
    }
}