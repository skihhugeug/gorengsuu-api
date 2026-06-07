namespace GorengSuuAPI1.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Nama { get; set; }
        public string NoHp { get; set; }
        public string Alamat { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItem> Items { get; set; }
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string NamaProduk { get; set; }
        public decimal Harga { get; set; }
        public int Jumlah { get; set; }
    }
}