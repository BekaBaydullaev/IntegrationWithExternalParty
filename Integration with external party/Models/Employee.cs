using CsvHelper.Configuration;
using IntegrationWithExternalParty.Models;

public sealed class EmployeeMap : ClassMap<Employee>
{
    public EmployeeMap()
    {
        Map(m => m.PayrollNumber).Name("Personnel_Records.Payroll_Number");
        Map(m => m.Forenames).Name("Personnel_Records.Forenames");
        Map(m => m.Surname).Name("Personnel_Records.Surname");
        Map(m => m.DateOfBirth).Name("Personnel_Records.Date_of_Birth").TypeConverter<CustomDateTimeConverter>();
        Map(m => m.Telephone).Name("Personnel_Records.Telephone");
        Map(m => m.Mobile).Name("Personnel_Records.Mobile");
        Map(m => m.Address).Name("Personnel_Records.Address");
        Map(m => m.Address2).Name("Personnel_Records.Address_2");
        Map(m => m.Postcode).Name("Personnel_Records.Postcode");
        Map(m => m.EmailHome).Name("Personnel_Records.EMail_Home");
        Map(m => m.StartDate).Name("Personnel_Records.Start_Date").TypeConverter<CustomDateTimeConverter>();

    }
}

namespace IntegrationWithExternalParty.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string PayrollNumber { get; set; }
        public string Forenames { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Telephone { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Postcode { get; set; }
        public string EmailHome { get; set; }
        public DateTime StartDate { get; set; }

        
    }
}
