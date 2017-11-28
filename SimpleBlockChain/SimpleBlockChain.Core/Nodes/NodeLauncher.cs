﻿using SimpleBlockChain.Core.Connectors;
using SimpleBlockChain.Core.Evts;
using SimpleBlockChain.Core.Messages;
using SimpleBlockChain.Core.Transactions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleBlockChain.Core.Nodes
{
    public class NodeLauncher : IDisposable
    {
        private readonly Networks _network;
        private readonly ServiceFlags _serviceFlag;
        private P2PNetworkConnector _p2pNetworkConnector;
        private P2PNode _p2pNode;
        private RPCNode _rpcNode;

        public NodeLauncher(Networks network, ServiceFlags serviceFlag)
        {
            _network = network;
            _serviceFlag = serviceFlag;
            _p2pNetworkConnector = new P2PNetworkConnector();
            _p2pNetworkConnector.ConnectEvent += P2PConnectEvent;
            _p2pNetworkConnector.DisconnectEvent += P2PDisconnectEvent;
            _p2pNode = new P2PNode(_network, _serviceFlag);
        }

        public RPCNode GetRpcNode()
        {
            return _rpcNode;
        }

        public P2PNode GetP2PNode()
        {
            return _p2pNode;
        }

        public Networks GetNetwork()
        {
            return _network;
        }

        public event EventHandler ConnectP2PEvent;
        public event EventHandler DisconnectP2PEvent;

        public void LaunchP2PNode(IEnumerable<byte> ipAddress = null)
        {
            _p2pNode.Start(ipAddress);
        }

        public void LaunchRPCNode()
        {

        }

        public Task ConnectP2PNetwork()
        {
            return _p2pNetworkConnector.Listen(_network);
        }

        public bool IsP2PNetworkRunning()
        {
            return _p2pNetworkConnector.IsRunning;
        }

        public ConcurrentBag<PeerConnector> GetActivePeers()
        {
            return _p2pNetworkConnector.GetActivePeers();
        }

        public void Broadcast(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            _p2pNetworkConnector.Broadcast(message);
        }

        public void Broadcast(BaseTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            _p2pNetworkConnector.Broadcast(transaction);
        }

        private void P2PConnectEvent(object sender, EventArgs e)
        {
            if (ConnectP2PEvent != null)
            {
                ConnectP2PEvent(this, EventArgs.Empty);
            }
        }

        private void P2PDisconnectEvent(object sender, EventArgs e)
        {
            if (DisconnectP2PEvent != null)
            {
                DisconnectP2PEvent(this, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            if (_p2pNode != null) _p2pNode.Dispose();
            _p2pNetworkConnector.Dispose();
        }
    }
}
