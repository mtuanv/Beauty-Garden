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
    [RoutePrefix("QuanLy/Tag")]
    public class TagsController : Controller
    {
        private BeautyGarden db = new BeautyGarden();

        // GET: Tags
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
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            var tag = db.Tags.Where(s => !s.DeletedDate.HasValue).Select(s => s);
            if (!String.IsNullOrEmpty(searchString))
            {
                tag = tag.Where(s => s.TenTag.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "ten_desc":
                    tag = tag.OrderByDescending(s => s.TenTag);
                    break;
                default:
                    tag = tag.OrderBy(s => s.TenTag);
                    break;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(tag.ToPagedList(pageNumber, pageSize));
        }

        // GET: Tags/Create
        [Route("Create")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Tags/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public ActionResult Create([Bind(Include = "MaTag,TenTag,CreatedDate,ModifiedDate,DeletedDate")] Tag tag)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    tag.CreatedDate = DateTime.UtcNow.AddHours(7);
                    db.Tags.Add(tag);
                    db.SaveChanges();
                    TempData["CreateSuccess"] = "Success";
                    return RedirectToAction("Index");
                }
                ViewBag.Error = "Error";
                return View(tag);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(tag);
            }
        }

        // GET: Tags/Edit/5
        [Route("Edit/{id:int}")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tag tag = db.Tags.Find(id);
            if (tag == null)
            {
                return HttpNotFound();
            }
            return View(tag);
        }

        // POST: Tags/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit/{id:int}")]
        public ActionResult Edit([Bind(Include = "MaTag,TenTag,CreatedDate,ModifiedDate,DeletedDate")] Tag tag)
        {
            var cd = db.Tags.AsNoTracking().Where(s => s.MaTag == tag.MaTag).FirstOrDefault().CreatedDate;
            try
            {
                if (ModelState.IsValid)
                {
                    tag.CreatedDate = cd;
                    tag.ModifiedDate = DateTime.UtcNow.AddHours(7);
                    db.Entry(tag).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["EditSuccess"] = "Success";
                    return RedirectToAction("Index");
                }
                ViewBag.Error = "Error";
                return View(tag);
            }
            catch(Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(tag);
            }
        }

        // POST: Tags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Delete/{id:int}")]
        public ActionResult DeleteConfirmed(int id)
        {
            Tag tag = db.Tags.Find(id);
            try
            {
                tag.DeletedDate = DateTime.UtcNow.AddHours(7);
                db.Entry(tag).State = EntityState.Modified;
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
