using ClosedXML.Excel;
using HF.UploadFile.Entity;

namespace HF.UploadFile.Logic
{
    public class ExcelUtil
    {
        public static byte[] BuildXlsxFile(List<ObjectAlter> reporte)
        {

            try
            {
                using (var workbook = new XLWorkbook())
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Add("Sheet1");

                    GenerateHeaders(worksheet);
                    GenerateBody(reporte, worksheet);

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return content;
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void GenerateBody(List<ObjectAlter> reporte, IXLWorksheet worksheet)
        {
            for (int index = 1; index <= reporte.Count; index++)
            {
                worksheet.Cell(index + 1, 1).Value = reporte[index - 1].CustomerName;
            }
        }

        private static void GenerateHeaders(IXLWorksheet worksheet)
        {
            worksheet.Cell(1, 1).Value = "customerName";
        }


    }
}
