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
    public class GioHangController : Controller
    {
        private BeautyGarden db = new BeautyGarden();

        // GET: GioHang
        [Authorize]
        public ActionResult Index()
        {
            if(TempData["OrderSuccess"] != null)
            {
                ViewBag.OrderSuccess = TempData["OrderSuccess"].ToString();
            }
            var currentUserId = User.Identity.GetUserId();
            var gioHang = db.GioHangs.Where(s => s.MaKhachHang.Equals(currentUserId)).FirstOrDefault();
            List<CartView> cart = new List<CartView>();
            if(gioHang != null)
            {
                var chiTietGioHangs = db.ChiTietGioHangs.Where(s => s.MaGioHang == gioHang.MaGioHang);
                foreach(var item in chiTietGioHangs)
                {
                    var sanPham = db.SanPhams.Where(s => s.MaSanPham == item.MaSanPham && !s.DeletedDate.HasValue &&s.TinhTrang != 3).FirstOrDefault();
                    if(sanPham != null)
                    {
                        CartView child = new CartView {
                            MaSanPham = sanPham.MaSanPham,
                            TenSanPham = sanPham.TenSanPham,
                            GiaThiTruong = sanPham.GiaThiTruong,
                            GiaBan = sanPham.GiaBan,
                            GiamGia = sanPham.GiamGia,
                            SoLuong = item.SoLuong,
                            SoLuongCon = sanPham.SoLuong,
                            Anh = db.Anhs.Where(s => s.MaSanPham == sanPham.MaSanPham).FirstOrDefault().LinkAnh
                        };
                        if(child.SoLuong > sanPham.SoLuong)
                        {
                            ViewBag.OverQuantity = "Over Quantity";
                        }
                        cart.Add(child);
                    }
                    else
                    {
                        db.ChiTietGioHangs.Remove(item);
                    }
                }
                db.SaveChanges();
            }
            ViewBag.Quantity = cart.Count();
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddCart(int MaSP, int SoLuong)
        {
            var currentUserId = User.Identity.GetUserId();
            var gioHang = db.GioHangs.Where(s => s.MaKhachHang.Equals(currentUserId)).FirstOrDefault();
            if (gioHang == null)
            {
                GioHang cart = new GioHang();
                cart.MaKhachHang = currentUserId;
                db.GioHangs.Add(cart);
                db.SaveChanges();

            }
            gioHang = db.GioHangs.Where(s => s.MaKhachHang.Equals(currentUserId)).FirstOrDefault();
            ChiTietGioHang existProduct = db.ChiTietGioHangs.Where(s => s.MaGioHang == gioHang.MaGioHang && s.MaSanPham == MaSP).FirstOrDefault();
            if(existProduct == null)
            {
                ChiTietGioHang chiTietGioHang = new ChiTietGioHang();
                chiTietGioHang.MaGioHang = gioHang.MaGioHang;
                chiTietGioHang.MaSanPham = MaSP;
                chiTietGioHang.SoLuong = SoLuong;
                db.ChiTietGioHangs.Add(chiTietGioHang);
                db.SaveChanges();
            }
            else
            {
                existProduct.SoLuong += SoLuong;
                db.Entry(existProduct).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteProductInCart(int MaSP)
        {
            var currentUserId = User.Identity.GetUserId();
            var gioHang = db.GioHangs.Where(s => s.MaKhachHang.Equals(currentUserId)).FirstOrDefault();
            if (gioHang == null)
            {
                return RedirectToAction("Index");
            }
            var chiTietGioHang = db.ChiTietGioHangs.Where(s => s.MaSanPham == MaSP).FirstOrDefault();
            if(chiTietGioHang == null)
            {
                return RedirectToAction("Index");
            }
            db.ChiTietGioHangs.Remove(chiTietGioHang);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public ActionResult UpdateProductInCart(int MaSP, int SoLuong)
        {
            var currentUserId = User.Identity.GetUserId();
            var gioHang = db.GioHangs.Where(s => s.MaKhachHang.Equals(currentUserId)).FirstOrDefault();
            if (gioHang == null)
            {
                return RedirectToAction("Index");
            }
            var chiTietGioHang = db.ChiTietGioHangs.Where(s => s.MaSanPham == MaSP).FirstOrDefault();
            if (chiTietGioHang == null)
            {
                return RedirectToAction("Index");
            }
            chiTietGioHang.SoLuong = SoLuong;
            db.Entry(chiTietGioHang).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult CountCart()
        {
            var currentUserId = User.Identity.GetUserId();
            var gioHang = db.GioHangs.Where(s => s.MaKhachHang.Equals(currentUserId)).FirstOrDefault();
            var result = 0;
            if (gioHang != null)
            {
                result = db.ChiTietGioHangs.Where(s => s.MaGioHang == gioHang.MaGioHang).Count();
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }

}