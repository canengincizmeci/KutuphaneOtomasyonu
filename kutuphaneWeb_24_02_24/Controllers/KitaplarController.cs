using kutuphaneWeb_24_02_24.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace kutuphaneWeb_24_02_24.Controllers
{
    public class KitaplarController : Controller
    {
        // GET: Kitaplar
        public ActionResult Index()
        {
            kutuphane_24_02_24Entities model = new kutuphane_24_02_24Entities();
            var veri = model.kitaplarTBL.Select(p => new kitaplar
            {
                id = p.id,
                kitapAd = p.kitapAd,
                kitapYazar = p.kitapYazar,
                aktiflik = p.aktiflik,
                sayfaSayisi = p.sayfaSayisi,
                oduncDurum = p.oduncDurum
            }).ToList();
            return View(veri);
        }
        public ActionResult Sil(int id)
        {
            kutuphane_24_02_24Entities model = new kutuphane_24_02_24Entities();
            var veri = model.kitaplarTBL.Where(p => p.id == id).FirstOrDefault();
            veri.aktiflik = false;
            model.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Guncelle(int id)
        {
            kutuphane_24_02_24Entities model = new kutuphane_24_02_24Entities();
            var ktg = model.kitaplarTBL.Where(p => p.id == id).FirstOrDefault();
            kitaplar kitap = new kitaplar()
            {
                id = ktg.id,
                kitapAd = ktg.kitapAd,
                kitapYazar = ktg.kitapYazar,
                sayfaSayisi = ktg.sayfaSayisi,
                aktiflik = ktg.aktiflik
            };
            return View(kitap);
        }
        [HttpPost]
        public ActionResult Guncelle(kitaplar kitap)
        {
            kutuphane_24_02_24Entities model = new kutuphane_24_02_24Entities();
            var veri = model.kitaplarTBL.Where(p => p.id == kitap.id).FirstOrDefault();
            veri.kitapAd = kitap.kitapAd;
            veri.kitapYazar = kitap.kitapYazar;
            veri.sayfaSayisi = kitap.sayfaSayisi;
            veri.aktiflik = true;
            model.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Ekle()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Ekle(kitaplar kitap)
        {
            kutuphane_24_02_24Entities model = new kutuphane_24_02_24Entities();
            model.kitaplarTBL.Add(new kitaplarTBL
            {
                kitapAd = kitap.kitapAd,
                kitapYazar = kitap.kitapYazar,
                sayfaSayisi = kitap.sayfaSayisi,
                aktiflik = true,
                oduncDurum = true
            });
            model.SaveChanges();
            return RedirectToAction("Index");
        }
       
        
        public ActionResult KitapDetay(int id)
        {
            kutuphane_24_02_24Entities model = new kutuphane_24_02_24Entities();
            var kitap = model.kitaplarTBL.Where(p => p.id == id).FirstOrDefault();
            var uye = kitap.islemlerTBL.LastOrDefault();
            ViewBag.id = uye.uyelerTBL.id;
            var okumaSuresi = 20;
            var kalanGun = okumaSuresi - (DateTime.Now - uye.islemTarihi.Value.Date).TotalDays;
            ViewBag.KalanGun = Math.Ceiling(kalanGun).ToString();
            ViewBag.Ad = uye.uyelerTBL.adSoyad;
            return View();
        }
        
        public ActionResult KitapIste(int id)
        {
            GecikmeMail(id);
            return RedirectToAction("Index");
        }
        public void GecikmeMail(int id)
        {
            kutuphane_24_02_24Entities model = new kutuphane_24_02_24Entities();
            var uye = model.uyelerTBL.Where(p => p.id == id).FirstOrDefault();

            //Burada kendi mail ve şifremi sildim gerekli yerlere girilecek şeyleri yazdım
            var cred = new NetworkCredential("mailAdres", "Sifre");
            var client = new SmtpClient("smtp.gmail.com", 587);
            var msg = new System.Net.Mail.MailMessage();
            msg.To.Add(uye.mail);
            msg.Subject = "Kitap İade Gecikmesi";
            msg.Body = "Sayın üyemiz Kitap İade Gününüz gecikmiştir lütfen kitabı iade ediniz";

            msg.IsBodyHtml = false;
            msg.From = new MailAddress("mail", "Ad", Encoding.UTF8);
            client.Credentials = cred; 
            client.EnableSsl = true;  
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; }; 
            client.Send(msg);
        }

    }
}