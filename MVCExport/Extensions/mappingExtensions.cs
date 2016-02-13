using MVCExport.DummyDataModel;
using MVCExport.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Web;

namespace MVCExport.Extensions
{
    public static class mappingExtensions
    {
        public static EmployeeListViewModel ToViewModel(this IEnumerable<Employee> dataModel)
        {
            ResourceManager rsm = new ResourceManager(Type.GetType("MVCExport.Resources.Messages"));
            EmployeeListViewModel viewModel = null;
            if (dataModel != null)
            {
                var omodel = dataModel;
                viewModel = new EmployeeListViewModel()
                {
                    Total = dataModel.Count(),
                    Headers = Type.GetType("MVCExport.Models.EmployeeListItemViewModel").GetProperties().Select(x => rsm.GetString(x.Name)).Where(x => !string.IsNullOrEmpty(x)),
                    Data = dataModel.Select(x => new List<string>()
                    {
                        x.EmployeeID.ToString(),
                        x.TitleOfCourtesy+" "+x.LastName +" "+ x.FirstName,
                        x.Title,
                        x.BirthDate.HasValue ? x.BirthDate.Value.ToString("MMM-dd,yyyy") : null,
                        x.HireDate.HasValue ? x.BirthDate.Value.ToString("MMM-dd,yyyy") : null,
                        x.Address+", "+x.PostalCode+"  "+x.City+"  "+x.Region,
                        x.Country,
                        x.HomePhone,
                    })

                };

            }

            return viewModel;
        }

        public static EmployeeListItemViewModel ToItemViewModel(this Employee dataModel)
        {
            EmployeeListItemViewModel vm = null;
            if (dataModel != null)
            {
                vm = new EmployeeListItemViewModel()
                {
                    Id = dataModel.EmployeeID,
                    FullName = dataModel.TitleOfCourtesy + " " + dataModel.LastName + " " + dataModel.FirstName,
                    Title = dataModel.Title,
                    BirthDate = dataModel.BirthDate.HasValue ? dataModel.BirthDate.Value.ToString("MMM-dd,yyyy") : null,
                    HireDate = dataModel.HireDate.HasValue ? dataModel.BirthDate.Value.ToString("MMM-dd,yyyy") : null,
                    Address = dataModel.Address + ", " + dataModel.PostalCode + "  " + dataModel.City + "  " + dataModel.Region,
                    Country = dataModel.Country,
                    HomePhone = dataModel.HomePhone,
                };
            }
            return vm;
        }

        public static EmployeeListDetailsItemViewModel ToViewModel(this  Employee dataModel)
        {
            ResourceManager rsm = new ResourceManager(Type.GetType("MVCExport.Resources.Messages"));
            EmployeeListDetailsItemViewModel viewModel = null;

            if (dataModel != null)
            {
                var omodel = dataModel.ReportsTo.HasValue ? new DummyEmployeeRepo().GetElementById(dataModel.ReportsTo.Value) : null;
                var tempVM = new EmployeeDetailsItemViewModel()
                {
                    Id = dataModel.EmployeeID,
                    LastName = dataModel.LastName,
                    FirstName = dataModel.FirstName,
                    Title = dataModel.Title,
                    TitleOfCourtesy = dataModel.TitleOfCourtesy,
                    BirthDate = dataModel.BirthDate.HasValue ? dataModel.BirthDate.Value.ToString("MMM-dd,yyyy") : null,
                    HireDate = dataModel.HireDate.HasValue ? dataModel.HireDate.Value.ToString("MMM-dd,yyyy") : null,
                    Address = dataModel.Address,
                    City = dataModel.City,
                    Region = dataModel.Region,
                    PostalCode = dataModel.PostalCode,
                    Country = dataModel.Country,
                    HomePhone = dataModel.HomePhone,
                    Notes = dataModel.Notes,
                    Manager = omodel != null ? omodel.FirstName + " " + omodel.LastName : null,
                    Photo = dataModel.Photo
                };
                viewModel = new EmployeeListDetailsItemViewModel()
                {
                    Title = tempVM.LastName + " " + tempVM.FirstName,
                    EmployeeDetails = tempVM.GetType().GetProperties().Select(x => new EmployeeItemDetailsItemViewModel()
                    {
                        Label = rsm.GetString(x.Name),
                        DateType = x.Name == "Photo" ? "image" : "string",
                        Value = tempVM.GetPropertyValue(x.Name) as string
                    }).Where(x => !string.IsNullOrEmpty(x.Label))

                };
            }
            return viewModel;
        }


        public static object GetPropertyValue(this EmployeeDetailsItemViewModel dataModel, string propName)
        {
            object val = null;
            var prop = dataModel.GetType().GetProperty(propName);
            if (prop != null)
            {
                val = prop.GetValue(dataModel, null);
            }

            return val;
        }
    }
}