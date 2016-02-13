using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCExport.DummyDataModel
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public TitleOfCourtesy? Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string AddressLine1 { get; set; }
        public string City { get; set; }
        public string StateProvinceName { get; set; }
        public string PostalCode { get; set; }
        public string CountryRegionName { get; set; }
        public DateTime ContactDate { get; set; }
    }



    public enum TitleOfCourtesy
    {
        Mr = 1,
        Ms = 2,
        Mrs = 4,
        Dr = 8,
        Pr = 16,
    }
}