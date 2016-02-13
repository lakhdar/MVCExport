using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCExport.Models
{
    public class CustomerViewModels
    {
    }

    public class CustomerVM
    {
        public string CustomerID { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string AddressLine1 { get; set; }
        public string City { get; set; }
        public string StateProvinceName { get; set; }
        public string PostalCode { get; set; }
        public string CountryRegionName { get; set; }
        public string ContactDate { get; set; }
    }
}