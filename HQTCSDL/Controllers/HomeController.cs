using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HQTCSDL.Models;

namespace HQTCSDL.Controllers
{
    public class HomeController : Controller
    {
        private readonly QL_KhachSanEntities_ db = new QL_KhachSanEntities_();
        public ActionResult Index()
        {
            // Lấy list loại phòng
            var loaiPhongs = db.LoaiPhong.ToList();

            // Lấy ảnh đại diện cho mỗi loại phòng (join 2 bước: LoaiPhong → Phong → HinhAnhPhong)
            var anhLoaiPhong = (
                from lp in db.LoaiPhong
                join p in db.Phong on lp.MaLoai equals p.MaLoai
                join h in db.HinhAnhPhong on p.MaPhong equals h.MaPhong
                group h by lp.MaLoai into g
                select new
                {
                    MaLoai = g.Key,
                    Anh = g.Select(x => x.DuongDan).FirstOrDefault()
                }
            ).ToDictionary(x => x.MaLoai, x => x.Anh);

            ViewBag.LoaiPhongs = loaiPhongs; 
            ViewBag.AnhLoaiPhong = anhLoaiPhong;  // dict lưu ảnh

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}