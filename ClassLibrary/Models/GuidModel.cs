using System;
using System.ComponentModel;
using System.Data;

namespace ClassLibrary
{
    public class GuidModel : INotifyPropertyChanged
    {
        #region members

        private int m_idguid = 0;
        private int m_idpost;
        private string m_guidfullname;
        private string m_guidshortname;
        private DateTime m_guidaccept;
        private DateTime m_guidend;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Idguid { get => m_idguid; set => m_idguid = value; }
        public int Idpost
        {
            get => m_idpost;
            set
            { 
                m_idpost = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Idpost"));
            }
        }
        public string Guidfullname
        {
            get => m_guidfullname;
            set
            {
                m_guidfullname = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Guidfullname"));

            }
        }
        public string Guidshortname
        {
            get => m_guidshortname;
            set
            {
                m_guidshortname = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Guidshortname"));
            }
        }
        public DateTime Guidaccept
        {
            get => m_guidaccept;
            set
            {
                m_guidaccept = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Guidaccept"));
            }
        }
        public DateTime Guidend
        {
            get => m_guidend;
            set
            {
                m_guidend = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Guidend"));
            }
        }

        #endregion

        #region Constructor
        public GuidModel()
        {
            Guidfullname = "";
            Guidshortname = "";
            Idpost = 1;
            Guidaccept = DateTime.Now;
            Guidend = new DateTime(1, 1, 1);
        }
        public GuidModel(int id)
        {
            string sql = $"select * from guids where idguid = {id}";
            DataTable tmp = DBWrapper.MySqlWrapper.Select(sql);
            if(tmp.Rows.Count > 0)
            {
                Idguid = id;
                Guidfullname = tmp.Rows[0]["guidfullname"].ToString();
                Guidshortname = tmp.Rows[0]["guidshortname"].ToString();
                Idpost = (int)tmp.Rows[0]["idpost"];
                Guidaccept = (DateTime)tmp.Rows[0]["guidaccept"];
                try
                {
                    Guidend = (DateTime)tmp.Rows[0]["guidend"];
                }
                catch { Guidend = new DateTime(1, 1, 1); }
          //      Guidend = (tmp.Rows[0]["guidend"].ToString() == "") ? null : (DateTime)tmp.Rows[0]["guidend"];
            }
        }
        #endregion

        #region Insert
        public int Insert()
        {
            string sql = $"insert into guids(guidfullname, guidshortname, idpost, guidaccept, guidend) values" +
                $"(\"{m_guidfullname.Replace("\"", "\"\"")}\"," +
                $"\"{m_guidshortname.Replace("\"", "\"\"")}\"," +
                $"{m_idpost}," +
                $"\"{m_guidaccept.ToString("yyyy-MM-dd")}\"," +
                $" NULL )";

            return DBWrapper.MySqlWrapper.Execute(sql);
        }
        #endregion

        #region Update
        public void Update()
        {
            string sql;
            if (m_guidend == new DateTime(1, 1, 1))
            {
                sql = $"update guids set " +
                    $"guidfullname = \"{m_guidfullname.Replace("\"", "\"\"")}\", " +
                    $"guidshortname = \"{m_guidshortname.Replace("\"", "\"\"")}\", " +
                    $"idpost = {m_idpost}," +
                    $"guidaccept = \"{m_guidaccept.ToString("yyyy-MM-dd")}\", " +
                    $"guidend = NULL " +
                    $"where idguid = {m_idguid}";
            }
            else
            {
                sql = $"update guids set " +
                    $"guidfullname = \"{m_guidfullname.Replace("\"", "\"\"")}\", " +
                    $"guidshortname = \"{m_guidshortname.Replace("\"", "\"\"")}\", " +
                    $"idpost = {m_idpost}," +
                    $"guidaccept = \"{m_guidaccept.ToString("yyyy-MM-dd")}\", " +
                    $"guidend = \"{m_guidend.ToString("yyyy-MM-dd")}\" " +
                    $"where idguid = {m_idguid}";
            }
            DBWrapper.MySqlWrapper.Execute(sql);
        }
        #endregion
    }
}
