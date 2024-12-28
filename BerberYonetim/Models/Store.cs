namespace WebApplication1.Models
{
    public class Store
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public StoreType Type { get; set; }
        public int NumberOfPersonal { get; set; }

        // Çalışanlar ile ilişki
        public ICollection<Employee>? Employees { get; set; }
    }

}