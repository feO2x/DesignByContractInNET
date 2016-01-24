using System.Diagnostics.Contracts;
// ReSharper disable InvocationIsSkipped

namespace DbcCodeContracts
{
    [ContractClass(typeof(InterfaceContract))]
    public interface IFoo
    {
        int Count { get; }
        void Put(int value);
    }

    [ContractClassFor(typeof(IFoo))]
    public abstract class InterfaceContract : IFoo
    {
        int IFoo.Count
        {
            get
            {
                Contract.Ensures(0 <= Contract.Result<int>());
                return default(int);
            }
        }

        void IFoo.Put(int value)
        {
            Contract.Requires(value >= 0);
        }
    }
}
