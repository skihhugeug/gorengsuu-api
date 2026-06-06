using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using GorengSuuAPI1.Models;

namespace GorengSuuAPI1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using var conn = new MySqlConnection(connStr);

            conn.Open();

            string sql = @"INSERT INTO users
                          (nama,email,password,role)
                          VALUES
                          (@nama,@email,@password,@role)";

            using var cmd = new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@nama", user.Nama);
            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters.AddWithValue("@password", user.Password);
            cmd.Parameters.AddWithValue("@role", "User");

            cmd.ExecuteNonQuery();

            return Ok(new
            {
                message = "Registrasi berhasil"
            });
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequest login)
        {
            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using var conn = new MySqlConnection(connStr);

            conn.Open();

            string sql = @"SELECT *
                           FROM users
                           WHERE email=@email
                           AND password=@password";

            using var cmd = new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@email", login.Email);
            cmd.Parameters.AddWithValue("@password", login.Password);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return Ok(new
                {
                    message = "Login berhasil",
                    nama = reader["nama"].ToString(),
                    role = reader["role"].ToString()
                });
            }

            return Unauthorized(new
            {
                message = "Email atau Password salah"
            });
        }
    }
}