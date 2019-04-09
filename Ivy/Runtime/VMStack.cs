using System;
using System.Collections.Generic;
using System.Linq;

namespace Ivy.Runtime
{
    public class VMStack
    {
        private List<ulong> _stack;
        private int _stackPointer = 0;
        
        public VMStack(int size)
        {
            _stack = new List<ulong>(size / sizeof(ulong));
            _stack.AddRange(Enumerable.Repeat<ulong>(0, size / sizeof(ulong)));
        }

        public void Push(ulong value)
        {
            _stack.Add(value);
            _stackPointer += 1;
        }

        public ulong Pop()
        {
            try
            {
                var value = _stack[_stack.Count - 1];
                _stack.RemoveAt(_stack.Count - 1);
                return value;
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new IndexOutOfRangeException("Tried to pop from an empty stack.");
            }
        }

        public ulong Peek()
        {
            try
            {
                return _stack[_stack.Count - 1];
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new IndexOutOfRangeException("Tried to peek an empty stack.");
            }
        }

        public ulong Get(int index) => _stack[index];

        public void Set(int index, ulong value) => _stack[index] = value;
    }
}