using System;
using System.Collections.Generic;
using System.Linq;

namespace Ivy.Runtime
{
    public class VMStack
    {
        private const int ByteSize = 1;
        private const int WordSize = 2;
        private const int DWordSize = 4;
        private const int QWordSize = 8;
        
        private List<byte> _stack;
        private int _stackPointer = 0;
        
        public VMStack(int size)
        {
            _stack = new List<byte>(size);
            _stack.AddRange(Enumerable.Repeat<byte>(0, size));
        }
        
        public void PushByte(byte value)
        {
            _stack[_stackPointer] = value;
            _stackPointer += ByteSize;
        }

        public byte PopByte()
        {
            var value = PeekByte();
            _stackPointer -= ByteSize;
            return value;
        }

        public byte PeekByte() => _stack[_stackPointer - ByteSize];

        public byte GetByte(int index) => _stack[index];
        
        public void PushWord(ushort value)
        {
            var bytes = BitConverter.GetBytes(value);
            for (var i = 0; i < WordSize; i++)
                _stack[_stackPointer + i] = bytes[i];
            _stackPointer += WordSize;
        }

        public ushort PopWord()
        {
            var value = PeekWord();
            _stackPointer -= WordSize;
            return value;
        }

        public ushort PeekWord() =>
            BitConverter.ToUInt16(_stack.GetRange(_stackPointer - 2, 2).ToArray());

        public ushort GetWord(int index) =>
            BitConverter.ToUInt16(_stack.GetRange(index, WordSize).ToArray());
        
        public void PushDWord(uint value)
        {
            var bytes = BitConverter.GetBytes(value);
            for (var i = 0; i < 4; i++)
                _stack[_stackPointer + i] = bytes[i];
            _stackPointer += 4;
        }

        public uint PopDWord()
        {
            var value = PeekDWord();
            _stackPointer -= DWordSize;
            return value;
        }

        public uint PeekDWord() =>
            BitConverter.ToUInt32(_stack.GetRange(_stackPointer - DWordSize, DWordSize).ToArray());

        public uint GetDWord(int index) =>
            BitConverter.ToUInt32(_stack.GetRange(index, DWordSize).ToArray());
        
        public void PushQWord(ulong value)
        {
            var bytes = BitConverter.GetBytes(value);
            for (var i = 0; i < QWordSize; i++)
                _stack[_stackPointer + i] = bytes[i];
            _stackPointer += QWordSize;
        }

        public ulong PopQWord()
        {
            var value = PeekQWord();
            _stackPointer -= QWordSize;
            return value;
        }

        public ulong PeekQWord() =>
            BitConverter.ToUInt64(_stack.GetRange(_stackPointer - QWordSize, QWordSize).ToArray());

        public ulong GetQWord(int index) =>
            BitConverter.ToUInt64(_stack.GetRange(index, QWordSize).ToArray());
    }
}