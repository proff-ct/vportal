using IronXL;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using MSacco_Dataspecs.MSSQLOperators;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MSacco_BLL.TransactionPortalServices
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
    public ApplicationDbContext()
        : base(Connection.ProductionConnectionString, throwIfV1Schema: false)
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

        public static class TransactionPortalSMSService
        {
            private const string BULK_SMS_FILE_STORAGE_PATH_NAME = "BulkSMSFilePath";
            private static ASP_Identity.IdentityConfig.TP_UserManager _tpUserManager;
            public static ASP_Identity.IdentityConfig.TP_UserManager TPUserManager
            {
                get =>
                    //return _tpUserManager ?? HttpContext.GetOwinContext().GetUserManager<ASP_Identity.IdentityConfig.TP_UserManager>();
                    _tpUserManager;
                private set => _tpUserManager = value;
            }
            static TransactionPortalSMSService()
            {

            }
            private static string GenerateFileName(string tpUserID, string smsFileName)
            {
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

            private static Client NewClient(Client client)
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

            private static ClientShortCode ClientShortCode(ClientShortCode code)
            {

                using (ASP_Identity.IdentityModels.TP_AppDBContext db = new ASP_Identity.IdentityModels.TP_AppDBContext())
                {
                    if (db.ClientShortCode.Any(x => x.ShorCode.Equals(code.ShorCode)))
                    {
                        return db.ClientShortCode.FirstOrDefault(x => x.ShorCode.Equals(code.ShorCode));
                    }

                    code.CodeType = db.ShortCodeType.First(sc => sc.CodeType == "C2B");
                    db.ClientShortCode.Add(code);

                    //db.ClientShortCode.Add(new ClientShortCode { Active = true, CodeType = db.ShortCodeType.Find(code.CodeType.Id), Client = db.Client.Find(code.Client.Id), ShorCode = code.ShorCode });
                    db.SaveChanges();
                    return code;
                }

            }
            private static ClientUser RegisterUserAccount(string userEmail, string businessShortCode)
            {
                ASP_Identity.IdentityModels.TP_AppUser user = new ASP_Identity.IdentityModels.TP_AppUser { UserName = userEmail, Email = userEmail };
                IdentityResult result = TPUserManager.Create(user, "MseeLazimaPasswordIsikosekanangeUsipangingwe");
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


                return null;
            }
            private static string GetFileStorageLocation()
            {
                string storageLocation = string.Empty;

                NameValueCollection section = @ConfigurationManager.GetSection("portalSecrets") as NameValueCollection;
                if(section!=null && section[BULK_SMS_FILE_STORAGE_PATH_NAME] != null)
                {
                    storageLocation = section[BULK_SMS_FILE_STORAGE_PATH_NAME];
                }

                if (string.IsNullOrEmpty(storageLocation))
                {
                    storageLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
                }

                return storageLocation;
            }
            public static bool UploadBulkSMS(VisibilityPortal_SMSMV sms, string userEmail, VisibilityPortal_SaccoInfo saccoInfo)
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

                        RegisterUserAccount(userEmail, sacco.CorporateNo);
                    }

                    // create file
                    WorkBook xlsWorkBook = new WorkBook(ExcelFileFormat.XLS);
                    WorkSheet xlsSheet = xlsWorkBook.CreateWorkSheet("SMS Recipient List");

                    int numContacts = sms.Recipients.Count;
                    for (int i = 1; i <= numContacts; i++)
                    {
                        xlsSheet[$"A{i}"].Value = sms.Recipients[i];
                    }

                    fileName = GenerateFileName(clientUser.User.Id, sms.SMSFileName);
                    filePath = GetFileStorageLocation();
                    
                    xlsWorkBook.SaveAs($"{filePath}/{fileName}");
                    
                    // update db accordingly
                    FilePath fp = new FilePath
                    {
                        Path = filePath,
                        Status = "Filed",
                        UploadedBy = db.Users.Find(clientUser.Id),
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


                    return uploadSuccessful;
                }
            }
        }

    }


}
