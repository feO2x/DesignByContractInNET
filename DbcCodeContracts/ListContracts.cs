using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
// ReSharper disable InvocationIsSkipped

namespace DbcCodeContracts
{
    // This contract for IList<T> does not work because
    // the actual code has to have the ContractClass
    // Attribute applied to it.

    //[ContractClassFor(typeof(IList<>))]
    //public abstract class ListContracts<T> : IList<T>
    //{
    //    [ContractInvariantMethod]
    //    private void Invariants()
    //    {
    //        Contract.Invariant(Count >= 0);
    //    }

    //    public IEnumerator<T> GetEnumerator()
    //    {
    //        throw new System.NotImplementedException();
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return GetEnumerator();
    //    }

    //    public void Add(T item)
    //    {
    //        Contract.Ensures(Count == Contract.OldValue(Count) + 1);
    //        Contract.Ensures(this[Count - 1].Equals(item));
            
    //    }

    //    public void Clear()
    //    {
    //        Contract.Ensures(Count == 0);
    //        Contract.Ensures(Contract.ForAll(this, i => i == null));
    //    }

    //    public bool Contains(T item)
    //    {
    //        throw new System.NotImplementedException();
    //    }

    //    public void CopyTo(T[] array, int arrayIndex)
    //    {
    //        Contract.Requires(array != null);
    //        Contract.Requires(arrayIndex >= 0);
    //        Contract.Requires(array.Length - arrayIndex >= Count);

    //        Contract.Ensures(Contract.ForAll(
    //                                         0,
    //                                         Count,
    //                                         i => array[arrayIndex + i].Equals(this[i])));
    //    }

    //    public bool Remove(T item)
    //    {
    //        throw new System.NotImplementedException();
    //    }

    //    public int Count { get; }

    //    public bool IsReadOnly
    //    {
    //        get
    //        {
    //            Contract.Ensures(true);
    //            return default(bool);
    //        }
    //    }

    //    public int IndexOf(T item)
    //    {
    //        Contract.Ensures(Contract.Result<int>() >= -1);
    //        Contract.Ensures(Contract.Result<int>() < Count);
    //        return default(int);
    //    }

    //    public void Insert(int index, T item)
    //    {
    //        Contract.Requires(index > 0);
    //        Contract.Requires(index <= Count);

    //        Contract.Ensures(Count == Contract.OldValue(Count) + 1);
    //        Contract.Ensures(this[index].Equals(item));
    //        Contract.Ensures(Contract.ForAll(
    //                                         index + 1, Count,
    //                                         i => this[i].Equals(Contract.OldValue(this[i - 1]))));
    //    }

    //    public void RemoveAt(int index)
    //    {
    //        Contract.Requires(index > 0 && index < Count);

    //        Contract.Ensures(Count == Contract.OldValue(Count) - 1);
    //        Contract.Ensures(Contract.ForAll(index, Count, i => this[i].Equals(Contract.OldValue(this[i + 1]))));
    //    }

    //    public T this[int index]
    //    {
    //        get { throw new System.NotImplementedException(); }
    //        set {
    //            Contract.Requires(index <= Count);
    //            Contract.Requires(index >= 0);

    //            Contract.Ensures(this[index].Equals(value));
    //        }
    //    }
    //}
}