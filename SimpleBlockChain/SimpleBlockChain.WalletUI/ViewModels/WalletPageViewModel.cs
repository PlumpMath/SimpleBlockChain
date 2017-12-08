﻿using SimpleBlockChain.Core;
using SimpleBlockChain.WalletUI.Commands;
using SimpleBlockChain.WalletUI.Events;
using System;
using System.Windows.Input;

namespace SimpleBlockChain.WalletUI.ViewModels
{
    public class WalletPageViewModel : BaseViewModel
    {
        private double _sendValue;
        private string _sendAddress;
        private bool _isMainNetChecked;
        private bool _isTestNetChecked;
        private int _nbBlocks;
        private bool _isConnected;
        private ICommand _mainNetCommand;
        private ICommand _testNetCommand;

        public WalletPageViewModel()
        {
            _isMainNetChecked = true;
            _isTestNetChecked = false;
            _isConnected = false;
            _nbBlocks = 0;
            SendMoney = new RelayCommand(p => SendMoneyExecute(), p => CanSendMoney());
            _mainNetCommand = new RelayCommand(p => ExecuteMainNet(), p => CanExecuteMainNet());
            _testNetCommand = new RelayCommand(p => ExecuteTestNet(), p => CanExecuteTestNet());
        }

        public ICommand SendMoney { get; private set; }
        public event EventHandler SendMoneyEvt;
        public event EventHandler<NetworkEventHandler> NetworkSwitchEvt;

        public ICommand MainNetCommand
        {
            get { return _mainNetCommand; }
        }

        public ICommand TestNetCommand
        {
            get { return _testNetCommand; }
        }

        public bool IsMainNetChecked
        {
            get
            {
                return _isMainNetChecked;
            }
            set
            {
                if (value != _isMainNetChecked)
                {
                    _isMainNetChecked = value;
                    NotifyPropertyChanged(nameof(IsMainNetChecked));
                }
            }
        }

        public bool IsTestNetChecked
        {
            get
            {
                return _isTestNetChecked;
            }
            set
            {
                if (value != _isTestNetChecked)
                {
                    _isTestNetChecked = value;
                    NotifyPropertyChanged(nameof(IsTestNetChecked));
                }
            }
        }

        public int NbBlocks
        {
            get
            {
                return _nbBlocks;
            }
            set
            {
                if (value != _nbBlocks)
                {
                    _nbBlocks = value;
                    NotifyPropertyChanged(nameof(NbBlocks));
                }
            }
        }

        public bool IsConnected
        {
            get
            {
                return _isConnected;
            }
            set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    NotifyPropertyChanged(nameof(IsConnected));
                }
            }
        }

        public double SendValue
        {
            get
            {
                return _sendValue;
            }
            set
            {
                if (value != _sendValue)
                {
                    _sendValue = value;
                    NotifyPropertyChanged(nameof(SendValue));
                }
            }
        }

        public string SendAddress
        {
            get
            {
                return _sendAddress;
            }
            set
            {
                if (_sendAddress != value)
                {
                    _sendAddress = value;
                    NotifyPropertyChanged(nameof(SendAddress));
                }
            }
        }

        private bool CanSendMoney()
        {
            return true;
        }

        private void SendMoneyExecute()
        {
            if (SendMoneyEvt != null)
            {
                SendMoneyEvt(this, EventArgs.Empty);
            }
        }

        public void ExecuteMainNet()
        {
            IsMainNetChecked = true;
            if (NetworkSwitchEvt != null)
            {
                NetworkSwitchEvt(this, new NetworkEventHandler(Networks.MainNet));
            }
        }

        private bool CanExecuteMainNet()
        {
            return true;
        }

        public void ExecuteTestNet()
        {
            IsTestNetChecked = true;
            if (NetworkSwitchEvt != null)
            {
                NetworkSwitchEvt(this, new NetworkEventHandler(Networks.TestNet));
            }
        }

        private bool CanExecuteTestNet()
        {
            return true;
        }
    }
}
