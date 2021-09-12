using ASPNETMVC_Group1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ASPNETMVC_Group1.Controllers
{
    public class SPController : Controller
    {
        private BeautyGarden db = new BeautyGarden();

        // GET: SP/Details/5
        [Route("SanPham/{id:int}")]
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SanPham sanPham = db.SanPhams.Find(id);
            if (sanPham == null || sanPham.DeletedDate.HasValue)
            {
                return HttpNotFound();
            }
            DetailSanPhamViewModel viewModel = new DetailSanPhamViewModel
            {
                sanPham = sanPham,
                anhs = db.Anhs.ToList(),
                cungXem = db.SanPhams.Where(s => s.MaNhanHieu == sanPham.MaNhanHieu && s.MaSanPham != sanPham.MaSanPham).ToList(),
                muaCung = db.SanPhams.Where(s => s.DanhMuc.MaDanhMucCha != sanPham.DanhMuc.MaDanhMucCha && s.MaSanPham != sanPham.MaSanPham).ToList(),
                cungLoai = db.SanPhams.Where(s => s.MaDanhMuc == sanPham.MaDanhMuc && s.MaSanPham != sanPham.MaSanPham).ToList()
            };
            if(Session["Seen"] != null)
            {
                var Seen = (List<DetailSanPhamViewModel>)Session["Seen"];
                if(!Seen.Any(s => s.sanPham.MaSanPham == viewModel.sanPham.MaSanPham))
                {
                    Seen.Add(viewModel);
                    Session["Seen"] = Seen;
                }
            }
            else
            {
                List<DetailSanPhamViewModel> Seen = new List<DetailSanPhamViewModel>();
                Seen.Add(viewModel);
                Session["Seen"] = Seen;
            }
            var lstdanhgia = db.DanhGias.Where(s => s.MaSanPham == id).ToList();
            if (lstdanhgia.Count == 0)
            {
                ViewBag.DanhGia = 0;
                ViewBag.Sao = 0;
                ViewBag.Count = 0;
            }
            else
            {
                int count = lstdanhgia.Count();
                int tong = 0;
                foreach (var dg in lstdanhgia)
                {
                    tong += (int)dg.DanhGia1;
                }
                ViewBag.DanhGia = Math.Round((double)tong / count,1);
                ViewBag.Sao = Math.Round((double)tong / count);
                ViewBag.Count = count;
            }
            return View(viewModel);
        }
    }
}