using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Funcs
{
    public interface IGenericCallbacks
    {
        T ExecuteSafeFunc<Inputs, T>(Func<Inputs, T> CoreFun, Inputs inputs, Action<Exception> OnErrorFun = null, Action FinallyFun = null);
        //@Todo - add the params
        void ExecuteSafeAction(Action CoreFun, Action<Exception, ILog> OnErrorFun = null, Action FinallyFun = null, bool ReThrow = false, string LogMsg = "Error");
    }
}
