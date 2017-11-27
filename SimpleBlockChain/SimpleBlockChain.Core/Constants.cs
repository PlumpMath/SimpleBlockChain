﻿using System.Collections.Generic;
using System.Numerics;

namespace SimpleBlockChain.Core
{
    public static class Constants
    {
        // public const string DNS_IP_ADDRESS = "127.0.0.1";
        public const string DNS_IP_ADDRESS = "192.168.76.135";
        // public const string DNS_IP_ADDRESS = "192.168.1.6";
        public const int SupportedProtocolVersion = 70015;
        public const uint DEFAULT_NBITS = 0x1d00ffff;
        public const uint DEFAULT_GENESIS_NONCE = 2083236893;
        public const uint DEFAULT_GENESIS_SEQUENCE = 0xFFFFFFFF;
        public const uint DEFAULT_GENESIS_TRANSACTION_VERSION = 1;
        public const uint DEFAULT_GENESIS_TRANSACTION_LOCKTIME = 0;
        public static BigInteger DEFAULT_GENESIS_PUBLIC_KEY_HASH = BigInteger.Parse("728452153457041532823387359015516014791234273651");
        public const string DEFAULT_SIGNATURE_CONTENT = "simple_block_chain";

        public class Ports
        {
            public const string MainNet = "8333";
            public const string TestNet = "18333";
            public const string RegTest = "18444";
        }

        public static List<string> MessageNameLst = new List<string> { Constants.MessageNames.Ping, MessageNames.Addr,
            MessageNames.Version, MessageNames.Verack, MessageNames.GetAddr, MessageNames.Inventory, MessageNames.Transaction };

        public class MessageNames
        {
            public const string Ping = "ping";
            public const string Pong = "pong";
            public const string Addr = "addr";
            public const string Version = "version";
            public const string Verack = "verack";
            public const string GetAddr = "getaddr";
            public const string Transaction = "tx";
            public const string Inventory = "inv";
        }
    }
}
