using ASPNETMVC_Group1.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASPNETMVC_Group1.Controllers
{
    public class TimKiemController : Controller
    {
        private BeautyGarden db = new BeautyGarden();

        // GET: TimKiem
        public ActionResult Index(string keyword, int? type, int? page)
        {
            if (type == null || ( type != 0 && type != 1)) type = 0;
            ViewBag.CurrentKeyWord = keyword;
            ViewBag.CurrentType = type;
            if(type == 0)
            {
                var sanPhams = db.SanPhams.Where(s => !s.DeletedDate.HasValue);
                if (!String.IsNullOrEmpty(keyword))
                {
                    sanPhams = sanPhams.Where(s => s.TenSanPham.Contains(keyword) || s.DanhMuc.TenDanhMuc.Contains(keyword) || s.NhanHieu.TenNhanHieu.Contains(keyword) || s.CongDung.Contains(keyword) || s.MoTa.Contains(keyword) || s.SanPhamTags.Any(x => x.Tag.TenTag.Contains(keyword)));
                }
                List<SearchView> viewModel = new List<SearchView>();
                foreach (var item in sanPhams)
                {
                    SearchView danhMucSP = new SearchView();
                    danhMucSP.MaSanPham = item.MaSanPham;
                    danhMucSP.TenSanPham = item.TenSanPham;
                    danhMucSP.GiaThiTruong = item.GiaThiTruong;
                    danhMucSP.GiaBan = item.GiaBan;
                    danhMucSP.GiamGia = item.GiamGia;
                    danhMucSP.anh = db.Anhs.Where(s => s.MaSanPham == item.MaSanPham).FirstOrDefault();
                    viewModel.Add(danhMucSP);
                }
                ViewBag.KetQua = viewModel.Count();
                var lst1 = viewModel.Where(s => s.TenSanPham.ToLower().IndexOf(keyword.ToLower()) != -1).OrderBy(s => s.TenSanPham.IndexOf(keyword));
                var lst2 = viewModel.Where(s => s.TenSanPham.ToLower().IndexOf(keyword.ToLower()) == -1).OrderBy(s => s.TenSanPham);
                viewModel = lst1.Concat(lst2).ToList();
                int pageSize = 24;
                int pageNumber = (page ?? 1);
                return View(viewModel.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                var tinTucs = db.TinTucs.Where(s => !s.DeletedDate.HasValue);
                if (!String.IsNullOrEmpty(keyword))
                {
                    tinTucs = tinTucs.Where(s => s.TieuDe.Contains(keyword) || s.LoaiTinTuc.Contains(keyword) || s.NoiDung.Contains(keyword));
                }
                List<SearchView> viewModel = new List<SearchView>();
                foreach (var item in tinTucs)
                {
                    SearchView danhMucTT = new SearchView();
                    danhMucTT.MaTinTuc = item.MaTinTuc;
                    danhMucTT.TieuDe = item.TieuDe;
                    danhMucTT.LoaiTinTuc = item.LoaiTinTuc;
                    danhMucTT.anh = db.Anhs.Where(s => s.MaTinTuc == item.MaTinTuc).FirstOrDefault();
                    danhMucTT.CreatedDate = item.CreatedDate;
                    viewModel.Add(danhMucTT);
                }
                ViewBag.KetQua = viewModel.Count();
                var lst1 = viewModel.Where(s => s.TieuDe.ToLower().IndexOf(keyword.ToLower()) != -1).OrderBy(s => s.TieuDe.IndexOf(keyword));
                var lst2 = viewModel.Where(s => s.TieuDe.ToLower().IndexOf(keyword.ToLower()) == -1).OrderBy(s => s.TieuDe);
                viewModel = lst1.Concat(lst2).ToList();
                int pageSize = 24;
                int pageNumber = (page ?? 1);
                return View(viewModel.ToPagedList(pageNumber, pageSize));
            }
        }

        [HttpGet]
        public JsonResult BoxSanPham(string keyword)
        {
            var sanPhams = db.SanPhams.Where(s => s.TenSanPham.Contains(keyword) || s.DanhMuc.TenDanhMuc.Contains(keyword) || s.NhanHieu.TenNhanHieu.Contains(keyword) || s.CongDung.Contains(keyword) || s.MoTa.Contains(keyword) || s.SanPhamTags.Any(x => x.Tag.TenTag.Contains(keyword)));
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
                string linkanh = "~/wwwroot/Products/" + danhMucSP.anh.LinkAnh;
                danhMucSP.anh.LinkAnh = Url.Content(linkanh);
                int gb = (int)item.GiaBan;
                int gtt = (int)item.GiaThiTruong;
                int tk = gtt - gb;
                string gb2 = "";
                if (item.GiamGia != null || item.GiamGia > 0)
                {
                    gb2 = double.Parse(gb.ToString()).ToString("#,###");
                    int gg = (int)item.GiamGia;
                    gb = (int)(gb - gb * ((double)gg / 100));
                    int digits = gb.ToString().Length;
                    gb = RoundingTo(gb, 1000);
                }
                danhMucSP.GBJS = double.Parse(gb.ToString()).ToString("#,###");
                danhMucSP.GTTJS = double.Parse(gtt.ToString()).ToString("#,###");
                viewModel.Add(danhMucSP);
            }
            var lst1 = viewModel.Where(s => s.TenSanPham.ToLower().IndexOf(keyword.ToLower()) != -1).OrderBy(s => s.TenSanPham.IndexOf(keyword));
            var lst2 = viewModel.Where(s => s.TenSanPham.ToLower().IndexOf(keyword.ToLower()) == -1).OrderBy(s => s.TenSanPham);
            viewModel = lst1.Concat(lst2).ToList();
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        private long RoundingTo(long myNum, long roundTo)
        {
            if (roundTo <= 0) return myNum;
            return (myNum + roundTo / 2) / roundTo * roundTo;
        }
        private long RoundingTo(long myNum, int roundTo)
        {
            return RoundingTo(myNum, (long)roundTo);
        }
        private int RoundingTo(int myNum, int roundTo)
        {
            return (int)RoundingTo((long)myNum, (long)roundTo);
        }
    }
}