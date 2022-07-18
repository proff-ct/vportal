using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Utilities;

namespace CallCenter_BLL.TransactionPortalServices
{
    namespace Models
    {
        namespace ASP_Identity
        {
            namespace IdentityConfig
            {
                public class TP_UserManager : UserManager<IdentityModels.TP_AppUser>
                {
                    public TP_UserManager(IUserStore<IdentityModels.TP_AppUser> store)
                        : base(store)
                    {
                    }

                    public static TP_UserManager Create(IdentityFactoryOptions<TP_UserManager> options, IOwinContext context)
                    {
                        TP_UserManager manager = new TP_UserManager(new UserStore<IdentityModels.TP_AppUser>(context.Get<IdentityModels.TP_AppDBContext>()));
                        // Configure validation logic for usernames
                        manager.UserValidator = new UserValidator<IdentityModels.TP_AppUser>(manager)
                        {
                            AllowOnlyAlphanumericUserNames = false,
                            RequireUniqueEmail = true
                        };

                        // Configure validation logic for passwords
                        manager.PasswordValidator = new PasswordValidator
                        {
                            RequiredLength = 6,
                            RequireNonLetterOrDigit = true,
                            RequireDigit = true,
                            RequireLowercase = true,
                            RequireUppercase = true,
                        };

                        // Configure user lockout defaults
                        manager.UserLockoutEnabledByDefault = true;
                        manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
                        manager.MaxFailedAccessAttemptsBeforeLockout = 5;

                        // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
                        // You can write your own provider and plug it in here.
                        manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<IdentityModels.TP_AppUser>
                        {
                            MessageFormat = "Your security code is {0}"
                        });
                        manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<IdentityModels.TP_AppUser>
                        {
                            Subject = "Security Code",
                            BodyFormat = "Your security code is {0}"
                        });
                        //manager.EmailService = new EmailService();
                        //manager.SmsService = new SmsService();
                        //var dataProtectionProvider = options.DataProtectionProvider;
                        //if (dataProtectionProvider != null)
                        //{
                        //    manager.UserTokenProvider =
                        //        new DataProtectorTokenProvider<IdentityModels.TP_AppUser>(dataProtectionProvider.Create("ASP.NET Identity"));
                        //}
                        return manager;
                    }
                }
            }
            namespace IdentityModels
            {
                public class TP_AppDBContext : IdentityDbContext<TP_AppUser>
                {
#if DEBUG
                    public TP_AppDBContext()
                        : base(Connection.TestingConnectionString, throwIfV1Schema: false)
                    {
                    }
#else
                    public TP_AppDBContext()
                        : base(@ConfigurationManager.ConnectionStrings[MS_DBConnectionStrings.TransactionPortalDBConnectionStringName].ConnectionString, throwIfV1Schema: false)
                    {
                    }
#endif

                    public DbSet<ShortCodeType> ShortCodeType { get; set; }
                    public DbSet<ClientShortCode> ClientShortCode { get; set; }
                    public DbSet<Client> Client { get; set; }
                    public DbSet<ClientUser> ClientUser { get; set; }
                    public DbSet<FilePath> FilePath { get; set; }
                    public DbSet<SmsPhone> SmsPhone { get; set; }
                    public DbSet<TransactionType> TransactionType { get; set; }
                    public DbSet<SmsBatchFile> SmsBatchFile { get; set; }
                    public DbSet<MessageBatch> Message { get; set; }

                    public static TP_AppDBContext Create()
                    {
                        return new TP_AppDBContext();
                    }
                }
                public class TP_AppUser : IdentityUser
                {
                    public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<TP_AppUser> manager)
                    {
                        // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
                        ClaimsIdentity userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
                        // Add custom user claims here
                        return userIdentity;
                    }
                }
            }
        }

        public class Client
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public bool Active { get; set; }
            public bool HasCredential { get; set; }
            public string CorporateNo { get; set; }
        }
        public class ClientUser
        {
            public int Id { get; set; }
            public virtual Client Client { get; set; }
            public virtual ASP_Identity.IdentityModels.TP_AppUser User { get; set; }
            public bool Active { get; set; }

        }
        public class ClientShortCode
        {
            public int Id { get; set; }
            public string ShorCode { get; set; }
            public virtual Client Client { get; set; }
            public virtual ShortCodeType CodeType { get; set; }
            public bool Active { get; set; }
        }
        public class ShortCodeType
        {
            public int Id { get; set; }
            public string CodeType { get; set; }
        }
        public class FilePath
        {
            public int Id { get; set; }
            public string Path { get; set; }
            public DateTime DateTime { get; set; }
            public virtual ASP_Identity.IdentityModels.TP_AppUser UploadedBy { get; set; }
            public string Status { get; set; }
            public string FileName { get; set; }
            public bool Active { get; set; }
            public virtual TransactionType TransactionType { get; set; }
        }

        public class SmsPhone
        {
            public string ID { get; set; }
            public string Phone { get; set; }
            public string Status { get; set; }
            public string Sms { get; set; }
            public virtual MessageBatch Message { get; set; }
        }
        public class SmsBatchFile
        {
            public int Id { get; set; }
            public string Msisdn { get; set; }
            public string Mesaage { get; set; }
            public string Status { get; set; }
            public virtual FilePath FilePath { get; set; }
        }
        public class MessageBatch
        {
            public int Id { get; set; }
            public string Messsages { get; set; }
            public virtual SmsBatchFile SmsBatchFile { get; set; }

        }

        public class TransactionType
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class SMSMV
        {
            public string Msisdn { get; set; }
            [Required]
            [DataType(DataType.MultilineText)]
            public string Message { get; set; }
            public string BatchName { get; set; }
            public HttpPostedFileBase File { get; set; }
            //public List<UploadMV> UploadedContact = new List<UploadMV>();
            //public int GroupId { get; set; }
            //public List<SelectListItem> GroupList = new List<SelectListItem>();

        }

        public class VisibilityPortal_SMSMV
        {
            [Required]
            [DataType(DataType.MultilineText)]
            public string Message { get; set; }

            public List<string> Recipients { get; set; }

            public string SMSFileName { get; set; }
        }

        public class VisibilityPortal_SaccoInfo
        {
            public string CorporateNo { get; set; }
            public string SaccoName { get; set; }
        }

        public class TransactionPortalSMSService
        {
            private const string BULK_SMS_FILE_STORAGE_PATH_NAME = "BulkSMSFilePath";
            private ASP_Identity.IdentityConfig.TP_UserManager TPUserManager
            {
                get;
                //_tpUserManager ?? HttpContext.GetOwinContext().GetUserManager<ASP_Identity.IdentityConfig.TP_UserManager>();

                set;
            }
            public TransactionPortalSMSService(IOwinContext owinContext)
            {
                TPUserManager = owinContext.GetUserManager<ASP_Identity.IdentityConfig.TP_UserManager>();
            }
            private string GenerateFileName(string tpUserID, string smsFileName)
            {
                // Set file extenstion - Defaulting to xls files
                int idxFileExtension = smsFileName.LastIndexOf('.');
                smsFileName = string.Format("{0}.xls", smsFileName.Substring(0, idxFileExtension));

                using (ASP_Identity.IdentityModels.TP_AppDBContext db = new ASP_Identity.IdentityModels.TP_AppDBContext())
                {
                    ClientUser clientUser = db.ClientUser.FirstOrDefault(x => x.User.Id.Equals(tpUserID));
                    // 2 char of client name
                    string fileName = clientUser.Client.Name.Substring(0, 2);
                    // 2 char of user email
                    fileName += "-" + clientUser.User.Email.Substring(0, 2);

                    fileName += "-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture) + "-" + smsFileName;

                    return fileName;
                }
            }

            private Client NewClient(Client client)
            {

                using (ASP_Identity.IdentityModels.TP_AppDBContext db = new ASP_Identity.IdentityModels.TP_AppDBContext())
                {
                    if (db.Client.Any(x => x.Name.Equals(client.Name)))
                    {
                        return db.Client.FirstOrDefault(x => x.Name.Equals(client.Name));
                    }

                    db.Client.Add(client);
                    db.SaveChanges();
                    return client;
                }


            }

            private ClientShortCode ClientShortCode(ClientShortCode code)
            {

                using (ASP_Identity.IdentityModels.TP_AppDBContext db = new ASP_Identity.IdentityModels.TP_AppDBContext())
                {
                    if (db.ClientShortCode.Any(x => x.ShorCode.Equals(code.ShorCode)))
                    {
                        return db.ClientShortCode.FirstOrDefault(x => x.ShorCode.Equals(code.ShorCode));
                    }

                    //code.CodeType = db.ShortCodeType.First(sc => sc.CodeType == "C2B");
                    db.ClientShortCode.Add(code);

                    //db.ClientShortCode.Add(new ClientShortCode { Active = true, CodeType = db.ShortCodeType.Find(code.CodeType.Id), Client = db.Client.Find(code.Client.Id), ShorCode = code.ShorCode });
                    db.SaveChanges();
                    return code;
                }

            }
            private ClientUser RegisterUserAccount(string userEmail, string businessShortCode)
            {
                ASP_Identity.IdentityModels.TP_AppUser user = new ASP_Identity.IdentityModels.TP_AppUser { UserName = userEmail, Email = userEmail };
                IdentityResult result = TPUserManager.Create(user, "Msee!LazimaPasswordIsikosekanangeUsipangwingwe2022+");
                if (result.Succeeded)
                {

                    using (ASP_Identity.IdentityModels.TP_AppDBContext db = new ASP_Identity.IdentityModels.TP_AppDBContext())
                    {
                        Client sacco = db.Client.FirstOrDefault(c => c.CorporateNo == businessShortCode);
                        if (db.ClientUser.Any(x => x.User.Email.Equals(userEmail)) || sacco == null)
                        {
                            return null;
                        }

                        ClientUser clUser = new ClientUser
                        {
                            Active = true,
                            Client = sacco,
                            User = db.Users.FirstOrDefault(x => x.UserName.Equals(userEmail))
                        };
                        db.ClientUser.Add(clUser);
                        db.SaveChanges();
                        
                        return clUser;
                    }
                }
                throw new ApplicationException($"Failed registering user {userEmail} on Transaction portal");
            }
            private static string GetFileStorageLocation()
            {
                string storageLocation = string.Empty;

                NameValueCollection section = @ConfigurationManager.GetSection("portalSecrets") as NameValueCollection;
                if (section != null && section[BULK_SMS_FILE_STORAGE_PATH_NAME] != null)
                {
                    storageLocation = section[BULK_SMS_FILE_STORAGE_PATH_NAME];
                }

                if (string.IsNullOrEmpty(storageLocation))
                {
                    storageLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
                }

                return storageLocation;
            }
            public bool UploadBulkSMS(VisibilityPortal_SMSMV sms, string userEmail, VisibilityPortal_SaccoInfo saccoInfo)
            {
                using (ASP_Identity.IdentityModels.TP_AppDBContext db = new ASP_Identity.IdentityModels.TP_AppDBContext())
                {
                    bool uploadSuccessful = false;
                    string filePath;
                    string fileName;

                    ClientUser clientUser = db.ClientUser.FirstOrDefault(x => x.User.Email.Equals(userEmail));
                    if (clientUser == null)
                    {
                        // create user if client not exist
                        Client sacco = db.Client.FirstOrDefault(c => c.CorporateNo == saccoInfo.CorporateNo);
                        if (sacco == null)
                        {
                            sacco = NewClient(new Client
                            {
                                CorporateNo = saccoInfo.CorporateNo,
                                Name = saccoInfo.SaccoName,
                                Active = true
                            });
                            
                            ClientShortCode(new ClientShortCode
                            {
                                Client = sacco,
                                ShorCode = sacco.CorporateNo,
                                Active = true
                            });
                        }
                        clientUser = RegisterUserAccount(userEmail, sacco.CorporateNo);
                    }

                    // create file
                    IWorkbook xlsWorkBook = new HSSFWorkbook();
                    ISheet xlsSheet = xlsWorkBook.CreateSheet("SMS Recipient List");

                    int numContacts = sms.Recipients.Count, IDX_PHONE_NO_CELL = 0;
                    for (int i = 0; i < numContacts; i++)
                    {
                        IRow row = xlsSheet.CreateRow(i);
                        ICell cell = row.CreateCell(IDX_PHONE_NO_CELL);

                        cell.SetCellValue(sms.Recipients[i]);
                    }

                    try
                    {
                        fileName = GenerateFileName(clientUser.User.Id, sms.SMSFileName);
                        filePath = GetFileStorageLocation();
                        string fullFilePath = Path.Combine(filePath, fileName);
                        using (FileStream stream = new FileStream(fullFilePath, FileMode.Create, FileAccess.Write))
                        {
                            xlsWorkBook.Write(stream);
                        }

                        // update db accordingly
                        FilePath fp = new FilePath
                        {
                            Path = fullFilePath,
                            Status = "Filed",
                            UploadedBy = db.Users.Find(clientUser.User.Id),
                            DateTime = DateTime.Now,
                            FileName = fileName,
                            TransactionType = db.TransactionType.FirstOrDefault(x => x.Name.Equals("sms")),
                            Active = true
                        };
                        db.FilePath.Add(fp);
                        db.SaveChanges();

                        SmsBatchFile sBF = new SmsBatchFile
                        {
                            Mesaage = sms.Message,
                            FilePath = db.FilePath.Find(fp.Id),
                            Status = "Filed"
                        };
                        db.SmsBatchFile.Add(sBF);
                        db.SaveChanges();

                        uploadSuccessful = true;
                    }
                    catch (ConfigurationException configEx)
                    {
                        Services.EmailService tpEmailService = new Services.EmailService();
                        tpEmailService.SendEmail(
                            "Missing Configuration For SMS Processing from Visibility Portal",
                            $"Update web.config or relevant config file since {clientUser.User.Email} of {saccoInfo.SaccoName} has tried sending bulk sms but a config exception has occurred: {configEx.Message} with ST: {configEx.StackTrace}",
                            "madote@coretec.co.ke"
                            );

                        throw new ApplicationException("SMS service is unable to process this request at the moment");
                    }
                    catch(DirectoryNotFoundException dirNFException)
                    {
                        Services.EmailService tpEmailService = new Services.EmailService();
                        tpEmailService.SendEmail(
                            "Folder for storing uploaded SMS files NOT YET CREATED for Visibility Portal",
                            $"Create folder since {clientUser.User.Email} of {saccoInfo.SaccoName} has tried sending bulk sms but the folder is not yet ready i.e. {dirNFException.Message} with ST: {dirNFException.StackTrace}",
                            "madote@coretec.co.ke"
                            );

                        throw new ApplicationException("SMS service is unable to process this request at the moment");
                    }

                    return uploadSuccessful;
                }
            }
        }

    }

    namespace Services
    {
        public class EmailService : IIdentityMessageService
        {
            public Task SendAsync(IdentityMessage message)
            {
                // Plug in your email service here to send an email.
                sendongmail(message.Subject, message.Body, message.Destination);
                SendEmail(message.Subject, message.Body, message.Destination);

                //gmailgateway(message.Subject, message.Body, message.Destination);
                return Task.FromResult(0);
            }
            public void SendEmail(string Subject, string Body, string mailTo)
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                mail.From = new MailAddress("coretec.msacco@gmail.com");
                mail.To.Add(mailTo);
                mail.CC.Add("kwachira@coretec.co.ke");
                mail.CC.Add("fkaigwa@coretec.co.ke");
                // mail.To.Add("aingosi@coretec.co.ke");

                mail.Subject = Subject;
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpServer.Port = 587;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new NetworkCredential("coretec.msacco@gmail.com", "!Team@123");
                SmtpServer.EnableSsl = true;
                try
                {
                    SmtpServer.Send(mail);
                }
                catch
                {
                    try
                    {
                        SmtpServer.Send(mail);
                    }
                    catch
                    {
                        try
                        {
                            SmtpServer.Send(mail);
                        }
                        catch (Exception ex)
                        {
                            try
                            {

                                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                                { return true; };
                                SmtpServer.Send(mail);
                            }
                            catch (Exception e)
                            {
                                try
                                {
                                    ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                                    { return true; };
                                    mail.To.Clear();
                                    mail.To.Add("coretectests@gmail.com");
                                    mail.Subject = "Report Not sent: Your service is not working Man";
                                    mail.Body = "Report Dated " + DateTime.Now.AddDays(-1).ToLongDateString() + "was not sent successfully imagine..Ebu angalia";
                                    if (mail.Attachments.Count() > 0) { mail.Attachments.Clear(); }
                                    SmtpServer.Send(mail);
                                }
                                catch
                                {
                                    //log error to db
                                }
                            }
                        }
                    }
                }
            }
            private void gmailgateway(string subject, string body, string sentto)
            {
                try
                {
                    var fromAddress = new MailAddress("coretec.msacco@gmail.com", "From coretec mobility");
                    var toAddress = new MailAddress(sentto, "To Name");
                    const string fromPassword = "!Team@123";
                    //const string subject1 = ""+subject;
                    //const string body1 = "";

                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                        Timeout = 20000
                    };
                    using (var message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = subject,
                        Body = body
                    })
                    {
                        smtp.Send(message);
                    }
                }
                catch (Exception ex)
                {

                    //throw;
                }
            }
            public void sendongmail(string subject, string body, string emailto)
            {
                try
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                    mail.Headers.Add("Message-Id", String.Concat("<", DateTime.Now.ToString("yyMMdd"), ".", DateTime.Now.ToString("HHmmss"), "@gmail.com>"));
                    mail.From = new MailAddress("m.sacco.coretec@gmail.com");

                    mail.To.Add(emailto);

                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;
                    SmtpServer.Port = 587;
                    SmtpServer.Credentials = new System.Net.NetworkCredential("m.sacco.coretec", "msacco20rsnma7771@#");
                    SmtpServer.EnableSsl = true;
                    SmtpServer.Send(mail);
                }
                catch (Exception ex)
                {

                }
            }
        }
    }

}
