using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace MVCExport.DummyDataModel
{
    public class DummyEmployeeRepo
    {
        IEnumerable<Employee> employees;

        public IEnumerable<Employee> Employees
        {
            get {
                if (employees == null)
                {
                    string path = HostingEnvironment.MapPath(@"~/App_Data\employees.json");
                    if (string.IsNullOrEmpty(path))
                    {
                        path = @"C:\Users\212394355\Downloads\App\DAL\MVCExport\MVCExport\App_Data\employees.json";

                    }
                    employees = JsonConvert.DeserializeObject<List<Employee>>(System.IO.File.ReadAllText(path));
                }
                return employees; 
            }
           
        }

        public IEnumerable<Employee> GetAll()
        {
            return Employees;
        }

        public Employee GetElementById(int id)
        {
            return Employees.FirstOrDefault(x => x.EmployeeID == id);
        }

    }


    
}