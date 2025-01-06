using System;

namespace altcoinsLibrary.interfaces
{
    interface IRPC
    {
        void GetAddress(string mnemoninc, string index, out string address);
        decimal GetBalance(string address);
        string Transfer(string Mnemonic, int index, decimal ssTransferValue, string ssToAddress);
    }
}
