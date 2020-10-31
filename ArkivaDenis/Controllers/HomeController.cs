using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArkivaDenis.Models;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity.Migrations;
using ArkivaDenis.helperclass;
using System.IO;
using System.IO.Compression;

namespace ArkivaDenis.Controllers
{

    public class HomeController : Controller
    {

        ArkivaDenisContext db = new ArkivaDenisContext();

        public ActionResult Index()
        {
            if (Session["CurrentUserName"] == null)
            {
                return View("Login");
            }
            else
                return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            Session["CurrentUserName"] = null;
            Session.Abandon();
            return View();
        }

        [HttpGet]
        public ActionResult Regj()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Regjistrohu(Llogaria llog)
        {
            var kerko = (from s in db.Llogaria
                         where s.Username == llog.Username
                         select s).FirstOrDefault();

            if (kerko != null)
            {
                ModelState.AddModelError("CustomError", "Përdoruesi ekziston");
                return View("Regj");
            }
            else
            {
                Llogaria llogari = new Llogaria();
                llogari.Emer = llog.Emer;
                llogari.Mbiemer = llog.Mbiemer;
                llogari.Password = Crypto.Hash(llog.Password);
                llogari.Username = llog.Username;
                db.Llogaria.Add(llogari);
                db.SaveChanges();
                return View("Regj");
            }
        }

        [HttpPost]
        public ActionResult Arkiva(Llogaria llog)
        {
            string password = Crypto.Hash(llog.Password);
            var rezultati = (from i in db.Llogaria
                             where i.Username == llog.Username && i.Password == password
                             select i).FirstOrDefault();

            if (rezultati != null)
            {
                Session["CurrentUserName"] = llog.Username;
                return View("Index");
            }
            else
            {
                ModelState.AddModelError("CustomError", "Të dhëna të pasakta");
                return View("Login");
            }
        }

        public ActionResult Inspektime(int id)
        {
            if (Session["CurrentUserName"] == null)
            {
                return View("Login");
            }
            else
            {
                var lista = (from s in db.Inspektime
                             where s.Subjekte.Id == id
                             select s).ToList();
                Subjekte sub = new Subjekte();

                sub = (from i in db.Subjekte
                       where i.Id == id
                       select i).FirstOrDefault();

                ViewBag.subjekt = sub.Id;

                if (sub != null)
                {
                    ViewBag.Data = sub.EmerSubjekt;
                }

                ViewBag.Inspektime = lista;
                return View();
            }
        }

        public ActionResult LlojDokumenti(int id)
        {
            if (Session["CurrentUserName"] == null)
            {
                return View("Login");
            }
            else
            {
                var lista = (from s in db.LlojDokument
                             where s.Inspektime.Any(x => x.Id == id)
                             select s).ToList();

                Inspektime inspek = new Inspektime();

                inspek = (from i in db.Inspektime
                          where i.Id == id
                          select i).FirstOrDefault();

                ViewBag.inspektim = inspek.Id;
                ViewBag.Subjekt = inspek.Subjekte.Id;
                ViewData["subjekte"] = inspek.Subjekte.EmerSubjekt;
                ViewData["inspektime"] = inspek.NrInspek;
                ViewBag.Id = id;
                ViewBag.Data = lista;

                return View();
            }
        }

        public ActionResult Dokumente(int Subjekt_Id, int Inspektim_Id, int Lloj_Id)
        {
            if (Session["CurrentUserName"] == null)
            {
                return View("Login");
            }
            else
            {
                var lista = (from s in db.Dokumente
                             where s.LlojDokument.Id == Lloj_Id && s.Subjekte_Id == Subjekt_Id && s.Inspektime_Id == Inspektim_Id
                             select s).ToList();

                LlojDokument lloj = new LlojDokument();

                lloj = (from i in db.LlojDokument
                        where i.Id == Lloj_Id
                        select i).FirstOrDefault();

                var inspektime = (from k in db.Inspektime
                                  where k.Id == Inspektim_Id
                                  select k).FirstOrDefault();

                var subjekt = (from l in db.Subjekte
                               where l.Id == Subjekt_Id
                               select l).FirstOrDefault();

                ViewBag.lloj = lloj.Id;
                ViewBag.subjekt = subjekt.Id;
                ViewBag.inspektim = inspektime.Id;
                ViewData["subjekte"] = subjekt.EmerSubjekt;
                ViewData["inspektime"] = inspektime.NrInspek;
                ViewBag.Inspektime_Id = inspektime.Id;
                ViewData["lloje"] = lloj.Emer;
                ViewBag.Data = lista;
                return View();
            }
        }

        public ActionResult Kerko(Filters filter)
        {
            bool shfaqDokumenta = false;
            bool shfaqSubjekte = false;
            bool shfaqInspektime = false;
            var inspektime = new List<Inspektime>();
            var dokumente = new List<Dokumente>();
            var lloje = new List<LlojDokument>();
            var subjekte = new List<Subjekte>();
            var helperList = new List<Inspektime>();
            ViewBag.Data = null;



            if (filter.Subjekte != null)
            {
                subjekte = db.Subjekte.Where(x => x.EmerSubjekt.Contains(filter.Subjekte)).ToList();
                shfaqSubjekte = true;
            }
            else
            {
                subjekte = db.Subjekte.ToList();
            }

            foreach (Subjekte sub in subjekte)
            {
                helperList.AddRange(db.Inspektime.Where(x => x.Subjekte.Id == sub.Id).ToList());
            }

            if (filter.Inspektime != null)
            {
                inspektime = (from a in db.Inspektime where a.NrInspek.Contains(filter.Inspektime) select a).ToList();
                inspektime = (from s in inspektime join d in helperList on s.Id equals d.Id select s).ToList();
                shfaqInspektime = true;
            }
            else
            {
                inspektime = helperList;
            }

            foreach (Inspektime ins in inspektime)
            {
                lloje.AddRange(db.LlojDokument.Where(x => x.Inspektime.Any(s => s.Id == ins.Id)).ToList());
            }

            if (filter.Lloj != null)
            {
                var lista = (from s in db.LlojDokument where s.Emer.Contains(filter.Lloj) select s).ToList();
                lloje = (from s in lloje join d in lista on s.Id equals d.Id select s).ToList();
                shfaqDokumenta = true;
            }


            foreach (LlojDokument llojdok in lloje)
            {
                var lista = (from s in db.Dokumente join d in db.LlojDokument on s.LlojDokument.Id equals d.Id where llojdok.Id == d.Id select s).ToList();
                var lista1 = (from s in lista join d in subjekte on s.Subjekte_Id equals d.Id select s).ToList();
                var lista2 = (from s in lista1 join d in inspektime on s.Inspektime_Id equals d.Id select s).ToList();
                dokumente.AddRange(lista2);
                dokumente = dokumente.Distinct().ToList();

            }

            if (filter.Emer != null)
            {
                var newlist = (from s in db.Dokumente.Where(s => s.EmerDok.Contains(filter.Emer)) select s).ToList();
                dokumente = (from s in dokumente join d in newlist on s.Id equals d.Id select s).ToList();
                dokumente = dokumente.Distinct().ToList();
                shfaqDokumenta = true;
            }

            if (filter.Indeksimet != null)
            {
                string str = filter.Indeksimet;
                string[] separator = { ";" };
                string[] strlist = str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                var ind = new List<Indeksimet>();

                for (int i = 0; i < strlist.Count(); i++)
                {
                    foreach (var item in db.Indeksimet)
                    {
                        if (item.Emer == strlist[i])
                        {
                            ind.Add(item);
                        }
                    }
                }
                var list2 = new List<Dokumente>();

                foreach (var item in ind)
                {
                    var list1 = db.Dokumente.Where(x => x.Indeksimet.Any(s => s.Emer == item.Emer)).ToList();
                    list2.AddRange(list1);
                }
                dokumente = (from s in dokumente
                             join d in list2
                             on s.Id equals d.Id
                             select s).ToList();
                dokumente = dokumente.Distinct().ToList();
                shfaqDokumenta = true;
            }

            if (filter.Rafti != null || filter.NrKutise != null || filter.Zyra != null)
            {
                if (filter.Rafti != null && filter.NrKutise == null && filter.Zyra == null)
                {
                    var list3 = db.Dokumente.Where(x => x.Rafti.Contains(filter.Rafti)).ToList();

                    dokumente = (from s in dokumente join d in list3 on s.Id equals d.Id select s).ToList();
                    dokumente = dokumente.Distinct().ToList();
                    shfaqDokumenta = true;
                }


                else if (filter.NrKutise != null && filter.Rafti == null && filter.Zyra == null)
                {
                    var list3 = db.Dokumente.Where(x => x.NrKutise.Contains(filter.NrKutise)).ToList();

                    dokumente = (from s in dokumente join d in list3 on s.Id equals d.Id select s).ToList();
                    dokumente = dokumente.Distinct().ToList();
                    shfaqDokumenta = true;
                }

                else if (filter.Zyra != null && filter.Rafti == null && filter.NrKutise == null)
                {
                    var list3 = db.Dokumente.Where(x => x.Zyra.Contains(filter.Zyra)).ToList();

                    dokumente = (from s in dokumente join d in list3 on s.Id equals d.Id select s).ToList();
                    dokumente = dokumente.Distinct().ToList();
                    shfaqDokumenta = true;
                }

                else
                {
                    var list3 = db.Dokumente.Where(x => x.Zyra.Contains(filter.Zyra) && x.NrKutise.Contains(filter.NrKutise) && x.Rafti.Contains(filter.Rafti)).ToList();
                    dokumente = (from s in dokumente join d in list3 on s.Id equals d.Id select s).ToList();
                    dokumente = dokumente.Distinct().ToList();
                    shfaqDokumenta = true;
                }
            }

          if (filter.Data1 != null || filter.Data2 != null)
            {
                if(filter.Data1!=null && filter.Data2!=null)
                {
                    var listData = db.Dokumente.Where(x => x.Data.CompareTo(filter.Data1)>=0 && x.Data.CompareTo(filter.Data2)<=0 ).ToList();
                    dokumente = (from s in dokumente join d in listData on s.Id equals d.Id select s).ToList();
                    shfaqDokumenta = true;
                }
            }
      

            if (shfaqDokumenta)
            {
                ViewBag.Data = dokumente;
                ViewBag.Tipi = "dokumenta";
            }
            else if(shfaqInspektime)
            {
                ViewBag.Data = inspektime;
                ViewBag.Tipi = "inspektime";
            }
            else if(shfaqSubjekte)
            {
                ViewBag.Data = subjekte;
                ViewBag.Tipi = "subjekte";
            }
            return View(filter);
        }
         

        [HttpPost]
        public ActionResult Shto(Filters filter, HttpPostedFileBase[] file)
        {
            using (ArkivaDenisContext db = new ArkivaDenisContext())
            {
                int inspektim = Convert.ToInt32(Session["id"]);
                int subjekt = Convert.ToInt32(Session["subjekt"]);
                int lloje = 0;
                string username = Session["CurrentUserName"] as string;
                List<Indeksimet> newindeksim = new List<Indeksimet>();
                int id = 0;
                string str = filter.Indeksimet;
                string[] separator = { "," };
                string[] strlist = str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                bool flag = false;

                if (file != null && file.Count() > 0)
                {
                    foreach (var item in file)
                    {
                        Dokumente dok = new Dokumente();
                        dok.Indeksimet = new List<Indeksimet>();

                        if (item != null)
                        {
                            var fileName = Path.GetFileName(item.FileName);
                            var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                            item.SaveAs(path);

                            LlojDokument lloj = (from s in db.LlojDokument
                                                 where s.Emer == filter.Lloj
                                                 select s).FirstOrDefault();

                            lloje = lloj.Id;
                            dok.EmerDok = fileName;
                            int a = Convert.ToInt32(Session["subjekt"]);
                            dok.Subjekte_Id = a;
                            int b = Convert.ToInt32(Session["inspektim"]);
                            dok.Inspektime_Id = b;
                            dok.LlojDokument = lloj;
                            DateTime Data = DateTime.Now;
                            dok.Data = Data.ToString("dd/MM/yyyy");

                            var llogaria = (from s in db.Llogaria
                                            where (s.Username == username)
                                            select s).FirstOrDefault();

                            dok.Zyra = filter.Zyra;
                            dok.Rafti = filter.Rafti;
                            dok.NrKutise = filter.NrKutise;
                            dok.Llogaria = llogaria;

                            for (int i = 0; i < strlist.Count(); i++)
                            {
                                if (db.Indeksimet.Count() != 0)
                                {
                                    foreach (var s in db.Indeksimet)
                                    {
                                        if (strlist[i] == s.Emer)
                                        {
                                            id = s.Id;
                                            flag = false;
                                            break;
                                        }
                                        else
                                        {
                                            flag = true;
                                        }
                                    }

                                    if (flag == false)
                                    {
                                        Indeksimet indeksim = new Indeksimet();
                                        indeksim = db.Indeksimet.Where(x => x.Id == id).FirstOrDefault();
                                        dok.Indeksimet.Add(indeksim);
                                        db.Dokumente.Add(dok);
                                    }

                                    else
                                    {
                                        Indeksimet index = new Indeksimet();
                                        index.Emer = strlist[i];
                                        db.Indeksimet.Add(index);
                                        dok.Indeksimet.Add(index);
                                        db.Dokumente.Add(dok);
                                    }
                                }

                                else
                                {
                                    Indeksimet index = new Indeksimet();
                                    index.Emer = strlist[i];
                                    db.Indeksimet.Add(index);
                                    dok.Indeksimet.Add(index);
                                    db.Dokumente.Add(dok);
                                }
                            }
                            db.SaveChanges();
                        }
                        else
                        {
                            TempData["UserMessage"] = "Nuk ngarkuat dokument";
                        }
                    }
                    return RedirectToAction("Dokumente", new { Subjekt_Id = subjekt, Inspektim_Id = inspektim, Lloj_Id = lloje });
                }
                else return RedirectToAction("Dokumente", new { Subjekt_Id = subjekt, Inspektim_Id = inspektim, Lloj_Id = lloje });
            }
        }

        public ActionResult Fshi(int Dokumente_Id, int Lloj_Id, int Subjekte_Id, int Inspektime_Id)
        {
            var dok = db.Dokumente.Where(x => x.Id == Dokumente_Id).FirstOrDefault();
            string username = Session["CurrentUserName"] as string;

            if (dok != null && username == dok.Llogaria.Username)
            {
                db.Dokumente.Remove(dok);
                db.SaveChanges();
            }
            else
            {
                return new HttpStatusCodeResult(401, "Nuk keni të drejta fshirje");
            }
            db.SaveChanges();

            return RedirectToAction("Dokumente", new { Subjekt_Id = Subjekte_Id, Inspektim_Id = Inspektime_Id, Lloj_Id = Lloj_Id });
        }

        public FileResult Shkarko(int Dokumente_Id)
        {
            var file = db.Dokumente.Where(x => x.Id == Dokumente_Id).FirstOrDefault();
            string fileName = file.EmerDok;
            string Filepath = Path.Combine(Server.MapPath("~/App_Data/uploads/" + fileName));
            byte[] fileBytes = System.IO.File.ReadAllBytes(Filepath);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        public ActionResult ShkarkoZipSubjekt()
        {
            var subjekte = db.Subjekte.ToList();
            byte[] compressedBytes;
            var dok = db.Dokumente.ToList();
            string fileNameZip = "dok.zip";

            using (var outStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
                {
                    foreach (var item in dok)
                    {
                        var fileInArchive = archive.CreateEntry(item.EmerDok, CompressionLevel.Optimal);
                        string Filepath = Path.Combine(Server.MapPath("~/App_Data/uploads/" + item.EmerDok));
                        byte[] fileBytes = System.IO.File.ReadAllBytes(Filepath);

                        using (var entryStream = fileInArchive.Open())
                        using (var fileToCompressStream = new MemoryStream(fileBytes))
                        {
                            fileToCompressStream.CopyTo(entryStream);
                        }
                    }
                }
                compressedBytes = outStream.ToArray();
            }
            return File(compressedBytes, "application/zip", fileNameZip);
        }

        public ActionResult ShkarkoSubjektSpecifik(int Subjekt_Id)
        {
            var subjekt = db.Subjekte.Where(x => x.Id == Subjekt_Id).FirstOrDefault();
            byte[] compressedBytes;
            var dok = db.Dokumente.ToList();
            List<Dokumente> result = new List<Dokumente>();
            string fileNameZip = subjekt.EmerSubjekt;
            fileNameZip = fileNameZip + ".zip";

            foreach (var item in dok)
            {
                if (item.Subjekte_Id == Subjekt_Id)
                {
                    result.Add(item);
                }
            }
            using (var outStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
                {
                    foreach (var item in result)
                    {
                        var fileInArchive = archive.CreateEntry(item.EmerDok, CompressionLevel.Optimal);
                        string Filepath = Path.Combine(Server.MapPath("~/App_Data/uploads/" + item.EmerDok));
                        byte[] fileBytes = System.IO.File.ReadAllBytes(Filepath);

                        using (var entryStream = fileInArchive.Open())
                        using (var fileToCompressStream = new MemoryStream(fileBytes))
                        {
                            fileToCompressStream.CopyTo(entryStream);
                        }
                    }
                }
                compressedBytes = outStream.ToArray();
            }
            return File(compressedBytes, "application/zip", fileNameZip);
        }

        public ActionResult ShkarkoInspektimSpecifk(int Inspektim_Id)
        {
            var inspektim = db.Inspektime.Where(x => x.Id == Inspektim_Id).FirstOrDefault();

            byte[] compressedBytes;
            var dok = db.Dokumente.ToList();
            List<Dokumente> result = new List<Dokumente>();
            string fileNameZip = inspektim.NrInspek + ".zip";

            foreach (var item in dok)
            {
                if (item.Inspektime_Id == Inspektim_Id)
                {
                    result.Add(item);
                }
            }
            using (var outStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
                {
                    foreach (var item in result)
                    {
                        var fileInArchive = archive.CreateEntry(item.EmerDok, CompressionLevel.Optimal);
                        string Filepath = Path.Combine(Server.MapPath("~/App_Data/uploads/" + item.EmerDok));
                        byte[] fileBytes = System.IO.File.ReadAllBytes(Filepath);

                        using (var entryStream = fileInArchive.Open())
                        using (var fileToCompressStream = new MemoryStream(fileBytes))
                        {
                            fileToCompressStream.CopyTo(entryStream);
                        }
                    }
                }
                compressedBytes = outStream.ToArray();
            }
            return File(compressedBytes, "application/zip", fileNameZip);
        }

        public ActionResult ShkarkoLlojSpecifik(int Subjekt_Id, int Inspektim_Id, int Lloj_Id)
        {
            List<Dokumente> dok = db.Dokumente.Where(x => x.Subjekte_Id == Subjekt_Id && x.Inspektime_Id == Inspektim_Id && x.LlojDokument.Id == Lloj_Id).ToList();
            byte[] compressedBytes;
            string fileNameZip = "";
            LlojDokument lloj = db.LlojDokument.Where(x => x.Id == Lloj_Id).FirstOrDefault();
            fileNameZip = lloj.Emer + ".zip";

            using (var outStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
                {
                    foreach (var item in dok)
                    {
                        var fileInArchive = archive.CreateEntry(item.EmerDok, CompressionLevel.Optimal);
                        string Filepath = Path.Combine(Server.MapPath("~/App_Data/uploads/" + item.EmerDok));
                        byte[] fileBytes = System.IO.File.ReadAllBytes(Filepath);

                        using (var entryStream = fileInArchive.Open())
                        using (var fileToCompressStream = new MemoryStream(fileBytes))
                        {
                            fileToCompressStream.CopyTo(entryStream);
                        }
                    }
                }
                compressedBytes = outStream.ToArray();
            }
            return File(compressedBytes, "application/zip", fileNameZip);
        }

        public ActionResult Preview(int Dokument_Id)
        {
            result result = new result();
            var dokument = db.Dokumente.Where(x => x.Id == Dokument_Id).FirstOrDefault();
            string Filepath = Path.Combine(Server.MapPath("~/App_Data/uploads/" + dokument.EmerDok));
            string extension = Path.GetExtension(dokument.EmerDok);

            result.Zyra = dokument.Zyra;
            result.Kutia = dokument.NrKutise;
            result.Rafti = dokument.Rafti;

            if (extension == ".pdf")
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(Filepath);
                result.Content = Convert.ToBase64String(fileBytes, 0, fileBytes.Length);
                var size = result.Content.Length;

                if (size > 2000000)
                {
                    var indexe = db.Indeksimet.Where(x => x.Dokumente.Any(s => s.Id == Dokument_Id)).ToList();
                    result.Content = "sizeExceeded";
                    result.Indeksime = new List<string>();
                    foreach (var item in indexe)
                    {
                        result.Indeksime.Add(item.Emer);
                    }
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                else
                {
                    var ind = db.Indeksimet.Where(x => x.Dokumente.Any(s => s.Id == Dokument_Id)).ToList();
                    result.Indeksime = new List<string>();
                    foreach (var item in ind)
                    {
                        result.Indeksime.Add(item.Emer);
                    }
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }

            else
            {
                var ind = db.Indeksimet.Where(x => x.Dokumente.Any(s => s.Id == Dokument_Id)).ToList();
                result.Indeksime = new List<string>();
                foreach (var item in ind)
                {
                    result.Indeksime.Add(item.Emer);
                }

                result.Content = "Notsupported";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ModifikoPreview(int Dokument_Id, int Subjekte_Id, int Inspektime_Id, int Lloje_Id, string Zyra1, string NrKutise1, string Rafti1, string Indeksimet)
        {
            var dok = db.Dokumente.Where(x => x.Id == Dokument_Id).FirstOrDefault();
            dok.Indeksimet = null;
            dok.Indeksimet = new List<Indeksimet>();

            dok.Zyra = Zyra1;
            dok.NrKutise = NrKutise1;
            dok.Rafti = Rafti1;

            if (Indeksimet != "")
            {
                string str = Indeksimet;
                string[] separator = { "," };
                string[] strlist = str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                bool flag = false;
                int id = 0;

                if (strlist.Count() < dok.Indeksimet.Count())
                {
                    dok.Indeksimet.Clear();
                    for (int l = 0; l < strlist.Count(); l++)
                    {
                        foreach(var item in db.Indeksimet)
                        {
                            if (item.Emer == strlist[l])
                            {
                                Indeksimet helper = new Indeksimet();
                                helper = item;
                                dok.Indeksimet.Add(helper);
                            }
                        }
                    }
                }

                else
                {
                    for (int i = 0; i < strlist.Count(); i++)
                    {
                        if (db.Indeksimet.Count() != 0)
                        {
                            foreach (var s in db.Indeksimet)
                            {
                                if (strlist[i] == s.Emer)
                                {
                                    id = s.Id;
                                    flag = false;
                                    break;
                                }
                                else
                                {
                                    flag = true;
                                }
                            }

                            if (flag == false)
                            {
                                Indeksimet indeksim = new Indeksimet();
                                indeksim = db.Indeksimet.Where(x => x.Id == id).FirstOrDefault();
                                dok.Indeksimet.Add(indeksim);
                            }

                            else
                            {
                                Indeksimet index = new Indeksimet();
                                index.Emer = strlist[i];
                                db.Indeksimet.Add(index);
                                dok.Indeksimet.Add(index);
                            }
                        }
                        else
                        {
                            Indeksimet index = new Indeksimet();
                            index.Emer = strlist[i];
                            db.Indeksimet.Add(index);
                        }
                    }
                }
            }

            else
            {
                dok.Indeksimet.Clear();
            }

            db.SaveChanges();
            return RedirectToAction("Dokumente", new { Subjekt_Id = Subjekte_Id, Inspektim_Id = Inspektime_Id, Lloj_Id = Lloje_Id });
        }
    }
}




