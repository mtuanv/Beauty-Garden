using ASPNETMVC_Group1.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ASPNETMVC_Group1.Controllers
{
    public class ThongTinController : Controller
    {
        private BeautyGarden db = new BeautyGarden();

        // GET: ThongTin
        [Route("ThongTin/{id}")]
        public ActionResult Index(string id, int? page)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            List<string> loai = new List<string> { "Sự kiện khuyến mãi", "Bí quyết làm đẹp", "Review sản phẩm" };
            var result = loai.Where(s => s.Equals(id));
            if (result.Count() == 0 || result == null)
            {
                return HttpNotFound();
            }
            if(page == null) page = 1;
            var tinTucs = db.TinTucs.Where(s => s.LoaiTinTuc.Equals(id) && !s.DeletedDate.HasValue).OrderByDescending(s => s.CreatedDate);
            List<DanhMucTTViewModel> viewModel = new List<DanhMucTTViewModel>();
            foreach (var item in tinTucs)
            {
                DanhMucTTViewModel danhMucTT = new DanhMucTTViewModel();
                danhMucTT.MaTinTuc = item.MaTinTuc;
                danhMucTT.TieuDe = item.TieuDe;
                danhMucTT.LoaiTinTuc = item.LoaiTinTuc;
                danhMucTT.NoiBat = item.NoiBat;
                danhMucTT.NoiDung = item.NoiDung;
                danhMucTT.anh = db.Anhs.Where(s => s.MaTinTuc == item.MaTinTuc).FirstOrDefault();
                danhMucTT.CreatedDate = item.CreatedDate;
                viewModel.Add(danhMucTT);
            }
            ViewBag.Title = id;
            int pageSize = 6;
            int pageNumber = (page ?? 1);
            return View(viewModel.ToPagedList(pageNumber, pageSize));
        }

        [Route("GioiThieu")]
        public ActionResult GioiThieu()
        {
            return View();
        }

        [Route("HeThong")]
        public ActionResult HeThong()
        {
            return View();
        }

        [Route("ChuongTrinh")]
        public ActionResult ChuongTrinh()
        {
            return View();
        }

        [Route("ChinhSach")]
        public ActionResult ChinhSach()
        {
            return View();
        }

        [Route("DieuKhoan")]
        public ActionResult DieuKhoan()
        {
            return View();
        }

        [Route("HuongDanMuaHang")]
        public ActionResult HuongDanMuaHang()
        {
            return View();
        }

        [Route("ThanhToanVanChuyen")]
        public ActionResult ThanhToanVanChuyen()
        {
            return View();
        }

        [Route("ChinhSachDoiTra")]
        public ActionResult ChinhSachDoiTra()
        {
            return View();
        }

        [Route("FAQs")]
        public ActionResult FAQs()
        {
            return View();
        }

        [Route("LienKet")]
        public ActionResult LienKet()
        {
            return View();
        }
    }
}