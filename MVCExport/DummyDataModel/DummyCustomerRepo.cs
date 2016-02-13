using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace MVCExport.DummyDataModel
{
    public class DummyCustomerRepo 
    {
        IEnumerable<Customer> customers;

        public IEnumerable<Customer> Customers
        {
            get {
                if (customers == null)
                {
                    string path = HostingEnvironment.MapPath(@"~/App_Data\customers.json");
                    if (string.IsNullOrEmpty(path))
                    {
                        path = @"C:\Users\212394355\Downloads\App\DAL\MVCExport\MVCExport\App_Data\customers.json";

                    }
                    customers = JsonConvert.DeserializeObject<List<Customer>>(System.IO.File.ReadAllText(path));
                }
                return customers; 
            }
           
        }

        public IEnumerable<Customer> GetAll()
        {
            return Customers;
        }

        public Customer GetElementById(int id)
        {
            return Customers.FirstOrDefault(x => x.CustomerID == id);
        }

    }
}