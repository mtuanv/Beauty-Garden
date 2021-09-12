using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ASPNETMVC_Group1.Models;
using PagedList;

namespace ASPNETMVC_Group1.Controllers
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("QuanLy/SanPhams")]
    public class SanPhamsController : Controller
    {
        private BeautyGarden db = new BeautyGarden();

        // GET: SanPhams
        [Route]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (TempData["CreateSuccess"] != null)
            {
                ViewBag.CreateSuccess = TempData["CreateSuccess"].ToString();
            }
            if (TempData["DeleteSuccess"] != null)
            {
                ViewBag.DeleteSuccess = TempData["DeleteSuccess"].ToString();
            }
            if (TempData["DeleteError"] != null)
            {
                ViewBag.DeleteError = TempData["DeleteError"].ToString();
            }
            if (TempData["StopSuccess"] != null)
            {
                ViewBag.StopSuccess = TempData["StopSuccess"].ToString();
            }
            if (TempData["StopError"] != null)
            {
                ViewBag.StopError = TempData["StopError"].ToString();
            }
            if (TempData["ContinueSuccess"] != null)
            {
                ViewBag.ContinueSuccess = TempData["ContinueSuccess"].ToString();
            }
            if (TempData["ContinueError"] != null)
            {
                ViewBag.ContinueError = TempData["ContinueError"].ToString();
            }
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SapTheoTen = String.IsNullOrEmpty(sortOrder) ? "ten_desc" : "";
            ViewBag.SapTheoDM = sortOrder == "dm" ? "dm_desc" : "dm";
            ViewBag.SapTheoNH = sortOrder == "nh" ? "nh_desc" : "nh";
            ViewBag.SapTheoSL = sortOrder == "sl" ? "sl_desc" : "sl";
            ViewBag.SapTheoGTT = sortOrder == "gtt" ? "gtt_desc" : "gtt";
            ViewBag.SapTheoGB = sortOrder == "gb" ? "gb_desc" : "gb";
            ViewBag.SapTheoGG = sortOrder == "gg" ? "gg_desc" : "gg";
            ViewBag.SapTheoTag = sortOrder == "tag" ? "tag_desc" : "tag";
            ViewBag.SapTheoTT = sortOrder == "tt" ? "tt_desc" : "tt";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            var sanPhams = db.SanPhams.Include(s => s.DanhMuc).Include(s => s.NhanHieu).Where(s => !s.DeletedDate.HasValue && !s.DanhMuc.DeletedDate.HasValue && !s.NhanHieu.DeletedDate.HasValue);
            if (!String.IsNullOrEmpty(searchString))
            {
                sanPhams = sanPhams.Where(s => s.TenSanPham.Contains(searchString) || s.DanhMuc.TenDanhMuc.Contains(searchString) || s.NhanHieu.TenNhanHieu.Contains(searchString) || s.CongDung.Contains(searchString) || s.MoTa.Contains(searchString) || s.SanPhamTags.Any(x => x.Tag.TenTag.Contains(searchString)));
            }
            switch (sortOrder)
            {
                case "ten_desc":
                    sanPhams = sanPhams.OrderByDescending(s => s.TenSanPham);
                    break;
                case "dm":
                    sanPhams = sanPhams.OrderBy(s => s.DanhMuc.TenDanhMuc);
                    break;
                case "dm_desc":
                    sanPhams = sanPhams.OrderByDescending(s => s.DanhMuc.TenDanhMuc);
                    break;
                case "nh":
                    sanPhams = sanPhams.OrderBy(s => s.NhanHieu.TenNhanHieu);
                    break;
                case "nh_desc":
                    sanPhams = sanPhams.OrderByDescending(s => s.NhanHieu.TenNhanHieu);
                    break;
                case "sl":
                    sanPhams = sanPhams.OrderBy(s => s.SoLuong);
                    break;
                case "sl_desc":
                    sanPhams = sanPhams.OrderByDescending(s => s.SoLuong);
                    break;
                case "gtt":
                    sanPhams = sanPhams.OrderBy(s => s.GiaThiTruong);
                    break;
                case "gtt_desc":
                    sanPhams = sanPhams.OrderByDescending(s => s.GiaThiTruong);
                    break;
                case "gb":
                    sanPhams = sanPhams.OrderBy(s => s.GiaBan);
                    break;
                case "gb_desc":
                    sanPhams = sanPhams.OrderByDescending(s => s.GiaBan);
                    break;
                case "gg":
                    sanPhams = sanPhams.OrderBy(s => s.GiamGia);
                    break;
                case "gg_desc":
                    sanPhams = sanPhams.OrderByDescending(s => s.GiamGia);
                    break;
                case "tag":
                    sanPhams = sanPhams.OrderBy(s => s.SanPhamTags.Where(x => x.MaSanPham == s.MaSanPham).Count());
                    break;
                case "tag_desc":
                    sanPhams = sanPhams.OrderByDescending(s => s.SanPhamTags.Where(x => x.MaSanPham == s.MaSanPham).Count());
                    break;
                case "tt":
                    sanPhams = sanPhams.OrderBy(s => s.TinhTrang);
                    break;
                case "tt_desc":
                    sanPhams = sanPhams.OrderByDescending(s => s.TinhTrang);
                    break;
                default:
                    sanPhams = sanPhams.OrderBy(s => s.TenSanPham);
                    break;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(sanPhams.ToPagedList(pageNumber, pageSize));
        }

        // GET: SanPhams/Create
        [Route("Create")]
        public ActionResult Create()
        {
            ViewBag.MaDanhMuc = new SelectList(db.DanhMucs.Where(s => s.MaDanhMuc != 1 && s.MaDanhMuc != 2 && s.MaDanhMuc != 3 && s.MaDanhMuc != 4 && s.MaDanhMuc != 5 && s.MaDanhMuc != 6
            && s.MaDanhMuc != 7 && s.MaDanhMuc != 8 && s.MaDanhMuc != 9 && s.MaDanhMuc != 30 && s.MaDanhMuc != 31 && s.MaDanhMuc != 32 && !s.DeletedDate.HasValue).OrderBy(s => s.TenDanhMuc), "MaDanhMuc", "TenDanhMuc");
            ViewBag.MaNhanHieu = new SelectList(db.NhanHieux.Where(s => !s.DeletedDate.HasValue).OrderBy(s => s.TenNhanHieu), "MaNhanHieu", "TenNhanHieu");
            ViewBag.Tags = new MultiSelectList(db.Tags.Where(s => !s.DeletedDate.HasValue).OrderBy(s => s.TenTag), "MaTag", "TenTag");
            return View();
        }

        // POST: SanPhams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public ActionResult Create(CreateSanPhamViewModel sanPham)
        {
            try
            {
                if (sanPham.Images == null || sanPham.Images.FirstOrDefault() == null)
                {
                    ModelState.AddModelError("Images", "Hãy chọn ảnh sản phẩm.");
                }
                else
                {
                    List<string> allowed = new List<string> { ".jpg", ".png", ".gif", ".jpeg", ".jfif" };
                    foreach (HttpPostedFileBase file in sanPham.Images)
                    {
                        FileInfo fi = new FileInfo(file.FileName);
                        if (!allowed.Any(s => s.Equals(fi.Extension)))
                        {
                            ModelState.AddModelError("Images", "Định dạng ảnh không phù hợp.");
                            break;
                        }
                    }
                }
                if (ModelState.IsValid)
                {
                    SanPham sp = new SanPham
                    {
                        TenSanPham = sanPham.TenSanPham,
                        CongDung = sanPham.CongDung,
                        MoTa = sanPham.MoTa,
                        SoLuong = 0,
                        TinhTrang = 2,
                        GiaThiTruong = sanPham.GiaThiTruong,
                        GiaBan = sanPham.GiaBan,
                        GiamGia = sanPham.GiamGia,
                        MaNhanHieu = sanPham.MaNhanHieu,
                        MaDanhMuc = sanPham.MaDanhMuc,
                        CreatedDate = DateTime.UtcNow.AddHours(7)
                    };
                    db.SanPhams.Add(sp);
                    db.SaveChanges();
                    var createTime = db.SanPhams.Max(x => x.CreatedDate);
                    var maSanPham = db.SanPhams.Where(x => x.CreatedDate == createTime).FirstOrDefault().MaSanPham;
                    foreach (HttpPostedFileBase file in sanPham.Images)
                    {
                        if (file != null)
                        {
                            FileInfo fi = new FileInfo(file.FileName);
                            var newfilename = "Image_" + DateTime.UtcNow.ToString("yyyyMMddHHmmssfff") + fi.Extension;
                            string UploadPath = Server.MapPath("~/wwwroot/Products/" + newfilename);
                            file.SaveAs(UploadPath);
                            Anh a = new Anh
                            {
                                MaSanPham = maSanPham,
                                LinkAnh = newfilename
                            };
                            db.Anhs.Add(a);
                            db.SaveChanges();
                        }
                    }
                    if (sanPham.Tags != null)
                    {
                        foreach (var tag in sanPham.Tags)
                        {
                            SanPhamTag spt = new SanPhamTag
                            {
                                MaSanPham = maSanPham,
                                MaTag = tag
                            };
                            db.SanPhamTags.Add(spt);
                            db.SaveChanges();
                        }
                    }
                    TempData["CreateSuccess"] = "Success";
                    return RedirectToAction("Index");
                }

                ViewBag.Error = "Error";
                ViewBag.MaDanhMuc = new SelectList(db.DanhMucs.Where(s => s.MaDanhMuc != 1 && s.MaDanhMuc != 2 && s.MaDanhMuc != 3 && s.MaDanhMuc != 4 && s.MaDanhMuc != 5 && s.MaDanhMuc != 6
            && s.MaDanhMuc != 7 && s.MaDanhMuc != 8 && s.MaDanhMuc != 9 && s.MaDanhMuc != 30 && s.MaDanhMuc != 31 && s.MaDanhMuc != 32 && !s.DeletedDate.HasValue), "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
                ViewBag.MaNhanHieu = new SelectList(db.NhanHieux.Where(s => !s.DeletedDate.HasValue), "MaNhanHieu", "TenNhanHieu", sanPham.MaNhanHieu);
                ViewBag.Tags = new MultiSelectList(db.Tags.Where(s => !s.DeletedDate.HasValue), "MaTag", "TenTag", sanPham.Tags);
                return View(sanPham);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.MaDanhMuc = new SelectList(db.DanhMucs.Where(s => s.MaDanhMuc != 1 && s.MaDanhMuc != 2 && s.MaDanhMuc != 3 && s.MaDanhMuc != 4 && s.MaDanhMuc != 5 && s.MaDanhMuc != 6
            && s.MaDanhMuc != 7 && s.MaDanhMuc != 8 && s.MaDanhMuc != 9 && s.MaDanhMuc != 30 && s.MaDanhMuc != 31 && s.MaDanhMuc != 32 && !s.DeletedDate.HasValue), "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
                ViewBag.MaNhanHieu = new SelectList(db.NhanHieux.Where(s => !s.DeletedDate.HasValue), "MaNhanHieu", "TenNhanHieu", sanPham.MaNhanHieu);
                ViewBag.Tags = new MultiSelectList(db.Tags.Where(s => !s.DeletedDate.HasValue), "MaTag", "TenTag", sanPham.Tags);
                return View(sanPham);
            }
        }

        // GET: SanPhams/Edit/5
        [Route("Edit/{id:int}")]
        public ActionResult Edit(int? id)
        {
            if (TempData["EditSuccess"] != null)
            {
                ViewBag.EditSuccess = TempData["EditSuccess"].ToString();
            }
            if (TempData["EditError"] != null)
            {
                ViewBag.EditError = TempData["EditError"].ToString();
            }
            if (TempData["EditTagSuccess"] != null)
            {
                ViewBag.EditTagSuccess = TempData["EditTagSuccess"].ToString();
            }
            if (TempData["EditTagError"] != null)
            {
                ViewBag.EditTagError = TempData["EditTagError"].ToString();
            }
            if (TempData["EditAnhSuccess"] != null)
            {
                ViewBag.EditAnhSuccess = TempData["EditAnhSuccess"].ToString();
            }
            if (TempData["EditAnhError"] != null)
            {
                ViewBag.EditAnhError = TempData["EditAnhError"].ToString();
            }
            if (id == null)
            {
                return HttpNotFound();
            }
            SanPham sanPham = db.SanPhams.Find(id);
            if (sanPham == null)
            {
                return HttpNotFound();
            }
            EditSanPhamViewModel viewModel = new EditSanPhamViewModel
            {
                MaSanPham = sanPham.MaSanPham,
                TenSanPham = sanPham.TenSanPham,
                MoTa = sanPham.MoTa,
                GiaThiTruong = (int)sanPham.GiaThiTruong,
                GiaBan = (int)sanPham.GiaBan,
                MaNhanHieu = (int)sanPham.MaNhanHieu,
                MaDanhMuc = (int)sanPham.MaDanhMuc
            };
            if (sanPham.CongDung != null)
            {
                viewModel.CongDung = sanPham.CongDung;
            }
            if (sanPham.GiamGia != null)
            {
                viewModel.GiamGia = sanPham.GiamGia;
            }
            if (db.Anhs.Where(s => s.MaSanPham == sanPham.MaSanPham).Count() != 0)
            {
                viewModel.Anhs = db.Anhs.Where(s => s.MaSanPham == sanPham.MaSanPham).ToList();
            }
            if (db.SanPhamTags.Where(s => s.MaSanPham == sanPham.MaSanPham).Count() != 0)
            {
                viewModel.Tags = db.SanPhamTags.Where(s => s.MaSanPham == sanPham.MaSanPham).Select(s => s.MaTag).ToArray();
            }
            ViewBag.MaDanhMuc = new SelectList(db.DanhMucs.Where(s => s.MaDanhMuc != 1 && s.MaDanhMuc != 2 && s.MaDanhMuc != 3 && s.MaDanhMuc != 4 && s.MaDanhMuc != 5 && s.MaDanhMuc != 6
            && s.MaDanhMuc != 7 && s.MaDanhMuc != 8 && s.MaDanhMuc != 9 && s.MaDanhMuc != 30 && s.MaDanhMuc != 31 && s.MaDanhMuc != 32 && !s.DeletedDate.HasValue), "MaDanhMuc", "TenDanhMuc", viewModel.MaDanhMuc);
            ViewBag.MaNhanHieu = new SelectList(db.NhanHieux.Where(s => !s.DeletedDate.HasValue), "MaNhanHieu", "TenNhanHieu", viewModel.MaNhanHieu);
            ViewBag.Tags = new MultiSelectList(db.Tags.Where(s => !s.DeletedDate.HasValue), "MaTag", "TenTag", viewModel.Tags);
            return View(viewModel);
        }

        // POST: SanPhams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit/{id:int}")]
        public ActionResult Edit(EditSanPhamViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValidField("TenSanPham") && ModelState.IsValidField("MoTa") && ModelState.IsValidField("GiaThiTruong") && ModelState.IsValidField("GiaBan") && ModelState.IsValidField("MaNhanHieu") && ModelState.IsValidField("MaDanhMuc"))
                {
                    SanPham sanPham = db.SanPhams.Find(viewModel.MaSanPham);
                    sanPham.TenSanPham = viewModel.TenSanPham;
                    sanPham.CongDung = viewModel.CongDung;
                    sanPham.MoTa = viewModel.MoTa;
                    sanPham.GiaThiTruong = viewModel.GiaThiTruong;
                    sanPham.GiaBan = viewModel.GiaBan;
                    sanPham.GiamGia = viewModel.GiamGia;
                    sanPham.MaNhanHieu = viewModel.MaNhanHieu;
                    sanPham.MaDanhMuc = viewModel.MaDanhMuc;
                    sanPham.ModifiedDate = DateTime.UtcNow.AddHours(7);
                    db.Entry(sanPham).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["EditSuccess"] = "Success";
                    return RedirectToAction("Edit", new { id = viewModel.MaSanPham });
                }
                TempData["EditError"] = "Error";
                return RedirectToAction("Edit", new { id = viewModel.MaSanPham });
            }
            catch (Exception ex)
            {
                TempData["EditError"] = ex.Message;
                return RedirectToAction("Edit", new { id = viewModel.MaSanPham });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("EditTag/{id:int}")]
        public ActionResult EditTag(EditSanPhamViewModel viewModel)
        {
            try
            {
                var lstSPT = db.SanPhamTags.Where(s => s.MaSanPham == viewModel.MaSanPham).ToList();
                foreach (var spt in lstSPT)
                {
                    db.SanPhamTags.Remove(spt);
                    db.SaveChanges();
                }
                if (viewModel.Tags != null)
                {
                    foreach (var tag in viewModel.Tags)
                    {
                        SanPhamTag spt = new SanPhamTag
                        {
                            MaSanPham = viewModel.MaSanPham,
                            MaTag = tag
                        };
                        db.SanPhamTags.Add(spt);
                        db.SaveChanges();
                    }
                }
                TempData["EditTagSuccess"] = "Success";
                return RedirectToAction("Edit", new { id = viewModel.MaSanPham });
            }
            catch (Exception ex)
            {
                TempData["EditTagError"] = ex.Message;
                return RedirectToAction("Edit", new { id = viewModel.MaSanPham });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("EditAnh/{id:int}")]
        public ActionResult EditAnh(EditSanPhamViewModel viewModel)
        {
            try
            {
                bool flag = true;
                if (viewModel.Images == null || viewModel.Images.FirstOrDefault() == null)
                {
                    flag = false;
                }
                else
                {
                    List<string> allowed = new List<string> { ".jpg", ".png", ".gif", ".jpeg", ".jfif" };
                    foreach (HttpPostedFileBase file in viewModel.Images)
                    {
                        FileInfo fi = new FileInfo(file.FileName);
                        if (!allowed.Any(s => s.Equals(fi.Extension)))
                        {
                            flag = false;
                            break;
                        }
                    }
                }
                if(flag)
                {
                    var lstAnh = db.Anhs.Where(s => s.MaSanPham == viewModel.MaSanPham).ToList();
                    foreach (var a in lstAnh)
                    {
                        if (a.LinkAnh != null)
                        {
                            string oldPath = Server.MapPath("~/wwwroot/Products/" + a.LinkAnh);
                            System.IO.File.Delete(oldPath);
                            db.Anhs.Remove(a);
                            db.SaveChanges();
                        }
                    }
                    foreach (HttpPostedFileBase file in viewModel.Images)
                    {
                        if (file != null)
                        {
                            FileInfo fi = new FileInfo(file.FileName);
                            var newfilename = "Image_" + DateTime.UtcNow.ToString("yyyyMMddHHmmssfff") + fi.Extension;
                            string UploadPath = Server.MapPath("~/wwwroot/Products/" + newfilename);
                            file.SaveAs(UploadPath);
                            Anh a = new Anh
                            {
                                MaSanPham = viewModel.MaSanPham,
                                LinkAnh = newfilename
                            };
                            db.Anhs.Add(a);
                            db.SaveChanges();
                        }
                    }
                    TempData["EditAnhSuccess"] = "Success";
                    return RedirectToAction("Edit", new { id = viewModel.MaSanPham });
                }    
                TempData["EditAnhError"] = "Error";
                return RedirectToAction("Edit", new { id = viewModel.MaSanPham });
            }
            catch (Exception ex)
            {
                TempData["EditAnhError"] = ex.Message;
                return RedirectToAction("Edit", new { id = viewModel.MaSanPham });
            }
        }

        // POST: SanPhams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Delete/{id:int}")]
        public ActionResult DeleteConfirmed(int id)
        {
            SanPham sanPham = db.SanPhams.Find(id);
            try
            {
                sanPham.DeletedDate = DateTime.UtcNow.AddHours(7);
                db.Entry(sanPham).State = EntityState.Modified;
                db.SaveChanges();
                TempData["DeleteSuccess"] = "Success";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["DeleteError"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("StopSale/{id:int}")]
        public ActionResult StopSale(int id)
        {
            SanPham sanPham = db.SanPhams.Find(id);
            try
            {
                sanPham.TinhTrang = 3;
                db.Entry(sanPham).State = EntityState.Modified;
                db.SaveChanges();
                TempData["StopSuccess"] = "Success";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["StopError"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("ContinueSale/{id:int}")]
        public ActionResult ContinueSale(int id)
        {
            SanPham sanPham = db.SanPhams.Find(id);
            try
            {
                if(sanPham.SoLuong > 0)
                {
                    sanPham.TinhTrang = 1;
                }
                else
                {
                    sanPham.TinhTrang = 2;
                }
                db.Entry(sanPham).State = EntityState.Modified;
                db.SaveChanges();
                TempData["ContinueSuccess"] = "Success";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ContinueError"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
