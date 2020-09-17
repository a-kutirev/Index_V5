using DBWrapper;
using System;
using System.ComponentModel;
using System.Data;

namespace ClassLibrary
{
    public class TourModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        #region Поля таблицы
        private int             m_idtour = 0;
        private int             m_idexpo_zone = 0;
        private int             m_idtourtype = 0;
        private int             m_tourduration = 0;
        private int             m_tourpersist = 0;
        private DateTime        m_tourstart = DateTime.Now;
        private DateTime        m_tourend = DateTime.Now;
        private string          m_tourname = "";

        public int Idtour
        {
            get => m_idtour;
            set
            {
                m_idtour = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Idtour"));
            }
        }
        public int Idexpo_zone
        {
            get => m_idexpo_zone;
            set
            {
                m_idexpo_zone = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Idexpo_zone"));
            }
        }
        public int Idtourtype
        {
            get => m_idtourtype;
            set
            {
                m_idtourtype = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Idtourtype"));
            }
        }
        public int Tourduration
        {
            get => m_tourduration;
            set
            {
                m_tourduration = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Tourduration"));
            }
        }
        public int Tourpersist
        {
            get => m_tourpersist;
            set
            {
                m_tourpersist = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Tourpersist"));
            }
        }
        public DateTime Tourstart
        {
            get => m_tourstart;
            set
            {
                m_tourstart = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Tourstart"));
            }
        }
        public DateTime Tourend
        {
            get => m_tourend;
            set
            {
                m_tourend = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Tourend"));
            }
        }
        public string Tourname
        {
            get => m_tourname;
            set
            {
                m_tourname = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Tourname"));
            }
        }
        #endregion

        #region Конструктор
        public TourModel()
        {

        }

        public TourModel(int id)
        {
            SelectByID(id);
        }
        #endregion

        #region CRUD
        public int Insert()
        {
            string sql = $"insert into tours(idexpo_zone, idtourtype, tourname, tourduration, tourpersist, tourstart, tourend) " +
                $"values({m_idexpo_zone}, {m_idtourtype}, \"{m_tourname.Replace("\"", "\"\"")}\", {m_tourduration}, {m_tourpersist}, " +
                $"\"{m_tourstart.ToString("yyyy-MM-dd")}\", \"{m_tourend.ToString("yyyy-MM-dd")}\")";            
            return MySqlWrapper.Execute(sql);
        }

        public void Update()
        {
            string sql = $"update tours set " +
                $"idexpo_zone = {m_idexpo_zone}, " +
                $"idtourtype = {m_idtourtype}, " +
                $"tourname = \"{m_tourname.Replace("\"", "\"\"")}\", " +
                $"tourduration = {m_tourduration}, " +
                $"tourpersist = {m_tourpersist}, " +
                $"tourstart = \"{m_tourstart.ToString("yyyy-MM-dd")}\", " +
                $"tourend = \"{m_tourend.ToString("yyyy-MM-dd")}\" " +
                $"where idtour = {m_idtour}";
            MySqlWrapper.Execute(sql);
        }
        #endregion

        #region Select
        public bool SelectByID(int id)
        {
            string sql = $"select * from tours where idtour = {id}";
            DataTable tmp = MySqlWrapper.Select(sql);
            if (tmp.Rows.Count == 0) return false;
            m_idtour = id;
            DataRow dr = tmp.Rows[0];
            m_idexpo_zone = (int)dr["idexpo_zone"];
            m_idtourtype = (int)dr["idtourtype"];
            m_tourname = dr["tourname"].ToString();
            m_tourduration = (int)dr["tourduration"];
            m_tourpersist = int.Parse(dr["tourpersist"].ToString());
            if(m_tourpersist != 1)
            {
                m_tourstart = DateTime.Parse(dr["tourstart"].ToString());
                m_tourend = DateTime.Parse(dr["tourend"].ToString());
            }
            return true;
        }
        #endregion
    }
}
