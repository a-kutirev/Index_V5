using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace ClassLibrary.Models
{
    public class EventGroupModel : INotifyPropertyChanged
    {
        #region Members

        private int m_ideventgroup = 0;
        private int m_idorganization = 0;
        private int m_idgeo = 0;
        private int m_idevent = 0;
        private int m_eventgroupamount = 0;

        private string m_eventgroupcomment = "";
        private string m_eventgroupdeletereason = "";
        private string m_eventgroupmaster = "";
        private string m_eventgroupage = "";
        private int m_eventgroupstat = 0;
        private int m_idcategory = 0;

        private int m_eventgroupstatus = 0;
        private DateTime m_eventgroupdate = DateTime.Now;            
        private TimeSpan m_eventgrouptime;

        public int ideventgroup
        {
            get => m_ideventgroup;
            set
            {
                m_ideventgroup = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Ideventgroup"));
            }
        }
        public int Idevent
        {
            get => m_idevent;
            set
            {
                m_idevent = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Idevent"));
            }
        }
        public int Eventgroupamount
        {
            get => m_eventgroupamount;
            set
            {
                m_eventgroupamount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Eventgroupamount"));
            }
        }
        public string Eventgroupcomment
        {
            get => m_eventgroupcomment;
            set
            {
                m_eventgroupcomment = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Eventgroupcomment"));
            }
        }
        public string Eventgroupdeletereason
        {
            get => m_eventgroupdeletereason;
            set
            {
                m_eventgroupdeletereason = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Eventgroupdeletereason"));
            }
        }
        public string Eventgroupmaster
        {
            get => m_eventgroupmaster;
            set
            {
                m_eventgroupmaster = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Eventgroupmaster"));
            }
        }
        public int Eventgroupstatus
        {
            get => m_eventgroupstatus;
            set
            {
                m_eventgroupstatus = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Eventgroupstatus"));
            }
        }
        public DateTime Eventgroupdate
        {
            get => m_eventgroupdate;
            set
            {
                m_eventgroupdate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Eventgroupdate"));
            }
        }
        public TimeSpan Eventgrouptime
        {
            get => m_eventgrouptime;
            set
            {
                m_eventgrouptime = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Eventgrouptime"));
            }
        }
        public int Eventgroupstat
        {
            get => m_eventgroupstat;
            set
            {
                m_eventgroupstat = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Eventgroupstat"));
            }
        }
        public int Idcategory
        {
            get => m_idcategory;
            set
            {
                m_idcategory = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Idcategory"));
            }
        }
        public string Eventgroupage
        {
            get => m_eventgroupage;
            set
            {
                m_eventgroupage = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Eventgroupage")) ;
            }
        }
        public int Idorganization
        {
            get => m_idorganization;
            set
            {
                m_idorganization = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Idorganization"));
            }
        }
        public int Idgeo
        {
            get => m_idgeo;
            set
            {
                m_idgeo = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Idgeo"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructor
        public EventGroupModel()
        {

        }
        public EventGroupModel(int id)
        {
            string sql = $"select * from eventgroups where ideventgroup = {id}";
            DataTable tmp = DBWrapper.MySqlWrapper.Select(sql);
            if(tmp.Rows.Count > 0)
            {
                m_ideventgroup = id;
                m_eventgroupdate = DateTime.Parse(
                    $"{DateTime.Parse(tmp.Rows[0]["eventgroupdate"].ToString()).ToString("yyyy-MM-dd")} " +
                    $"{DateTime.Parse(tmp.Rows[0]["eventgrouptime"].ToString()).ToString("HH:mm:ss")}");
                m_eventgrouptime = new TimeSpan(m_eventgroupdate.Hour, m_eventgroupdate.Minute, 0);
                m_eventgroupstatus =  int.Parse(tmp.Rows[0]["eventgroupstatus"].ToString());
                m_eventgroupmaster = tmp.Rows[0]["eventgroupmaster"].ToString();
                m_eventgroupcomment = tmp.Rows[0]["eventgroupcomment"].ToString();
                m_eventgroupdeletereason = tmp.Rows[0]["eventgroupdeletereason"].ToString();
                m_eventgroupamount = int.Parse(tmp.Rows[0]["eventgroupamount"].ToString());
                m_idevent = int.Parse(tmp.Rows[0]["idevent"].ToString());
                m_eventgroupage = tmp.Rows[0]["eventgroupage"].ToString();
                try { m_eventgroupstat = int.Parse(tmp.Rows[0]["eventgroupstat"].ToString()); } catch { m_eventgroupstat = 0; }
                try { m_idcategory = int.Parse(tmp.Rows[0]["idcategory"].ToString()); } catch { m_idcategory = 0; }
                try { m_idorganization = int.Parse(tmp.Rows[0]["idorganization"].ToString()); } catch { m_idorganization = 0; }
                try { m_idgeo = int.Parse(tmp.Rows[0]["idgeo"].ToString()); } catch { m_idgeo = 0; }
            }
        }
        #endregion

        #region Insert
        public int Insert()
        {
            string sql = $"insert into eventgroups" +
                $"(eventgroupdate," +
                $"eventgrouptime," +
                $"eventgroupstatus," +
                $"eventgroupmaster," +
                $"eventgroupcomment, " +
                $"eventgroupdeletereason, " +
                $"eventgroupamount, " +
                $"idevent, " +
                $"eventgroupstat, " +
                $"idcategory, " +
                $"eventgroupage, " +
                $"idorganization, " +
                $"idgeo) values " +
                $"(" +
                $"'{m_eventgroupdate.ToString("yyyy-MM-dd")}'," +
                $"'{m_eventgrouptime.ToString("hh\\:mm\\:ss")}'," +
                $"{m_eventgroupstatus}, " +
                $"'{m_eventgroupmaster}'," +
                $"'{m_eventgroupcomment}', " +
                $"'{m_eventgroupdeletereason}'," +
                $"{m_eventgroupamount}, " +
                $"{m_idevent}, " +
                $"{m_eventgroupstat}," +
                $"{m_idcategory}," +
                $"'{m_eventgroupage}'," +
                $"{m_idorganization}," +
                $"{m_idgeo})";
            return DBWrapper.MySqlWrapper.Execute(sql);
        }
        #endregion

        #region Update
        public void Update()
        {
            string sql = $"update eventgroups set " +
                $"eventgroupdate =            '{m_eventgroupdate.ToString("yyyy-MM-dd")}'," +
                $"eventgrouptime =            '{m_eventgrouptime.ToString("hh\\:mm\\:ss")}'," +
                $"eventgroupstatus =           {m_eventgroupstatus}," +
                $"eventgroupmaster =          '{m_eventgroupmaster}'," +
                $"eventgroupcomment =         '{m_eventgroupcomment}'," +
                $"eventgroupdeletereason =    '{m_eventgroupdeletereason}'," +
                $"eventgroupamount =           {m_eventgroupamount}," +
                $"idevent =                    {m_idevent} ," +
                $"eventgroupstat =             {m_eventgroupstat}, " +
                $"idcategory =                 {m_idcategory}, " +
                $"eventgroupage =             '{m_eventgroupage}', " +
                $"idorganization =             {m_idorganization}, " +
                $"idgeo =                      {m_idgeo} " +
                $"where ideventgroup =         {m_ideventgroup}";
            DBWrapper.MySqlWrapper.Execute(sql);
        }
        #endregion

        #region Get List of Masters Id
        public List<int> GetListMasters()
        {
            List<int> result = new List<int>();

            if(m_eventgroupmaster != "")
            {
                string[] tmp = m_eventgroupmaster.Split('#');
                for (int i = 0; i < tmp.Length; i++)
                    result.Add(int.Parse(tmp[i]));
            }

            return result;
        }
        #endregion
    }
}
