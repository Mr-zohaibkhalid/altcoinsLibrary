
using System;
using System.Linq;
using altcoinsLibrary.interfaces;
using NBitcoin;
using NBitcoin.RPC;

namespace altcoinsLibrary.implementation
{
    public class RPC : IRPC
    {
        private RPCClient client;
        public RPC(altcurrencies currency)
        {
            this.client = new RPCClient(Constant.NetworkCredential, "http://0.0.0.0:" + Constant.GetNetwork(currency).RPCPort, Constant.GetNetwork(currency));
            //this.client = client;

        }
        public decimal EstimateFee()
        {

            return client.EstimateSmartFee(1).FeeRate.FeePerK.ToDecimal(MoneyUnit.BTC);
        }
        public void GetAddress(string mnemoninc, string index, out string address)
        {
            string[] temp = mnemoninc.Split(' ');
            string file = string.Empty;
            temp.Take(24).ToList().ForEach(x => file += x + " ");
            file = file.Remove(file.Length - 1);
            Mnemonic restoreNnemo = new Mnemonic(file);
            ExtKey masterKey = restoreNnemo.DeriveExtKey();
            KeyPath keypth = new KeyPath("m/44'/0'/0'/0/" + index);
            ExtKey key = masterKey.Derive(keypth);
            address = key.ScriptPubKey.GetDestinationAddress(client.Network).ToString();

            client.ImportAddress(new BitcoinPubKeyAddress(address), address, false);
        }

        public decimal GetBalance(string address)
        {
            var bitcoinAddress = BitcoinAddress.Create(address);
            var coins = client.ListUnspent(1, 1000000000, bitcoinAddress);
            decimal Sum = 0;
            foreach (var item in coins)
            {
                Sum = Sum + item.Amount.ToDecimal(MoneyUnit.BTC);
            }
            return Sum;
        }
        public string Transfer(string Mnemonic, int index, decimal ssTransferValue, string ssToAddress)
        {
            try
            {
                Mnemonic restoreNnemo = new Mnemonic(Mnemonic);
                ExtKey masterKey = restoreNnemo.DeriveExtKey();
                KeyPath keypth = new KeyPath("m/44'/0'/0'/0/" + index);
                ExtKey key = masterKey.Derive(keypth);
                string PublicKey = key.PrivateKey.PubKey.GetAddress(client.Network).ToString();
                string ssPrivateKey = key.PrivateKey.GetBitcoinSecret(client.Network).ToString();
                //client.ImportAddress(new BitcoinPubKeyAddress(PublicKey), PublicKey, false);
                //transaction Started
                var transaction = new Transaction();
                var bitcoinPrivateKey = new BitcoinSecret(ssPrivateKey);
                var fromAddress = bitcoinPrivateKey.GetAddress().ToString();
                decimal addressBalanceConfirmed = GetBalance(fromAddress);
                if (addressBalanceConfirmed <= ssTransferValue)
                    throw new Exception("The wallet doesn't have enough funds!");

                var bitcoinAddress = BitcoinAddress.Create(fromAddress);
                var listOfUnspents = client.ListUnspent(1, 1000000, bitcoinAddress);
                int txsIn = 0;
                foreach (var item in listOfUnspents)
                {
                    OutPoint outpointToSpend = item.OutPoint;
                    transaction.Inputs.Add(new TxIn() { PrevOut = outpointToSpend });
                    transaction.Inputs[txsIn].ScriptSig = bitcoinPrivateKey.ScriptPubKey;
                    txsIn = txsIn + 1;
                }

                //add address to send money
                var toPubKeyAddress = BitcoinAddress.Create(ssToAddress);
                TxOut toAddressTxOut = new TxOut()
                {
                    Value = new Money((decimal)ssTransferValue, MoneyUnit.BTC),
                    ScriptPubKey = toPubKeyAddress.ScriptPubKey
                };
                transaction.Outputs.Add(toAddressTxOut);

                decimal change = addressBalanceConfirmed - ssTransferValue - (decimal)0.0001; //Minerfee
                if (change > 0)
                {
                    var fromPubKeyAddress = BitcoinAddress.Create(fromAddress);
                    TxOut changeAddressTxOut = new TxOut()
                    {
                        Value = new Money((decimal)change, MoneyUnit.BTC),
                        ScriptPubKey = fromPubKeyAddress.ScriptPubKey
                    };
                    transaction.Outputs.Add(changeAddressTxOut);
                }
                // sign transaction
                transaction.Sign(bitcoinPrivateKey, false);

                return client.SendRawTransaction(transaction).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //transaction ended
        }
    }
}
