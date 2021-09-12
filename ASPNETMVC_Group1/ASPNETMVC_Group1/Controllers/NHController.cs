using ASPNETMVC_Group1.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASPNETMVC_Group1.Controllers
{
    public class NHController : Controller
    {
        private BeautyGarden db = new BeautyGarden();

        // GET: NH
        [Route("ThuongHieu/{id}")]
        public ActionResult Index(int? id, int? page)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            NhanHieu nhanHieu = db.NhanHieux.Find(id);
            if (nhanHieu == null || nhanHieu.DeletedDate.HasValue)
            {
                return HttpNotFound();
            }
            if (page == null) page = 1;
            var sanPhams = db.SanPhams.Where(s => s.MaNhanHieu == id && !s.DeletedDate.HasValue);
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
            ViewBag.Title = nhanHieu.TenNhanHieu;
            int pageSize = 24;
            int pageNumber = (page ?? 1);
            return View(viewModel.ToPagedList(pageNumber, pageSize));
        }
    }
}