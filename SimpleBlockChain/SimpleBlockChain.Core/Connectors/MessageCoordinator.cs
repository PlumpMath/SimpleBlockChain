﻿using SimpleBlockChain.Core.Common;
using SimpleBlockChain.Core.Evts;
using SimpleBlockChain.Core.Messages;
using SimpleBlockChain.Core.Messages.ControlMessages;
using SimpleBlockChain.Core.Messages.DataMessages;
using SimpleBlockChain.Core.Parsers;
using SimpleBlockChain.Core.Storages;
using SimpleBlockChain.Core.Stores;
using SimpleBlockChain.Core.Transactions;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Connectors
{
    public class MessageCoordinator
    {
        private readonly MessageParser _messageParser;
        private readonly PeersRepository _peersStorage;

        public MessageCoordinator()
        {
            _messageParser = new MessageParser();
            _peersStorage = new PeersRepository();
        }

        public Message Receive(Message message, PeerConnector peer, P2PNetworkConnector p2pNetworkConnector)
        {
            if (message.GetCommandName() == Constants.MessageNames.Version) // RETURNS VERSION.
            {
                var msg = message as VersionMessage;
                var instance = PeersStore.Instance();
                var transmittingNode = instance.GetMyIpAddress();
                var receivingNode = msg.TransmittingNode;
                return new VersionMessage(transmittingNode, receivingNode, msg.Nonce, msg.UserAgent, msg.StartHeight, msg.Relay, msg.MessageHeader.Network);
            }

            if (message.GetCommandName() == Constants.MessageNames.Ping) // RETURNS PONG.
            {
                var msg = message as PingMessage;
                var pong = new PongMessage(msg.Nonce, msg.MessageHeader.Network);
                return pong;
            }

            if (message.GetCommandName() == Constants.MessageNames.Verack) // RETURNS VERACK MESSAGE.
            {
                var msg = message as VerackMessage;
                return new VerackMessage(msg.MessageHeader.Network);
            }

            if (message.GetCommandName() == Constants.MessageNames.Addr) // RETURNS THE ADDRS.
            {
                var msg = message as AddrMessage;
                if (msg.IpAddresses != null)
                {
                    foreach (var ipAddress in msg.IpAddresses)
                    {
                        PeerEventStore.Instance().NewPeer(ipAddress);
                    }
                }

                return null;
            }

            if (message.GetCommandName() == Constants.MessageNames.GetAddr) // RETURNS THE ADDRS.
            {
                var msg = message as GetAddressMessage;
                var ipAdrLst = _peersStorage.GetAll();
                var response = new AddrMessage(new CompactSize { Size = (ulong)ipAdrLst.Count() }, msg.MessageHeader.Network);
                foreach (var ipAdr in ipAdrLst)
                {
                    response.IpAddresses.Add(ipAdr);
                }

                return response;
            }

            if (message.GetCommandName() == Constants.MessageNames.MemPool) // RETURNS THE INVENTORY.
            {
                var msg = message as MemPoolMessage;
                var memoryPool = MemoryPool.Instance();
                var txIds = memoryPool.GetTransactions().Select(t => t.GetTxId());
                var inventories = new List<Inventory>();
                foreach (var txId in txIds)
                {
                    inventories.Add(new Inventory(InventoryTypes.MSG_TX, txId));
                }

                return new InventoryMessage(inventories, msg.MessageHeader.Network);
            }

            if (message.GetCommandName() == Constants.MessageNames.GetData) // RETURNS ALL THE DATA TO THE PEER.
            {
                var msg = message as GetDataMessage;
                var messages = Execute(msg);
                if (messages != null)
                {
                    foreach(var m in messages)
                    {
                        peer.Execute(m.Serialize());
                    }
                }

                return null;
            }

            if (message.GetCommandName() == Constants.MessageNames.Transaction) // ADD TRANSACTION INTO MEMORY POOL & BROADCAST IT.
            {
                var msg = message as TransactionMessage;
                AddTransaction(msg.Transaction);
                p2pNetworkConnector.Broadcast(msg.Transaction, msg.MessageHeader.Ipv6);
                return null;
            }

            if (message.GetCommandName() == Constants.MessageNames.Block) // ADD THE BLOCK.
            {
                return null;
            }

            return null;
        }

        public Message Launch(PeerConnector peerConnector, Message message)
        {
            if (message.GetCommandName() == Constants.MessageNames.Inventory)
            {
                Launch(peerConnector, message as InventoryMessage);
            }

            if(message.GetCommandName() == Constants.MessageNames.MemPool)
            {
                Launch(peerConnector, message as MemPoolMessage);
            }

            if (message.GetCommandName() == Constants.MessageNames.GetAddr)
            {
                return Launch(peerConnector, message as GetAddressMessage);
            }
            
            if (message.GetCommandName() == Constants.MessageNames.Addr)
            {
                Launch(peerConnector, message as AddrMessage);
            }

            if (message.GetCommandName() == Constants.MessageNames.Version)
            {
                return Launch(peerConnector, message as VersionMessage);
            }

            if (message.GetCommandName() == Constants.MessageNames.Ping)
            {
                return Launch(peerConnector, message as PingMessage);
            }

            return null;
        }

        private Message Launch(PeerConnector peerConnector, PingMessage message)
        {
            var payload = peerConnector.Execute(message.Serialize());
            var result = _messageParser.Parse(payload.ToArray());
            if (result.GetCommandName() != Constants.MessageNames.Pong)
            {
                return null;
            }

            return result as PongMessage;
        }

        private Message Launch(PeerConnector peerConnector, VersionMessage message)
        {
            var payload = peerConnector.Execute(message.Serialize());
            var result = _messageParser.Parse(payload.ToArray());
            if (result.GetCommandName() == Constants.MessageNames.Version)
            {
                var versionMessage = result as VersionMessage;
                var verackMessage = new VerackMessage(message.MessageHeader.Network);
                payload = peerConnector.Execute(verackMessage.Serialize());
                result = _messageParser.Parse(payload.ToArray());
                if (result.GetCommandName() == Constants.MessageNames.Verack)
                {
                    return result;
                }
            }

            return null;
        }

        private void Launch(PeerConnector peerConnector, AddrMessage message)
        {
            peerConnector.Execute(message.Serialize());
        }

        private Message Launch(PeerConnector peerConnector, GetAddressMessage message)
        {
            var payload = peerConnector.Execute(message.Serialize()); // S : GETADDR.
            return _messageParser.Parse(payload.ToArray());
        }

        private void Launch(PeerConnector peerConnector, InventoryMessage message)
        {
            var payload = peerConnector.Execute(message.Serialize()); // S : INVENTORY.
            var response = _messageParser.Parse(payload.ToArray());
            if (response.GetCommandName() == Constants.MessageNames.GetData) // R : GETDATA.
            {
                var inventories = Execute(response as GetDataMessage);
                foreach(var inventory in inventories)
                {
                    peerConnector.Execute(inventory.Serialize()); // S : TX & B.
                }
            }
        }

        private void Launch(PeerConnector peerConnector, MemPoolMessage message)
        {
            var payload = peerConnector.Execute(message.Serialize()); // S : MEMPOOL.
            var response = _messageParser.Parse(payload.ToArray());
            if (response.GetCommandName() != Constants.MessageNames.Inventory)
            {
                return;
            }

            response = Execute(response as InventoryMessage); // S : GETDATA.
            if (response == null)
            {
                return;
            }
            peerConnector.Execute(response.Serialize());
        }

        private IEnumerable<Message> Execute(GetDataMessage msg)
        {
            var blockChain = BlockChainStore.Instance().GetBlockChain();
            var messages = new List<Message>();
            if (msg.Inventories.Any())
            {
                foreach (var inventory in msg.Inventories)
                {
                    switch (inventory.Type)
                    {
                        case InventoryTypes.MSG_TX:
                            var tx = MemoryPool.Instance().GetTransactions().FirstOrDefault(t => t.GetTxId().SequenceEqual(inventory.Hash));
                            if (tx != null)
                            {
                                messages.Add(new TransactionMessage(tx, msg.MessageHeader.Network));
                            }
                            break;
                        case InventoryTypes.MSG_BLOCK:
                            var block = blockChain.GetBlock(inventory.Hash);
                            if (block != null)
                            {
                                messages.Add(new BlockMessage(block, msg.MessageHeader.Network));
                            }
                            break;
                    }
                }
            }

            return messages;
        }

        private Message Execute(InventoryMessage msg)
        {
            var blockChain = BlockChainStore.Instance().GetBlockChain();
            var inventories = new List<Inventory>();
            if (msg.Inventories.Any())
            {
                foreach (var inventory in msg.Inventories)
                {
                    bool addIntoInventory = false;
                    switch (inventory.Type)
                    {
                        case InventoryTypes.MSG_TX:
                            addIntoInventory = !MemoryPool.Instance().GetTransactions().Any(t => t.GetTxId() == inventory.Hash);
                            break;
                        case InventoryTypes.MSG_BLOCK:
                            addIntoInventory = !blockChain.ContainsBlock(inventory.Hash);
                            break;
                    }

                    if (addIntoInventory)
                    {
                        inventories.Add(new Inventory(inventory.Type, inventory.Hash));
                    }
                }
            }

            if (inventories.Any())
            {
                return new GetDataMessage(inventories, msg.MessageHeader.Network);
            }

            return null;
        }

        private static void AddTransaction(BaseTransaction transaction)
        {
            if (transaction == null)
            {
                return;
            }

            var instance = MemoryPool.Instance();
            var transactions = instance.GetTransactions();
            if (transactions.Any(t => t.GetTxId().SequenceEqual(transaction.GetTxId())))
            {
                return;
            }

            var blockChain = BlockChainStore.Instance().GetBlockChain();
            if (blockChain.ContainsTransaction(transaction.GetTxId()))
            {
                return;
            }

            instance.AddTransaction(transaction);
        }
    }
}
