using Hangfire;
using HF.UploadFile.Entity;
using Newtonsoft.Json;

namespace HF.Admin
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        readonly string cn_qa = "Server=(local)\\SQLEXPRESS;Database=HF_QA;User Id=sa;Password=sql12345;";
        readonly string cn_prod = "Server=(local)\\SQLEXPRESS;Database=HF_PROD;User Id=sa;Password=sql12345;";
        readonly string archivos_compartidos = @"C:\inetpub\ArchivosCompartidos";

        enum MODULES
        {
            UPLOAD_FILE,
            UPLOAD_FILE_TO_FTP
        }

        private void LoadModules()
        {
            cboModules.DataSource = Enum.GetValues(typeof(MODULES));
        }

        void Inyectar_UploadFile()
        {
            using (UploadFile.Inicio objL = new UploadFile.Inicio())
            {
                var bucketStorage = new BucketStorage
                {
                    proyect_id = "pe-gcp-01",
                    json_file = "credentials_gcp.json",
                    ruta_gs = "",
                    ruta_local = archivos_compartidos
                };

                if (chkIsProd.Checked)
                {
                    bucketStorage.bucket_name = "file-batch-repository-prod";
                }
                else
                {
                    bucketStorage.bucket_name = "file-batch-repository-qa";
                }

                RecurringJob.AddOrUpdate(
                                   "Upload file", // Nombre del job
                                   () => objL.UploadFile(null, bucketStorage, CancellationToken.None), "0 4 * * *", //"0 0 0 ? 2 MON#5", // la llamada
                                    TimeZoneInfo.Local // 
                                   , queue: "upload_file"
                                   );

            }
        }

        void Inyectar_UploadFileToFTP()
        {
            using (UploadFile.Inicio objL = new UploadFile.Inicio())
            {
                var ftpStorage = new FTPStorage
                {
                    server = "ftp-example.pe",
                    user = "user-example",
                    pass = "123456",
                    port = 22
                };

                if (chkIsProd.Checked)
                {

                }
                else
                {

                }

                RecurringJob.AddOrUpdate(
                                   "Upload file to FTP", // Nombre del job
                                   () => objL.UploadFileToFTP(null, ftpStorage, CancellationToken.None), "0 4 * * *", //"0 0 0 ? 2 MON#5", // la llamada
                                    TimeZoneInfo.Local // 
                                   , queue: "upload_file_to_ftp"
                                   );

            }
        }

        private void SetConfig()
        {
            var path = Path.Combine(Application.StartupPath, "appsettings.json");
            var is_prod_new = chkIsProd.Checked ? "true" : "false";

            var configuracion_new = new Configuration() { is_prod = is_prod_new };

            var json_new = JsonConvert.SerializeObject(configuracion_new, Formatting.Indented);

            File.WriteAllText(path, json_new);

            if (chkIsProd.Checked)
            {
                GlobalConfiguration.Configuration.UseSqlServerStorage(cn_prod);
            }
            else
            {
                GlobalConfiguration.Configuration.UseSqlServerStorage(cn_qa);
            }

            chkIsProd.Enabled = false;

        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            try
            {
                SetConfig();

                var module = (MODULES)cboModules.SelectedValue;

                switch (module)
                {
                    case MODULES.UPLOAD_FILE:
                        TestUploadFileToStorage();
                        break;
                    case MODULES.UPLOAD_FILE_TO_FTP:
                        TestUploadFileToFTP();
                        break;
                    default:
                        break;
                }

                } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnInyect_Click(object sender, EventArgs e)
        {
            try
            {
                SetConfig();

                var module = (MODULES)cboModules.SelectedValue;

                switch (module)
                {
                    case MODULES.UPLOAD_FILE:
                        Inyectar_UploadFile();
                        break;
                    case MODULES.UPLOAD_FILE_TO_FTP:
                        Inyectar_UploadFileToFTP();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void TestUploadFileToStorage()
        {
            using (UploadFile.Inicio objL = new UploadFile.Inicio())
            {
                BucketStorage bucketStorage = new BucketStorage
                {
                    proyect_id = "pe-gcp-duemint-01",
                    json_file = "credentials_gcp_duemint.json",
                    ruta_gs = "DUEMINT",
                    ruta_local = archivos_compartidos
                };

                if (chkIsProd.Checked)
                {
                    bucketStorage.bucket_name = "promotions-batch-repository-prod";
                }
                else
                {
                    bucketStorage.bucket_name = "promotions-batch-repository-qa";
                }

                _ = objL.UploadFile(null, bucketStorage, CancellationToken.None);

            }
        }

        void TestUploadFileToFTP()
        {
            using (UploadFile.Inicio objL = new UploadFile.Inicio())
            {
                var ftpStorage = new FTPStorage
                {
                    //server = "ftp.gnu.org",
                    server = "ftp.example.com",
                    user = "anonymous",
                    pass = "",
                    port = 21,
                    ruta_local = archivos_compartidos
                };

                _ = objL.UploadFileToFTP(null, ftpStorage, CancellationToken.None);

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadModules();
        }
    }
}