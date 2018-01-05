﻿using System;
using System.Collections.Generic;

namespace SimpleBlockChain.Core.Compiler
{
    public class SolidityOpCode
    {
        private static SolidityOpCode _instance;
        private static Dictionary<byte, SolidityOpCodes> intToTypeMap = new Dictionary<byte, SolidityOpCodes>();

        private SolidityOpCode()
        {
            foreach(SolidityOpCodes opCode in Enum.GetValues(typeof(SolidityOpCodes)))
            {
                var key = (byte)((byte)opCode & 0xFF);
                intToTypeMap.Add(key, opCode);
            }
        }

        public static SolidityOpCode Instance()
        {
            if (_instance == null)
            {
                _instance = new SolidityOpCode();
            }

            return _instance;
        }

        public SolidityOpCodes? GetCode(byte code)
        {
            var key = (byte)((byte)code & 0xFF);
            if (!intToTypeMap.ContainsKey(key))
            {
                return null;
            }

            return intToTypeMap[key];
        }
    }

    public enum SolidityOpCodes : byte
    {
        STOP = 0x00,
        ADD = 0x01,
        MUL = 0x02,
        SUB = 0x03,
        DIV = 0x04,
        SDIV = 0x05,
        MOD = 0x06,
        SMOD = 0x07,
        ADDMOD = 0x08,
        MULMOD = 0x09,
        EXP = 0x0a,
        SIGNEXTEND = 0x0b,
        LT = 0x10,
        GT = 0x11,
        SLT  = 0x12,
        SGT = 0x13,
        EQ = 0x14,
        ISZERO = 0x15,
        AND = 0x16,
        OR = 0x17,
        XOR = 0x18,
        NOT = 0x19,
        BYTE = 0x1a,
        SHA3 = 0x20,
        ADDRESS = 0x30,
        BALANCE = 0x31,
        ORIGIN = 0x32,
        CALLER = 0x33,
        CALLVALUE = 0x34,
        CALLDATALOAD = 0x35,
        CALLDATASIZE = 0x36,
        CALLDATACOPY = 0x37,
        CODESIZE = 0x38,
        CODECOPY = 0x39,
        RETURNDATASIZE = 0x3d,
        RETURNDATACOPY = 0x3e,
        GASPRICE = 0x3a,
        EXTCODESIZE = 0x3b,
        EXTCODECOPY = 0x3c,
        BLOCKHASH = 0x40,
        COINBASE = 0x41,
        TIMESTAMP = 0x42,
        NUMBER = 0x43,
        DIFFICULTY = 0x44,
        GASLIMIT = 0x45,
        POP = 0x50,
        MLOAD = 0x51,
        MSTORE = 0x52,
        MSTORE8 = 0x53,
        SLOAD = 0x54,
        SSTORE = 0x55,
        JUMP = 0x56,
        JUMPI = 0x57,
        PC = 0x58,
        MSIZE = 0x5a,
        JUMPDEST = 0x5b,
        PUSH1 = 0x60,
        PUSH2 = 0x61,
        PUSH3 = 0x62,
        PUSH4 = 0x63,
        PUSH5 = 0x64,
        PUSH6 = 0x65,
        PUSH7 = 0x66,
        PUSH8 = 0x67,
        PUSH9 = 0x68,
        PUSH10 = 0x69,
        PUSH11 = 0x6a,
        PUSH12 = 0x6b,
        PUSH13 = 0x6c,
        PUSH14 = 0x6d,
        PUSH15 = 0x6e,
        PUSH16 = 0x6f,
        PUSH17 = 0x70,
        PUSH18 = 0x71,
        PUSH19 = 0x72,
        PUSH20 = 0x73,
        PUSH21 = 0x74,
        PUSH22 = 0x75,
        PUSH23 = 0x76,
        PUSH24 = 0x77,
        PUSH25 = 0x78,
        PUSH26 = 0x79,
        PUSH27 = 0x7a,
        PUSH28 = 0x7b,
        PUSH29 = 0x7c,
        PUSH30 = 0x7d,
        PUSH31 = 0x7e,
        PUSH32 = 0x7f,
        DELEGATECALL = 0xf4,
        STATICCALL = 0xfa,
        REVERT = 0xfd,
        SUICIDE = 0xff,
        DUP1 = 0x80
    }
}