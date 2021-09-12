using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ASPNETMVC_Group1.Models;
using PagedList;

namespace ASPNETMVC_Group1.Controllers
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("QuanLy/DanhMuc")]
    public class DanhMucsController : Controller
    {
        private BeautyGarden db = new BeautyGarden();

        // GET: DanhMucs
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
            ViewBag.SapTheoTen = String.IsNullOrEmpty(sortOrder) ? "ten_desc" : "";
            ViewBag.SapTheoTenCha = sortOrder == "pr" ? "pr_desc" : "pr";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            var danhmuc = db.DanhMucs.Where(s => !s.DeletedDate.HasValue && s.MaDanhMucCha.HasValue &&
            s.MaDanhMuc != 7 && s.MaDanhMuc != 8 && s.MaDanhMuc != 9 && s.MaDanhMuc != 30 && s.MaDanhMuc != 31 && s.MaDanhMuc != 32).Select(s => s);
            if (!String.IsNullOrEmpty(searchString))
            {
                danhmuc = danhmuc.Where(s => s.TenDanhMuc.Contains(searchString) || s.DanhMuc2.TenDanhMuc.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "ten_desc":
                    danhmuc = danhmuc.OrderByDescending(s => s.TenDanhMuc);
                    break;
                case "pr":
                    danhmuc = danhmuc.OrderBy(s => s.DanhMuc2.TenDanhMuc);
                    break;
                case "pr_desc":
                    danhmuc = danhmuc.OrderByDescending(s => s.DanhMuc2.TenDanhMuc);
                    break;
                default:
                    danhmuc = danhmuc.OrderBy(s => s.TenDanhMuc);
                    break;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(danhmuc.ToPagedList(pageNumber, pageSize));
        }

        // GET: DanhMucs/Create
        [Route("Create")]
        public ActionResult Create()
        {
            var danhmuccha = db.DanhMucs.Where(s => s.MaDanhMuc == 3 || s.MaDanhMuc == 4 || s.MaDanhMuc == 5 || s.MaDanhMuc == 6 || s.MaDanhMuc == 7 || s.MaDanhMuc == 8 || s.MaDanhMuc == 9
            || s.MaDanhMuc == 30 || s.MaDanhMuc == 31 || s.MaDanhMuc == 32);
            ViewBag.MaDanhMucCha = new SelectList(danhmuccha, "MaDanhMuc", "TenDanhMuc");
            return View();
        }

        // POST: DanhMucs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public ActionResult Create([Bind(Include = "MaDanhMuc,TenDanhMuc,MaDanhMucCha,CreatedDate,ModifiedDate,DeletedDate")] DanhMuc danhMuc)
        {
            if (!danhMuc.MaDanhMucCha.HasValue)
            {
                ViewBag.Error = "Error";
                var danhmuccha = db.DanhMucs.Where(s => s.MaDanhMuc == 3 || s.MaDanhMuc == 4 || s.MaDanhMuc == 5 || s.MaDanhMuc == 6 || s.MaDanhMuc == 7 || s.MaDanhMuc == 8 || s.MaDanhMuc == 9
            || s.MaDanhMuc == 30 || s.MaDanhMuc == 31 || s.MaDanhMuc == 32);
                ViewBag.MaDanhMucCha = new SelectList(danhmuccha, "MaDanhMuc", "TenDanhMuc");
                return View(danhMuc);
            }
            try
            {
                if (ModelState.IsValid)
                {
                    danhMuc.CreatedDate = DateTime.UtcNow.AddHours(7);
                    db.DanhMucs.Add(danhMuc);
                    db.SaveChanges();
                    TempData["CreateSuccess"] = "Success";
                    return RedirectToAction("Index");
                }
                ViewBag.Error = "Error";
                var danhmuccha = db.DanhMucs.Where(s => s.MaDanhMuc == 3 || s.MaDanhMuc == 4 || s.MaDanhMuc == 5 || s.MaDanhMuc == 6 || s.MaDanhMuc == 7 || s.MaDanhMuc == 8 || s.MaDanhMuc == 9
            || s.MaDanhMuc == 30 || s.MaDanhMuc == 31 || s.MaDanhMuc == 32);
                ViewBag.MaDanhMucCha = new SelectList(danhmuccha, "MaDanhMuc", "TenDanhMuc");
                return View(danhMuc);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                var danhmuccha = db.DanhMucs.Where(s => s.MaDanhMuc == 3 || s.MaDanhMuc == 4 || s.MaDanhMuc == 5 || s.MaDanhMuc == 6 || s.MaDanhMuc == 7 || s.MaDanhMuc == 8 || s.MaDanhMuc == 9
            || s.MaDanhMuc == 30 || s.MaDanhMuc == 31 || s.MaDanhMuc == 32);
                ViewBag.MaDanhMucCha = new SelectList(danhmuccha, "MaDanhMuc", "TenDanhMuc");
                return View(danhMuc);
            }
        }

        // GET: DanhMucs/Edit/5
        [Route("Edit/{id:int}")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DanhMuc danhMuc = db.DanhMucs.Find(id);
            if (danhMuc == null)
            {
                return HttpNotFound();
            }
            var danhmuccha = db.DanhMucs.Where(s => s.MaDanhMuc == 3 || s.MaDanhMuc == 4 || s.MaDanhMuc == 5 || s.MaDanhMuc == 6 || s.MaDanhMuc == 7 || s.MaDanhMuc == 8 || s.MaDanhMuc == 9
             || s.MaDanhMuc == 30 || s.MaDanhMuc == 31 || s.MaDanhMuc == 32);
            ViewBag.MaDanhMucCha = new SelectList(danhmuccha, "MaDanhMuc", "TenDanhMuc", danhMuc.MaDanhMucCha);
            return View(danhMuc);
        }

        // POST: DanhMucs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit/{id:int}")]
        public ActionResult Edit([Bind(Include = "MaDanhMuc,TenDanhMuc,MaDanhMucCha,CreatedDate,ModifiedDate,DeletedDate")] DanhMuc danhMuc)
        {
            if (!danhMuc.MaDanhMucCha.HasValue)
            {
                ViewBag.Error = "Error";
                var danhmuccha = db.DanhMucs.Where(s => s.MaDanhMuc == 3 || s.MaDanhMuc == 4 || s.MaDanhMuc == 5 || s.MaDanhMuc == 6 || s.MaDanhMuc == 7 || s.MaDanhMuc == 8 || s.MaDanhMuc == 9
            || s.MaDanhMuc == 30 || s.MaDanhMuc == 31 || s.MaDanhMuc == 32);
                ViewBag.MaDanhMucCha = new SelectList(danhmuccha, "MaDanhMuc", "TenDanhMuc", danhMuc.MaDanhMucCha);
                return View(danhMuc);
            }
            var cd = db.DanhMucs.AsNoTracking().Where(s => s.MaDanhMuc == danhMuc.MaDanhMuc).FirstOrDefault().CreatedDate;
            try
            {
                if (ModelState.IsValid)
                {
                    danhMuc.CreatedDate = cd;
                    danhMuc.ModifiedDate = DateTime.UtcNow.AddHours(7);
                    db.Entry(danhMuc).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["EditSuccess"] = "Success";
                    return RedirectToAction("Index");
                }
                ViewBag.Error = "Error";
                var danhmuccha = db.DanhMucs.Where(s => s.MaDanhMuc == 3 || s.MaDanhMuc == 4 || s.MaDanhMuc == 5 || s.MaDanhMuc == 6 || s.MaDanhMuc == 7 || s.MaDanhMuc == 8 || s.MaDanhMuc == 9
            || s.MaDanhMuc == 30 || s.MaDanhMuc == 31 || s.MaDanhMuc == 32);
                ViewBag.MaDanhMucCha = new SelectList(danhmuccha, "MaDanhMuc", "TenDanhMuc", danhMuc.MaDanhMucCha);
                return View(danhMuc);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                var danhmuccha = db.DanhMucs.Where(s => s.MaDanhMuc == 3 || s.MaDanhMuc == 4 || s.MaDanhMuc == 5 || s.MaDanhMuc == 6 || s.MaDanhMuc == 7 || s.MaDanhMuc == 8 || s.MaDanhMuc == 9
            || s.MaDanhMuc == 30 || s.MaDanhMuc == 31 || s.MaDanhMuc == 32);
                ViewBag.MaDanhMucCha = new SelectList(danhmuccha, "MaDanhMuc", "TenDanhMuc", danhMuc.MaDanhMucCha);
                return View(danhMuc);
            }
        }

        // POST: DanhMucs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Delete/{id:int}")]
        public ActionResult DeleteConfirmed(int id)
        {
            DanhMuc danhMuc = db.DanhMucs.Find(id);
            try
            {
                danhMuc.DeletedDate = DateTime.UtcNow.AddHours(7);
                db.Entry(danhMuc).State = EntityState.Modified;
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
