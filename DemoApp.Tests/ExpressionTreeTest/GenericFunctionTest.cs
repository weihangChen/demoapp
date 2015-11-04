using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Helpers.Service;
using System.Collections.Generic;
using Helpers.ExpressionTree;
using Helpers.GenericModel;
using System.Linq;
using Domain.Model;

namespace DemoApp.Tests.ExpressionTreeTest
{
    [TestClass]
    public class GenericFunctionTest
    {
        public List<Person> Persons { get; set; }


        [TestInitialize]
        public void TestDataSetup()
        {
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
        public void GenericSortTest()
        {
            var sortingService = new SortingService(new ExpressionBuilder());

            var vm = new SortingModel();
            vm.SortingColumnCurrent = "";
            vm.ShouldReOrder = true;
            vm.SortingColumnNew = "BornDate";
            vm.SortingOrderCurrent = SortingOrder.Ascending;


            var resultSet = sortingService.GenericSortQuery<Person>(Persons.AsQueryable(), vm);
            Assert.IsTrue(resultSet.FirstOrDefault().Name.Equals("kelly"));

            // same column is clicked, order changed
            resultSet = sortingService.GenericSortQuery<Person>(Persons.AsQueryable(), vm);
            Assert.IsTrue(resultSet.FirstOrDefault().Name.Equals("joe"));

            //different column is clicked, ascending as default
            vm.SortingColumnNew = "Name";
            vm.ShouldReOrder = true;
            resultSet = sortingService.GenericSortQuery<Person>(Persons.AsQueryable(), vm);
            Assert.IsTrue(resultSet.FirstOrDefault().Name.Equals("anna"));

        }

        /// <summary>
        /// do not perform orderby or where, only paging
        /// </summary>
        [TestMethod]
        public void GenericPagingTest()
        {
            var query = Persons.AsQueryable();
            var vm = new SortingFilterModel { PageSize = 3 };
            var filterService = new FilterService(new ExpressionBuilder(), new SortingService(new ExpressionBuilder()));

            var resultSet = filterService.GenericPaging(query, vm.AsSortingFilterModel);

            //assert the page total
            Assert.IsTrue(vm.PageTotal == 4, "page totle calculation is incorrect");
            //assert the content within page 1, no need to default orderby since it only do take no skip
            Assert.IsTrue(resultSet.Select(x => x.Name).Contains("joe"), "the paging result contains incorrect elment");
            Assert.IsTrue(resultSet.Select(x => x.Name).Contains("kelly"), "the paging result contains incorrect elment");
            Assert.IsTrue(resultSet.Select(x => x.Name).Contains("nancy"), "the paging result contains incorrect elment");

            //set pageindex other than 1, run the function again, test if orderby + skip + take returns the correct resultset
            vm.PageIndex = 3;
            vm.SortingColumnCurrent = "BornDate";
            vm.SortingOrderCurrent = SortingOrder.Ascending;
            resultSet = filterService.GenericPaging(query, vm.AsSortingFilterModel);
            var compareList = Persons.OrderByDescending(x => x.BornDate).Skip(6).Take(3).ToList();
            Assert.IsTrue(resultSet.ToList()[0].PersonID == compareList[0].PersonID);
            Assert.IsTrue(resultSet.ToList()[1].PersonID == compareList[1].PersonID);
            Assert.IsTrue(resultSet.ToList()[2].PersonID == compareList[2].PersonID);
        }

        /// <summary>
        /// test the SortingFilterModel.ReCaculateSinglePageDatas, make sure the correct amount of SinglePageData + correct values is yield, so the front end navigation will work
        /// </summary>
        [TestMethod]
        public void GenericPagingSinglePageDatasCalculationTest()
        {
            //setup view model
            var filterSerivce = new FilterService(new ExpressionBuilder(), new SortingService(new ExpressionBuilder()));
            var vm = new SortingFilterModel();
            vm.PageSize = 20;


            //test senario 1, total as 900, index 1, should return 5 singlepagedata, with the page1 as selected
            vm.PageTotal = 900;
            vm.PageIndex = 1;
            filterSerivce.ReCaculateSinglePageDatas(vm);

            var singlePageDatas = vm.SinglePageDatas;

            Assert.IsTrue(singlePageDatas.FirstOrDefault() != null && singlePageDatas.FirstOrDefault().IsCurrentPageSelected == true, "the first page is not selected");

            Assert.IsTrue(singlePageDatas.Where(x => x.IsCurrentPageSelected == true).Count() == 1, "only one page is selected");
            Assert.IsTrue(singlePageDatas.Count() == 5, "total 5 pages as output");


            //test senario 2, set the pageindex to 6, then 4,5,6,7,8 should be presented
            vm.PageIndex = 6;
            filterSerivce.ReCaculateSinglePageDatas(vm);

            singlePageDatas = vm.SinglePageDatas;
            Assert.IsTrue(singlePageDatas.Count() == 5, "total 5 pages as output");
            Assert.IsTrue(singlePageDatas.IndexOf(singlePageDatas.FirstOrDefault(x => x.IsCurrentPageSelected)) == 2, "incorrect page is seleted");

            //test senario 3, move to last page, then 896, 897, 898, 899, 900 should be presented
            vm.PageIndex = 900;
            filterSerivce.ReCaculateSinglePageDatas(vm);

            singlePageDatas = vm.SinglePageDatas;
            Assert.IsTrue(singlePageDatas.Count() == 5, "total 5 pages as output");
            var index = singlePageDatas.IndexOf(singlePageDatas.FirstOrDefault(x => x.IsCurrentPageSelected));
            Assert.IsTrue(index == 4, "incorrect page is seleted");
        }




        /// <summary>
        /// combine where and paging, check if pageindex resets automatically when where clause returns fewer pages then current pageindex, there is 
        /// </summary>
        [TestMethod]
        public void GenericPagingWhereMixTest()
        {

            //setup view model
            var vm = new SortingFilterModel();
            vm.SortingColumnCurrent = "BornDate";
            vm.SortingOrderCurrent = SortingOrder.Ascending;
            var filterService = new FilterService(new ExpressionBuilder(), new SortingService(new ExpressionBuilder()));
            var query = Persons.AsQueryable();
            //total 10, 4 pages, set index to 4
            vm.PageIndex = 4;
            query = query.Where(x => x.HasSupernaturalAbility == true);

            var resultSet = filterService.GenericPaging(query, vm.AsSortingFilterModel);
            //test1 the pageindex should have been resetted to 1
            Assert.IsTrue(vm.PageIndex == 1, "the where clause will make pagetotal 2, page index should be resetted to 1");
            //test2 navigate to page 2
            vm.PageIndex = 2;
            resultSet = filterService.GenericPaging(query, vm.AsSortingFilterModel);
            Assert.IsTrue(vm.PageIndex == 2);
        }



        [TestMethod]
        public void GenericContainTest()
        {
            var vm = new SortingFilterModel();
            var filterService = new FilterService(new ExpressionBuilder(), new SortingService(new ExpressionBuilder()));
            var query = Persons.AsQueryable();
            query = filterService.AppendQueryWithContains<Person>("PhoneNr", "12345", query, vm.AsSortingFilterModel);
            Assert.IsTrue(query.Count() == 4);
        }
    }
}
