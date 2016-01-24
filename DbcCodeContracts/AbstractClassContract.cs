using System.Diagnostics.Contracts;
// ReSharper disable InvocationIsSkipped

namespace DbcCodeContracts
{
    [ContractClass(typeof(FooContract))]
    public abstract class Foo
    {
        public abstract int Count { get; }

        public abstract void Put(int value);
    }

    [ContractClassFor(typeof(Foo))]
    public abstract class FooContract : Foo
    {
        public override int Count
        {
            get
            {
                Contract.Ensures(0 <= Contract.Result<int>());
                return default(int);
            }
        }

        public override void Put(int value)
        {
            Contract.Requires(value >= 0);
        }
    }
}
