using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using GorengSuuAPI1.Models;

namespace GorengSuuAPI1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public OrderController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult CreateOrder(Order order)
        {
            string connStr = _configuration.GetConnectionString("DefaultConnection");
            using var conn = new MySqlConnection(connStr);
            conn.Open();

            string sql = @"INSERT INTO orders (nama, no_hp, alamat, total, status)
                          VALUES (@nama, @no_hp, @alamat, @total, @status)";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@nama", order.Nama);
            cmd.Parameters.AddWithValue("@no_hp", order.NoHp);
            cmd.Parameters.AddWithValue("@alamat", order.Alamat);
            cmd.Parameters.AddWithValue("@total", order.Total);
            cmd.Parameters.AddWithValue("@status", "Menunggu Konfirmasi");
            cmd.ExecuteNonQuery();

            long orderId = cmd.LastInsertedId;

            foreach (var item in order.Items)
            {
                string sqlItem = @"INSERT INTO order_items (order_id, nama_produk, harga, jumlah)
                                  VALUES (@order_id, @nama_produk, @harga, @jumlah)";
                using var cmdItem = new MySqlCommand(sqlItem, conn);
                cmdItem.Parameters.AddWithValue("@order_id", orderId);
                cmdItem.Parameters.AddWithValue("@nama_produk", item.NamaProduk);
                cmdItem.Parameters.AddWithValue("@harga", item.Harga);
                cmdItem.Parameters.AddWithValue("@jumlah", item.Jumlah);
                cmdItem.ExecuteNonQuery();
            }

            return Ok(new { message = "Pesanan berhasil dibuat", orderId = orderId });
        }

        [HttpGet]
        public IActionResult GetOrders()
        {
            string connStr = _configuration.GetConnectionString("DefaultConnection");
            using var conn = new MySqlConnection(connStr);
            conn.Open();

            string sql = "SELECT * FROM orders ORDER BY created_at DESC";
            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            var orders = new List<Order>();
            while (reader.Read())
            {
                orders.Add(new Order
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Nama = reader["nama"].ToString(),
                    NoHp = reader["no_hp"].ToString(),
                    Alamat = reader["alamat"].ToString(),
                    Total = Convert.ToDecimal(reader["total"]),
                    Status = reader["status"].ToString(),
                    CreatedAt = Convert.ToDateTime(reader["created_at"])
                });
            }

            return Ok(orders);
        }

        [HttpGet("{id}")]
        public IActionResult GetOrderItems(int id)
        {
            string connStr = _configuration.GetConnectionString("DefaultConnection");
            using var conn = new MySqlConnection(connStr);
            conn.Open();

            string sql = "SELECT * FROM order_items WHERE order_id=@id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();

            var items = new List<OrderItem>();
            while (reader.Read())
            {
                items.Add(new OrderItem
                {
                    Id = Convert.ToInt32(reader["id"]),
                    OrderId = Convert.ToInt32(reader["order_id"]),
                    NamaProduk = reader["nama_produk"].ToString(),
                    Harga = Convert.ToDecimal(reader["harga"]),
                    Jumlah = Convert.ToInt32(reader["jumlah"])
                });
            }

            return Ok(items);
        }

        [HttpGet("cek/{noHp}")]
        public IActionResult GetOrderByNoHp(string noHp)
        {
            string connStr = _configuration.GetConnectionString("DefaultConnection");
            using var conn = new MySqlConnection(connStr);
            conn.Open();

            string sql = "SELECT * FROM orders WHERE no_hp=@no_hp ORDER BY created_at DESC";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@no_hp", noHp);
            using var reader = cmd.ExecuteReader();

            var orders = new List<Order>();
            while (reader.Read())
            {
                orders.Add(new Order
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Nama = reader["nama"].ToString(),
                    NoHp = reader["no_hp"].ToString(),
                    Alamat = reader["alamat"].ToString(),
                    Total = Convert.ToDecimal(reader["total"]),
                    Status = reader["status"].ToString(),
                    CreatedAt = Convert.ToDateTime(reader["created_at"])
                });
            }

            return Ok(orders);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateStatus(int id, Order order)
        {
            string connStr = _configuration.GetConnectionString("DefaultConnection");
            using var conn = new MySqlConnection(connStr);
            conn.Open();

            string sql = "UPDATE orders SET status=@status WHERE id=@id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@status", order.Status);
            cmd.ExecuteNonQuery();

            return Ok(new { message = "Status pesanan berhasil diupdate" });
        }
    }
}