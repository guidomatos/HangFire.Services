using Common;
using Hangfire.Console;
using Hangfire.Server;
using HF.UploadFile.Data;
using HF.UploadFile.Entity;
using Renci.SshNet;
using System.Net;
using System.Reflection;

namespace HF.UploadFile.Logic
{
    public class Logic : IDisposable
    {
        private HFConfig _hfConfig;
        string PATH_LOCAL = string.Empty; //(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase)).ToString().Replace("file:\\", "");
        string PATH_EXE = (Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase)).ToString().Replace("file:\\", "");
        string subFolder = "ORSAN";

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            GC.Collect();
        }

        internal async Task TransferExcel(PerformContext context, BucketStorage bstorage, CancellationToken ctoken)
        {
            try
            {
                string archivo_excel = DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xlsx";
                PATH_LOCAL = bstorage.ruta_local;


                _hfConfig = new HFConfig();

                Log.WriteLine(context, "Iniciará el Flujo: is_prod: " + _hfConfig.IsProd);
                Log.WriteLine(context, "Archivo a generar: " + archivo_excel);
                Log.WriteLine(context, "Ruta archivo: " + PATH_LOCAL);
                Log.WriteLine(context, "Ruta credenciales: " + bstorage.json_file);
                Log.WriteLine(context, "Bucket Name: " + bstorage.bucket_name);

                int total_acumulado = 0;

                using (DataSql objD = new DataSql(_hfConfig.SqlConfig))
                {
                    Log.WriteLine(context, "Ejecutando POSTPRECIOTOMAKRO...");

                    byte[] excel_ready = new byte[0];

                    while (true)
                    {

                        var dataPendiente = await objD.ExecuteObjetoAlter();
                        var totalRegistros = dataPendiente.Count;

                        total_acumulado += totalRegistros;

                        if (totalRegistros == 0)
                        {
                            Log.WriteLine(context, "No hay más registros que enviar");
                            break;
                        }

                        Log.WriteLine(context, "Escribiendo en excel " + dataPendiente.Count.ToString("N0"));

                        excel_ready = await ConvertToExcel(dataPendiente, archivo_excel);

                        break;

                    }

                    if (excel_ready.Length > 0)
                    {
                        Log.WriteLine(context, "Subiendo archivo xls a ...");
                        
                        //bool ok = await UploadFileToGCP(archivo_excel, bstorage);
                        //if (ok)
                        //{
                        //    Log.WriteLine(context, "Archivo xls subido con Éxito!");
                        //}
                        //else
                        //{
                        //    Log.WriteLine(context, "Error al subir");
                        //}
                        
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(context, "Error Excepcion: " + ex.Message);
            }
            finally
            {
                Log.WriteLine(context, "Finalizo el flujo");
            }

        }


        internal async Task TransferExcelToFTP(PerformContext context, FTPStorage ftpstorage, CancellationToken ctoken)
        {
            try
            {
                string archivo_excel = DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xlsx";
                PATH_LOCAL = ftpstorage.ruta_local;


                _hfConfig = new HFConfig();

                Log.WriteLine(context, "Iniciará el Flujo: is_prod: " + _hfConfig.IsProd);
                Log.WriteLine(context, "Archivo a generar: " + archivo_excel);
                Log.WriteLine(context, "Ruta archivo: " + PATH_LOCAL);

                int total_acumulado = 0;

                using (DataSql objD = new DataSql(_hfConfig.SqlConfig))
                {
                    Log.WriteLine(context, "Ejecutando...");

                    byte[] excel_ready = new byte[0];

                    while (true)
                    {

                        var dataPendiente = await objD.ExecuteObjetoAlter();
                        var totalRegistros = dataPendiente.Count;

                        total_acumulado += totalRegistros;

                        if (totalRegistros == 0)
                        {
                            Log.WriteLine(context, "No hay más registros que enviar");
                            break;
                        }

                        Log.WriteLine(context, "Escribiendo en excel " + dataPendiente.Count.ToString("N0"));

                        excel_ready = await ConvertToExcel(dataPendiente, archivo_excel);

                        break;

                    }

                    if (excel_ready.Length > 0)
                    {
                        Log.WriteLine(context, "Subiendo archivo xls a FTP...");
                        //Log.WriteLine(context, $"Server FTP: {ftpstorage.server}", ConsoleTextColor.Cyan);
                        //Log.WriteLine(context, $"User FTP: {ftpstorage.user}", ConsoleTextColor.Cyan);
                        //Log.WriteLine(context, $"Pass FTP: {ftpstorage.pass}", ConsoleTextColor.Cyan);
                        //Log.WriteLine(context, $"Port FTP: {ftpstorage.port}", ConsoleTextColor.Cyan);
                        //Log.WriteLine(context, $"Ruta Local FTP: {ftpstorage.ruta_local}", ConsoleTextColor.Cyan);
                        

                        var respuestaSFTP = UploadFileToFTP(archivo_excel, subFolder, "Repositorio_Listado_Documentos", ftpstorage, context, "orsan.xls");

                        //var respuestaFTP = await UploadFileToFTP2(archivo_excel, subFolder, ftpstorage, context);
                        //Log.WriteLine(context, $"DescripcionStatus FTP: {respuestaFTP.descripcion}", ConsoleTextColor.Cyan);
                        //Log.WriteLine(context, $"Intento FTP: {respuestaFTP.intento}", ConsoleTextColor.Cyan);
                        //Log.WriteLine(context, $"Despedida FTP: {respuestaFTP.despedida}", ConsoleTextColor.Cyan);

                        if (respuestaSFTP.ok)
                        {
                            Log.WriteLine(context, "Archivo excel subido con Éxito!", ConsoleTextColor.Green);
                        }
                        else
                        {
                            Log.WriteLine(context, "Error al subir FTP", ConsoleTextColor.Red);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(context, "Error Excepcion: " + ex.Message);
            }
            finally
            {
                Log.WriteLine(context, "Finalizo el flujo");
            }

        }

        private async Task<byte[]> ConvertToExcel(List<ObjectAlter> data, string archivo)
        {
            try
            {

                string folder = Path.Combine(subFolder);
                string dia = DateTime.Today.ToString("dd_MM_yyyy");

                string ruta_local = Path.Combine(PATH_LOCAL, folder, dia, archivo);

                if (!Directory.Exists(Path.Combine(PATH_LOCAL, folder, dia)))
                {
                    Directory.CreateDirectory(Path.Combine(PATH_LOCAL, folder, dia));
                }

                byte[] excel_stream = ExcelUtil.BuildXlsxFile(data);

                await File.WriteAllBytesAsync(ruta_local, excel_stream);

                return excel_stream;

            }
            catch (Exception)
            {
                throw;
            }
        }

        private FTPRespuesta UploadFileToFTP(string archivo, string nom_carpeta, string nom_carpeta_remoto, FTPStorage ftpstorage, PerformContext context, string archivo_remoto)
        {
            try
            {
                int intentos = 10;
                int intento = 0;

                var respuesta = new FTPRespuesta();
                respuesta.ok = false;

                string carpeta = Path.Combine(nom_carpeta);
                string dia = DateTime.Today.ToString("dd_MM_yyyy");

                string ruta_local = Path.Combine(PATH_LOCAL, carpeta, dia, archivo);

                if (!File.Exists(ruta_local))
                {
                    throw new Exception($"No se encuentra el archivo {ruta_local}");
                }

                try
                {
                    using (var client = new SftpClient(ftpstorage.server, ftpstorage.port, ftpstorage.user, ftpstorage.pass))
                    {
                        while (intento < intentos)
                        {
                            intento++;
                            respuesta.intento = intento;

                            client.Connect();

                            if (!client.Exists(nom_carpeta_remoto))
                            {
                                client.CreateDirectory(nom_carpeta_remoto);
                            }

                            using (FileStream fileStream = File.Open(ruta_local, FileMode.Open, FileAccess.Read))
                            {
                                client.UploadFile(fileStream, $@"{nom_carpeta_remoto}\{archivo_remoto}");
                            }

                            client.Disconnect();
                            
                            respuesta.ok = true;
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLine(context, $"Exception: {ex.Message}", ConsoleTextColor.Red);
                }

                return respuesta;
            }
            catch (Exception)
            {
                throw;
            }
        }


        private async Task<FTPRespuesta> UploadFileToFTP2(string archivo, string nom_carpeta, FTPStorage ftpStorage, PerformContext context)
        {
            try
            {
                int intentos = 10;
                int intento = 0;

                FTPRespuesta respuesta = new FTPRespuesta();
                respuesta.ok = false;


                string carpeta = Path.Combine(nom_carpeta);
                string dia = DateTime.Today.ToString("dd_MM_yyyy");

                string ruta_local = Path.Combine(PATH_LOCAL, carpeta, dia, archivo);

                if (!File.Exists(ruta_local))
                {
                    throw new Exception($"No se encuentra el archivo {ruta_local}");
                }

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://{ftpStorage.server}:{ftpStorage.port}/{archivo}");
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(ftpStorage.user, ftpStorage.pass);
                request.KeepAlive = true;
                request.EnableSsl = false;

                using (FileStream fileStream = File.Open(ruta_local, FileMode.Open, FileAccess.Read))
                {

                    while (intento < intentos)
                    {
                        try
                        {
                            intento++;
                            respuesta.intento = intento;

                            using (Stream requestStream = await request.GetRequestStreamAsync())
                            {
                                await fileStream.CopyToAsync(requestStream);

                                requestStream.Close();
                                fileStream.Close();

                                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                                {
                                    respuesta.descripcion = response.StatusDescription;
                                    respuesta.saludo = response.WelcomeMessage;
                                    respuesta.despedida = response.ExitMessage;

                                    if (response.StatusCode == FtpStatusCode.ClosingData)
                                    {
                                        respuesta.ok = true;
                                        break;
                                    }

                                    response.Close();
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            Log.WriteLine(context, $"Exception: {ex.Message}", ConsoleTextColor.Red);
                        }
                    }

                }

                return respuesta;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}