using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreCookieAuthentication.Models
{
    public class Booking
    {
        public string id { get; set; }
        public string Date { get; set; }

        public string Time { get; set; }
        public string User_id { get; set; }

        public string Room_id { get; set; }

        public string Status { get; set; }
    }
}
