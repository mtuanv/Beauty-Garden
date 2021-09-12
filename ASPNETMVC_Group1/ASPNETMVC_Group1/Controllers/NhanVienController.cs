using ASPNETMVC_Group1.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ASPNETMVC_Group1.Controllers
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("QuanLy/NhanVien")]
    public class NhanVienController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private BeautyGarden db = new BeautyGarden();

        public NhanVienController()
        {
        }

        public NhanVienController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: NhanVien
        [Route]
        public ActionResult Index()
        {
            if (TempData["CreateSuccess"] != null)
            {
                ViewBag.CreateSuccess = TempData["CreateSuccess"].ToString();
            }
            if (TempData["EditSuccess"] != null)
            {
                ViewBag.EditSuccess = TempData["EditSuccess"].ToString();
            }
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
            if (TempData["ChangePassSuccess"] != null)
            {
                ViewBag.ChangePassSuccess = TempData["ChangePassSuccess"].ToString();
            }
            if (TempData["ChangeAvatarSuccess"] != null)
            {
                ViewBag.ChangeAvatarSuccess = TempData["ChangeAvatarSuccess"].ToString();
            }
            var allusers = db.AspNetUsers.ToList();
            var staffs = allusers.Where(x => x.AspNetRoles.Select(role => role.Name).Contains("Nhân viên")).ToList();
            var admins = allusers.Where(x => x.AspNetRoles.Select(role => role.Name).Contains("Admin")).ToList();
            var model = new GroupedUserViewModel { Staffs = staffs, Admins = admins };
            return View(model);
        }

        [Route("Create")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<ActionResult> Create(RegisterViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        UserManager.AddToRole(user.Id, "Nhân viên");
                        AspNetUser user1 = db.AspNetUsers.Find(user.Id);
                        user1.Ten = model.Ten;
                        user1.ThanhPho = model.ThanhPho;
                        user1.QuanHuyen = model.QuanHuyen;
                        user1.DiaChi = model.DiaChi;
                        user1.PhoneNumber = model.PhoneNumber;
                        user1.GioiTinh = model.GioiTinh;
                        user1.NgaySinh = model.NgaySinh;
                        user1.LockoutEnabled = false;
                        user1.LockoutEndDateUtc = null;
                        user1.EmailConfirmed = true;
                        db.Entry(user1).State = EntityState.Modified;
                        db.SaveChanges();
                        TempData["CreateSuccess"] = "Success";
                        return RedirectToAction("Index");
                    }
                    AddErrors(result);
                }
                ViewBag.Error = "Error";
                return View(model);
            }
            catch(Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(model);
            }
        }

        [Route("Edit/{id}")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser user = db.AspNetUsers.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit/{id}")]
        public ActionResult Edit(AspNetUser model)
        {
            var user = db.AspNetUsers.AsNoTracking().Where(s => s.Id.Equals(model.Id)).FirstOrDefault();
            model.UserName = user.UserName;
            model.Email = user.Email;
            model.EmailConfirmed = user.EmailConfirmed;
            model.Anh = user.Anh;
            model.LockoutEnabled = user.LockoutEnabled;
            model.LockoutEndDateUtc = user.LockoutEndDateUtc;
            model.PasswordHash = user.PasswordHash;
            model.SecurityStamp = user.SecurityStamp;
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["EditSuccess"] = "Success";
                    return RedirectToAction("Index");
                }
                ViewBag.Error = "Error";
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(model);
            }
        }

        [Route("Avatar/{id}")]
        public ActionResult Avatar(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser user = db.AspNetUsers.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Avatar/{id}")]
        public ActionResult Avatar(AspNetUser model)
        {
            var user = db.AspNetUsers.Find(model.Id);
            if (user == null)
            {
                ViewBag.Error = "Error";
                return View(model);
            }
            if (Request.Files["File"] != null && Request.Files["File"].ContentLength > 0)
            {
                try
                {
                    if (user.Anh != null)
                    {
                        string oldPath = Server.MapPath("~/wwwroot/Avatars/" + user.Anh);
                        System.IO.File.Delete(oldPath);
                    }
                    var f = Request.Files["File"];
                    FileInfo fi = new FileInfo(f.FileName);
                    var newfilename = "Image_" + DateTime.UtcNow.ToString("yyyyMMddHHmmssfff") + fi.Extension;
                    string UploadPath = Server.MapPath("~/wwwroot/Avatars/" + newfilename);
                    f.SaveAs(UploadPath);
                    user.Anh = newfilename;
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["ChangeAvatarSuccess"] = "Success";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;
                    return View(model);
                }
            }
            else
            {
                ViewBag.Error = "Error";
                return View(model);
            }
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

        [Route("Password/{id}")]
        public ActionResult Password(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.Id = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Password/{id}")]
        public async Task<ActionResult> Password(ResetPasswordStaffViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Error";
                return View(model);
            }
            try
            {
                var token = await UserManager.GeneratePasswordResetTokenAsync(model.Id);
                var result = await UserManager.ResetPasswordAsync(model.Id, token, model.NewPassword);
                if (result.Succeeded)
                {
                    TempData["ChangePassSuccess"] = "Success";
                    return RedirectToAction("Index");
                }
                ViewBag.Error = "Error";
                AddErrors(result);
                return View();
            }
            catch(Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                if (error.Contains("Email") && error.Contains("is already taken"))
                {
                    var changeLanguage = Regex.Replace(error, "is already taken", "đã được sử dụng");
                    ModelState.AddModelError("", changeLanguage);
                }
                else if (error.Contains("Name") && error.Contains("is already taken"))
                {
                    continue;
                }
                else if (error.Contains("Passwords must have"))
                {
                    ModelState.AddModelError("", "Mật khẩu phải có ít nhất một ký tự không phải chữ cái hoặc chữ số. Mật khẩu phải có ít nhất một chữ thường ('a' - 'z'). Mật khẩu phải có ít nhất một chữ hoa ('A' - 'Z').");
                }
                else
                {
                    ModelState.AddModelError("", error);
                }
            }
        }
    }
}