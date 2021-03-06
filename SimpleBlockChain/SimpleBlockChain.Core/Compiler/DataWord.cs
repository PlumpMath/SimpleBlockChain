﻿using Org.BouncyCastle.Math;
using SimpleBlockChain.Core.Extensions;
using SimpleBlockChain.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Compiler
{
    public class DataWord : ICloneable
    {
        private byte[] _data;
        public static DataWord ZERO = new DataWord(new byte[32]);
        public static DataWord ZERO_EMPTY_ARRAY = new DataWord(new byte[0]);
        public static BigInteger _2_256 = BigInteger.ValueOf(2).Pow(256);
        public static BigInteger MAX_VALUE = _2_256.Subtract(BigInteger.One);

        public DataWord()
        {
            _data = new byte[32];
        }
        
        public DataWord(string code) : this(code.FromHexString().ToArray())
        {

        }
        
        public DataWord(int num) : this(ByteBuffer.Allocate(4).PutInt(num)) {  }

        public DataWord(ByteBuffer byteBuffer)
        {
            var buffer = new byte[32];
            var data = byteBuffer.GetArray();
            Array.Copy(data, 0, buffer, 32 - data.Length, data.Length);
            _data = buffer;
        }

        public DataWord(byte[] data)
        {
            Init(data);
        }

        private void Init(byte[] data)
        {
            var result = new List<byte>();
            for(var i = 0; i < 32 - data.Count(); i++)
            {
                result.Add(0);
            }

            result.AddRange(data);
            _data = result.ToArray();
        }
        
        public void Mul(DataWord word)
        {
            BigInteger result = GetValue().Multiply(word.GetValue());
            _data = ByteUtil.CopyToArray(result.And(MAX_VALUE));
        }

        public void Sub(DataWord word)
        {
            BigInteger result = GetValue().Subtract(word.GetValue());
            _data = ByteUtil.CopyToArray(result.And(MAX_VALUE));
        }

        public void Exp(DataWord word)
        {
            var result = GetValue().ModPow(word.GetValue(), _2_256);
            _data = ByteUtil.CopyToArray(result);
        }

        public void BNot()
        {
            if (IsZero())
            {
                _data = ByteUtil.CopyToArray(MAX_VALUE);
                return;
            }

            _data = ByteUtil.CopyToArray(MAX_VALUE.Subtract(GetValue()));
        }

        public void Add(DataWord word)
        {
            byte[] result = new byte[32];
            for (int i = 31, overflow = 0; i >= 0; i--)
            {
                int v = (_data[i] & 0xff) + (word.GetData()[i] & 0xff) + overflow;
                result[i] = (byte)v;
                overflow = v >> 8;
            }

            _data = result;
        }

        public void Div(DataWord word)
        {
            if (word.IsZero())
            {
                this.And(ZERO);
                return;
            }

            BigInteger result = GetValue().Divide(word.GetValue());
            _data = ByteUtil.CopyToArray(result.And(MAX_VALUE));
        }

        public DataWord And(DataWord w2)
        {
            for (int i = 0; i < _data.Count(); ++i)
            {
                _data[i] &= w2.GetData()[i];
            }

            return this;
        }

        public int GetInt()
        {
            int intVal = 0;
            foreach(byte aData in _data)
            {
                intVal = (intVal << 8) + (aData & 0xff);
            }

            return intVal;
        }

        public long GetLong()
        {
            long longVal = 0;
            foreach (byte aData in _data)
            {
                longVal = (longVal << 8) + (aData & 0xff);
            }

            return longVal;
        }

        public int GetIntValueSafe()
        {
            int bytesOccupied = GetBytesOccupied();
            int intValue = GetInt();
            if (bytesOccupied > 4 || intValue < 0) { return int.MaxValue; }
            return intValue;
        }

        public long GetLongValueSafe()
        {
            int bytesOccupied = GetBytesOccupied();
            long longValue = GetLong();
            if (bytesOccupied > 8 || longValue < 0) return long.MaxValue;
            return longValue;
        }

        public DataWord Or(DataWord w2)
        {
            for (int i = 0; i < _data.Count(); ++i)
            {
                _data[i] |= w2.GetData()[i];
            }
            return this;
        }

        public DataWord XOR(DataWord w2)
        {

            for (int i = 0; i < _data.Count(); ++i)
            {
                _data[i] ^= w2.GetData()[i];
            }
            return this;
        }

        public byte[] GetData()
        {
            return _data;
        }

        public byte[] GetReverseData()
        {
            var data = GetDataWithoutFixSize().ToList();
            for (var i = data.Count; i < 32; i++)
            {
                data.Add(0);
            }

            return data.ToArray();
        }

        public bool IsZero()
        {
            foreach (byte tmp in _data)
            {
                if (tmp != 0) return false;
            }

            return true;
        }

        public BigInteger GetValue()
        {
            return new BigInteger(1, _data);
        }

        public BigInteger GetSValue()
        {
            return new BigInteger(_data);
        }
        
        public int GetBytesOccupied()
        {
            int firstNonZero = ByteUtil.FirstNonZeroByte(_data);
            if (firstNonZero == -1) { return 0; }
            return 31 - firstNonZero + 1;
        }

        private byte[] GetDataWithoutFixSize()
        {
            var startIndex = 0;
            for(var i = 0; i < _data.Count(); i++)
            {
                var r = _data.ElementAt(i);
                if (r != 0x00)
                {
                    startIndex = i;
                    break;
                }
            }

            var result = _data.ToList().Skip(startIndex).ToArray();
            return result;
        }

        public object Clone()
        {
            return new DataWord(_data);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var b = obj as DataWord;
            if (b == null)
            {
                return false;
            }

            return GetData().SequenceEqual(b.GetData());
        }
    }
}
