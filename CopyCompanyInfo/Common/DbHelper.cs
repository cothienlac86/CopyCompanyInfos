using System;
using System.Data.SQLite;
using System.Data;
using System.IO;

namespace CopyCompanyInfo.Common
{
    public static class DbHelper
    {
        private static readonly log4net.ILog CopyLogger =
                    log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().Name);
        private static SQLiteConnection m_Connection;

        public static SQLiteConnection GetConnection()
        {
            try
            {

                var dbPath = Environment.CurrentDirectory + @"\Database\CompanyInfo.sqlite";
                if (m_Connection == null)
                {
                    m_Connection = new SQLiteConnection(string.Format("Data Source={0};Version=3;Initial Catalog=main;UseUTF16Encoding=True;",dbPath));
                    if (m_Connection.State == ConnectionState.Closed)
                        m_Connection.Open();
                }
                else
                {
                    if (m_Connection.State == ConnectionState.Closed)
                        m_Connection.Open();
                }
            }
            catch
            {
                throw;
            }
            return m_Connection;
        }

        public static void Close()
        {
            try
            {
                if (m_Connection.State == ConnectionState.Open)
                {
                    m_Connection.Close();
                }
            }
            catch (Exception ex)
            {
                CopyLogger.Error(string.Format("Trace Error:{0} \n Error Message:{1}",
                    ex.ToString(), ex.Message));
            }
        }

        public static DataSet ExecuteQuery(string statment)
        {
            var sqlCon = GetConnection();
            var sqlCmd = new SQLiteCommand();
            DataSet result = new DataSet();
            try
            {
                sqlCmd.Connection = sqlCon;
                sqlCmd.CommandText = statment;
                var sqlAdap = new SQLiteDataAdapter(sqlCmd);
                if (sqlAdap != null)
                    sqlAdap.Fill(result);
                sqlAdap.Dispose();
            }
            catch (Exception ex)
            {
                CopyLogger.Error(string.Format("Trace Error:{0} \n Error Message:{1}",
                    ex.ToString(), ex.Message));
            }
            finally
            {
                Close();
                sqlCmd.Dispose();
            }
            return result;
        }

        public static void ExecuteNoneQuery(string statement)
        {
            var sqlCon = GetConnection();
            var sqlCmd = new SQLiteCommand(statement, sqlCon);
            try
            {
                sqlCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                CopyLogger.Error(string.Format("Trace Error:{0} \n Error Message:{1}",
                    ex.ToString(), ex.Message));
            }
            finally
            {
                sqlCmd.Dispose();
                Close();
            }
        }

        public static object ExecuteScalar(string statement)
        {
            var sqlCon = GetConnection();
            object result = null;
            try
            {
                var sqlCmd = new SQLiteCommand(statement, sqlCon);
                result = sqlCmd.ExecuteScalar();
                sqlCmd.Dispose();
            }
            catch (Exception ex)
            {
                CopyLogger.Error(string.Format("Trace Error:{0} \n Error Message:{1}",
                    ex.ToString(), ex.Message));
            }
            finally
            {
                Close();
            }
            return result;
        }
    }
}
