using AspNetCoreCookieAuthentication.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVCCoreApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCoreCookieAuthentication;
using AspNetCoreCookieAuthentication.Data;
using System.ComponentModel.DataAnnotations;


namespace AspNetCoreCookieAuthentication.Controllers
{
    public class HomeController : Controller
    {
        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;
        List<Booking> lbooking = new List<Booking>();
        void connectionString()
        {
            con.ConnectionString = "data source=DESKTOP-KQB5L3N; database=ASPNetCookie; integrated security = SSPI; MultipleActiveResultSets=True;";
        }

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [Authorize]
        public IActionResult Index()
        {
            FetchData();
            return View("User",lbooking);
        }

        public void FetchData()
        {

            if(lbooking.Count >0)
            {
                lbooking.Clear();
            }
            try
            {
                connectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = "select * from Booking_Order ";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    lbooking.Add(new Booking()
                    {
                        id = dr["id"].ToString(),
                        Date = dr["Date"].ToString(),
                        Time = dr["Time"].ToString(),
                        User_id = dr["User_id"].ToString(),
                        Room_id = dr["Room_id"].ToString(),
                        Status = dr["Status"].ToString()
                    });
                }
                con.Close();
            }
            catch (System.Reflection.TargetInvocationException e)
            {

                throw;
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [Authorize]
        public IActionResult Product()
        {
            return View("/Views/Product.cshtml");
        }
        public IActionResult User()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View("/Views/Account/Register.cshtml");
        }
        [HttpPost]
        public IActionResult Register(string fname, string lname, string email, string password)
        {

            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "INSERT INTO Users(Email, Password, Firstname, Lastname) VALUES('"+email+"','"+password+ "','" + fname + "','" + lname + "')";
            SqlCommand command = new SqlCommand(com.CommandText, con);
            command.ExecuteReader();
            con.Close();
            return View("/Views/Account/Login.cshtml");
 
        }


        public IActionResult Login()
        {
            return View("/Views/Account/Login.cshtml");
        }

        [HttpPost]
        public IActionResult Cancel(string did)
        {
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "DELETE FROM Booking_Order WHERE id = '"+did+"'";
            SqlCommand command = new SqlCommand(com.CommandText, con);
            command.ExecuteReader();
            con.Close();
            FetchData();
            return View("User", lbooking);
        }

        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View("/Views/Account/Login.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> VerifyAsync(string Name,string Password)
        {
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "select * from Users where Email='" + Name + "' and password='" + Password + "'";
            dr = com.ExecuteReader();
            if (dr.Read())
            {
                con.Close();
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, Name.ToString()),
                    new Claim(ClaimTypes.Name, Name),
                    new Claim("UserDefined", "whatever"),
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        principal,
                        new AuthenticationProperties { IsPersistent = true });

                return LocalRedirect("/Home");
            }
            else
            {
                con.Close();
                return View("/Views/Account/Register.cshtml");
            }

        }

        public IActionResult Show(string date)
        {
            //2022-12-30
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "select * from Booking_Order where Date='" + date + "'";
            dr = com.ExecuteReader();
            if (dr.Read())
            {
                con.Close();
                return View("User");
            }
            else
            {
                con.Close();
                return View("User");
            }
        }


        [HttpPost]
        public IActionResult Book([FromBody] Booking booking)
        {
            connectionString();
            con.Open();
            com.Connection = con;
            //com.CommandText = "INSERT INTO Booking_Order(Date, Time, User_id, Room_id, Status) VALUES('" + date + "','" + time + "','" + user + "','" + room + "','unconfirmed')";
            com.CommandText = "INSERT INTO Booking_Order(Date, Time, User_id, Room_id, Status) VALUES('" + booking.Date + "','" + booking.Time + "','" + booking.User_id + "','" + booking.Room_id + "','unconfirmed')";
            SqlCommand command = new SqlCommand(com.CommandText, con);
            command.ExecuteReader();
            con.Close();

            FetchData();
            return View("User", lbooking);

            //return Json(booking);

        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
