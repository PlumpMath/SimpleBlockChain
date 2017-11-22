﻿using System;
using System.Collections.Generic;

namespace SimpleBlockChain.Core.Blocks
{
    public class BlockChain
    {
        public BlockChain()
        {
            Blocks = new List<Block>
            {
                Block.BuildGenesisBlock()
            };
        }

        public IEnumerable<Block> Blocks { get; private set; }

        public BlockChain AddBlock(Block block)
        {
            if (block == null)
            {
                throw new ArgumentNullException(nameof(block));
            }

            return this;
        }
    }
}
