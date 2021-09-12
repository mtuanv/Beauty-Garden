using ASPNETMVC_Group1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ASPNETMVC_Group1.Controllers
{
    public class TTController : Controller
    {
        private BeautyGarden db = new BeautyGarden();

        // GET: TT
        [Route("TinTuc/{id:int}")]
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TinTuc tinTuc = db.TinTucs.Find(id);
            if (tinTuc == null || tinTuc.DeletedDate.HasValue)
            {
                return HttpNotFound();
            }
            DetailsTinTucViewModel viewModel = new DetailsTinTucViewModel
            {
                tinTuc = tinTuc,
                ttKhacs = db.TinTucs.Where(s => !s.DeletedDate.HasValue && s.MaTinTuc != tinTuc.MaTinTuc && s.LoaiTinTuc.Equals(tinTuc.LoaiTinTuc)).OrderByDescending(s => s.CreatedDate).ToList(),
                anhs = db.Anhs.ToList()
            };
            return View(viewModel);
        }
    }
}