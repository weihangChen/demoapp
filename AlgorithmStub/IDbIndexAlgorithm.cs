using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmStub
{
    public interface IDbIndexAlgorithm<T> : ICollection<T>
        where T : IComparable
    {
        void Add(IList<T> dataCollection);


        ICollection<T> Contains(T from, T to);

    }
}
