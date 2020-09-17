using System;
using System.ComponentModel;
using System.Data;

namespace ClassLibrary.Models
{
    public class DaysOptionModel : INotifyPropertyChanged
    {
        #region Member Fields
        private int m_iddaysoption = 0;
        private DateTime m_daysoptiondate;

        private int m_workday;
        private int m_offcontrol;
        private int m_obzor;
        private int m_addguid;
        private int m_usestarthour;
        private int m_starthour;


        public event PropertyChangedEventHandler PropertyChanged;

        public int Iddaysoption
        {
            get => m_iddaysoption;
            set
            {
                m_iddaysoption = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Iddaysoption"));
            }
        }
        public DateTime Daysoptiondate
        {
            get => m_daysoptiondate;
            set
            {
                m_daysoptiondate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Daysoptiondate"));
            }
        }

        public int Workday
        {
            get => m_workday;
            set
            {
                m_workday = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Workday"));
            }
        }
        public int Offcontrol
        {
            get => m_offcontrol;
            set
            {
                m_offcontrol = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Offcontrol"));
            }
        }
        public int Obzor
        {
            get => m_obzor;
            set
            {
                m_obzor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Obzor"));
            }
        }
        public int Addguid
        {
            get => m_addguid;
            set
            {
                m_addguid = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Addguid"));
            }
        }
        public int Usestarthour
        {
            get => m_usestarthour;
            set
            {
                m_usestarthour = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Usestarthour"));
            }
        }
        public int Starthour
        {
            get => m_starthour;
            set
            {
                m_starthour = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Starthour"));
            }
        }
        #endregion

        #region Constructor
        public DaysOptionModel()
        { }
        public DaysOptionModel(DateTime date)
        {
            string sql = $"select * from daysoptions where daysoptiondate = \"{date.ToString("yyyy-MM-dd")}\"";
            DataTable tmp = DBWrapper.MySqlWrapper.Select(sql);
            if (tmp.Rows.Count > 0)
            {
                Iddaysoption = (int)tmp.Rows[0]["iddaysoption"];
                Daysoptiondate = date;
                Workday = int.Parse(tmp.Rows[0]["workday"].ToString());
                Offcontrol = int.Parse(tmp.Rows[0]["offcontrol"].ToString());
                Obzor = int.Parse(tmp.Rows[0]["obzor"].ToString());
                Addguid = int.Parse(tmp.Rows[0]["addguid"].ToString());
                Usestarthour = int.Parse(tmp.Rows[0]["usestarthour"].ToString());
                Starthour = int.Parse(tmp.Rows[0]["starthour"].ToString());
            }
        }
        #endregion

        #region Insert
        public int Insert()
        {
            string sql = $"INSERT INTO daysoptions(daysoptiondate,workday,offcontrol,obzor,addguid,usestarthour,starthour) " +
                            $"VALUES('{Daysoptiondate.ToString("yyyy-MM-dd")}', {Workday},{Offcontrol},{Obzor},{Addguid},{Usestarthour},{Starthour})";
            m_iddaysoption = DBWrapper.MySqlWrapper.Execute(sql);
            return m_iddaysoption;
        }
        #endregion

        #region Update
        public void Update()
        {
            string sql = $"update daysoptions set " +
                $"daysoptiondate = \"{m_daysoptiondate.ToString("yyyy-MM-dd")}\", " +
                $"workday = {Workday}, " +
                $"offcontrol = {Offcontrol}, " +
                $"obzor = {Obzor}, " +
                $"addguid = {Addguid}, " +
                $"usestarthour = {Usestarthour}, " +
                $"starthour = {Starthour} "+
                $"where iddaysoption = {m_iddaysoption}";
            DBWrapper.MySqlWrapper.Execute(sql);
        }
        #endregion
    }
}
