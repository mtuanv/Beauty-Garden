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
    [RoutePrefix("QuanLy/NhanHieu")]
    public class NhanHieusController : Controller
    {
        private BeautyGarden db = new BeautyGarden();

        // GET: NhanHieus
        [Route]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if(TempData["CreateSuccess"] != null)
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
            if(searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            var nhanhieu = db.NhanHieux.Where(s => !s.DeletedDate.HasValue).Select(s => s);
            if (!String.IsNullOrEmpty(searchString))
            {
                nhanhieu = nhanhieu.Where(s => s.TenNhanHieu.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "ten_desc":
                    nhanhieu = nhanhieu.OrderByDescending(s => s.TenNhanHieu);
                    break;
                default:
                    nhanhieu = nhanhieu.OrderBy(s => s.TenNhanHieu);
                    break;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(nhanhieu.ToPagedList(pageNumber, pageSize));
        }

        // GET: NhanHieus/Create
        [Route("Create")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: NhanHieus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public ActionResult Create([Bind(Include = "MaNhanHieu,TenNhanHieu,CreatedDate,ModifiedDate,DeletedDate")] NhanHieu nhanHieu)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    nhanHieu.CreatedDate = DateTime.UtcNow.AddHours(7);
                    db.NhanHieux.Add(nhanHieu);
                    db.SaveChanges();
                    TempData["CreateSuccess"] = "Success";
                    return RedirectToAction("Index");
                }
                ViewBag.Error = "Error";
                return View(nhanHieu);
            }
            catch(Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(nhanHieu);
            }
        }

        // GET: NhanHieus/Edit/5
        [Route("Edit/{id:int}")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NhanHieu nhanHieu = db.NhanHieux.Find(id);
            if (nhanHieu == null)
            {
                return HttpNotFound();
            }
            return View(nhanHieu);
        }

        // POST: NhanHieus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit/{id:int}")]
        public ActionResult Edit([Bind(Include = "MaNhanHieu,TenNhanHieu,CreatedDate,ModifiedDate,DeletedDate")] NhanHieu nhanHieu)
        {
            var cd = db.NhanHieux.AsNoTracking().Where(s => s.MaNhanHieu == nhanHieu.MaNhanHieu).FirstOrDefault().CreatedDate;
            try
            {
                if (ModelState.IsValid)
                {
                    nhanHieu.CreatedDate = cd;
                    nhanHieu.ModifiedDate = DateTime.UtcNow.AddHours(7);
                    db.Entry(nhanHieu).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["EditSuccess"] = "Success";
                    return RedirectToAction("Index");
                }
                ViewBag.Error = "Error";
                return View(nhanHieu);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(nhanHieu);
            }
        }

        // POST: NhanHieus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Delete/{id:int}")]
        public ActionResult DeleteConfirmed(int id)
        {
            NhanHieu nhanHieu = db.NhanHieux.Find(id);
            try
            {
                nhanHieu.DeletedDate = DateTime.UtcNow.AddHours(7);
                db.Entry(nhanHieu).State = EntityState.Modified;
                db.SaveChanges();
                TempData["DeleteSuccess"] = "Success";
                return RedirectToAction("Index");
            }
            catch(Exception ex)
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
