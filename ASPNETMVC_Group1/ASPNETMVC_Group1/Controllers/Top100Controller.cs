using ASPNETMVC_Group1.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASPNETMVC_Group1.Controllers
{
    public class Top100Controller : Controller
    {
        private BeautyGarden db = new BeautyGarden();

        // GET: Top100
        public ActionResult Index(int? page)
        {
            if (page == null) page = 1; 
            var sanPhams = db.SanPhams.Where(s => !s.DeletedDate.HasValue).OrderByDescending(s => s.ChiTietDonHangs.Where(x => x.DonHang.TrangThai == 3).Sum(x => x.SoLuong)).Take(100);
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
            int pageSize = 24;
            int pageNumber = (page ?? 1);
            return View(viewModel.ToPagedList(pageNumber, pageSize));
        }
    }
}