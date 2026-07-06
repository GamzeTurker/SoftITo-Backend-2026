namespace dpperUygulama.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }


        // Navigation
        public List<Vehicle> Vehicles { get; set; }
    }
}
