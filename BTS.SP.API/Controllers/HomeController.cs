using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BTS.SP.API.Controllers
{
    public class Record
    {
        public string FName { get; set; }
        public string LName { get; set; }
        public string Address { get; set; }
    }
    public class HomeController : Controller
    {
       
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(int id = 0)
        {
            List<Record> obj = new List<Record>();
            obj = RecordInfo();
            StringBuilder str = new StringBuilder();
            str.Append("<table border=`" + "1px" + "`b>");
            str.Append("<tr>");
            str.Append("<td><b><font face=Arial Narrow size=3>FName</font></b></td>");
            str.Append("<td><b><font face=Arial Narrow size=3>LName</font></b></td>");
            str.Append("<td><b><font face=Arial Narrow size=3>Address</font></b></td>");
            str.Append("</tr>");
            foreach (Record val in obj)
            {
                str.Append("<tr style=\"border: 1px solid black\">");
                str.Append("<td><font face=Arial Narrow size=" + "14px" + ">" + val.FName.ToString() + "</font></td>");
                str.Append("<td><font face=Arial Narrow size=" + "14px" + ">" + val.LName.ToString() + "</font></td>");
                str.Append("<td><font face=Arial Narrow size=" + "14px" + ">" + val.Address.ToString() + "</font></td>");
                str.Append("</tr>");
            }
            str.Append("</table>");
            HttpContext.Response.AddHeader("content-disposition", "attachment; filename=Information" + DateTime.Now.Year.ToString() + ".xls");
            this.Response.ContentType = "application/vnd.ms-excel";
            byte[] temp = System.Text.Encoding.UTF8.GetBytes(str.ToString());
            return File(temp, "application/vnd.ms-excel");
        }
        public List<Record> RecordInfo()
        {
            List<Record> recordobj = new List<Record>();
            recordobj.Add(new Record { FName = "Smith", LName = "Singh", Address = "Knpur" });
            recordobj.Add(new Record { FName = "John", LName = "Kumar", Address = "Lucknow" });
            recordobj.Add(new Record { FName = "Vikram", LName = "Kapoor", Address = "Delhi" });
            recordobj.Add(new Record { FName = "Tanya", LName = "Shrma", Address = "Banaras" });
            recordobj.Add(new Record { FName = "Malini", LName = "Ahuja", Address = "Gujrat" });
            recordobj.Add(new Record { FName = "Varun", LName = "Katiyar", Address = "Rajasthan" });
            recordobj.Add(new Record { FName = "Arun  ", LName = "Singh", Address = "Jaipur" });
            recordobj.Add(new Record { FName = "Ram", LName = "Kapoor", Address = "Panjab" });
            recordobj.Add(new Record { FName = "Vishakha", LName = "Singh", Address = "Banglor" });
            recordobj.Add(new Record { FName = "Tarun", LName = "Singh", Address = "Kannauj" });
            recordobj.Add(new Record { FName = "Mayank", LName = "Dubey", Address = "Farrukhabad" });
            return recordobj;
        }
    }
}