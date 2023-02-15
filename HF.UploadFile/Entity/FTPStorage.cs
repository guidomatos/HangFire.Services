namespace HF.UploadFile.Entity
{
    public class FTPStorage
    {
        public string server { get; set; }
        public string user { get; set; }
        public string pass { get; set; }
        public int port { get; set; }
        public string ruta_local { get; set; }
    }
    public class FTPRespuesta
    {
        public bool ok { get; set; }
        public string descripcion { get; set; }
        public string saludo { get; set; }
        public string despedida { get; set; }
        public int intento { get; set; }
    }
}
