namespace dpperUygulama.Models
{
    
        public class Employee
        {
            public int EmployeeId { get; set; }

            public string EmployeeName { get; set; }

            public string JobTitle { get; set; }


            // Navigation
            public List<ServiceRecord> ServiceRecords { get; set; }
        
    }
}
