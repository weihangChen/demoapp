# This app relies on Autofac as IOC Container to wire up ojbects and Moq as mocking framework and include following features

 1. "Helpers.Funcs" - demonstration of "Action" and "Func" as delegate, to save some space for try catch
            you can see the usage of it in "HomeController.Index()"
  
 2. expressiontree built at runtime, encapsulated within "Helpers.ExpressionTree.Rule" object, to build up chains of expressions (in recursive manner) such as "Where(x => x.propertyA.ToLower().Contains(TargetValue.ToLower()))", which works for both in-memory object and agains database. Generic functions for sorting, filtering and paging is written to demonstrate the usage of  "Helpers.ExpressionTree.Rule" objects
 
 3. autofac runtime resolve, you can see the usage of it in "HomeController.Index()"
            the initial config happens here "DemoApp.App_Start.AutofacConfig.ConfigureContainer()"
            within "DemoApp.App_Start.AutofacConfig" I define a static callback method which is assigned to "Helpers.Service.AutofacFactory.CustomWireupCallback" when "Helpers.Service.AutofacFactory" is resolved, so "Helpers.Service.AutofacFactory" uses this callback method to resolve object at runtime
            
  
 4. two set of unit tests are written, one to test through the generic function for paging, sorting and filtering, the second set of test demonstrate how to mock up MVC controller context, DbContext, DbSet etc. to test through controller methods
