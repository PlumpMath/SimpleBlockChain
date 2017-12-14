﻿using Microsoft.AspNetCore.Hosting;
using SimpleBlockChain.Core.Factories;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Repositories;
using SimpleBlockChain.Core.Validators;
using System;

namespace SimpleBlockChain.Core.Nodes
{
    public class RPCNode : IDisposable
    {
        private readonly IWalletRepository _walletRepository;
        private readonly Networks _network;
        private readonly IBlockChainFactory _blockChainFactory;
        private readonly ITransactionHelper _transactionHelper;
        private readonly ITransactionValidator _transactionValidator;
        private readonly IBlockValidator _blockValidator;
        private IWebHost _host;

        internal RPCNode(IWalletRepository walletRepository, Networks network, IBlockChainFactory blockChainFactory, ITransactionHelper transactionHelper, ITransactionValidator transactionValidator, IBlockValidator blockValidator)
        {
            _walletRepository = walletRepository;
            _network = network;
            _blockChainFactory = blockChainFactory;
            _transactionHelper = transactionHelper;
            _transactionValidator = transactionValidator;
            _blockValidator = blockValidator;
        }

        public void Start()
        {
            var rpcNodeStartup = new RPCNodeStartup(_walletRepository, _network, _blockChainFactory, _transactionHelper, _transactionValidator, _blockValidator);
            _host = new WebHostBuilder().UseKestrel()
                .UseUrls($"http://localhost:{PortsHelper.GetRPCPort(_network)}")
                .Configure((app) =>
                {
                    rpcNodeStartup.Configure(app);
                })
                .Build();
            _host.Start();
        }

        public void Dispose()
        {
            if (_host == null) { return; }
            _host.Dispose();
            _host = null;
        }
    }
}
