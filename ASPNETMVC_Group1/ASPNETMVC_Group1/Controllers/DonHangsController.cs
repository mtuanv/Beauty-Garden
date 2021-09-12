using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ASPNETMVC_Group1.Models;
using PagedList;

namespace ASPNETMVC_Group1.Controllers
{
    [Authorize(Roles = "Admin, Nhân viên")]
    [RoutePrefix("QuanLy/DonHang")]
    public class DonHangsController : Controller
    {
        private BeautyGarden db = new BeautyGarden();

        // GET: DonHangs
        [Route]
        public ActionResult Index(string currentFilter, string searchString, int? page)
        {
            if (TempData["XNSuccess"] != null)
            {
                ViewBag.XNSuccess = TempData["XNSuccess"].ToString();
            }
            if (TempData["HuySuccess"] != null)
            {
                ViewBag.HuySuccess = TempData["HuySuccess"].ToString();
            }
            if (TempData["HTSuccess"] != null)
            {
                ViewBag.HTSuccess = TempData["HTSuccess"].ToString();
            }
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            var donHangs = db.DonHangs.Include(d => d.AspNetUser);
            if (!String.IsNullOrEmpty(searchString))
            {
                donHangs = donHangs.Where(s => s.MaDonHang.ToString().Contains(searchString) || s.Ten.Contains(searchString) || s.ThanhPho.Contains(searchString) || s.QuanHuyen.Contains(searchString) || s.DiaChi.Contains(searchString));
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(donHangs.OrderByDescending(s => s.NgayDat).ToPagedList(pageNumber, pageSize));
        }

        // GET: DonHangs/Details/5
        [Route("Details/{id:int}")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            DonHang dh = db.DonHangs.Find(id);
            if (dh == null || dh.TrangThai == 4)
            {
                return HttpNotFound();
            }
            DonHangViewModel donHang = new DonHangViewModel
            {
                MaDonHang = dh.MaDonHang,
                NgayDat = dh.NgayDat,
                TrangThai = dh.TrangThai,
                Ten = dh.Ten,
                DiaChi = dh.DiaChi,
                ThanhPho = dh.ThanhPho,
                QuanHuyen = dh.QuanHuyen,
                SDT = dh.SDT,
                detailDonHangs = new List<DetailDonHang>()
            };
            var chitietdonhangs = db.ChiTietDonHangs.Where(s => s.MaDonHang == dh.MaDonHang).ToList();
            foreach (var detail in chitietdonhangs)
            {
                DetailDonHang ct = new DetailDonHang
                {
                    TenSanPham = db.SanPhams.Where(s => s.MaSanPham == detail.MaSanPham).FirstOrDefault().TenSanPham,
                    DonGia = detail.DonGia,
                    GiamGia = detail.GiamGia,
                    SoLuong = detail.SoLuong,
                };
                donHang.detailDonHangs.Add(ct);
            }
            return View(donHang);
        }

        [Route("In/{id:int}")]
        public ActionResult In(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            DonHang dh = db.DonHangs.Find(id);
            if (dh == null || dh.TrangThai == 4)
            {
                return HttpNotFound();
            }
            DonHangViewModel donHang = new DonHangViewModel
            {
                MaDonHang = dh.MaDonHang,
                NgayDat = dh.NgayDat,
                TrangThai = dh.TrangThai,
                Ten = dh.Ten,
                DiaChi = dh.DiaChi,
                ThanhPho = dh.ThanhPho,
                QuanHuyen = dh.QuanHuyen,
                SDT = dh.SDT,
                detailDonHangs = new List<DetailDonHang>()
            };
            var chitietdonhangs = db.ChiTietDonHangs.Where(s => s.MaDonHang == dh.MaDonHang).ToList();
            foreach (var detail in chitietdonhangs)
            {
                DetailDonHang ct = new DetailDonHang
                {
                    TenSanPham = db.SanPhams.Where(s => s.MaSanPham == detail.MaSanPham).FirstOrDefault().TenSanPham,
                    DonGia = detail.DonGia,
                    GiamGia = detail.GiamGia,
                    SoLuong = detail.SoLuong,
                };
                donHang.detailDonHangs.Add(ct);
            }
            return View(donHang);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("XacNhan/{id:int}")]
        public ActionResult XacNhan(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            DonHang dh = db.DonHangs.Find(id);
            if (dh == null)
            {
                return RedirectToAction("Index");
            }
            if(dh.TrangThai == 1)
            {
                dh.TrangThai = 2;
                dh.ModifiedDate = DateTime.UtcNow.AddHours(7);
            }
            db.Entry(dh).State = EntityState.Modified;
            db.SaveChanges();
            TempData["XNSuccess"] = "Success";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Huy/{id:int}")]
        public ActionResult Huy(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            DonHang dh = db.DonHangs.Find(id);
            if (dh == null)
            {
                return RedirectToAction("Index");
            }
            if(dh.TrangThai == 1 || dh.TrangThai == 2)
            {
                dh.TrangThai = 4;
                dh.ModifiedDate = DateTime.UtcNow.AddHours(7);
                db.Entry(dh).State = EntityState.Modified;
                db.SaveChanges();
                var chiTietDonHangs = db.ChiTietDonHangs.Where(s => s.MaDonHang == dh.MaDonHang);
                foreach (var item in chiTietDonHangs)
                {
                    SanPham sp = db.SanPhams.Find(item.MaSanPham);
                    sp.SoLuong += item.SoLuong;
                    if (sp.TinhTrang != 3)
                    {
                        sp.TinhTrang = 1;
                    }
                    db.Entry(sp).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
            TempData["HuySuccess"] = "Success";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("HoanThanh/{id:int}")]
        public ActionResult HoanThanh(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            DonHang dh = db.DonHangs.Find(id);
            if (dh == null)
            {
                return RedirectToAction("Index");
            }
            if (dh.TrangThai == 2)
            {
                dh.TrangThai = 3;
                dh.ModifiedDate = DateTime.UtcNow.AddHours(7);
            }
            db.Entry(dh).State = EntityState.Modified;
            db.SaveChanges();
            TempData["HTSuccess"] = "Success";
            return RedirectToAction("Index");
        }
    }
}
