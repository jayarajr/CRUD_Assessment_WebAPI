
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace CRUD_Assessment.Services
{
    public class CommonService
    {
        public bool executeQuery(string strQuery)
        {
            bool success = true;
            try
            {
                using (var connection = new SQLiteConnection("Data Source=Assessment.db; Version = 3; New = True; Compress = True; "))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = strQuery;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                success = false;
                throw ex;
            }
            return success;

        }

        public DataSet executeDataset(string strQuery)
        {

            DataSet ds = new DataSet();
            try
            {
                using (var connection = new SQLiteConnection("Data Source=Assessment.db; Version = 3; New = True; Compress = True; "))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = strQuery;
                    SQLiteDataAdapter da = new SQLiteDataAdapter(strQuery, connection);
                    da.Fill(ds);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
    }
}
