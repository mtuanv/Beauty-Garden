using ASPNETMVC_Group1.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ASPNETMVC_Group1.Controllers
{
    [Authorize(Roles = "Admin, Nhân viên")]
    public class QuanLyController : Controller
    {
        private BeautyGarden db = new BeautyGarden();

        [ChildActionOnly]
        public ActionResult LoginPartial()
        {
            var user = db.AspNetUsers.Find(User.Identity.GetUserId());
            return PartialView("LoginPartial", user);
        }

        // GET: QuanLy
        public ActionResult Index()
        {
            ViewBag.ChoXuLy = db.DonHangs.Where(s => s.TrangThai == 1).Count();
            ViewBag.HoanThanh = db.DonHangs.Where(s => s.TrangThai == 3).Count();
            ViewBag.Huy = db.DonHangs.Where(s => s.TrangThai == 4).Count();
            ViewBag.KhachHang = db.AspNetUsers.Where(x => x.AspNetRoles.Select(role => role.Name).Contains("Khách hàng") && !x.LockoutEnabled).Count();
            List<Top3DashboardViewModel> model = new List<Top3DashboardViewModel>();
            var top3 = db.ChiTietDonHangs.Where(s => s.DonHang.TrangThai == 3)
                                         .GroupBy(s => s.MaSanPham)
                                         .Select(s => new
                                         {
                                             Key = s.Key,
                                             Value = s.Sum(x => x.SoLuong),
                                             MaSanPham = s.FirstOrDefault().MaSanPham
                                         })
                                         .OrderByDescending(s => s.Value)
                                         .Take(3);
            foreach (var item in top3)
            {
                Top3DashboardViewModel top = new Top3DashboardViewModel
                {
                    MaSanPham = (int)item.MaSanPham,
                    TenSanPham = db.SanPhams.Find(item.MaSanPham).TenSanPham,
                    SoLuongBan = (int)item.Value,
                    Anh = db.Anhs.Where(s => s.MaSanPham == item.MaSanPham).FirstOrDefault().LinkAnh
                };
                model.Add(top);
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult CountChoXuLy()
        {
            var result = db.DonHangs.Where(s => s.TrangThai == 1).Count();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}