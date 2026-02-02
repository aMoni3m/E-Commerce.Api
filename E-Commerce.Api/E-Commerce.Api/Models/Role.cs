namespace E_Commerce.Api.Models
{
    public class Role
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public ICollection<Customer>? Customers { get; set; }
    }
}