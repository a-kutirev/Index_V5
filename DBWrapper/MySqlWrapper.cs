using LogLibrary;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading;

namespace DBWrapper
{
    public static class MySqlWrapper
    {
        private static MySqlConnection connection = null;
        private static MySqlConnection connectionRemote = null;
        public static string connectionString = "SERVER=127.0.0.1;DATABASE=tours_v5;UID=administrator;PASSWORD=456Park();convert zero datetime=True";
        public static string connectionStringRemote = "SERVER=akutirev.ru;DATABASE=tours_v5;UID=administrator;PASSWORD=456Park();convert zero datetime=True";

        #region Выборка мероприятий за период с сортировкой по времени
        private static readonly string selectEventBetweenDateq = "select * from eventgroups where eventgroupdate between '{0}' and '{1}' " +
            "order by eventgroupdate, eventgrouptime";
        public static DataTable SelectEventBetweenDate(DateTime sd, DateTime ed)
        {
            string sql = string.Format(selectEventBetweenDateq, sd.ToString("yyyy-MM-dd"), ed.ToString("yyyy-MM-dd"));
            return Select(sql);
        }

        #endregion

        #region Выборка групп за период с сортировкой по времени
        private static readonly string selectGroupBetweenDate =
            "select * from allgroup where groupdate between '{0}' and '{1}' order by groupdate, grouptime";
        public static DataTable SelectGroupBetweenDate(DateTime sd, DateTime ed)
        {
            string sql = string.Format(selectGroupBetweenDate, sd.ToString("yyyy-MM-dd"), ed.ToString("yyyy-MM-dd"));
            return Select(sql);
        }

        #endregion

        #region Выборка групп по дате с сортировкой по времени
        private static readonly string selectGroupByDate = "SELECT * FROM tours_v5.allgroup where groupdate = '{0}' order by grouptime";
        public static DataTable SelectGroupByDate(DateTime date)
        {
            string sql = string.Format(selectGroupByDate, date.ToString("yyyy-MM-dd"));
            return Select(sql);
        }
        #endregion

        #region Выборка мероприятий по дате с сортировкой по времени
        private static readonly string selectEventByDate = "SELECT * FROM tours_v5.eventgroups where eventgroupdate = '{0}' order by eventgrouptime";
        public static DataTable SelectEventByDate(DateTime date)
        {
            string sql = string.Format(selectEventByDate, date.ToString("yyyy-MM-dd"));
            return Select(sql);
        }
        #endregion

        #region Выборка контактов по организации
        private static readonly string selectContactsByIdHeader = "SELECT idcontact FROM commongroup_contacts where idcommongroup = \"{0}\"";
        public static DataTable SelectContactsByIdHeader(int id)
        {
            string sql = string.Format(selectContactsByIdHeader, id);
            return Select(sql);
        }
        #endregion

        #region Выборка отсутствующих  на дату
        private static readonly string selectMissingOnDatetime = "CALL GetMissingsGuidByDatetime(\"{0}\");";
        public static DataTable SelectMissingOnDatetime(DateTime date)
        {
            string sql = string.Format(selectMissingOnDatetime, date.ToString("yyyy-MM-dd HH:mm:00"));
            return Select(sql);
        }
        #endregion

        #region Выборка отсутствующих  на дату
        private static readonly string selectMissingOnDate = "CALL GetMissingsGuidByDate(\"{0}\");";
        public static DataTable SelectMissingOnDate(DateTime date)
        {
            string sql = string.Format(selectMissingOnDate, date.ToString("yyyy-MM-dd"));
            return Select(sql);
        }
        #endregion

        #region Выборка объявлений  на дату
        private static readonly string selectNotesByDate = "CALL GetNotesByDate(\"{0}\");";
        public static DataTable SelectNotesOnDate(DateTime date)
        {
            string sql = string.Format(selectNotesByDate, date.ToString("yyyy-MM-dd"));
            return Select(sql);
        }
        #endregion        

        #region Организации
        private static readonly string selectAllOrganizations = "select * from organizations";
        public static DataTable SelectAllOrganizations()
        {
            return Select(selectAllOrganizations);
        }
        #endregion

        #region География
        private static readonly string selectAllGeos = "select * from geos";
        public static DataTable SelectAllGeos()
        {
            return Select(selectAllGeos);
        }

        #endregion

        private static MySqlConnection GetConnection()
        {
            if (connection == null) connection = new MySqlConnection(connectionString);
            if (connection.State == ConnectionState.Closed) connection.Open();
            return connection;
        }

        private static MySqlConnection GetConnectionRemote()
        {
            if (connectionRemote == null) connectionRemote = new MySqlConnection(connectionStringRemote);
            if (connectionRemote.State == ConnectionState.Closed) connectionRemote.Open();
            return connectionRemote;
        }

        public static int Execute(string sql)
        {            
            MySqlConnection cc;
            MySqlCommand com = null;
            try
            {
                cc = GetConnection();
                com = new MySqlCommand(sql, cc);
                com.ExecuteNonQuery();
            }
            catch // There is already an open DataReader associated with this Connection which must be closed first
            {
                MySqlConnection cc1 = new MySqlConnection(connectionString);
                if (cc1.State == ConnectionState.Closed) cc1.Open();
                com = new MySqlCommand(sql, cc1);
                com.ExecuteNonQuery();
            }
            int iid = (int)com.LastInsertedId;

            Log.Add(sql, iid);
            //try
            //{
            //    Thread thread = new Thread(() => ExecuteRemote(sql));
            //    thread.Start();
            //}
            //catch
            //{

            //}

            return iid;
        }

        private static void ExecuteRemote(string sql)
        {
            MySqlConnection cc;
            MySqlCommand com = null;
            try
            {
                cc = GetConnectionRemote();
                com = new MySqlCommand(sql, cc);
                com.ExecuteNonQuery();
            }
            catch // There is already an open DataReader associated with this Connection which must be closed first
            {
                MySqlConnection cc1 = new MySqlConnection(connectionStringRemote);
                if (cc1.State == ConnectionState.Closed) cc1.Open();
                com = new MySqlCommand(sql, cc1);
                com.ExecuteNonQuery();
            }
        }


        public static DataTable Select(string sql)
        {
            MySqlConnection cc;
            MySqlCommand com;
            try
            {
                cc = GetConnection();
                com = new MySqlCommand(sql, cc);
                com.ExecuteNonQuery();
            }
            catch // There is already an open DataReader associated with this Connection which must be closed first
            {
                MySqlConnection cc1 = new MySqlConnection(connectionString);
                if (cc1.State == ConnectionState.Closed) cc1.Open();
                com = new MySqlCommand(sql, cc1);
                com.ExecuteNonQuery();
            }

            var sqlAdapter = new MySqlDataAdapter(com);
            var dataTabe = new DataTable();

            sqlAdapter.Fill(dataTabe);
            sqlAdapter.Update(dataTabe);

            return dataTabe;
        }
    }
}
