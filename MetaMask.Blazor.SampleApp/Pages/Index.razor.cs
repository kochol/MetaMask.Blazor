﻿using MetaMask.Blazor.Exceptions;
using Microsoft.AspNetCore.Components;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace MetaMask.Blazor.SampleApp.Pages
{
    public partial class Index
    {
        [Inject]
        public MetaMaskService MetaMaskService { get; set; } = default!;

        public bool HasMetaMask { get; set; }
        public string? SelectedAddress { get; set; }
        public string? TransactionCount { get; set; }
        public string? SignedData { get; set; }
        public string? FunctionResult { get; set; }
        public string? RpcResult { get; set; }

        protected override async Task OnInitializedAsync()
        {
            HasMetaMask = await MetaMaskService.HasMetaMask();
            bool isSiteConnected = await MetaMaskService.IsSiteConnected();
            if (isSiteConnected)
                await GetSelectedAddress();
        }

        public async Task ConnectMetaMask()
        {
            await MetaMaskService.ConnectMetaMask();
            await GetSelectedAddress();
        }

        public async Task GetSelectedAddress()
        {
            SelectedAddress = await MetaMaskService.GetSelectedAddress();
            Console.WriteLine($"Address: {SelectedAddress}");
        }

        public async Task GetTransactionCount()
        {
            var transactionCount = await MetaMaskService.GetTransactionCount();
            TransactionCount = $"Transaction count: {transactionCount}";
        }

        public async Task SignData(string label, string value)
        {
            try
            {
                var result = await MetaMaskService.SignTypedData("test label", "test value");
                SignedData = $"Signed: {result}";
            }
            catch(UserDeniedException)
            {
                SignedData = "User Denied";
            }
            catch(Exception ex)
            {
                SignedData = $"Exception: {ex}";
            }
        }

        public async Task CallSmartContractFunctionExample1()
        {
            try
            {
                string address = "0x21253c6f5E16a56031dc8d8AF0bb372bc4A4DfDA";
                BigInteger weiValue = 0;
                string data = GetEncodedFunctionCall();

                data = data[2..]; //Remove the 0x from the generated string
                var result = await MetaMaskService.SendTransaction(address, weiValue, data);
                FunctionResult = $"TX Hash: {result}";
            }
            catch (UserDeniedException)
            {
                FunctionResult = "User Denied";
            }
            catch (Exception ex)
            {
                FunctionResult = $"Exception: {ex}";
            }
        }

        public async Task CallSmartContractFunctionExample2()
        {
            try
            {
                string address = "0x722BcdA7BD1a0f8C1c9b7c0eefabE36c1f0fBF2a";
                BigInteger weiValue = 1000000000000000;
                string data = GetEncodedFunctionCallExample2();

                data = data[2..]; //Remove the 0x from the generated string
                var result = await MetaMaskService.SendTransaction(address, weiValue, data);
                FunctionResult = $"TX Hash: {result}";
            }
            catch (UserDeniedException)
            {
                FunctionResult = "User Denied";
            }
            catch (Exception ex)
            {
                FunctionResult = $"Exception: {ex}";
            }
        }

        private string GetEncodedFunctionCall()
        {
            //This example uses Nethereum.ABI to create the ABI encoded string to call a smart contract function

            //Smart contract has a function called "share"
            FunctionABI function = new FunctionABI("share", false);

            //With 4 inputs
            var inputsParameters = new[] {
                    new Parameter("address", "receiver"),
                    new Parameter("string", "appId"),
                    new Parameter("string", "shareType"),
                    new Parameter("string", "data")
                };
            function.InputParameters = inputsParameters;

            var functionCallEncoder = new FunctionCallEncoder();

            var data = functionCallEncoder.EncodeRequest(function.Sha3Signature, inputsParameters,
                "0x92B143F46C3F8B4242bA85F800579cdF73882e98",
                "MetaMask.Blazor",
                "Sample",
                DateTime.UtcNow.ToString());
            return data;
        }

        private string GetEncodedFunctionCallExample2()
        {
            //This example uses Nethereum.ABI to create the ABI encoded string to call a smart contract function

            //Smart contract has a function called "share"
            FunctionABI function = new FunctionABI("setColor", false);

            //With 4 inputs
            var inputsParameters = new[] {
                    new Parameter("string", "color")
                };
            function.InputParameters = inputsParameters;

            var functionCallEncoder = new FunctionCallEncoder();

            var data = functionCallEncoder.EncodeRequest(function.Sha3Signature, inputsParameters, new object[] { "green" } );

            return data;
        }


        public async Task GenericRpc()
        {
            var result = await MetaMaskService.GenericRpc("eth_requestAccounts");
            RpcResult = $"RPC result: {result}";
        }
    }
}
