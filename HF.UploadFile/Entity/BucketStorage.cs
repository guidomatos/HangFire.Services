namespace HF.UploadFile.Entity
{
    public class BucketStorage
    {
        public string proyect_id { get; set; }
        public string bucket_name { get; set; }
        public string ruta_gs { get; set; }
        public string json_file { get; set; }
        public string ruta_local { get; set; }
    }
}
