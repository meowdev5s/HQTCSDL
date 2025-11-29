using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HQTCSDL.Models;
using HQTCSDL.Models.ViewModels;
using System.Data.Entity;

namespace HQTCSDL.Controllers
{
    public class RoomController : Controller
    {
        private readonly QL_KhachSanEntities_ db = new QL_KhachSanEntities_();

        public ActionResult Index(int? loai, int? songuoi, int? nguoilon, int? treem, string ngayden, string ngaydi)
        {
            // Gộp số người
            if (nguoilon.HasValue || treem.HasValue)
            {
                songuoi = (nguoilon ?? 0) + (treem ?? 0);
            }

            // --- Parse ngày ---
            DateTime today = DateTime.Today;
            DateTime? dDen = null, dDi = null;
            bool hasDateError = false;

            DateTime tmp;
            if (!string.IsNullOrWhiteSpace(ngayden) && DateTime.TryParse(ngayden, out tmp))
                dDen = tmp.Date;

            if (!string.IsNullOrWhiteSpace(ngaydi) && DateTime.TryParse(ngaydi, out tmp))
                dDi = tmp.Date;

            // --- Validate ngày ---
            if (dDen.HasValue && dDen.Value < today)
            {
                ViewBag.DateError = "Ngày đến không được nhỏ hơn ngày hiện tại.";
                hasDateError = true;
            }
            else if (dDi.HasValue && dDi.Value <= today)
            {
                ViewBag.DateError = "Ngày đi phải lớn hơn ngày hiện tại.";
                hasDateError = true;
            }
            else if (dDen.HasValue && dDi.HasValue && dDi.Value <= dDen.Value)
            {
                ViewBag.DateError = "Ngày đi phải lớn hơn ngày đến.";
                hasDateError = true;
            }

            // Query cơ bản
            var query = db.Phong
                .Include(p => p.LoaiPhong)
                .Include(p => p.HinhAnhPhong)
                .Where(p => p.TrangThai == "Trống");

            if (loai.HasValue)
                query = query.Where(p => p.MaLoai == loai.Value);

            if (songuoi.HasValue)
                query = query.Where(p => p.LoaiPhong.SoNguoiToiDa >= songuoi.Value);

            // --- LỌC THEO NGÀY: loại phòng trùng khoảng thời gian ---
            if (!hasDateError && dDen.HasValue && dDi.HasValue)
            {
                // Phòng đã được đặt trong khoảng [dDen, dDi)
                var phongDaDat = from ct in db.ChiTietDatPhong
                                 join dp in db.DatPhong on ct.MaDatPhong equals dp.MaDatPhong
                                 where dp.TrangThai == "Chờ xác nhận"
                                    || dp.TrangThai == "Đã xác nhận"
                                    // Trùng khoảng: NOT (NgayTra <= dDen OR NgayNhan >= dDi)
                                    && !(DbFunctions.TruncateTime(dp.NgayTra) <= dDen.Value
                                         || DbFunctions.TruncateTime(dp.NgayNhan) >= dDi.Value)
                                 select ct.MaPhong;

                query = query.Where(p => !phongDaDat.Contains(p.MaPhong));
            }

            var rooms = hasDateError
                ? new List<RoomListItemViewModel>() // nếu ngày sai: không trả phòng
                : query.Select(p => new RoomListItemViewModel
                {
                    MaPhong = p.MaPhong,
                    TenPhong = p.TenPhong,
                    TenLoai = p.LoaiPhong.TenLoai,
                    Gia = p.LoaiPhong.Gia ?? 0,
                    SoNguoiToiDa = p.LoaiPhong.SoNguoiToiDa ?? 0,
                    TrangThai = p.TrangThai,
                    AnhDaiDien = p.HinhAnhPhong
                                     .Select(h => h.DuongDan)
                                     .FirstOrDefault()
                }).ToList();

            var vm = new RoomListViewModel
            {
                Rooms = rooms,
                LoaiPhongs = db.LoaiPhong.ToList()
            };

            return View(vm);
        }

        public ActionResult Details(int id)
        {
            var phong = db.Phong
                .Include(p => p.LoaiPhong)
                .Include(p => p.HinhAnhPhong)
                .Include(p => p.DanhGia.Select(d => d.KhachHang))
                .SingleOrDefault(p => p.MaPhong == id);

            if (phong == null) return HttpNotFound();

            return View(phong);
        }
    }
}