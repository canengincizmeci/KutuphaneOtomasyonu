using kutuphaneWeb_24_02_24.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace kutuphaneWeb_24_02_24.Controllers
{
    public class UyelerController : Controller
    {
        // GET: Uyeler
        public ActionResult Index()
        {
            kutuphane_24_02_24Entities model = new kutuphane_24_02_24Entities();
            var veri = model.uyelerTBL.Select(p => new uyeler
            {
                id = p.id,
                adSoyad = p.adSoyad,
                kitapSayisi = p.kitapSayisi,
                mail = p.mail,
                aktiflik = p.aktiflik
            }).ToList();
            return View(veri);
        }
        public ActionResult Sil(int id)
        {
            kutuphane_24_02_24Entities model = new kutuphane_24_02_24Entities();
            var veri = model.uyelerTBL.Where(p => p.id == id).FirstOrDefault();
            veri.aktiflik = false;
            model.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Guncelle(int id)
        {
            kutuphane_24_02_24Entities model = new kutuphane_24_02_24Entities();
            var veri = model.uyelerTBL.Where(p => p.id == id).FirstOrDefault();
            uyeler uye = new uyeler()
            {
                id = veri.id,
                adSoyad = veri.adSoyad,
                kitapSayisi = veri.kitapSayisi,
                aktiflik = veri.aktiflik,
                mail = veri.mail
            };
            return View(uye);
        }
        [HttpPost]
        public ActionResult Guncelle(uyeler uye)
        {
            kutuphane_24_02_24Entities model = new kutuphane_24_02_24Entities();
            var veri = model.uyelerTBL.Where(p => p.id == uye.id).FirstOrDefault();
            veri.adSoyad = uye.adSoyad;
            veri.kitapSayisi = uye.kitapSayisi;
            veri.aktiflik = true;
            veri.mail = uye.mail;
            model.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Ekle()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Ekle(uyeler uye)
        {
            kutuphane_24_02_24Entities model = new kutuphane_24_02_24Entities();
            model.uyelerTBL.Add(new uyelerTBL
            {
                adSoyad = uye.adSoyad,
                kitapSayisi = 0,
                aktiflik = true,
                mail = uye.mail
            });
            model.SaveChanges();
            MesajGonder(uye.adSoyad, uye.mail);
            
            return RedirectToAction("Index");
        }
        public void MesajGonder(string adSoyad, string mail)
        {


            //Burada kendi mail ve şifremi sildim gerekli yerlere girilecek şeyleri yazdım
            var cred = new NetworkCredential("mailAdres", "Sifre");
            var client = new SmtpClient("smtp.gmail.com", 587);
            var msg = new System.Net.Mail.MailMessage();
            msg.To.Add(mail);
            msg.Subject = "Konu";
            
            msg.Body = "Kaydınız başarıyla eklenmiştir";
            msg.IsBodyHtml = false;
            msg.From = new MailAddress("mail", "Ad", Encoding.UTF8);
            client.Credentials = cred; 
            client.EnableSsl = true;   
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; }; 
            client.Send(msg);
        }
        public void MesajGonderIletisim(string adSoyad, string mail, string icerik)
        {



            //Burada kendi mail ve şifremi sildim gerekli yerlere girilecek şeyleri yazdım
            var cred = new NetworkCredential("mailAdres", "Sifre");
            var client = new SmtpClient("smtp.gmail.com", 587);
            var msg = new System.Net.Mail.MailMessage();
            msg.To.Add(mail);
            msg.Subject = "Konu";
            msg.Body = "Sayın " + adSoyad + " " + icerik;
            msg.IsBodyHtml = false;
            msg.From = new MailAddress("mail", "Ad", Encoding.UTF8);
            client.Credentials = cred; 
            client.EnableSsl = true;   
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            client.Send(msg);
        }


        [HttpGet]
        public ActionResult Mesaj(int id)
        {
            kutuphane_24_02_24Entities model = new kutuphane_24_02_24Entities();
            var uye = model.uyelerTBL.Where(p => p.id == id).FirstOrDefault();
            uyeler uyem = new uyeler()
            {
                id = uye.id,
                adSoyad = uye.adSoyad,
                kitapSayisi = uye.kitapSayisi,
                aktiflik = uye.aktiflik,
                mail = uye.mail
            };
            return View(uyem);
        }
        [HttpPost]
        public ActionResult Mesaj(int id, string icerik)
        {
            kutuphane_24_02_24Entities model = new kutuphane_24_02_24Entities();
            var uyeAd = model.uyelerTBL.Where(p => p.id == id).FirstOrDefault().adSoyad;
            var mail = model.uyelerTBL.Where(p => p.id == id).FirstOrDefault().mail;
            MesajGonderIletisim(uyeAd, mail, icerik);

            return RedirectToAction("Index");
        }
    }
}