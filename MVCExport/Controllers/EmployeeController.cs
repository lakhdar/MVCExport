using Newtonsoft.Json;
using MVCExport.DummyDataModel;
using MVCExport.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.UI;
using MVCExport.Resources;
using MVCExport.Models;
using Microsoft.Reporting.WebForms;

namespace MVCExport.Controllers
{
    public class EmployeeController : Controller
    {
        DummyEmployeeRepo _dataSource;

        public EmployeeController()
        {
            _dataSource = new DummyEmployeeRepo();
        }

        public ActionResult MyExportCSV()
        {
            IEnumerable<Employee> dataList = _dataSource.GetAll();
            IEnumerable<EmployeeListItemViewModel> vmList = dataList.Select(x => x.ToItemViewModel());

            return new CsvFileResult<EmployeeListItemViewModel>(vmList, "toto.csv");
        }

        public ActionResult MyCustomExportCSV()
        {

            IEnumerable<string> headers = new[] { 
                Messages. FullName , 
                Messages. Title ,
                Messages. PhoneNumber ,
                Messages.Address 
            };

            IEnumerable<Employee> dataList = _dataSource.GetAll();
            Func<Employee, IEnumerable<string>> map = x => new[] { x.TitleOfCourtesy + " " + x.LastName + " " + x.FirstName, x.Title, x.HomePhone, x.Address + ", " + x.PostalCode + "  " + x.City + "  " + x.Region };
            return new CsvFileResult<Employee>(dataList, "employees.csv", map, headers);
        }

        public ActionResult  ExportExcelGridView()
        {
            IEnumerable<Employee> dataList = _dataSource.GetAll();
            IEnumerable<EmployeeListItemViewModel> vmList = dataList.Select(x => x.ToItemViewModel()).ToList();

            return new XlsGridViewFileResult<EmployeeListItemViewModel>(vmList, "employees.xls");
        }

        public ActionResult ExportExcelGridViewWithMap()
        {
            IEnumerable<string> headers = new[] { 
                Messages. FullName , 
                Messages. Title ,
                Messages. PhoneNumber ,
                Messages.Address 
            };

            IEnumerable<Employee> dataList = _dataSource.GetAll();
            Func<Employee, IEnumerable<string>> map = x => new[] { x.TitleOfCourtesy + " " + x.LastName + " " + x.FirstName, x.Title, x.HomePhone, x.Address + ", " + x.PostalCode + "  " + x.City + "  " + x.Region };

            return new XlsGridViewFileResult<Employee>(dataList,map,headers, "employees.xls") ;
        }

        public ActionResult ExportExcelRazorView()
        {
            IEnumerable<string> headers = new[] { 
                Messages. FullName , 
                Messages. Title ,
                Messages. PhoneNumber ,
                Messages.Address 
            };

            IEnumerable<Employee> dataList = _dataSource.GetAll();
            Func<Employee, IEnumerable<string>> map = x => new[] { x.TitleOfCourtesy + " " + x.LastName + " " + x.FirstName, x.Title, x.HomePhone, x.Address + ", " + x.PostalCode + "  " + x.City + "  " + x.Region };
            EmployeeListViewModel vm = new EmployeeListViewModel()
            {
                Total = dataList.Count(),
                Headers = headers,
                Data = dataList.Select(x => map(x))
            };
            return new XlsViewFileResult<Employee>(vm,  this.ControllerContext, @"ExcelGridExport","employees.xls");
        }


        public ActionResult ExcelExport()
        {
            IEnumerable<Employee> dataList = _dataSource.GetAll();
            IEnumerable<EmployeeListItemViewModel> vmList = dataList.Select(x => x.ToItemViewModel());

            return new XlsFileResult<EmployeeListItemViewModel>(vmList, "toto.csv");
        }


        public ActionResult PdfExport()
        {
            IEnumerable<string> headers = new[] { 
                Messages. FullName , 
                Messages. Title ,
                Messages. PhoneNumber ,
                Messages.Address 
            };

            IEnumerable<Employee> dataList = _dataSource.GetAll();
            Func<Employee, IEnumerable<string>> map = x => new[] { x.TitleOfCourtesy + " " + x.LastName + " " + x.FirstName, x.Title, x.HomePhone, x.Address + ", " + x.PostalCode + "  " + x.City + "  " + x.Region };
            EmployeeListViewModel vm = new EmployeeListViewModel()
            {
                Total = dataList.Count(),
                Headers = headers,
                Data = dataList.Select(x => map(x))
            };

            return new PdfFileResult<Employee>(vm, this.ControllerContext, @"PDFGridExport", "employees.pdf");
        }


        public ActionResult RDLExport()
        {
            IEnumerable<string> headers = new[] { 
                Messages. FullName , 
                Messages. Title ,
                Messages. PhoneNumber ,
                Messages.Address 
            };

            IEnumerable<Employee> dataList = _dataSource.GetAll();
            Func<Employee, IEnumerable<string>> map = x => new[] { 
                x.TitleOfCourtesy + " " + x.LastName + " " + x.FirstName, 
                x.Title, 
                x.HomePhone, 
                x.Address + ", " + x.PostalCode + "  " + x.City + "  " + x.Region,
                x.BirthDate.ToString(),
                x.HireDate.ToString()
            
            };
            

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Report/EmployeesList.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = dataList.Select(x=>new { 
               Name =x.TitleOfCourtesy + " " + x.LastName + " " + x.FirstName, 
               Title=x.Title, 
               PhoneNumber=x.HomePhone, 
               Address= x.Address + ", " + x.PostalCode + "  " + x.City + "  " + x.Region,
               BirthDate= x.BirthDate.ToString(),
               HireDate=  x.HireDate.ToString()
            
            });

            localReport.DataSources.Add(reportDataSource);

            string reportType = "PDF";
            string mimeType="";
            string encoding;
            string fileNameExtension;
            string deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>PDF</OutputFormat>" +
            "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            renderedBytes = localReport.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
            return File(renderedBytes, mimeType, "totoi."+fileNameExtension); 
        }

        // GET: Employee
        public ActionResult Index()
        {
            var dataModel = _dataSource.GetAll();
            var viewModel = dataModel.ToViewModel();


            return View(viewModel);
        }

        // GET: Employee/Details/5
        public ActionResult Details(int id)
        {
            var dataModel = _dataSource.GetElementById(id);
            var viewModel = dataModel.ToViewModel();

            return View(viewModel);
        }

        // GET: Employee/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Employee/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Employee/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Employee/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Employee/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        private static List<string> ProductToStringArray(Customer x)
        {
            return new List<string> {  
                x.CustomerID.ToString(),
                x.Title.ToString(),
                x.FirstName,
                x.LastName,
                x.PhoneNumber,
                x.EmailAddress,
                x.AddressLine1,
                x.City,
                x.StateProvinceName,
                x.PostalCode,
                x.CountryRegionName, };
        }
    }
}
