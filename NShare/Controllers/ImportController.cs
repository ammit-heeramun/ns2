using NShare.Models;
using NShare.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace NShare.Controllers
{
    public class ImportController : Controller
    {
        private NShareEntities db = new NShareEntities();
        // GET: Import
        public ActionResult Import()
        {
            return View();
        }

        public ActionResult GetData()
        {
            List<Share> shareList = db.Shares.ToList<Share>();


            return Json(new { data = shareList }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult List()
        {
            List<Share> shareList = db.Shares.ToList<Share>();

            var sharesViewModal = new SharesViewModal
            {
                Shares=shareList
            };

            return View(sharesViewModal);

        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase xmlFile)
        {
            string[] arrError = new string[] { };

            if (xmlFile.ContentType.Equals("application/xml") || xmlFile.ContentType.Equals("text/xml"))
            {
                var xmlPath = Server.MapPath("~/FileUpload/" + xmlFile.FileName);
                xmlFile.SaveAs(xmlPath);
                XDocument xDoc = XDocument.Load(xmlPath);
                List<Share> shareList = readXML(xDoc).ToList();


                int x = 0;

                foreach (var i in shareList)
                {

                    if (i.quantity <= 0)
                    {
                        arrError[x] = "Error: Share ID:" + i.id + " quantity is less than ZERO or equal to ZERO.";
                        continue;
                    }

                    if (i.unit_price < 0)
                    {
                        arrError[x] = "Error: Share ID:" + i.id + " unit price is less than ZERO.";
                        continue;
                    }
                    var v = db.Shares.Where(a => a.id.Equals(i.id)).FirstOrDefault();

                    if (v != null)
                    {
                        v.id = i.id;
                        v.name = i.name;
                        v.quantity = i.quantity;
                        v.unit_price = i.unit_price;
                        v.closed_date = i.closed_date;
                    }
                    else
                    {
                        db.Shares.Add(i);
                    }
                    db.SaveChanges();
                    x++;
                }

                if (arrError.Length > 0)
                {
                    ViewBag.Error = "File have partial errors";
                }
                else
                {
                    ViewBag.Success = "File uploaded successfully..";
                }

            }
            else
            {
                ViewBag.Error = "Invalid file(Upload xml file only)";
            }

            return View("Import");
        }

        private static IEnumerable<Share> readXML(XDocument xDoc)
        {
            return xDoc.Descendants("share").Select
                            (share => new Share
                            {
                                id = Convert.ToInt32(share.Element("id").Value),
                                name = share.Element("name").Value,
                                quantity = Convert.ToInt32(share.Element("quantity").Value),
                                unit_price = Convert.ToInt32(share.Element("unit_price").Value),
                                closed_date = Convert.ToDateTime(string.IsNullOrEmpty(share.Element("closed_date").Value) ? null : share.Element("closed_date").Value)
                            });


        }
    }

}