using altcoinsLibrary.implementation;
using altcoinsLibrary.interfaces;
using Ninject;

namespace altcoinsLibrary
{
    public class altcoinsRegisteration
    {
        public static void BindAll(IKernel kernel)
        {
            kernel.Bind<IRPC>().To<RPC>();
        }
    }
}
