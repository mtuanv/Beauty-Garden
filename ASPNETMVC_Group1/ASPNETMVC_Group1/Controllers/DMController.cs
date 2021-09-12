using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ASPNETMVC_Group1.Models;
using PagedList;

namespace ASPNETMVC_Group1.Controllers
{
    public class DMController : Controller
    {
        private BeautyGarden db = new BeautyGarden();

        // GET: DM
        [Route("DanhMuc/{id}")]
        public ActionResult Index(int? id, int? page, string sortOrder)
        {
            if(id == null)
            {
                return HttpNotFound();
            }
            DanhMuc danhMuc = db.DanhMucs.Find(id);
            if(danhMuc == null || danhMuc.DeletedDate.HasValue)
            {
                return HttpNotFound();
            }
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SapTheoMoiVe = sortOrder == "1" ? "0" : "1";
            ViewBag.SapTheoGia = sortOrder == "2" ? "3" : "2";
            ViewBag.SapTheoDanhGia = sortOrder == "4" ? "5" : "4";
            ViewBag.SapTheoMuaNhieu = sortOrder == "6" ? "7" : "6";
            var sanPhams = db.SanPhams.Select(s => s);
            if (id == 1 || id == 2)
            {
                sanPhams = sanPhams.Where(s => s.DanhMuc.DanhMuc2.MaDanhMucCha == id && !s.DeletedDate.HasValue);
            }
            else if (id == 2 || id == 3 || id == 4 || id == 5 || id == 6 || id == 7 || id == 8 || id == 9 || id == 30 || id == 31 || id == 32)
            {
                sanPhams = sanPhams.Where(s => s.DanhMuc.MaDanhMucCha == id && !s.DeletedDate.HasValue);
            }
            else
            {
                sanPhams = sanPhams.Where(s => s.MaDanhMuc == id && !s.DeletedDate.HasValue);
            }
            switch (sortOrder)
            {
                case "0":
                    sanPhams = sanPhams.OrderByDescending(s => s.CreatedDate);
                    break;
                case "1":
                    sanPhams = sanPhams.OrderBy(s => s.CreatedDate);
                    break;
                case "2":
                    sanPhams = sanPhams.OrderBy(s => s.GiaBan);
                    break;
                case "3":
                    sanPhams = sanPhams.OrderByDescending(s => s.GiaBan);
                    break;
                case "4":
                    sanPhams = sanPhams.OrderBy(s => s.DanhGias.Where(x => x.MaSanPham == s.MaSanPham).Sum(x => x.DanhGia1) / s.DanhGias.Where(x => x.MaSanPham == s.MaSanPham).Count());
                    break;
                case "5":
                    sanPhams = sanPhams.OrderByDescending(s => s.DanhGias.Where(x => x.MaSanPham == s.MaSanPham).Sum(x => x.DanhGia1) / s.DanhGias.Where(x => x.MaSanPham == s.MaSanPham).Count());
                    break;
                case "6":
                    sanPhams = sanPhams.OrderBy(s => s.ChiTietDonHangs.Where(x => x.MaSanPham == s.MaSanPham && x.DonHang.TrangThai == 3).Count());
                    break;
                case "7":
                    sanPhams = sanPhams.OrderByDescending(s => s.ChiTietDonHangs.Where(x => x.MaSanPham == s.MaSanPham && x.DonHang.TrangThai == 3).Count());
                    break;
                default:
                    sanPhams = sanPhams.OrderByDescending(s => s.CreatedDate);
                    break;
            }
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
                viewModel.Add(danhMucSP);
            }
            ViewBag.Title = danhMuc.TenDanhMuc;
            int pageSize = 24;
            int pageNumber = (page ?? 1);
            return View(viewModel.ToPagedList(pageNumber, pageSize));
        }
    }
}