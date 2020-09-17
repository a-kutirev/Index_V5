using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogLibrary
{
    public static class LogDB
    {
        private static MySqlConnection connection = null;
        public static string connectionString;

        private static string m_server = "";
        private static string m_database = "";
        private static string m_user = "";
        private static string m_password = "";

        private static RegistryKey currentUser = Registry.CurrentUser;
        private static string path = @"SOFTWARE\HistoryParkNsk";

        public static string Server
        {
            get
            {
                if (m_server == "")
                {
                    RegistryKey rk_park = currentUser.OpenSubKey(path, true);
                    object v = rk_park.GetValue("ServerV5");
                    if (v == null)
                    {
                        rk_park.SetValue("ServerV5", "127.0.0.1");
                        m_server = "127.0.0.1";
                    }
                    else
                    {
                        m_server = v.ToString();
                    }
                }
                return m_server;
            }
            set
            {
                m_server = value;
                RegistryKey rk_park = currentUser.OpenSubKey(path, true);
                rk_park.SetValue("ServerV5", m_server);
            }
        }
        public static string Database
        {
            get
            {
                if (m_database == "")
                {
                    RegistryKey rk_park = currentUser.OpenSubKey(path, true);
                    object v = rk_park.GetValue("Database2");
                    if (v == null)
                    {
                        rk_park.SetValue("Database2", "tours_v5");
                        m_database = "tours_v5";
                    }
                    else
                    {
                        m_database = v.ToString();
                    }
                }
                return m_database;
            }
            set
            {
                m_database = value;
                RegistryKey rk_park = currentUser.OpenSubKey(path, true);
                rk_park.SetValue("Database2", m_database);
            }
        }
        public static string User
        {
            get
            {
                if (m_user == "")
                {
                    RegistryKey rk_park = currentUser.OpenSubKey(path, true);
                    object v = rk_park.GetValue("User");
                    if (v == null)
                    {
                        rk_park.SetValue("User", "administrator");
                        m_user = "administrator";
                    }
                    else
                    {
                        m_user = v.ToString();
                    }
                }
                return m_user;
            }
            set
            {
                m_user = value;
                RegistryKey rk_park = currentUser.OpenSubKey(path, true);
                rk_park.SetValue("User", m_user);
            }
        }
        public static string Password
        {
            get
            {
                if (m_password == "")
                {
                    RegistryKey rk_park = currentUser.OpenSubKey(path, true);
                    object v = rk_park.GetValue("Password");
                    if (v == null)
                    {
                        rk_park.SetValue("Password", "456Park()");
                        m_password = "456Park()";
                    }
                    else
                    {
                        m_password = v.ToString();
                    }
                }
                return m_password;
            }
            set
            {
                m_password = value;
                RegistryKey rk_park = currentUser.OpenSubKey(path, true);
                rk_park.SetValue("Password", m_password);
            }
        }

        private static MySqlConnection GetConnection()
        {
            string server = Server;
            string db = "changelog";
            string user = User;
            string password = Password;

            string connectionTemplate = "SERVER={0};DATABASE={1};UID={2};PASSWORD={3};convert zero datetime=True";
            connectionString = string.Format(connectionTemplate, server, db, user, password);

            connection = new MySqlConnection(connectionString);
            if (connection.State == ConnectionState.Closed) connection.Open();
            return connection;
        }
        public static void Execute(string sql)
        {
            if (connection == null) connection = GetConnection();

            MySqlCommand command = new MySqlCommand(sql, connection);
            command.ExecuteNonQuery();
        }
        public static DataTable Select(DateTime dt)
        {
            string sql = $"SELECT * FROM changelog.log where date(datetimelog) = '{dt.ToString("yyyy-MM-dd")}'";

            if (connection == null) connection = GetConnection();

            MySqlCommand command = new MySqlCommand(sql, connection);
            command.ExecuteNonQuery();

            var sqlAdapter = new MySqlDataAdapter(command);
            var dataTabe = new DataTable();

            sqlAdapter.Fill(dataTabe);
            sqlAdapter.Update(dataTabe);

            return dataTabe;
        }
    }
}
