using System;
using System.Data.SQLite;
using System.Data;

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

                if (m_Connection == null)
                {
                    m_Connection = new SQLiteConnection("Data Source=~/../Database/CompanyInfo.sqlite;Version=3;Initial Catalog=main;UseUTF16Encoding=True;");
                    if (m_Connection.State == ConnectionState.Closed)
                        m_Connection.Open();
                }
                else
                {
                    if (m_Connection.State == ConnectionState.Closed)
                        m_Connection.Open();
                }
            }
            catch (Exception)
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
            DataSet result = new DataSet();
            try
            {
                var sqlCmd = new SQLiteCommand(statment, sqlCon);
                var sqlAdap = new SQLiteDataAdapter(sqlCmd);
                if (sqlAdap != null)
                    sqlAdap.Fill(result);
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

        public static void ExecuteNoneQuery(string statement)
        {
            var sqlCon = GetConnection();
            try
            {
                var sqlCmd = new SQLiteCommand(statement, sqlCon);
                sqlCmd.ExecuteNonQuery();
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
