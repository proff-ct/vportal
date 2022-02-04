using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSacco_BLL.Controllers
{
    [Authorize]
    public class SMSController : Controller
    {
        // GET: SMS
        public ActionResult Index()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                try
                {

                    return View(GetSmsMv());
                }
                catch (Exception)
                {
                    //throw;
                    return View();
                }
            }
        }
        public ActionResult Sent()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                List<UploadMV> upload = new List<UploadMV>();
                try
                {

                    string user = User.Identity.GetUserId();
                    upload = db.FilePath.Where(x => x.UploadedBy.Id.Equals(user) && x.TransactionType.Name.Equals("sms")).ToList().Select(p => new UploadMV { DateUploaded = p.DateTime, FileName = p.FileName, UploadBy = p.UploadedBy.Email.Remove(p.UploadedBy.Email.IndexOf("@")), Status = p.Status }).OrderByDescending(x => x.DateUploaded).ToList();
                }
                catch (Exception)
                {
                    throw;
                }
                return View(upload);
            }
        }
        private SMSMV GetSmsMv()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                SMSMV upload = new SMSMV();
                string user = User.Identity.GetUserId();

                //upload.UploadedContact = upload.UploadedContact = db.FilePath.Where(x => x.UploadedBy.Id.Equals(user) && x.TransactionType.Name.Equals("sms") && x.Status.Equals("pending")).ToList().Select(p => new UploadMV { DateUploaded = p.DateTime, FileName = p.FileName, UploadBy = p.UploadedBy.Email, Status = p.Status }).ToList();
                return upload;
            }
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {

            try
            {
                //  if(file.)
            }
            catch (Exception)
            {
                throw;
            }

            return RedirectToAction("index", GetSmsMv());
        }


        [HttpPost]
        public ActionResult Index(SMSMV sms)
        {
            // Check that we have a Message to send else abort
            if (String.IsNullOrEmpty(sms.Message))
            {
                ModelState.AddModelError("Message", errorMessage: "Please specify the SMS Message to send");
                return View(sms);
            }

            string serverMessage = string.Empty, fileName = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(sms.Msisdn) || sms.File != null)
                {

                    //string user = User.Identity.GetUserId();
                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        string user = User.Identity.GetUserId();
                        ClientUser clientUser = db.ClientUser.FirstOrDefault(x => x.User.Id.Equals(user));
                        // 2 char of client name
                        fileName = (clientUser.Client.Name).Substring(0, 2);
                        // 2 char of user email
                        fileName += "-" + (clientUser.User.Email).Substring(0, 2);

                        fileName += "-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture) + "-" + Path.GetFileName(sms.File.FileName);

                        if (!UploadController.ValidateFile(fileName))
                        {
                            TempData["message"] = "Only Excel Files Are Allowed...In their original format as downloaded..Do not delete columns, do not add rows, do not modify them in any way...Try again.";
                            return RedirectToAction("index");
                        }

                        string path = Server.MapPath("~" + "\\file\\" + fileName);
                        sms.File.SaveAs(path);

                        FilePath fp = new FilePath
                        {
                            Path = path,
                            Status = Properties.Settings.Default.StatusFiled,
                            UploadedBy = db.Users.Find(User.Identity.GetUserId()),
                            DateTime = DateTime.Now,
                            FileName = fileName,
                            TransactionType = db.TransactionType.FirstOrDefault(x => x.Name.Equals("sms")),
                            Active = true
                        };
                        db.FilePath.Add(fp); db.SaveChanges();

                        SmsBatchFile sBF = new SmsBatchFile
                        {
                            Mesaage = sms.Message,
                            FilePath = db.FilePath.Find(fp.Id),
                            Status = Properties.Settings.Default.StatusFiled,
                        };
                        db.SmsBatchFile.Add(sBF);
                        db.SaveChanges();
                    }
                    return RedirectToAction("index");
                }
                else
                {
                    serverMessage = "No Contact found add or Attached contact list and Try Again. THANK YOU";
                }
            }
            catch (Exception ex)
            {
                serverMessage = ex.Message;
                //throw;
            }
            TempData["message"] = TempData["message"] = serverMessage;
            return RedirectToAction("index", GetSmsMv());
        }
    }

}
