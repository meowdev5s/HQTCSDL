using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HQTCSDL.Models.ViewModels
{
    public class RoomListItemViewModel
    {
        public int MaPhong { get; set; }
        public string TenPhong { get; set; }
        public string TenLoai { get; set; }
        public decimal Gia { get; set; }
        public int SoNguoiToiDa { get; set; }
        public string TrangThai { get; set; }
        public string AnhDaiDien { get; set; }
    }

    public class RoomListViewModel
    {
        public List<RoomListItemViewModel> Rooms { get; set; }
        public List<LoaiPhong> LoaiPhongs { get; set; }
    }
}