using ASPNETMVC_Group1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ASPNETMVC_Group1.Controllers
{
    public class HomeController : Controller
    {
        private BeautyGarden db = new BeautyGarden();
        public ActionResult Index()
        {
            HomeViewModel viewModel = new HomeViewModel
            {
                khuyenMais = db.SanPhams.Where(s => s.GiamGia.HasValue && s.GiamGia > 0 && !s.DeletedDate.HasValue).ToList(),
                moiVes = db.SanPhams.Where(s => s.SanPhamTags.Any(x => x.MaTag == 1) && !s.DeletedDate.HasValue).OrderByDescending(s => s.CreatedDate).ToList(),
                banChays = db.SanPhams.Where(s => !s.DeletedDate.HasValue).OrderByDescending(s => s.ChiTietDonHangs.Where(x => x.DonHang.TrangThai == 3).Sum(x => x.SoLuong)).ToList(),
                trangDiems = db.SanPhams.Where(s => s.DanhMuc.DanhMuc2.MaDanhMucCha == 1 && !s.DeletedDate.HasValue).ToList(),
                chamSocDas = db.SanPhams.Where(s => s.DanhMuc.DanhMuc2.MaDanhMucCha == 2 && !s.DeletedDate.HasValue).ToList(),
                tinNoiBats = db.TinTucs.Where(s => s.NoiBat == true && !s.DeletedDate.HasValue).ToList(),
                anhs = db.Anhs.ToList()
            };
            return View(viewModel);
        }

        [Route("LienHe")]
        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SendMailContact(Email email)
        {
            bool result = false;
            if (ModelState.IsValid)
                result = SendEmail("beautygardenhaui@gmail.com", "Liên hệ", "<p><b>Tên:</b> " + email.Ten + "<br/><b>Số điện thoại:</b> " + email.SDT + "<br/><b>Email:</b> " + email.Mail + "<br/><b>Địa chỉ:</b> " + email.DiaChi + "<br/><b>Nội dung:</b> " + email.NoiDung + "</p>");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SendMailGetEvent(EmailEvent email)
        {
            bool result = false;
            if (ModelState.IsValid)
                result = SendEmail("beautygardenhaui@gmail.com", "Đăng ký nhận thông tin khuyến mãi", "<p><b>Email:</b> " + email.Mail + "</p>");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public bool SendEmail(string toEmail, string subject, string emailBody)
        {
            try
            {
                string senderEmail = System.Configuration.ConfigurationManager.AppSettings["MailFrom"].ToString();
                string senderPassword = System.Configuration.ConfigurationManager.AppSettings["Password"].ToString();
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.Timeout = 100000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(senderEmail, senderPassword);
                MailMessage mailMessage = new MailMessage(senderEmail, toEmail, subject, emailBody);
                mailMessage.IsBodyHtml = true;
                mailMessage.BodyEncoding = UTF8Encoding.UTF8;
                client.Send(mailMessage);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}