namespace WebApplication1.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Store ile ilişki
        public int? StoreId { get; set; }
        public Store Store { get; set; }
        
        // Yetenekler ile ilişki (Many-to-Many)
        public ICollection<Skill> Skills { get; set; } = new List<Skill>();
    }
}