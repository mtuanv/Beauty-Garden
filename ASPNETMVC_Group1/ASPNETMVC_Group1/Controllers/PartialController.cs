using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ASPNETMVC_Group1.Models;

namespace ASPNETMVC_Group1.Controllers
{
    public class PartialController : Controller
    {
        private BeautyGarden db = new BeautyGarden();

        [ChildActionOnly]
        public ActionResult DisplayDanhMuc()
        {
            var danhMucs = db.DanhMucs.Include(d => d.DanhMuc2);
            return PartialView("DanhMuc", danhMucs.Where(s => !s.DeletedDate.HasValue));
        }

        [ChildActionOnly]
        public ActionResult DisplayThuongHieu()
        {
            return PartialView("ThuongHieu", db.NhanHieux.Where(s => !s.DeletedDate.HasValue));
        }

        [ChildActionOnly]
        public ActionResult DanhMucSanPham()
        {
            ListDanhMucViewModel viewModel = new ListDanhMucViewModel
            {
                danhMucs = db.DanhMucs.Where(s => !s.DeletedDate.HasValue && s.MaDanhMuc != 75).ToList(),
                sanPhams = db.SanPhams.Where(s => !s.DeletedDate.HasValue && !s.DanhMuc.DeletedDate.HasValue && !s.NhanHieu.DeletedDate.HasValue).ToList()
            };
            return PartialView("DanhMucSanPham", viewModel);
        }

        [ChildActionOnly]
        public ActionResult TinNoiBat(int? id)
        {
            var tinTucs = db.TinTucs.Select(s => s);
            if(id != null)
            {
                tinTucs = tinTucs.Where(s => !s.DeletedDate.HasValue && s.MaTinTuc != id && s.NoiBat == true).OrderByDescending(s => s.CreatedDate);
            }
            else
            {
                tinTucs = tinTucs.Where(s => !s.DeletedDate.HasValue && s.NoiBat == true).OrderByDescending(s => s.CreatedDate);
            }
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
            return PartialView("TinNoiBat", viewModel);
        }

        [ChildActionOnly]
        public ActionResult SanPhamMoiVe(int? id)
        {
            var sanPhams = db.SanPhams.Select(s => s);
            if (id != null)
            {
                sanPhams = sanPhams.Where(s => !s.DeletedDate.HasValue && s.MaSanPham != id && s.SanPhamTags.Any(x => x.Tag.TenTag.Equals("New"))).OrderByDescending(s => s.CreatedDate);
            }
            else
            {
                sanPhams = sanPhams.Where(s => !s.DeletedDate.HasValue && s.SanPhamTags.Any(x => x.Tag.TenTag.Equals("New"))).OrderByDescending(s => s.CreatedDate);
            }
            List<DanhMucSPViewModel> viewModel = new List<DanhMucSPViewModel>();
            foreach (var item in sanPhams)
            {
                DanhMucSPViewModel danhMucSP = new DanhMucSPViewModel();
                danhMucSP.MaSanPham = item.MaSanPham;
                danhMucSP.TenSanPham = item.TenSanPham;
                danhMucSP.GiaBan = item.GiaBan;
                danhMucSP.GiaThiTruong = item.GiaThiTruong;
                danhMucSP.GiamGia = item.GiamGia;
                danhMucSP.anh = db.Anhs.Where(s => s.MaSanPham == item.MaSanPham).FirstOrDefault();
                danhMucSP.CreatedDate = item.CreatedDate;
                viewModel.Add(danhMucSP);
            }
            return PartialView("SanPhamMoiVe", viewModel);
        }

        [ChildActionOnly]
        public ActionResult ThuongHieuNoiBat()
        {
            ListThuongHieuViewModel viewModel = new ListThuongHieuViewModel
            {
                nhanHieus = db.NhanHieux.Where(s => !s.DeletedDate.HasValue).OrderByDescending(s => s.SanPhams.Count()).Take(10).ToList(),
                sanPhams = db.SanPhams.Where(s => !s.DeletedDate.HasValue && !s.DanhMuc.DeletedDate.HasValue && !s.NhanHieu.DeletedDate.HasValue).ToList()
            };
            return PartialView("ThuongHieuNoiBat", viewModel);
        }

        [ChildActionOnly]
        public ActionResult Seen()
        {
            List<DetailSanPhamViewModel> Seen = (List<DetailSanPhamViewModel>)Session["Seen"];
            return PartialView("Seen", Seen);
        }
    }

}