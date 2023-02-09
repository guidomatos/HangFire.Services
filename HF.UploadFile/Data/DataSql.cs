using Common;
using HF.UploadFile.Entity;
using System.Data;
using System.Data.SqlClient;

namespace HF.UploadFile.Data
{
    internal class DataSql : IDisposable
    {
        private readonly string _cn = string.Empty;

        public DataSql(SqlConfig sqlConfig)
        {
            _cn = sqlConfig.Cn;
        }

        private string CadenaConexion()
        {
            try
            {
                return _cn;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<ObjectAlter>> ExecuteObjetoAlter()
        {

            try
            {
                using (SqlConnection cn = new SqlConnection(CadenaConexion()))
                {
                    await cn.OpenAsync();
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "dbo.GetCustomer";

                        using (var dr = await cmd.ExecuteReaderAsync())
                        {
                            ObjectAlter? objeto = null;
                            var listObjetoAlter = new List<ObjectAlter>();

                            while (await dr.ReadAsync())
                            {
                                objeto = new ObjectAlter
                                {
                                    CustomerName = dr["CustomerName"].ToString()
                                };

                                listObjetoAlter.Add(objeto);
                            }

                            return listObjetoAlter;
                        }
                    }
                    await cn.CloseAsync();
                }
            }
            catch (Exception)
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