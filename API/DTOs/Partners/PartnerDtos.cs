namespace TextileBilling.Core.DTOs.Partners
{
    public class VendorDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string GSTNumber { get; set; }
        public bool IsActive { get; set; }
    }

    public class CustomerDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string RegistrationNumber { get; set; }
        public decimal LoyaltyPoints { get; set; }
    }
}
