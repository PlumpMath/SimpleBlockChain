﻿using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Helpers;
using System;
using System.Net;

namespace SimpleBlockChain.Wallet
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("==== Welcome to SimpleBlockChain (WALLET) ====");
            var network = MenuHelper.ChooseNetwork();
            var ipBytes = IPAddress.Parse("192.254.72.190").MapToIPv6().GetAddressBytes(); // VIRTUAL NETWORK.
            var launcher = new NodeLauncher(network, ServiceFlags.NODE_NONE, ipBytes);
            launcher.Launch(); // LAUNCH NODE.
            launcher.ConnectP2PNetwork(); // CONNECT TO P2PNETWORK.
            // DisplayMenu();
            // FOR EACH TRANSACTION AN ADDRESS IS GENERATED.

            // IMPLEMENT A FULL SERVICE WALLET.
            // https://bitcoin.org/en/developer-guide#full-service-wallets
            // TODO : Generate a PRIVATE KEY.
            // TODO : Get my address.
            // TODO : Enters an ADDRESS & BROADCAST a TRANSACTION.
            // TODO : Display HOW MUCH LEFT IN MY WALLET.
        }

        private static void DisplayMenu()
        {
            Console.WriteLine("What-do you want to do ?");
            MenuHelper.DisplayMenuItem("1. Send a transaction");
            MenuHelper.DisplayMenuItem("2. See my amount of bitcoins");
            MenuHelper.DisplayMenuItem("3. Exit the application");
            var number = MenuHelper.EnterNumber();
            switch(number)
            {
                case 1:
                    DisplayMenu();
                    return;
                case 2:
                    DisplayWalletInformation();
                    DisplayMenu();
                    return;
                case 3:
                    Console.WriteLine("Bye bye");
                    Console.ReadLine();
                    return;
            }

            MenuHelper.DisplayError("Please enter a correct option number");
            DisplayMenu();
        }

        private static void DisplayWalletInformation()
        {

        }
    }
}
