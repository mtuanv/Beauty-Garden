using ASPNETMVC_Group1.Models;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASPNETMVC_Group1.Controllers
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("QuanLy/KhachHang")]
    public class KhachHangController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private BeautyGarden db = new BeautyGarden();

        public KhachHangController()
        {
        }

        public KhachHangController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: KhachHang
        [Route]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (TempData["LockoutSuccess"] != null)
            {
                ViewBag.LockoutSuccess = TempData["LockoutSuccess"].ToString();
            }
            if (TempData["LockoutError"] != null)
            {
                ViewBag.LockoutError = TempData["LockoutError"].ToString();
            }
            if (TempData["UnlockSuccess"] != null)
            {
                ViewBag.UnlockSuccess = TempData["UnlockSuccess"].ToString();
            }
            if (TempData["UnlockError"] != null)
            {
                ViewBag.UnlockError = TempData["UnlockError"].ToString();
            }
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SapTheoTen = String.IsNullOrEmpty(sortOrder) ? "ten_desc" : "";
            ViewBag.SapTheoDon = sortOrder == "order" ? "order_desc" : "order";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            var customers = db.AspNetUsers.Where(x => x.AspNetRoles.Select(role => role.Name).Contains("Khách hàng")).ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                customers = customers.Where(s => s.Ten.Contains(searchString) || s.PhoneNumber.Contains(searchString)
                || s.Email.Contains(searchString) || s.GioiTinh.Contains(searchString) || s.NgaySinh.ToString().Contains(searchString)
                || s.ThanhPho.Contains(searchString) || s.QuanHuyen.Contains(searchString) || s.DiaChi.Contains(searchString)).ToList();
            }
            bool flag = false;
            switch (sortOrder)
            {
                case "ten_desc":
                    foreach (var c in customers)
                    {
                        var ten = c.Ten.Substring(c.Ten.LastIndexOf(' ') + 1, c.Ten.Length - c.Ten.LastIndexOf(' ') - 1);
                        var ho = c.Ten.Substring(0, c.Ten.LastIndexOf(' '));
                        c.Ten = ten + " " + ho;
                    }
                    customers = customers.AsEnumerable().OrderByDescending(x => x.Ten, StringComparer.Create(new CultureInfo("vi-VN"), false)).AsQueryable().ToList();
                    flag = true;
                    break;
                case "order":
                    customers = customers.OrderBy(x => x.DonHangs.Where(y => y.TrangThai == 3).Count()).ToList();
                    break;
                case "order_desc":
                    customers = customers.OrderByDescending(x => x.DonHangs.Where(y => y.TrangThai == 3).Count()).ToList();
                    break;
                default:
                    foreach (var c in customers)
                    {
                        var ten = c.Ten.Substring(c.Ten.LastIndexOf(' ') + 1, c.Ten.Length - c.Ten.LastIndexOf(' ') - 1);
                        var ho = c.Ten.Substring(0, c.Ten.LastIndexOf(' '));
                        c.Ten = ten + " " + ho;
                    }
                    customers = customers.AsEnumerable().OrderBy(x => x.Ten, StringComparer.Create(new CultureInfo("vi-VN"), false)).AsQueryable().ToList();
                    flag = true;
                    break;
            }
            if (flag)
            {
                foreach (var c in customers)
                {
                    var ho = c.Ten.Substring(c.Ten.IndexOf(' ') + 1, c.Ten.Length - c.Ten.IndexOf(' ') - 1);
                    var ten = c.Ten.Substring(0, c.Ten.IndexOf(' '));
                    c.Ten = ho + " " + ten;
                }
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(customers.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost, ActionName("Lockout")]
        [ValidateAntiForgeryToken]
        [Route("Lockout/{id}")]
        public ActionResult Lockout(string id)
        {
            AspNetUser user = db.AspNetUsers.Find(id);
            try
            {
                user.LockoutEnabled = true;
                user.LockoutEndDateUtc = DateTime.MaxValue;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                TempData["LockoutSuccess"] = "Success";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["LockoutError"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost, ActionName("Unlock")]
        [ValidateAntiForgeryToken]
        [Route("Unlock/{id}")]
        public ActionResult Unlock(string id)
        {
            AspNetUser user = db.AspNetUsers.Find(id);
            try
            {
                user.LockoutEnabled = false;
                user.LockoutEndDateUtc = null;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                TempData["UnlockSuccess"] = "Success";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["UnlockError"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}