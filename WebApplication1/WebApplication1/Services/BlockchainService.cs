using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Contracts;
using System.Threading.Tasks;
using System.Collections.Generic;

// It's good practice to put services in their own folder.
// You can create a "Services" folder in your project if you like.
namespace ShipmentApp.Services
{
    public class BlockchainService
    {
        private readonly Web3 _web3;
        private readonly Contract _contract;
        private readonly string _senderAddress;
        private readonly string _contractAddress = "0x5FbDB2315678afecb367f032d93F642f64180aa3"; // <-- PASTE YOUR DEPLOYED ADDRESS HERE

        // --- IMPORTANT ---
        // Paste the entire ABI JSON array from your ShipmentTracker.json file inside the @"..."
        // It will be very long. Make sure to get the whole thing, including the outer brackets [ ].
        private readonly string _contractAbi = @"[ { \"inputs\": [ { \"internalType\": \"string\", \"name\": \"_trackingId\", \"type\": \"string\" }, { \"internalType\": \"string\", \"name\": \"_dataHash\", \"type\": \"string\" } ], \"name\": \"addShipmentUpdate\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": true, \"internalType\": \"string\", \"name\": \"trackingId\", \"type\": \"string\" }, { \"indexed\": false, \"internalType\": \"string\", \"name\": \"dataHash\", \"type\": \"string\" }, { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"timestamp\", \"type\": \"uint256\" } ], \"name\": \"ShipmentUpdated\", \"type\": \"event\" }, { \"inputs\": [ { \"internalType\": \"string\", \"name\": \"_trackingId\", \"type\": \"string\" } ], \"name\": \"getShipmentHistory\", \"outputs\": [ { \"internalType\": \"string[]\", \"name\": \"\", \"type\": \"string[]\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"string\", \"name\": \"\", \"type\": \"string\" } ], \"name\": \"shipmentHistory\", \"outputs\": [ { \"internalType\": \"string[]\", \"name\": \"\", \"type\": \"string[]\" } ], \"stateMutability\": \"view\", \"type\": \"function\" } ]";

        public BlockchainService()
        {
            // ACCOUNT SETUP: Use the private key of the first test account from your Hardhat node.
            // WARNING: In a real app, never hardcode private keys! This is for local testing only.
            var privateKey = "0xac0974bec39a17e36ba4a6b4d238ff944bacb478cbed5efcae784d7bf4f2ff80"; // Default private key for Hardhat account #0
            var account = new Account(privateKey);

            // CONNECTION SETUP: Connect to your local Hardhat node.
            _web3 = new Web3(account, "http://127.0.0.1:8545");
            _senderAddress = account.Address;

            _contract = _web3.Eth.GetContract(_contractAbi, _contractAddress);
        }

        /// <summary>
        /// Adds a new shipment hash to the blockchain.
        /// </summary>
        public async Task<string> AddShipmentUpdateAsync(string trackingId, string dataHash)
        {
            var addUpdateFunction = _contract.GetFunction("addShipmentUpdate");
            var transactionReceipt = await addUpdateFunction.SendTransactionAndWaitForReceiptAsync(_senderAddress, null, null, trackingId, dataHash);
            return transactionReceipt.TransactionHash;
        }

        /// <summary>
        /// Retrieves the history of hashes for a shipment from the blockchain.
        /// </summary>
        public async Task<List<string>> GetShipmentHistoryAsync(string trackingId)
        {
            var getHistoryFunction = _contract.GetFunction("getShipmentHistory");
            var result = await getHistoryFunction.CallAsync<List<string>>(trackingId);
            return result;
        }
    }
}