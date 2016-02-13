using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVCExport;
using MVCExport.DummyDataModel;
using System.Collections.Generic;
using MVCExport.Models;
using System.Linq;
using MVCExport.Extensions;

namespace MVCExport.Tests.FilreResult
{
    [TestClass]
    public class MyCsvFileResultTests
    {
        public IEnumerable<Customer> Customers { get; set; }

        [TestInitialize()]
        public void MyTestInitialize()
        {
            Customers = new DummyCustomerRepo().GetAll();
        }

       [TestMethod]
        public void TestMethod1()
        {

           

        }

    }
}
