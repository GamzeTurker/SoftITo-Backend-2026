namespace dpperUygulama.Models
{
    public class ServiceRecord
    {
        public int ServiceId { get; set; }


        public int VehicleId { get; set; }

        public int EmployeeId { get; set; }


        public DateTime ServiceDate { get; set; }

        public string Description { get; set; }

        public decimal Cost { get; set; }


        // Navigation
        public Vehicle Vehicle { get; set; }

        public Employee Employee { get; set; }

        public string Brand { get; set; }
        public string Plate { get; set; }
        public string EmployeeName { get; set; }
        public string FullName { get; set; }
    }
}
