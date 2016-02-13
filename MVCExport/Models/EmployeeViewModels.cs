using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCExport.Models 
{
    public class EmployeeViewModels
    {
    }


    public class EmployeeListItemViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Title { get; set; }
        public string BirthDate { get; set; }
        public string HireDate { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string HomePhone { get; set; }
    }

    public class EmployeeListViewModel
    {
        public int Total { get; set; }
        public IEnumerable<string> Headers { get; set; }
        public IEnumerable<IEnumerable<string>> Data { get; set; }
    }



    public class EmployeeDetailsItemViewModel
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Photo { get; set; }
        public string Title { get; set; }
        public string TitleOfCourtesy { get; set; }
        public string BirthDate { get; set; }
        public string HireDate { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string HomePhone { get; set; }
        public string Notes { get; set; }
        public string Manager { get; set; }
         
    }

    public class EmployeeItemDetailsItemViewModel
    {
        public string Label { get; set; }
        public string Value { get; set; }
        public string DateType { get; set; }
    }

    public class EmployeeListDetailsItemViewModel
    {
        public string Title { get; set; }
        public IEnumerable<EmployeeItemDetailsItemViewModel> EmployeeDetails { get; set; }
    }
}