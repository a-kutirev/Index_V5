using System;
using System.ComponentModel;
using System.Data;

namespace ClassLibrary
{
    public class MissingModel: INotifyPropertyChanged
    {
        #region Members
        public event PropertyChangedEventHandler PropertyChanged;

        private int m_idmissing = 0;
        private int m_idguid = 1;
        private DateTime m_missingbegin = new DateTime(1,1,1);
        private DateTime m_missingend = new DateTime(1,1,1);
        private string m_missingcomment = "";
        private int m_missingnotfullday = 0;

        public int Idguid
        {
            get => m_idguid;
            set
            {
                m_idguid = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Idguid"));                
            }
        }
        public DateTime Missingbegin
        {
            get => m_missingbegin;
            set
            {
                m_missingbegin = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Missingbegin"));
            }
        }
        public DateTime Missingend
        {
            get => m_missingend;
            set
            {
                m_missingend = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Missingend"));
            }
        }
        public string Missingcomment
        {
            get => m_missingcomment;
            set
            {
                m_missingcomment = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Missingcomment"));
            }
        }
        public int Missingnotfullday
        {
            get => m_missingnotfullday;
            set
            {
                m_missingnotfullday = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Missingnotfullday"));
            }
        }
        public int Idmissing
        {
            get => m_idmissing;
            set
            {
                m_idmissing = value;                
            }
        }
        #endregion

        #region Constructor
        public MissingModel()
        {

        }
        public MissingModel(int id)
        {
            string sql = $"select * from missings where idmissing = {id}";
            DataTable tmp = DBWrapper.MySqlWrapper.Select(sql);

            Idmissing = id;
            Idguid = (int)tmp.Rows[0]["idguid"];
            Missingbegin = (DateTime)tmp.Rows[0]["missingbegin"];
            Missingend = (DateTime)tmp.Rows[0]["missingend"];
            Missingcomment = tmp.Rows[0]["missingcomment"].ToString();
            Missingnotfullday = int.Parse(tmp.Rows[0]["missingnotfullday"].ToString());
        }        
        #endregion

        #region Insert & Update
        public int Insert()
        {
            string sql = $"insert into missings(idguid, missingbegin, missingend, missingcomment, missingnotfullday) " +
                $"values({Idguid},\"{Missingbegin.ToString("yyyy-MM-dd HH:mm:00")}\", \"{Missingend.ToString("yyyy-MM-dd HH:mm:00")}\"," +
                $"\"{Missingcomment}\", {Missingnotfullday})";
            return DBWrapper.MySqlWrapper.Execute(sql);
        }
        public void Update()
        {
            string sql = $"update missings set " +
                $"idguid = {Idguid}, " +
                $"missingbegin = \"{Missingbegin.ToString("yyyy-MM-dd HH:mm:00")}\", " +
                $"missingend = \"{Missingend.ToString("yyyy-MM-dd HH:mm:00")}\", " +
                $"missingcomment = \"{Missingcomment}\", " +
                $"missingnotfullday = {Missingnotfullday} " +
                $"where idmissing = {Idmissing}";
            DBWrapper.MySqlWrapper.Execute(sql);
        }
        #endregion
    }
}
