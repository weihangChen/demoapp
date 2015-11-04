using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.Annotations;
using Helpers.Extensions;
namespace Helpers.Funcs
{
    [RegisterService]
    public class GenericCallbacks : IGenericCallbacks
    {
        private ILog Logger;
        public GenericCallbacks()
        {
            Logger = LogManager.GetLogger(this.GetType());
        }
        public T ExecuteSafeFunc<Inputs, T>(Func<Inputs, T> CoreFun, Inputs inputs, Action<Exception> OnErrorFun = null, Action FinallyFun = null)
        {
            Object obj = null;
            try
            {
                obj = CoreFun(inputs);
            }
            catch (Exception exception)
            {
                if (OnErrorFun != null)
                {

                    OnErrorFun(exception);

                }
                else
                {

                    Logger.Error("Error", exception);
                }
            }
            finally
            {
                if (FinallyFun != null)
                    FinallyFun();
            }
            return (T)obj;
        }

        public void ExecuteSafeAction(Action CoreFun, Action<Exception, ILog> OnErrorFun = null, Action FinallyFun = null, bool ReThrow = false, string LogMsg = "")
        {
            try
            {
                CoreFun();
            }
            catch (Exception exception)
            {
                if (OnErrorFun != null)
                {
                    OnErrorFun(exception, Logger);
                }
                else
                {
                    if (LogMsg.IsNotEmpty())
                        Logger.Error(LogMsg, exception);
                    if (ReThrow)
                        throw exception;
                }
            }
            finally
            {
                if (FinallyFun != null)
                    FinallyFun();
            }
        }
    }
}
