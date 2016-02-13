using MVCExport.DummyDataModel;
using MVCExport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCExport.Extensions
{
    public static class CustomerExtensions
    {

        public static CustomerVM ToViewModel(this  Customer dataModel)
        {
            CustomerVM vm = null;
            if (dataModel != null)
            {
                vm = new CustomerVM()
                {
                    CustomerID = dataModel.CustomerID.ToString(),
                    Title = dataModel.Title.HasValue ? dataModel.Title.ToString() : null,
                    FirstName = dataModel.FirstName,
                    LastName = dataModel.LastName,
                    PhoneNumber = dataModel.PhoneNumber,
                    EmailAddress = dataModel.EmailAddress,
                    AddressLine1 = dataModel.AddressLine1,
                    City = dataModel.City,
                    StateProvinceName = dataModel.StateProvinceName,
                    PostalCode = dataModel.PostalCode,
                    CountryRegionName = dataModel.CountryRegionName,
                    ContactDate = dataModel.ContactDate == null ? dataModel.ContactDate.ToString() : null,
                };
            }

            return vm;
        }


    }
}