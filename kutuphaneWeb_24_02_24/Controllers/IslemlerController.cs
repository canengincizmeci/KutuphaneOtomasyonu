using kutuphaneWeb_24_02_24.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace kutuphaneWeb_24_02_24.Controllers
{
    public class IslemlerController : Controller
    {
        // GET: Islemler
        public ActionResult Index()
        {
            kutuphane_24_02_24Entities model = new kutuphane_24_02_24Entities();
            var uyeler = model.uyelerTBL.Select(p => new uyeler
            {
                id = p.id,
                adSoyad = p.adSoyad,
                aktiflik = p.aktiflik,
                kitapSayisi = p.kitapSayisi,
                mail = p.mail
            }).ToList();
            return View(uyeler);
        }
        public ActionResult IslemSayfasi(int id)
        {
            kutuphane_24_02_24Entities model = new kutuphane_24_02_24Entities();
            var veri = model.uyelerTBL.Where(p => p.id == id).FirstOrDefault();
            uyeler uye = new uyeler()
            {
                id = veri.id,
                adSoyad = veri.adSoyad,
                kitapSayisi = veri.kitapSayisi,
                mail = veri.mail,
                aktiflik = veri.aktiflik

            };
          
            var veriler = model.GetLatestBookTransactions(id);
            ViewBag.Veriler = veriler.ToList();
          


          
            return View(uye);
        }
        public void MesajGonderIletisim(int Uyeid, int kitapId)
        {
            kutuphane_24_02_24Entities model = new kutuphane_24_02_24Entities();
            var kisi = model.uyelerTBL.Where(p => p.id == Uyeid).FirstOrDefault();
            var kitap = model.kitaplarTBL.Where(p => p.id == kitapId).FirstOrDefault();
            //Burada kendi mail ve şifremi sildim gerekli yerlere girilecek şeyleri yazdım
            var cred = new NetworkCredential("mailAdres","Sifre");
            var client = new SmtpClient("smtp.gmail.com", 587);
            var msg = new System.Net.Mail.MailMessage();
            msg.To.Add(kisi.mail);
            msg.Subject = "İade";
            msg.Body = "Sayın " + kisi.adSoyad + " " + kitap.kitapAd + " " + "Adlı Kitabınız başarıyla iade edilmiştir";
            msg.IsBodyHtml = false;
            //mail kısmına kendi mailiniz yazın
            msg.From = new MailAddress("mail", "Ad", Encoding.UTF8);
            client.Credentials = cred; 
            client.EnableSsl = true;   
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; }; 
            client.Send(msg);
        }
        public ActionResult Iade(int id, int uyeId)
        {
            kutuphane_24_02_24Entities model = new kutuphane_24_02_24Entities();

            islemler islem = new islemler()
            {
                islemTur = true,
                kitapID = id,
                uyeID = uyeId,

            };
            model.islemlerTBL.Add(new islemlerTBL()
            {
                islemTur = islem.islemTur,
                kitapID = islem.kitapID,
                uyeID = islem.uyeID,
                islemTarihi = DateTime.Today
            });
            //Üyenin kitap sayısını 1 azaltma
            var uyeVerisi = model.uyelerTBL.Where(p => p.id == uyeId).FirstOrDefault();
            uyeVerisi.kitapSayisi -= 1;
            //Odunc Durmunu true yapma
            var kitapVerisi = model.kitaplarTBL.Where(p => p.id == id).FirstOrDefault();
            kitapVerisi.oduncDurum = true;
            //Mail gönderme
            model.SaveChanges();
            MesajGonderIletisim(uyeId, kitapVerisi.id);
            return RedirectToAction("islemSayfasi", new { id = uyeId });
        }
        public ActionResult OduncSayfasi(int id)
        {
            kutuphane_24_02_24Entities model = new kutuphane_24_02_24Entities();
            var kitaplar = model.kitaplarTBL.Select(p => new kitaplar
            {
                id = p.id,
                kitapAd = p.kitapAd,
                kitapYazar = p.kitapYazar,
                aktiflik = p.aktiflik,
                oduncDurum = p.oduncDurum,
                sayfaSayisi = p.sayfaSayisi
            }).ToList();
            ViewBag.uyeId = id;
            return View(kitaplar);
        }
        public ActionResult OduncMesaj(int uyeid, int kitapId)
        {
            kutuphane_24_02_24Entities model = new kutuphane_24_02_24Entities();
            var kisi = model.uyelerTBL.Where(p => p.id == uyeid).FirstOrDefault();
            var kitap = model.kitaplarTBL.Where(p => p.id == kitapId).FirstOrDefault();
            //Burada kendi mail ve şifremi sildim gerekli yerlere girilecek şeyleri yazdım
            var cred = new NetworkCredential("mailAdres", "Sifre");
            var client = new SmtpClient("smtp.gmail.com", 587);
            var msg = new System.Net.Mail.MailMessage();
            msg.To.Add(kisi.mail);
            msg.Subject = "İade";
            msg.Body = "Sayın " + kisi.adSoyad + " " + kitap.kitapAd + " " + "Adlı Kitap başarıyla Odunç  size edilmiştir teslim tarihiniz " + DateTime.Today.AddDays(20) + " iyi okumalar dileriz.";
            msg.IsBodyHtml = false;
            msg.From = new MailAddress("mail", "Ad", Encoding.UTF8);
            client.Credentials = cred; 
            client.EnableSsl = true;   
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            client.Send(msg);
            return RedirectToAction("islemSayfasi", new { id = uyeid });
        }
        //İşlemler tablosuna Bir işlem eklemek
        //Kitaplar tablosunda kitabın odunc durumunu değistirmek
        //Uyeler tablosunda kitapSayisinı 1 artırmak
        public ActionResult OduncVer(int kitapId, int uyeId)
        {
            kutuphane_24_02_24Entities model = new kutuphane_24_02_24Entities();
            //İşlemler tablosuna işlem eklemek
            islemler islem = new islemler()
            {
                islemTur = true,
                kitapID = kitapId,
                uyeID = uyeId,

            };
            model.islemlerTBL.Add(new islemlerTBL()
            {
                islemTur = islem.islemTur,
                kitapID = islem.kitapID,
                uyeID = islem.uyeID,
                islemTarihi = DateTime.Today
            });
            //Kitaplar Tablsosynda kitavın odunc durmnunu depiltirmek
            var kitapVerisi = model.kitaplarTBL.Where(p => p.id == kitapId).FirstOrDefault();
            kitapVerisi.oduncDurum = false;
            //Üyenin kitap sayısını 1 artırma
            var uyeVerisi = model.uyelerTBL.Where(p => p.id == uyeId).FirstOrDefault();
            uyeVerisi.kitapSayisi += 1;
            model.SaveChanges();
            OduncMesaj(uyeId, kitapId);
            return RedirectToAction("islemSayfasi", new { id = uyeId });
        }
    }
}