using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ASPNETMVC_Group1.Models;
using System.Data.Entity;
using System.IO;
using System.Collections.Generic;

namespace ASPNETMVC_Group1.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private BeautyGarden db = new BeautyGarden();

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        [ChildActionOnly]
        public new ActionResult Profile()
        {
            var currentUserId = User.Identity.GetUserId();
            var model = db.AspNetUsers.Find(currentUserId);
            return PartialView("Profile", model);
        }

        [ChildActionOnly]
        public ActionResult Avatar()
        {
            return PartialView("Avatar");
        }

        [ChildActionOnly]
        public ActionResult Password()
        {
            return PartialView("Password");
        }

        [ChildActionOnly]
        public ActionResult Order()
        {
            List<DonHangViewModel> model = new List<DonHangViewModel>();
            var currentUserId = User.Identity.GetUserId();
            var lstDonHang = db.DonHangs.Where(s => s.MaKhachHang.Equals(currentUserId)).OrderByDescending(s => s.NgayDat);
            foreach(var item in lstDonHang)
            {
                DonHangViewModel donHang = new DonHangViewModel
                {
                    MaDonHang = item.MaDonHang,
                    NgayDat = item.NgayDat,
                    TrangThai = item.TrangThai,
                    detailDonHangs = new List<DetailDonHang>()
                };
                var chitietdonhangs = db.ChiTietDonHangs.Where(s => s.MaDonHang == item.MaDonHang).ToList();
                foreach(var detail in chitietdonhangs)
                {
                    DetailDonHang ct = new DetailDonHang
                    {
                        MaSanPham = (int)detail.MaSanPham,
                        TenSanPham = db.SanPhams.Where(s => s.MaSanPham == detail.MaSanPham).FirstOrDefault().TenSanPham,
                        DonGia = detail.DonGia,
                        GiamGia = detail.GiamGia,
                        SoLuong = detail.SoLuong,
                        Anh = db.Anhs.Where(s => s.MaSanPham == detail.MaSanPham).FirstOrDefault().LinkAnh
                    };
                    if (db.DanhGias.Any(s => s.MaSanPham == detail.MaSanPham && s.MaDonHang == item.MaDonHang))
                    {
                        ct.DanhGia = db.DanhGias.Where(s => s.MaSanPham == detail.MaSanPham && s.MaDonHang == item.MaDonHang).FirstOrDefault().DanhGia1;
                    }
                    donHang.detailDonHangs.Add(ct);
                }
                model.Add(donHang);
            }
            return PartialView("Order", model);
        }

        [HttpGet]
        public JsonResult HuyDonHang(int MaDonHang)
        {
            var currentUserId = User.Identity.GetUserId();
            var donHang = db.DonHangs.Where(s => s.MaDonHang == MaDonHang && s.MaKhachHang.Equals(currentUserId)).FirstOrDefault();
            if(donHang.TrangThai == 1)
            {
                donHang.TrangThai = 4;
                donHang.ModifiedDate = DateTime.UtcNow.AddHours(7);
                db.Entry(donHang).State = EntityState.Modified;
                db.SaveChanges();
                var chiTietDonHangs = db.ChiTietDonHangs.Where(s => s.MaDonHang == donHang.MaDonHang);
                foreach(var item in chiTietDonHangs)
                {
                    SanPham sp = db.SanPhams.Find(item.MaSanPham);
                    sp.SoLuong += item.SoLuong;
                    if(sp.TinhTrang != 3)
                    {
                        sp.TinhTrang = 1;
                    }
                    db.Entry(sp).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
            return Json(new { success = "Change order status success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DanhGia(int MaDonHang, int MaSanPham, int DanhGia)
        {
            var currentUserId = User.Identity.GetUserId();
            var donHang = db.DonHangs.Where(s => s.MaDonHang == MaDonHang && s.MaKhachHang.Equals(currentUserId)).FirstOrDefault();
            if (donHang.TrangThai == 3)
            {
                if(db.DonHangs.Any(s => s.ChiTietDonHangs.Any(x => x.MaSanPham == MaSanPham)))
                {
                    DanhGia danhGia = db.DanhGias.Where(s => s.MaKhachHang.Equals(currentUserId) && s.MaSanPham == MaSanPham && s.MaDonHang == MaDonHang).FirstOrDefault();
                    if(danhGia != null)
                    {
                        danhGia.DanhGia1 = DanhGia;
                        db.Entry(danhGia).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        danhGia = new DanhGia
                        {
                            MaKhachHang = currentUserId,
                            MaSanPham = MaSanPham,
                            MaDonHang = MaDonHang,
                            DanhGia1 = DanhGia
                        };
                        db.DanhGias.Add(danhGia);
                        db.SaveChanges();
                    }
                }
            }
            return Json(new { success = "Rate success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ChangeProfile(AspNetUser model)
        {
            var currentUserId = User.Identity.GetUserId();
            var user = db.AspNetUsers.Find(currentUserId);
            if(user == null)
            {
                return Json(new { error = "Undefined user" }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                user.Ten = model.Ten;
                user.PhoneNumber = model.PhoneNumber;
                user.GioiTinh = model.GioiTinh;
                user.NgaySinh = model.NgaySinh;
                user.ThanhPho = model.ThanhPho;
                user.QuanHuyen = model.QuanHuyen;
                user.DiaChi = model.DiaChi;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = "Change profile success" }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ChangeAvatar()
        {
            var currentUserId = User.Identity.GetUserId();
            var user = db.AspNetUsers.Find(currentUserId);
            if (user == null)
            {
                return Json(new { error = "Undefined user" }, JsonRequestBehavior.AllowGet);
            }
            if (Request.Files["File"] != null && Request.Files["File"].ContentLength > 0)
            {
                try
                {
                    if(user.Anh != null)
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
                    return Json(new { success = "Change avatar success" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { error = "No files selected." }, JsonRequestBehavior.AllowGet);
            }
        }
        //
        // GET: /Manage/Index
        //public async Task<ActionResult> Index(ManageMessageId? message)
        [Authorize(Roles = "Khách hàng")]
        public ActionResult Index()
        {
            //ViewBag.StatusMessage =
            //    message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
            //    : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
            //    : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
            //    : message == ManageMessageId.Error ? "An error has occurred."
            //    : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
            //    : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
            //    : "";

            //var userId = User.Identity.GetUserId();
            //var model = new IndexViewModel
            //{
            //    HasPassword = HasPassword(),
            //    PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
            //    TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
            //    Logins = await UserManager.GetLoginsAsync(userId),
            //    BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId)
            //};
            var currentUserId = User.Identity.GetUserId();
            var user = db.AspNetUsers.Find(currentUserId);
            var model = new IndexViewModel
            {
                Avatar = user.Anh,
                Ten = user.Ten
            };
            return View(model);
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Your security code is: " + code
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
            // Send an SMS through the SMS provider to verify the phone number
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Failed to verify phone");
            return View(model);
        }

        //
        // POST: /Manage/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        //
        // GET: /Manage/ChangePassword
        //public ActionResult ChangePassword()
        //{
        //    return View();
        //}

        //
        // POST: /Manage/ChangePassword
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
        //    if (result.Succeeded)
        //    {
        //        var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //        if (user != null)
        //        {
        //            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //        }
        //        return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
        //    }
        //    AddErrors(result);
        //    return View(model);
        //}
        [HttpPost]
        public async Task<JsonResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { error = "Change password failed" }, JsonRequestBehavior.AllowGet);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return Json(new { success = "Change password success" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { error1 = "Old password wrong" }, JsonRequestBehavior.AllowGet);
        }
        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

#region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

#endregion
    }
}