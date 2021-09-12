using ASPNETMVC_Group1.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASPNETMVC_Group1.Controllers
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("QuanLy/NhapHang")]
    public class NhapHangController : Controller
    {
        private BeautyGarden db = new BeautyGarden();

        // GET: NhapHang
        [Route]
        public ActionResult Index(string searchString, int? page)
        {
            if (TempData["NhapSuccess"] != null)
            {
                ViewBag.NhapSuccess = TempData["NhapSuccess"].ToString();
            }
            if (TempData["NhapError"] != null)
            {
                ViewBag.NhapError = TempData["NhapError"].ToString();
            }
            if (page == 0) page = 1;
            ViewBag.SearchString = searchString;
            var sanPhams = db.SanPhams.Where(s => !s.DeletedDate.HasValue && !s.DanhMuc.DeletedDate.HasValue && !s.NhanHieu.DeletedDate.HasValue);
            if (!String.IsNullOrEmpty(searchString))
            {
                sanPhams = sanPhams.Where(s => s.TenSanPham.Contains(searchString) || s.DanhMuc.TenDanhMuc.Contains(searchString) || s.NhanHieu.TenNhanHieu.Contains(searchString) || s.CongDung.Contains(searchString) || s.MoTa.Contains(searchString) || s.SanPhamTags.Any(x => x.Tag.TenTag.Contains(searchString)));
            }
            sanPhams = sanPhams.OrderBy(s => s.TenSanPham);
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            ViewBag.Page = page;
            return View(sanPhams.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Nhap/{id:int}")]
        public ActionResult Nhap(NhapHangViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    SanPham sanPham = db.SanPhams.Find(model.MaSanPham);
                    var sTime = ResetTimeToStartOfDay(DateTime.UtcNow.AddHours(7));
                    var eTime = ResetTimeToEndOfDay(DateTime.UtcNow.AddHours(7));
                    NhapHang nhap = db.NhapHangs.Where(s => s.CreatedDate >= sTime && s.CreatedDate <= eTime).FirstOrDefault();
                    if (nhap == null)
                    {
                        NhapHang nhapHang = new NhapHang();
                        nhapHang.CreatedDate = DateTime.UtcNow.AddHours(7);
                        db.NhapHangs.Add(nhapHang);
                        db.SaveChanges();
                        nhap = db.NhapHangs.Where(s => s.CreatedDate >= sTime && s.CreatedDate <= eTime).FirstOrDefault();
                    }
                    ChiTietNhapHang ctnh = db.ChiTietNhapHangs.Where(s => s.MaNhapHang == nhap.MaNhapHang && s.MaSanPham == model.MaSanPham).FirstOrDefault();
                    if (ctnh == null)
                    {
                        ChiTietNhapHang chiTietNhapHang = new ChiTietNhapHang();
                        chiTietNhapHang.MaSanPham = model.MaSanPham;
                        chiTietNhapHang.MaNhapHang = nhap.MaNhapHang;
                        chiTietNhapHang.GiaNhap = model.GiaNhap;
                        chiTietNhapHang.SoLuong = model.SoLuong;
                        db.ChiTietNhapHangs.Add(chiTietNhapHang);
                        db.SaveChanges();
                    }
                    else
                    {
                        ctnh.SoLuong = ctnh.SoLuong + model.SoLuong;
                        db.Entry(ctnh).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    if (sanPham.SoLuong == null)
                    {
                        sanPham.SoLuong = model.SoLuong;
                    }
                    else
                    {
                        sanPham.SoLuong = sanPham.SoLuong + model.SoLuong;
                    }
                    sanPham.TinhTrang = 1;
                    db.Entry(sanPham).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["NhapSuccess"] = "Success";
                    return RedirectToAction("Index", new { page = model.page, searchString = model.search });
                }
                catch (Exception ex)
                {
                    TempData["NhapError"] = ex.Message;
                    return RedirectToAction("Index", new { page = model.page, searchString = model.search });
                }
            }
            else
            {
                TempData["NhapError"] = "Error";
                return RedirectToAction("Index", new { page = model.page, searchString = model.search });
            }
        }

        private DateTime ResetTimeToStartOfDay(DateTime dateTime)
        {
            return new DateTime(
             dateTime.Year,
             dateTime.Month,
             dateTime.Day,
             0, 0, 0, 0);
        }
        private DateTime ResetTimeToEndOfDay(DateTime dateTime)
        {
            return new DateTime(
             dateTime.Year,
             dateTime.Month,
             dateTime.Day,
             23, 59, 59, 999);
        }
    }
}