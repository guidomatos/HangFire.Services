﻿using Common;
using Hangfire.Server;
using HF.UploadFile.Data;
using HF.UploadFile.Entity;
using System.Reflection;

namespace HF.UploadFile.Logic
{
    public class Logic : IDisposable
    {
        private HFConfig _hfConfig;
        string PATH_LOCAL = string.Empty; //(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase)).ToString().Replace("file:\\", "");
        string PATH_EXE = (Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase)).ToString().Replace("file:\\", "");

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

        private async Task<byte[]> ConvertToExcel(List<ObjectAlter> data, string archivo)
        {
            try
            {

                string folder = Path.Combine("excel");
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

        

    }
}