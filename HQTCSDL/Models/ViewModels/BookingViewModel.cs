using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HQTCSDL.Models.ViewModels
{
    public class BookingViewModel
    {
        public int MaPhong { get; set; }
        public string TenPhong { get; set; }
        public DateTime NgayNhan { get; set; }
        public DateTime NgayTra { get; set; }
        public decimal DonGia { get; set; }
    }
}