using Autofac;
using Autofac.Core;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Service
{
    public class AutofacFactory : IAutofacFactory
    {
        public IContainer container { get; set; }
        private Action<ContainerBuilder> CustomWireupCallback;

        public AutofacFactory(Action<ContainerBuilder> CustomWireupCallback)
        {
            this.CustomWireupCallback = CustomWireupCallback;
        }
        protected void IOCConfigInit()
        {
            var builder = new ContainerBuilder();
            if (CustomWireupCallback != null)
                CustomWireupCallback(builder);
            container = builder.Build();

        }


        protected void PreResolveCheck()
        {
            if (container == null)
                IOCConfigInit();
        }


        public T ResolveObjectByParameterType<T>(params object[] parameters)
        {
            if (parameters.ToList().Count == 0)
                return ResolveObject<T>();

            var autofacParams = parameters.Select(x => new TypedParameter(x.GetType(), x));
            return ResolveObject<T>(autofacParams);
        }


        protected T ResolveObject<T>(IEnumerable<Parameter> parameters = null)
        {
            PreResolveCheck();
            T result = default(T);
            //using (var scope = container.BeginLifetimeScope())
            //{
            if (parameters == null)
                result = container.Resolve<T>();
            else
                result = container.Resolve<T>(parameters);
            //}
            return result;
        }


        protected T[] ResolveAllWithParameters<T>(IEnumerable<Parameter> parameters)
        {
            PreResolveCheck();
            return container.Resolve<IEnumerable<T>>(parameters).ToArray();
        }

    }
}
