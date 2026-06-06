using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using GorengSuuAPI1.Models;

namespace GorengSuuAPI1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ProductController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            string connStr =
                _configuration.GetConnectionString("DefaultConnection");

            var products = new List<Product>();

            using var conn = new MySqlConnection(connStr);

            conn.Open();

            string sql = "SELECT * FROM products";

            using var cmd = new MySqlCommand(sql, conn);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                products.Add(new Product
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Nama = reader["nama"].ToString(),
                    Harga = Convert.ToDecimal(reader["harga"]),
                    Gambar = reader["gambar"].ToString(),
                    Stok = Convert.ToInt32(reader["stok"])
                });
            }

            return Ok(products);
        }

        [HttpPost]
        public IActionResult AddProduct(Product product)
        {
            string connStr =
                _configuration.GetConnectionString("DefaultConnection");

            using var conn = new MySqlConnection(connStr);

            conn.Open();

            string sql = @"INSERT INTO products
                  (nama,harga,gambar,stok)
                  VALUES
                  (@nama,@harga,@gambar,@stok)";

            using var cmd = new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@nama", product.Nama);
            cmd.Parameters.AddWithValue("@harga", product.Harga);
            cmd.Parameters.AddWithValue("@gambar", product.Gambar);
            cmd.Parameters.AddWithValue("@stok", product.Stok);

            cmd.ExecuteNonQuery();

            return Ok(new
            {
                message = "Produk berhasil ditambahkan"
            });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, Product product)
        {
            string connStr =
                _configuration.GetConnectionString("DefaultConnection");

            using var conn = new MySqlConnection(connStr);

            conn.Open();

            string sql = @"UPDATE products
                   SET nama=@nama,
                       harga=@harga,
                       gambar=@gambar,
                       stok=@stok
                   WHERE id=@id";

            using var cmd = new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@nama", product.Nama);
            cmd.Parameters.AddWithValue("@harga", product.Harga);
            cmd.Parameters.AddWithValue("@gambar", product.Gambar);
            cmd.Parameters.AddWithValue("@stok", product.Stok);

            cmd.ExecuteNonQuery();

            return Ok(new
            {
                message = "Produk berhasil diupdate"
            });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            string connStr =
                _configuration.GetConnectionString("DefaultConnection");

            using var conn = new MySqlConnection(connStr);

            conn.Open();

            string sql = "DELETE FROM products WHERE id=@id";

            using var cmd = new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();

            return Ok(new
            {
                message = "Produk berhasil dihapus"
            });
        }

    }


}