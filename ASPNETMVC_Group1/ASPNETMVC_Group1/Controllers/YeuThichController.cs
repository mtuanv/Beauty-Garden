using ASPNETMVC_Group1.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASPNETMVC_Group1.Controllers
{
    public class YeuThichController : Controller
    {
        private BeautyGarden db = new BeautyGarden();

        // GET: YeuThich
        [Authorize]
        public ActionResult Index()
        {
            var currentUserId = User.Identity.GetUserId();
            var sanPhams = db.SanPhams.Where(s => !s.DeletedDate.HasValue && s.YeuThiches.Any(x => x.MaSanPham == s.MaSanPham && x.MaKhachHang.Equals(currentUserId)));
            List<DanhMucSPViewModel> viewModel = new List<DanhMucSPViewModel>();
            foreach (var item in sanPhams)
            {
                DanhMucSPViewModel danhMucSP = new DanhMucSPViewModel();
                danhMucSP.MaSanPham = item.MaSanPham;
                danhMucSP.TenSanPham = item.TenSanPham;
                danhMucSP.GiaThiTruong = item.GiaThiTruong;
                danhMucSP.GiaBan = item.GiaBan;
                danhMucSP.GiamGia = item.GiamGia;
                danhMucSP.anh = db.Anhs.Where(s => s.MaSanPham == item.MaSanPham).FirstOrDefault();
                viewModel.Add(danhMucSP);
            }
            return View(viewModel);
        }

        [HttpPost]
        public JsonResult AddOrDeleteYeuThich(int MaSP)
        {
            string result = "";
            var currentUserId = User.Identity.GetUserId();
            var user = db.AspNetUsers.Find(currentUserId);
            if (user == null)
            {
                return Json(new
                {
                    redirectUrl = Url.Action("Login","Account"),
                    isRedirect = true
                });
            }
            YeuThich yeuThich = db.YeuThiches.Where(s => s.MaKhachHang.Equals(currentUserId) && s.MaSanPham == MaSP).FirstOrDefault();
            if (yeuThich == null)
            {
                YeuThich y = new YeuThich();
                y.MaKhachHang = currentUserId;
                y.MaSanPham = MaSP;
                db.YeuThiches.Add(y);
                db.SaveChanges();
                result = "Thêm yêu thích thành công";
            }
            else
            {
                db.YeuThiches.Remove(yeuThich);
                db.SaveChanges();
                result = "Xoá yêu thích thành công";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CountYeuThich()
        {
            var currentUserId = User.Identity.GetUserId();
            var result = db.YeuThiches.Where(s => s.MaKhachHang.Equals(currentUserId)).Count();
            var user = db.AspNetUsers.Find(currentUserId);
            if (user == null)
            {
                result = 0;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}