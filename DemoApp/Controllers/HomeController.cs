using DataAccess.DAL;
using Domain.Model;
using Helpers.Funcs;
using Helpers.Service;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Helpers.Extensions;
using Helpers.GenericModel;

namespace DemoApp.Controllers
{
    public class HomeController : Controller
    {
        private IAutofacFactory ObjectFactory;
        private IGenericCallbacks GenericCallbacks;
        private ILog Logger;
        private UnitOfWork WorkUnit;
        private ISortingService SortingService;
        private IFilterService FilterService;
        public HomeController(UnitOfWork WorkUnit, ISortingService SortingService, IFilterService FilterService, IAutofacFactory ObjectFactory, IGenericCallbacks GenericCallbacks)
        {
            this.Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            this.WorkUnit = WorkUnit;
            this.SortingService = SortingService;
            this.FilterService = FilterService;
            this.ObjectFactory = ObjectFactory;
            this.GenericCallbacks = GenericCallbacks;
        }


        /// <summary>
        ///  dont expect seeing stuff at the UI, I run following code just to demonstrate some functions
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {

            //demo 1 - use factory to create a superman
            var superman = ObjectFactory.ResolveObjectByParameterType<ISuperman>(new Person { Name = "Marc", HasSupernaturalAbility = true }, Ability.MetalLiquidBody);

            //demo 2 - save some try catch space
            GenericCallbacks.ExecuteSafeAction(superman.SaveTheWorld);

            return View();
        }

        /// <summary>
        /// no view coupled to this method, used only to demonstrate how to unit test controller method
        /// purpose is to show the mocking of dbcontext, dbset, unitofwork and repository
        /// </summary>
        /// <returns></returns>
        public ActionResult UnitTestDemo1()
        {
            var vm = new MyViewModel(WorkUnit.PersonRepository.GetAsList());
            return View(vm);
        }

        /// <summary>
        /// no view coupled to this method, used only to demonstrate how to unit test controller method
        /// purpose is to show the mocking of ajax method and viewengine
        /// </summary>
        /// <returns></returns>
        public ActionResult UnitTestDemo2()
        {
            var result = new JsonResult();
            var vm = new MyViewModel(WorkUnit.PersonRepository.GetAsList());
            result.Data = new { statuscode = "200", data = this.GetPartialViewAsStr("_PersonGrid", vm) };
            return result;
        }


        public ActionResult GridDisplay()
        {
            var data = GetDummy().OrderByDescending(x=>x.BornDate).AsQueryable();
            var vm = new MyViewModel { SortingColumnCurrent = "BornDate" };
            vm.Persons = FilterService.GenericPaging(data, vm.AsSortingFilterModel).ToList();
            return View(vm);
        }


        public ActionResult GridFilter(MyViewModel vm)
        {
            var result = new JsonResult();
            if (!Request.IsAjaxRequest())
            {
                result.Data = new { statuscode = "400", error = "Invalid Request" };
                return result;
            }


            var query = GetDummy().AsQueryable();

            //generic sort
            query = SortingService.GenericSortQuery<Person>(query, vm);
            //generic sort
            if (vm.FreeText.IsNotEmpty() && vm.FreeText.ToList().Count > 4)
            {
                query = FilterService.AppendQueryWithContains<Person>("PhoneNr", vm.FreeText.ToLower(), query, vm.AsSortingFilterModel);
            }
            //generic paging
            query = FilterService.GenericPaging(query, vm.AsSortingFilterModel);

            //return data
            vm.Persons = query.ToList();

            ModelState.Clear();
            var modalViewStr = this.GetPartialViewAsStr("_Grid", vm);
            result.Data = new { statuscode = "200", data = modalViewStr };
            return result;
        }



        protected List<Person> GetDummy()
        {
            var Persons = new List<Person>();
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
            return Persons;
        }


    }


    public class MyViewModel : SortingFilterModel
    {
        public MyViewModel()
        {
            this.Persons = new List<Person>();
        }
        public MyViewModel(List<Person> Persons)
        {
            this.Persons = Persons;
        }
        public List<Person> Persons { get; set; }
        public string FreeText { get; set; }
    }
}