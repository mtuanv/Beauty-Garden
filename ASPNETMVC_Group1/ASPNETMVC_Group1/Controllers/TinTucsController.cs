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
    [RoutePrefix("QuanLy/TinTucs")]
    public class TinTucsController : Controller
    {
        private BeautyGarden db = new BeautyGarden();

        private IList<SelectListItem> loaiTinTuc()
        {
            IList<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem{Text = "Sự kiện khuyến mãi", Value = "Sự kiện khuyến mãi"},
                new SelectListItem{Text = "Bí quyết làm đẹp", Value = "Bí quyết làm đẹp"},
                new SelectListItem{Text = "Tin báo chí", Value = "Tin báo chí"},
                new SelectListItem{Text = "Review sản phẩm", Value = "Review sản phẩm"},
            };
            return items;
        }

        private IList<SelectListItem> noiBat()
        {
            IList<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem{Text = "Có", Value = "true"},
                new SelectListItem{Text = "Không", Value = "false"}
            };
            return items;
        }

        // GET: TinTucs
        [Route]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (TempData["CreateSuccess"] != null)
            {
                ViewBag.CreateSuccess = TempData["CreateSuccess"].ToString();
            }
            if (TempData["EditSuccess"] != null)
            {
                ViewBag.EditSuccess = TempData["EditSuccess"].ToString();
            }
            if (TempData["DeleteSuccess"] != null)
            {
                ViewBag.DeleteSuccess = TempData["DeleteSuccess"].ToString();
            }
            if (TempData["DeleteError"] != null)
            {
                ViewBag.DeleteError = TempData["DeleteError"].ToString();
            }
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SapTheoTD = String.IsNullOrEmpty(sortOrder) ? "td_desc" : "";
            ViewBag.SapTheoLoai = sortOrder == "loai" ? "loai_desc" : "loai";
            ViewBag.SapTheoNB = sortOrder == "nb" ? "nb_desc" : "nb";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            var tinTucs = db.TinTucs.Where(s => !s.DeletedDate.HasValue);
            if (!String.IsNullOrEmpty(searchString))
            {
                tinTucs = tinTucs.Where(s => s.TieuDe.Contains(searchString) || s.LoaiTinTuc.Contains(searchString) || s.NoiDung.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "td_desc":
                    tinTucs = tinTucs.OrderByDescending(s => s.TieuDe);
                    break;
                case "loai":
                    tinTucs = tinTucs.OrderBy(s => s.LoaiTinTuc);
                    break;
                case "loai_desc":
                    tinTucs = tinTucs.OrderByDescending(s => s.LoaiTinTuc);
                    break;
                case "nb":
                    tinTucs = tinTucs.OrderBy(s => s.NoiBat);
                    break;
                case "nb_desc":
                    tinTucs = tinTucs.OrderByDescending(s => s.NoiBat);
                    break;
                default:
                    tinTucs = tinTucs.OrderBy(s => s.TieuDe);
                    break;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(tinTucs.ToPagedList(pageNumber, pageSize));
        }

        // GET: TinTucs/Create
        [Route("Create")]
        public ActionResult Create()
        {
            
            IList<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem{Text = "Sự kiện khuyến mãi", Value = "Sự kiện khuyến mãi"},
                new SelectListItem{Text = "Bí quyết làm đẹp", Value = "Bí quyết làm đẹp"},
                new SelectListItem{Text = "Review sản phẩm", Value = "Review sản phẩm"},
            };
            ViewBag.LoaiTinTuc = new SelectList(loaiTinTuc(),"Value", "Text");
            ViewBag.NoiBat = new SelectList(noiBat(), "Value", "Text");
            return View();
        }

        // POST: TinTucs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public ActionResult Create(CreateTinTucViewModel tinTuc)
        {
            try
            {
                if(tinTuc.Image != null)
                {
                    List<string> allowed = new List<string> { ".jpg", ".png", ".gif", ".jpeg", ".jfif" };
                    FileInfo fi = new FileInfo(tinTuc.Image.FileName);
                    if (!allowed.Any(s => s.Equals(fi.Extension)))
                    {
                        ModelState.AddModelError("Image", "Định dạng ảnh không phù hợp.");
                    }
                }
                if (ModelState.IsValid)
                {
                    TinTuc tt = new TinTuc
                    {
                        TieuDe = tinTuc.TieuDe,
                        NoiDung = tinTuc.NoiDung,
                        NoiBat = tinTuc.NoiBat,
                        LoaiTinTuc = tinTuc.LoaiTinTuc,
                        CreatedDate = DateTime.UtcNow.AddHours(7)
                    };
                    db.TinTucs.Add(tt);
                    db.SaveChanges();
                    var createTime = db.TinTucs.Max(x => x.CreatedDate);
                    var maTinTuc = db.TinTucs.Where(x => x.CreatedDate == createTime).FirstOrDefault().MaTinTuc;
                    if(tinTuc.Image != null)
                    {
                        FileInfo fi = new FileInfo(tinTuc.Image.FileName);
                        var newfilename = "Image_" + DateTime.UtcNow.ToString("yyyyMMddHHmmssfff") + fi.Extension;
                        string UploadPath = Server.MapPath("~/wwwroot/News/" + newfilename);
                        tinTuc.Image.SaveAs(UploadPath);
                        Anh a = new Anh
                        {
                            MaTinTuc = maTinTuc,
                            LinkAnh = newfilename
                        };
                        db.Anhs.Add(a);
                        db.SaveChanges();
                    }
                    TempData["CreateSuccess"] = "Success";
                    return RedirectToAction("Index");
                }
                ViewBag.Error = "Error";
                ViewBag.LoaiTinTuc = new SelectList(loaiTinTuc(), "Value", "Text", tinTuc.LoaiTinTuc);
                ViewBag.NoiBat = new SelectList(noiBat(), "Value", "Text", tinTuc.NoiBat);
                return View(tinTuc);
            }
            catch(Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.LoaiTinTuc = new SelectList(loaiTinTuc(), "Value", "Text", tinTuc.LoaiTinTuc);
                ViewBag.NoiBat = new SelectList(noiBat(), "Value", "Text", tinTuc.NoiBat);
                return View(tinTuc);
            }
        }

        // GET: TinTucs/Edit/5
        [Route("Edit/{id:int}")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TinTuc tinTuc = db.TinTucs.Find(id);
            if (tinTuc == null)
            {
                return HttpNotFound();
            }
            EditTinTucViewModel viewModel = new EditTinTucViewModel
            {
                MaTinTuc = tinTuc.MaTinTuc,
                TieuDe = tinTuc.TieuDe,
                NoiDung = tinTuc.NoiDung,
                LoaiTinTuc = tinTuc.LoaiTinTuc,
                NoiBat = tinTuc.NoiBat
            };
            if(db.Anhs.Where(s => s.MaTinTuc == tinTuc.MaTinTuc).Count() != 0)
            {
                viewModel.Anh = db.Anhs.Where(s => s.MaTinTuc == tinTuc.MaTinTuc).FirstOrDefault();
            }
            ViewBag.LoaiTinTuc = new SelectList(loaiTinTuc(), "Value", "Text", tinTuc.LoaiTinTuc);
            ViewBag.NoiBat = new SelectList(noiBat(), "Value", "Text", tinTuc.NoiBat);
            return View(viewModel);
        }

        // POST: TinTucs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit/{id:int}")]
        public ActionResult Edit(EditTinTucViewModel viewModel)
        {
            try
            {
                if (viewModel.Image != null)
                {
                    List<string> allowed = new List<string> { ".jpg", ".png", ".gif", ".jpeg", ".jfif" };
                    FileInfo fi = new FileInfo(viewModel.Image.FileName);
                    if (!allowed.Any(s => s.Equals(fi.Extension)))
                    {
                        ModelState.AddModelError("Image", "Định dạng ảnh không phù hợp.");
                    }
                }
                if (ModelState.IsValid)
                {
                    TinTuc tinTuc = db.TinTucs.Find(viewModel.MaTinTuc);
                    tinTuc.TieuDe = viewModel.TieuDe;
                    tinTuc.NoiDung = viewModel.NoiDung;
                    tinTuc.LoaiTinTuc = viewModel.LoaiTinTuc;
                    tinTuc.NoiBat = viewModel.NoiBat;
                    tinTuc.ModifiedDate = DateTime.UtcNow.AddHours(7);
                    db.Entry(tinTuc).State = EntityState.Modified;
                    db.SaveChanges();
                    if(viewModel.Image != null)
                    {
                        var Anh = db.Anhs.Where(s => s.MaTinTuc == viewModel.MaTinTuc).FirstOrDefault();
                        if(Anh != null)
                        {
                            if(Anh.LinkAnh != null)
                            {
                                string oldPath = Server.MapPath("~/wwwroot/News/" + Anh.LinkAnh);
                                System.IO.File.Delete(oldPath);
                                db.Anhs.Remove(Anh);
                                db.SaveChanges();
                            }
                        }
                        FileInfo fi = new FileInfo(viewModel.Image.FileName);
                        var newfilename = "Image_" + DateTime.UtcNow.ToString("yyyyMMddHHmmssfff") + fi.Extension;
                        string UploadPath = Server.MapPath("~/wwwroot/News/" + newfilename);
                        viewModel.Image.SaveAs(UploadPath);
                        Anh a = new Anh
                        {
                            MaTinTuc = viewModel.MaTinTuc,
                            LinkAnh = newfilename
                        };
                        db.Anhs.Add(a);
                        db.SaveChanges();
                    }
                    TempData["EditSuccess"] = "Success";
                    return RedirectToAction("Index");
                }
                ViewBag.Error = "Error";
                ViewBag.LoaiTinTuc = new SelectList(loaiTinTuc(), "Value", "Text", viewModel.LoaiTinTuc);
                ViewBag.NoiBat = new SelectList(noiBat(), "Value", "Text", viewModel.NoiBat);
                return View(viewModel);
            }
            catch(Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.LoaiTinTuc = new SelectList(loaiTinTuc(), "Value", "Text", viewModel.LoaiTinTuc);
                ViewBag.NoiBat = new SelectList(noiBat(), "Value", "Text", viewModel.NoiBat);
                return View(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("NB/{id:int}")]
        public ActionResult NB(int id)
        {
            TinTuc tinTuc = db.TinTucs.Find(id);
            try
            {
                tinTuc.NoiBat = true;
                db.Entry(tinTuc).State = EntityState.Modified;
                db.SaveChanges();
                TempData["NBSuccess"] = "Success";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["NBError"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("KNB/{id:int}")]
        public ActionResult KNB(int id)
        {
            TinTuc tinTuc = db.TinTucs.Find(id);
            try
            {
                tinTuc.NoiBat = false;
                db.Entry(tinTuc).State = EntityState.Modified;
                db.SaveChanges();
                TempData["KNBSuccess"] = "Success";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["KNBError"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        // POST: TinTucs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Delete/{id:int}")]
        public ActionResult DeleteConfirmed(int id)
        {
            TinTuc tinTuc = db.TinTucs.Find(id);
            try
            {
                tinTuc.DeletedDate = DateTime.UtcNow.AddHours(7);
                db.Entry(tinTuc).State = EntityState.Modified;
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
