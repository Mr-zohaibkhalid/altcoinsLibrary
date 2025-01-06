
using System;
using System.Net;
using NBitcoin;

namespace altcoinsLibrary
{
    internal class Constant
    {
        public static string Username = "Admin000";
        public static string Password = "Admin000";

        public static NetworkCredential NetworkCredential => new NetworkCredential(Username, Password);
        public static Network GetNetwork(altcurrencies currency)
        {
            if (currency == altcurrencies.BTC)
            {
                return Network.Main;
            }
            else if (currency == altcurrencies.LTC)
            {
                return NBitcoin.Altcoins.Litecoin.Instance.Mainnet;
            }
            else if (currency == altcurrencies.BCH)
            {
                return NBitcoin.Altcoins.BCash.Instance.Mainnet;
            }
            else if (currency == altcurrencies.BTG)
            {
                return NBitcoin.Altcoins.BGold.Instance.Mainnet;
            }
            else if (currency == altcurrencies.DASH)
            {
                return NBitcoin.Altcoins.Dash.Instance.Mainnet;
            }
            else if (currency == altcurrencies.DOGE)
            {
                return NBitcoin.Altcoins.Dash.Instance.Mainnet;
            }
            else
            {
                throw new Exception("Coin does not supported");
            }
        }

    }


}
