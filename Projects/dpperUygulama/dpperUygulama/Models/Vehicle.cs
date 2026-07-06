namespace dpperUygulama.Models
{
    public class Vehicle
    {
        public int VehicleId { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public string Plate { get; set; }


        public int CustomerId { get; set; }


        // Navigation
        public Customer Customer { get; set; }

        public List<ServiceRecord> ServiceRecords { get; set; }
    }
}
