using Hangfire;
using Hangfire.MAMQSqlExtension;
using Hangfire.Server;
using HF.UploadFile.Entity;
using System.ComponentModel;

namespace HF.UploadFile
{
    public class Inicio: IDisposable
    {
        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)] //reintentos, por defecto son 10
        [DisableConcurrentExecution(timeoutInSeconds: 5)] //hacer que espere 1 segundo para que inicie el siguiente y si se lanzan 2 al mismo tiempo o continuo solo entra 1 y los otros caen en deleted
        [DisplayName("UploadFile")]
        [RetryInQueue("upload_file")]
        public async Task UploadFile(PerformContext context, BucketStorage bucketStorage, CancellationToken cancellationToken)
        {
            try
            {
                using (Logic.Logic objL = new Logic.Logic())
                {
                    await objL.TransferExcel(context, bucketStorage, cancellationToken);
                }
            }
            catch
            {
                throw;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            GC.Collect();
        }

    }
}