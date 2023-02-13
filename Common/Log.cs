using Hangfire.Console;
using Hangfire.Server;
using System.Diagnostics;

namespace Common
{
    public class Log
    {
        private static ReaderWriterLock locker = new ReaderWriterLock();

        public static void WriteSuccess(PerformContext context, string mensaje, bool logFile = true)
            => WriteLine(context, mensaje, ConsoleTextColor.Green, logFile);
        public static void WriteError(PerformContext context, string mensaje, bool logFile = true)
            => WriteLine(context, mensaje, ConsoleTextColor.Red, logFile);

        public static void WriteLine(PerformContext context, string mensaje, ConsoleTextColor color = null, bool logFile = true)
        {
            string hora_actual = "[" + DateTime.Now.ToString("hh:mm:ss tt") + "] ";

            Debug.WriteLine(hora_actual + mensaje);
            Console.WriteLine(hora_actual + mensaje);

            if (color == null)
            {
                context.WriteLine(hora_actual + mensaje);
            }
            else
            {
                context.WriteLine(color, hora_actual + mensaje);
            }

            if (logFile)
            {
                WriteToFile(hora_actual + mensaje);
            }
        }

        private static void WriteToFile(string Message)
        {
            try
            {
                DateTime dateTime = DateTime.Now.Date;
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string pathLog = Path.Combine(baseDirectory, "Logs");

                if (!Directory.Exists(pathLog))
                {
                    Directory.CreateDirectory(pathLog);
                }

                string str = dateTime.ToShortDateString().Replace('/', '_');
                string archivoLog = pathLog + "\\ServiceLog_" + str + ".txt";

                locker.AcquireWriterLock(int.MaxValue);

                using (StreamWriter streamWriter = File.AppendText(archivoLog))
                {
                    streamWriter.WriteLine(Message);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error WriteToFile: " + ex.Message);
            }
            finally
            {
                locker.ReleaseWriterLock();
            }

        }
    }
}