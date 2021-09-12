using ASPNETMVC_Group1.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASPNETMVC_Group1.Controllers
{
    [Authorize]
    public class ThanhToanController : Controller
    {
        private BeautyGarden db = new BeautyGarden();

        // GET: ThanhToan
        public ActionResult Index()
        {
            var currentUserId = User.Identity.GetUserId();
            var user = db.AspNetUsers.Find(currentUserId);
            var gioHang = db.GioHangs.Where(s => s.MaKhachHang.Equals(currentUserId)).FirstOrDefault();
            ThanhToanView model = new ThanhToanView
            {
                HoTen = user.Ten,
                SDT = user.PhoneNumber,
                ThanhPho = user.ThanhPho,
                QuanHuyen = user.QuanHuyen,
                DiaChi = user.DiaChi,
                cart = new List<CartView>()
            };
            if (gioHang != null)
            {
                var chiTietGioHangs = db.ChiTietGioHangs.Where(s => s.MaGioHang == gioHang.MaGioHang);
                if (chiTietGioHangs.Count() == 0 || chiTietGioHangs == null)
                {
                    return RedirectToAction("Index", "GioHang");
                }
                foreach (var item in chiTietGioHangs)
                {
                    var sanPham = db.SanPhams.Where(s => s.MaSanPham == item.MaSanPham && !s.DeletedDate.HasValue && s.TinhTrang != 3).FirstOrDefault();
                    if (sanPham != null)
                    {
                        CartView child = new CartView
                        {
                            MaSanPham = sanPham.MaSanPham,
                            TenSanPham = sanPham.TenSanPham,
                            GiaThiTruong = sanPham.GiaThiTruong,
                            GiaBan = sanPham.GiaBan,
                            GiamGia = sanPham.GiamGia,
                            SoLuong = item.SoLuong,
                            SoLuongCon = sanPham.SoLuong,
                            Anh = db.Anhs.Where(s => s.MaSanPham == sanPham.MaSanPham).FirstOrDefault().LinkAnh
                        };
                        if (child.SoLuong > sanPham.SoLuong)
                        {
                            ViewBag.OverQuantity = "Over Quantity";
                        }
                        model.cart.Add(child);
                    }
                    else
                    {
                        db.ChiTietGioHangs.Remove(item);
                    }
                }
                db.SaveChanges();
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "GioHang");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ThanhToanView model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var currentUserId = User.Identity.GetUserId();
                    var gioHang = db.GioHangs.Where(s => s.MaKhachHang.Equals(currentUserId)).FirstOrDefault();
                    var chiTietGioHangs = db.ChiTietGioHangs.Where(s => s.MaGioHang == gioHang.MaGioHang);
                    bool flag = true;
                    foreach (var item in chiTietGioHangs)
                    {
                        var sanPham = db.SanPhams.Where(s => s.MaSanPham == item.MaSanPham && !s.DeletedDate.HasValue && s.TinhTrang != 3).FirstOrDefault();
                        if (item.SoLuong > sanPham.SoLuong)
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        return RedirectToAction("Index");
                    }
                    DonHang donHang = new DonHang
                    {
                        MaKhachHang = currentUserId,
                        NgayDat = DateTime.UtcNow.AddHours(7),
                        Ten = model.HoTen,
                        ThanhPho = model.ThanhPho,
                        QuanHuyen = model.QuanHuyen,
                        DiaChi = model.DiaChi,
                        SDT = model.SDT,
                        TrangThai = 1,
                        GhiChu = model.GhiChu,
                        CreatedDate = DateTime.UtcNow.AddHours(7)
                    };
                    db.DonHangs.Add(donHang);
                    db.SaveChanges();
                    foreach (var item in chiTietGioHangs)
                    {
                        var sanPham = db.SanPhams.Where(s => s.MaSanPham == item.MaSanPham && !s.DeletedDate.HasValue).FirstOrDefault();
                        if (sanPham != null)
                        {
                            ChiTietDonHang chiTietDonHang = new ChiTietDonHang
                            {
                                MaDonHang = donHang.MaDonHang,
                                MaSanPham = item.MaSanPham,
                                SoLuong = item.SoLuong,
                                DonGia = sanPham.GiaBan
                            };
                            if(sanPham.GiamGia != null || sanPham.GiamGia != 0)
                            {
                                chiTietDonHang.GiamGia = sanPham.GiamGia;
                            }
                            db.ChiTietDonHangs.Add(chiTietDonHang);
                            sanPham.SoLuong -= chiTietDonHang.SoLuong;
                            if(sanPham.SoLuong == 0)
                            {
                                sanPham.TinhTrang = 2;
                            }
                            db.Entry(sanPham).State = EntityState.Modified;
                            db.ChiTietGioHangs.Remove(item);
                        }
                        else
                        {
                            db.ChiTietGioHangs.Remove(item);
                        }
                    }
                    db.SaveChanges();
                    TempData["OrderSuccess"] = "Success";
                    return RedirectToAction("Index", "GioHang");
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}