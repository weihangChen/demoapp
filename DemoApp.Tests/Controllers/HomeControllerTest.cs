using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DemoApp;
using DemoApp.Controllers;
using DemoApp.Tests.Mock;
using DemoApp.DAL;
using Domain.Model;
using DataAccess.DAL;

namespace DemoApp.Tests.Controllers
{

    [TestClass]
    public class HomeControllerTest
    {
        private DOTNETMockingContext<MyDbContext> mockingContext;
        private List<Person> Persons;
        [TestInitialize]
        public void Init()
        {
            mockingContext = new DOTNETMockingContext<MyDbContext>();
            Persons = new List<Person>();
            Persons.Add(new Person { BornDate = new DateTime(1914, 1, 18), PersonID = 1, Name = "joe", PhoneNr = "123456", HasSupernaturalAbility = true });
            Persons.Add(new Person { BornDate = new DateTime(1815, 1, 18), PersonID = 2, Name = "kelly", PhoneNr = "555551", HasSupernaturalAbility = true });
            Persons.Add(new Person { BornDate = new DateTime(1865, 10, 10), PersonID = 3, Name = "nancy", PhoneNr = "555552", HasSupernaturalAbility = true });
            Persons.Add(new Person { BornDate = new DateTime(1866, 10, 11), PersonID = 4, Name = "james", PhoneNr = "555553", HasSupernaturalAbility = true });
            Persons.Add(new Person { BornDate = new DateTime(1867, 10, 11), PersonID = 5, Name = "anna", PhoneNr = "123451" });
            Persons.Add(new Person { BornDate = new DateTime(1868, 10, 11), PersonID = 6, Name = "robert", PhoneNr = "123457" });
            Persons.Add(new Person { BornDate = new DateTime(1869, 10, 11), PersonID = 7, Name = "kim", PhoneNr = "123458" });
            Persons.Add(new Person { BornDate = new DateTime(1870, 10, 11), PersonID = 8, Name = "lee", PhoneNr = "555550" });
            Persons.Add(new Person { BornDate = new DateTime(1871, 10, 11), PersonID = 9, Name = "jerry", PhoneNr = "00000-11", HasSupernaturalAbility = true });
            Persons.Add(new Person { BornDate = new DateTime(1872, 10, 11), PersonID = 10, Name = "ben", PhoneNr = "00000-22" });



        }
        [TestMethod]
        public void ControllerTestDemo()
        {
            // Arrange
            var mockSet = MockHelper.PopulateAndReturnMockSDbSet<Person>(Persons);
            mockingContext.mockDbContext.Setup(m => m.Persons).Returns(mockSet.Object);
            mockingContext.mockDbContext.Setup(x => x.Set<Person>()).Returns(mockSet.Object);
            mockingContext.unit.Setup(x => x.PersonRepository).Returns(new GenericRepository<Person>(mockingContext.mockDbContext.Object));

            var controller = new HomeController(mockingContext.unit.Object, null, null, null, null);
            MockHelper.SetUpController<HomeController, MyDbContext>(controller, mockingContext);

            //first controller method test
            var result = controller.UnitTestDemo1();
            var model = ((ViewResult)result).Model as MyViewModel;
            Assert.AreEqual(10, model.Persons.Count);
            Assert.IsTrue(model.Persons.Exists(x => x.Name.Equals("ben") && x.PersonID == 10));


            //second controller method test
            //mockingContext.SetAsAjax();
            MockHelper.MockViewEngine();
            result = controller.UnitTestDemo2();
            var jsondata = ((JsonResult)result).Data;
            Assert.AreEqual("200", jsondata.GetType().GetProperty("statuscode").GetValue(jsondata, null));

        }

    }
}
