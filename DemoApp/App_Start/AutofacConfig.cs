using Autofac;
using Autofac.Integration.Mvc;
using DataAccess.DAL;
using DemoApp.Controllers;
using DemoApp.DAL;
using Domain.Model;
using Helpers.Annotations;
using Helpers.ExpressionTree;
using Helpers.Funcs;
using Helpers.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;


namespace DemoApp.App_Start
{
    public class AutofacConfig
    {
        public static void ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            //  mvc - config
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).Where(t => t.GetCustomAttributes(typeof(RegisterServiceAttribute), false) != null).AsSelf();
            builder.RegisterControllers(typeof(HomeController).Assembly);


            builder.RegisterType<GenericCallbacks>().As<IGenericCallbacks>().InstancePerRequest();
            builder.RegisterType<Helpers.ExpressionTree.ExpressionBuilder>().As<IExpressionBuilder>();
            builder.RegisterType<SortingService>().As<ISortingService>();
            builder.RegisterType<FilterService>().As<IFilterService>();
            //builder.RegisterType<UnitOfWork>();
            builder.RegisterType<MyDbContext>();

            builder.Register(
               (c, p) =>
               {
                   return new UnitOfWork(c.Resolve<MyDbContext>());
               }
            ).AsSelf();


            builder.Register(
                (c, p) =>
                {
                    return new AutofacFactory(AutofacConfig.CustomWireupCallback);
                }
            ).As<IAutofacFactory>();





            //custom module config
            //builder.RegisterModule(new LoggingModule());
            // Register dependencies in filter attributes
            builder.RegisterFilterProvider();

            // Register dependencies in custom views
            builder.RegisterSource(new ViewRegistrationSource());

            var container = builder.Build();
            // Set MVC DI resolver to use our Autofac container
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            //for test
            //var resolved_model = container.Resolve<SettingViewModel>();


        }


        public static void CustomWireupCallback(ContainerBuilder builder)
        {
            builder.Register((c, p) =>
            {
                return new Superman(p.TypedAs<Person>(), p.TypedAs<Ability>());
            }).As<ISuperman>();
        }
    }
}
