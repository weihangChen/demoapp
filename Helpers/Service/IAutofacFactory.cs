using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Service
{
    public interface IAutofacFactory
    {
        T ResolveObjectByParameterType<T>(params object[] parameters);
    }
}
