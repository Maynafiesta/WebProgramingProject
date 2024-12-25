namespace KuaforYonetim.Models
{
    public class Hizmet
    {
        public int HizmetID { get; set; }
        public string Adı { get; set; }
        public int Süre { get; set; }  // Dakika
        public decimal Ücret { get; set; }
        public int SalonID { get; set; }
        public Salon Salon { get; set; }  // İlişkili salon
    }
}
