using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using System.Data.Entity;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web;
using System.Web.Routing;
using DataAccess.DAL;

namespace DemoApp.Tests.Mock
{
    public class MockHelper
    {
        public static Mock<DbSet<T>> PopulateAndReturnMockSDbSet<T>(List<T> data) where T : class
        {
            var dataQuery = data.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();
            mockSet.Setup(x => x.Add(It.IsAny<T>())).Returns((T x) => x).Callback((T x) =>
            {
                data.Add(x);
            });

            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(dataQuery.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(dataQuery.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(dataQuery.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => dataQuery.GetEnumerator());
            return mockSet;
        }

        public static void SetUpController<T, G>(T controller, DOTNETMockingContext<G> mockingContext)
            where T : Controller
            where G : DbContext
        {
            controller.ControllerContext = mockingContext.mockControllerContext.Object;
            controller.Url = mockingContext.UrlHelper;
        }


        public static void MockViewEngine()
        {
            ViewEngines.Engines.Clear();
            var view = new Mock<IView>();
            var engine = new Mock<IViewEngine>();
            var viewEngineResult = new ViewEngineResult(view.Object, engine.Object);
            engine.Setup(x => x.FindPartialView(It.IsAny<ControllerContext>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(viewEngineResult);
            ViewEngines.Engines.Add(engine.Object);
        }

    }


    public class DOTNETMockingContext<T> where T : DbContext
    {

        public DOTNETMockingContext(string defaultUserName = "User")
        {
            //context
            var Identity = new GenericIdentity(defaultUserName);
            Principal = new GenericPrincipal(Identity, null);
            mockHttpContext = new Mock<HttpContextBase>();
            Request = new Mock<HttpRequestBase>();
            mockHttpContext.Setup(t => t.User).Returns(() => Principal);
            mockHttpContext.Setup(t => t.Cache).Returns(HttpRuntime.Cache);
            mockHttpContext.Setup(x => x.Request).Returns(Request.Object);

            mockControllerContext = new Mock<ControllerContext>();

            mockControllerContext.Setup(t => t.HttpContext).Returns(mockHttpContext.Object);

            //route + urlhelper

            RequestContext = new RequestContext(mockHttpContext.Object, new RouteData());
            UrlHelper = new UrlHelper(RequestContext, RouteTable.Routes);

            //data access layer
            mockDbContext = new Mock<T>();
            unit = new Mock<UnitOfWork>(mockDbContext.Object);
        }

        //.NET environment
        public GenericPrincipal Principal { get; set; }
        public Mock<ControllerContext> mockControllerContext { get; set; }
        public Mock<HttpRequestBase> Request { get; set; }
        public RequestContext RequestContext { get; set; }
        public UrlHelper UrlHelper { get; set; }
        public Mock<HttpContextBase> mockHttpContext { get; set; }

        //data access layer related
        public Mock<T> mockDbContext { get; set; }
        public Mock<UnitOfWork> unit { get; set; }

        public void SetAsAjax()
        {
            Request.SetupGet(x => x.Headers).Returns(
                new System.Net.WebHeaderCollection { { "X-Requested-With", "XMLHttpRequest" } }
            );
        }
    }
}

