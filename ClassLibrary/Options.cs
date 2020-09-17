using ClassLibrary.Models;
using DBWrapper;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;

namespace ClassLibrary
{
    public static class Options
    {
        private static RegistryKey currentUser = Registry.CurrentUser;
        private static string path = @"SOFTWARE\HistoryParkNsk";
        private static Dictionary<int, int> ToursDuration = null;
        private static Dictionary<int, int> ToursZone = null;
        private static Dictionary<int, string> GuidNameById = null;
        private static Dictionary<int, string> CategoryNameById = null;
        private static int fontSize = 18;

        public static int DefaultIdGeo = 5;
        public static int DefaultIdOrg = 181;
        public static bool needUpdate = false;
        public static string Username;
        public static string FullUsername;

        #region Отображение
        private static bool? m_HideDeleted = null;
        private static bool? m_HideCompleted = null;
        private static bool? m_HideEmptyComments = null;
        private static bool? m_useOldInterface = null;
        public static bool? HideDeleted
        {
            get
            {
                if (m_HideDeleted == null)
                {
                    RegistryKey rk_park = currentUser.OpenSubKey(path, true);
                    object v = rk_park.GetValue("HideDeleted_5");
                    if (v == null)
                    {
                        rk_park.SetValue("HideDeleted_5", 0);
                        m_HideDeleted = false;
                    }
                    else
                    {
                        m_HideDeleted = v.ToString() == "True";
                    }
                }
                return m_HideDeleted;
            }
            set
            {
                m_HideDeleted = value;
                RegistryKey rk_park = currentUser.OpenSubKey(path, true);
                rk_park.SetValue("HideDeleted_5", m_HideDeleted);
            }
        }
        public static bool? HideCompleted
        {
            get
            {
                if (m_HideCompleted == null)
                {
                    RegistryKey rk_park = currentUser.OpenSubKey(path, true);
                    object v = rk_park.GetValue("HideCompleted_5");
                    if (v == null)
                    {
                        rk_park.SetValue("HideCompleted_5", 0);
                        m_HideCompleted = false;
                    }
                    else
                    {
                        m_HideCompleted = v.ToString() == "True";
                    }
                }
                return m_HideCompleted;
            }
            set
            {
                m_HideCompleted = value;
                RegistryKey rk_park = currentUser.OpenSubKey(path, true);
                rk_park.SetValue("HideCompleted_5", m_HideCompleted);
            }
        }
        public static bool? HideEmptyComments
        {
            get
            {
                if (m_HideEmptyComments == null)
                {
                    RegistryKey rk_park = currentUser.OpenSubKey(path, true);
                    object v = rk_park.GetValue("HideEmptyComments_5");
                    if (v == null)
                    {
                        rk_park.SetValue("HideEmptyComment_5s", 0);
                        m_HideEmptyComments = false;
                    }
                    else
                    {
                        m_HideEmptyComments = v.ToString() == "True";
                    }
                }
                return m_HideEmptyComments;
            }
            set
            {
                m_HideEmptyComments = value;
                RegistryKey rk_park = currentUser.OpenSubKey(path, true);
                rk_park.SetValue("HideEmptyComments_5", m_HideEmptyComments);
            }
        }
        public static bool? UseOldInterface {
            get
            {
                if (m_useOldInterface == null)
                {
                    RegistryKey rk_park = currentUser.OpenSubKey(path, true);
                    object v = rk_park.GetValue("UseOldInterface");
                    if (v == null)
                    {
                        rk_park.SetValue("UseOldInterface", 0);
                        m_useOldInterface = false;
                    }
                    else
                    {
                        m_useOldInterface = v.ToString() == "True";
                    }
                }
                return m_useOldInterface;
            }
            set
            {
                m_useOldInterface = value;
                RegistryKey rk_park = currentUser.OpenSubKey(path, true);
                rk_park.SetValue("UseOldInterface", m_useOldInterface);
            }
        }

        #endregion

        #region Сеть
        private static string m_server = "";
        private static string m_database = "";
        private static string m_user = "";
        private static string m_password = "";

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

        public static int FontSize { get => fontSize; set => fontSize = value; }
        #endregion

        public static bool Check()
        {
            RegistryKey rk = currentUser.OpenSubKey(path);
            if (rk == null) return false;
            object rk1 = rk.GetValue("Use3DTheme");
            if (rk1 == null) return false;
            object rk2 = rk.GetValue("HideEmptyComments");
            if (rk2 == null) return false;
            object rk3 = rk.GetValue("HideCompleted");
            if (rk3 == null) return false;
            object rk4 = rk.GetValue("HideDeleted");
            if (rk4 == null) return false;
            object rk5 = rk.GetValue("Server");
            if (rk5 == null) return false;
            object rk6 = rk.GetValue("Database");
            if (rk6 == null) return false;
            object rk7 = rk.GetValue("User");
            if (rk7 == null) return false;
            object rk8 = rk.GetValue("Password");
            if (rk8 == null) return false;
            object rk9 = rk.GetValue("Version");
            if (rk9 == null) return false;
            return true;
        }
        public static void CreateKey()
        {
            RegistryKey rk_park = currentUser.CreateSubKey(path);

            object rk1 = rk_park.GetValue("Use3DTheme");
            if (rk1 == null) rk_park.SetValue("Use3DTheme", 1);
            object rk2 = rk_park.GetValue("HideEmptyComments_5");
            if (rk2 == null) rk_park.SetValue("HideEmptyComments_5", 1);
            object rk3 = rk_park.GetValue("HideCompleted_5");
            if (rk3 == null) rk_park.SetValue("HideCompleted_5", 0);
            object rk4 = rk_park.GetValue("HideDeleted_5");
            if (rk4 == null) rk_park.SetValue("HideDeleted_5", 0);
            object rk5 = rk_park.GetValue("Server");
            if (rk5 == null) rk_park.SetValue("Server", "");
            object rk6 = rk_park.GetValue("Database");
            if (rk6 == null) rk_park.SetValue("Database", "");
            object rk7 = rk_park.GetValue("User");
            if (rk7 == null) rk_park.SetValue("User", "");
            object rk8 = rk_park.GetValue("Password");
            if (rk8 == null) rk_park.SetValue("Password", "");
            object rk9 = rk_park.GetValue("Version");
            if (rk9 == null) rk_park.SetValue("Version", "4.0");
        }
        public static int GetZoneIdByTourId(int idtour)
        {
            if ((ToursZone == null) || (!ToursZone.ContainsKey(idtour)))
            {
                ToursZone = new Dictionary<int, int>();
                string sql = "select * from tours";
                List<TourModel> tmp = (List<TourModel>)DBWrapper.MySqlWrapper.Select(sql).ToList<TourModel>();
                for (int i = 0; i < tmp.Count; i++)
                {
                    ToursZone.Add(tmp[i].Idtour, tmp[i].Idexpo_zone);
                }
            }

            return ToursZone[idtour];
        }
        public static int GetDuration(int idtour)
        {
            if ((ToursDuration == null) || (!ToursDuration.ContainsKey(idtour)))
            {
                ToursDuration = new Dictionary<int, int>();
                string sql = "select * from tours";
                List<TourModel> tmp = (List<TourModel>)DBWrapper.MySqlWrapper.Select(sql).ToList<TourModel>();
                for (int i = 0; i < tmp.Count; i++)
                {
                    ToursDuration.Add(tmp[i].Idtour, tmp[i].Tourduration);
                }
            }

            return ToursDuration[idtour];
        }
        public static string GetGuidName(int idguid)
        {
            if ((GuidNameById == null)||(!GuidNameById.ContainsKey(idguid)))
            {
                GuidNameById = new Dictionary<int, string>();
                string sql = "select * from guids";
                List<GuidModel> tmp = (List<GuidModel>)DBWrapper.MySqlWrapper.Select(sql).ToList<GuidModel>();
                for (int i = 0; i < tmp.Count; i++)
                {
                    GuidNameById.Add(tmp[i].Idguid, tmp[i].Guidfullname);
                }
            }
            return GuidNameById[idguid];
        }
        public static string GetCategName(int idcat)
        {
            if ((CategoryNameById == null)||(!CategoryNameById.ContainsKey(idcat)))
            {
                CategoryNameById = new Dictionary<int, string>();
                string sql = "select * from categories";
                DataTable tmp = DBWrapper.MySqlWrapper.Select(sql);
                for (int i = 0; i < tmp.Rows.Count; i++)
                {
                    int id = (int)tmp.Rows[i]["idcategorie"];
                    string word = tmp.Rows[i]["categoryname"].ToString();
                    CategoryNameById.Add(id, word);
                }
            }
            return CategoryNameById[idcat];
        }
        public static List<string> GetStatisticsByCode(int code)
        {
            List<string> result = new List<string>();

            if ((code & 1) >= 1) result.Add("Экскурсия по письму");
            if ((code & 2) >= 1) result.Add("Экскурсия по приказу");
            if ((code & 4) >= 1) result.Add("Бесплатно билеты и экскурсии");
            if ((code & 8) >= 1) result.Add("Бесплатно только экскурсии");
            if ((code & 16) >= 1) result.Add("Бесплатно только билеты");
            if ((code & 32) >= 1) result.Add("Оплачено через бухгалтерию");
            if ((code & 64) >= 1) result.Add("Сборная группа");
            if ((code & 128) >= 1) result.Add("Без предварительной записи");
            if ((code & 256) >= 1) result.Add("Экскурсия по соглашению");

            return result;
        }
        public static string GetNameHeaderById(int id)
        {
            string result = "";

            string sql = $"select * from groupheaders where idgroupheader = {id}";
            DataTable tmp = DBWrapper.MySqlWrapper.Select(sql);

            if (tmp.Rows.Count > 0)
            {
                int idOrg = (int)tmp.Rows[0]["idorganization"];
                int idGeo = (int)tmp.Rows[0]["idgeo"];

                sql = $"select organizationname as str from organizations where idorganization = {idOrg} " +
                        $"union " +
                        $"select geoname from geos where idgeo = {idGeo}";
                tmp = DBWrapper.MySqlWrapper.Select(sql);


                if(tmp.Rows.Count == 2)
                {
                    result = $"{tmp.Rows[0]["str"].ToString()} ({tmp.Rows[1]["str"].ToString()})";
                }
                else
                {
                    result = $"{tmp.Rows[0]["str"].ToString()} ()";
                }
            }

            return result;
        }
        public static int GetFloorIdByTourId(int id)
        {
            string sql = $"select idfloor from expo_zones where idexpo_zone = (select idexpo_zone from tours where idtour = {id})";
            DataTable tmp = MySqlWrapper.Select(sql);
            return (tmp.Rows.Count > 0) ? (int)tmp.Rows[0]["idfloor"] : -1;
        }
        public static string GetContactsByCommogGroupId(int id)
        {
            string result = "";

            string sql = $"select * from commongroup_contacts where idcommongroup = {id}";
            List<commongroup_contactModel> cnts =
                (List<commongroup_contactModel>)MySqlWrapper.Select(sql).ToList<commongroup_contactModel>();

            for(int i = 0; i < cnts.Count; i++)
            {
                ContactModel model = new ContactModel(cnts[i].Idcontact);
                result += $"{model.Contactname} ({model.Contactpost}) {model.Contactphone}; ";
            }

            return result;
        }
        public static string GetGuidStringByIds(List<int> ids)
        {
            string result = "";

            for (int i = 0; i < ids.Count; i++)
            {
                result += ClassLibrary.Options.GetGuidName(ids[i]);
                if (i != (ids.Count - 1))
                    result += ", ";
            }

            return result;
        }
        public static Dictionary<int, int> GetEventCounter(DateTime from, DateTime to)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();

            string sql = $"select * from eventgroups where eventgroupdate between '{from.ToString("yyyy-MM-dd")}' and '{to.ToString("yyyy-MM-dd")}'";
            List<EventGroupModel> models = (List<EventGroupModel>)MySqlWrapper.Select(sql).ToList<EventGroupModel>();

            for(int i = 0; i < models.Count; i++)
            {
                int evId = models[i].Idevent;
                if (!result.ContainsKey(evId))
                    result.Add(evId, 0);
                result[evId]++;
            }

            return result;
        }
        public static Dictionary<int, int> GetEventGuidCounter(DateTime from, DateTime to)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();

            string sql = $"select * from eventgroups where eventgroupdate between '{from.ToString("yyyy-MM-dd")}' and '{to.ToString("yyyy-MM-dd")}'";
            List<EventGroupModel> models = (List<EventGroupModel>)MySqlWrapper.Select(sql).ToList<EventGroupModel>();

            for (int i = 0; i < models.Count; i++)
            {
                List<int> idguids = models[i].GetListMasters();
                for(int j = 0; j < idguids.Count; j++)
                {
                    int idg = idguids[j];
                    if (!result.ContainsKey(idg))
                        result.Add(idg, 0);
                    result[idg]++;
                }
            }

            return result;
        }
    }

    public static class DayOptions
    {
        private static DateTime m_date;
        private static bool m_workDay;
        private static bool m_useStartHour;
        private static int m_startHour;
        private static int m_dayLength;
        public static ExpoChecker ExpoCheckerMain = new ExpoChecker();
        public static GuidChecker GuidCheckerMain = new GuidChecker();

        public static DateTime Date
        {
            get => m_date;
            set
            {
                m_date = value;                

                DaysOptionModel dom = new DaysOptionModel(m_date);
                m_workDay =             dom.Workday == 1;
                m_useStartHour =        dom.Usestarthour == 1;
                if(m_useStartHour) m_startHour = dom.Starthour;
                else
                {
                    if (m_date.DayOfWeek == DayOfWeek.Sunday || m_date.DayOfWeek == DayOfWeek.Saturday)
                        m_startHour = 11;
                    else
                        m_startHour = 10;
                }
                m_dayLength = 8;

                ExpoCheckerMain.Date = value;
            }
        }
        public static bool WorkDay
        {
            get => m_workDay;           
        }
        public static bool UseStartHour 
        { 
            get => m_useStartHour;
        }
        public static int StartHour 
        { 
            get => m_startHour; 
        }
        public static int DayLength 
        { 
            get => m_dayLength; 
        }
    }

    public  static class Conventers
    {
        #region Conventers

        public static TimeSpan IndexToTimeSpan(int index)
        {
            int h = (int)(index / 4);
            int m = (index % 4) * 15;

            return new TimeSpan(h, m, 0);
        }

        public static int TimeSpanToIndex(TimeSpan time)
        {
            int h = time.Hours;
            int m = time.Minutes;
            int index = h * 4 + (int)(m / 15);
            return index;
        }

        #endregion
    }
}
