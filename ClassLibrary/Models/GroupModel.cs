using DBWrapper;
using System;
using System.ComponentModel;
using System.Data;

namespace ClassLibrary
{
    public class GroupModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Поля таблицы

        protected int m_idgroup = 0;
        protected int m_idcommongroup = 0;
        protected int m_idtour = 1;
        protected int m_idguid = 1;
        protected int m_idcategory = 1;
        protected int m_groupaddition = 0;
        protected int m_groupstatus = 0;
        protected int m_groupstatistic = 0;
        protected int m_groupnum = 0;
        protected string m_groupamount = "";
        protected string m_groupage = "";
        protected string m_groupcomment = "";
        protected string m_groupdeletereason = "";
        protected DateTime m_groupdate = DateTime.Now;
        protected TimeSpan m_grouptime = new TimeSpan(0);

        public int Idgroup
        {
            get => m_idgroup;
            set
            {
                m_idgroup = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Idgroup"));
            }
        }
        public int Idcommongroup
        {
            get => m_idcommongroup; set
            {
                m_idcommongroup = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Idcommongroup"));
            }
        }
        public int Idtour
        {
            get => m_idtour; set
            {
                m_idtour = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Idtour"));
            }
        }
        public int Idguid
        {
            get => m_idguid; set
            {
                m_idguid = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Idguid"));
            }
        }
        public int Idcategory
        {
            get => m_idcategory; set
            {
                m_idcategory = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Idcategory"));
            }
        }
        public int Groupaddition
        {
            get => m_groupaddition; set
            {
                m_groupaddition = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Groupaddition"));
            }
        }
        public int Groupstatus
        {
            get => m_groupstatus; set
            {
                m_groupstatus = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Groupstatus"));
            }
        }
        public int Groupstatistic
        {
            get => m_groupstatistic; set
            {
                m_groupstatistic = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Groupstatistic"));
            }
        }
        public int Groupnum
        {
            get => m_groupnum; set
            {
                m_groupnum = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Groupnum"));
            }
        }
        public DateTime Groupdate
        {
            get => m_groupdate; set
            {
                m_groupdate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Groupdate"));
            }
        }
        public TimeSpan Grouptime
        {
            get => m_grouptime; set
            {
                m_grouptime = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Grouptime"));
            }
        }
        public string Groupamount
        {
            get => m_groupamount; set
            {
                m_groupamount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Groupamount"));
            }
        }
        public string Groupage
        {
            get => m_groupage; set
            {
                m_groupage = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Groupage"));
            }
        }
        public string Groupcomment
        {
            get => m_groupcomment; set
            {
                m_groupcomment = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Groupcomment"));
            }
        }
        public string Groupdeletereason
        {
            get => m_groupdeletereason; set
            {
                m_groupdeletereason = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Groupdeletereason"));
            }
        }


        #endregion

        #region Конструктор
        public GroupModel()
        {

        }
        public GroupModel(DataRow dr)
        {
            FillFromDataRow(dr);
        }
        public GroupModel(int id)
        {
            SelectByID(id);
        }

        #endregion

        #region CRUD
        public int Insert()
        {
            string sql = $"insert into _groups(" +
                $"idcommongroup, groupdate, grouptime, " +
                $"idtour, idguid, groupamount, groupage," +
                $" groupaddition, groupstatus, groupstatistic, idcategory, " +
                $"groupcomment, groupdeletereason, groupnum" +
                $")values(" +
                $"{m_idcommongroup}, \"{m_groupdate.ToString("yyyy-MM-dd")}\",\"{m_grouptime.ToString("hh\\:mm\\:ss")}\", " +
                $"{m_idtour}, {m_idguid}, \"{m_groupamount.Replace("\"", "\"\"")}\", \"{m_groupage.Replace("\"", "\"\"")}\", " +
                $"{m_groupaddition}, {m_groupstatus}, {m_groupstatistic}, {m_idcategory}, " +
                $"\"{m_groupcomment.Replace("\"", "\"\"")}\", \"{m_groupdeletereason.Replace("\"", "\"\"")}\", {m_groupnum}" +
                $")";
            m_idgroup = MySqlWrapper.Execute(sql);
            return m_idgroup;
        }

        public void Update()
        {
            string sql = $"update _groups set " +
                $"idcommongroup = {m_idcommongroup}, " +
                $"groupdate = \"{m_groupdate.ToString("yyyy-MM-dd")}\", " +
                $"grouptime = \"{m_grouptime.ToString()}\", " +
                $"idtour = {m_idtour}, " +
                $"idguid = {m_idguid}, " +
                $"groupamount = \"{m_groupamount.Replace("\"", "\"\"")}\", " +
                $"groupage = \"{m_groupage.Replace("\"", "\"\"")}\", " +
                $"groupaddition = {m_groupaddition}, " +
                $"groupstatus = {m_groupstatus}, " +
                $"groupstatistic = {m_groupstatistic}, " +
                $"idcategory = {m_idcategory}, " +
                $"groupcomment = \"{m_groupcomment.Replace("\"", "\"\"")}\", " +
                $"groupdeletereason = \"{m_groupdeletereason.Replace("\"", "\"\"")}\", " +
                $"groupnum = {m_groupnum} "+
                $"where idgroup = {m_idgroup}";
            MySqlWrapper.Execute(sql);
        }
        #endregion

        #region Select

        protected void FillFromDataRow(DataRow tmp)
        {
            m_idgroup = (int)tmp["idgroup"];
            m_idcategory = (int)tmp["idcategory"];
            m_idcommongroup = (int)tmp["idcommongroup"];
            m_idguid = (int)tmp["idguid"];
            m_idtour = (int)tmp["idtour"];
            m_groupaddition = (int)tmp["groupaddition"];
            m_groupage = tmp["groupage"].ToString();
            m_groupamount = tmp["groupamount"].ToString();
            m_groupcomment = tmp["groupcomment"].ToString();
            m_groupdate = DateTime.Parse(
                (DateTime.Parse(tmp["groupdate"].ToString())).ToString("yyyy-MM-dd") + " " +
                (DateTime.Parse(tmp["grouptime"].ToString())).ToString("HH:mm:ss"));
            m_grouptime = new TimeSpan(m_groupdate.Hour, m_groupdate.Minute, 0);
            m_groupdeletereason = tmp["groupdeletereason"].ToString();
            m_groupnum = (int)tmp["groupnum"];
            m_groupstatistic = (int)tmp["groupstatistic"];
            m_groupstatus = (int)tmp["groupstatus"];

        }

        public bool SelectByID(int id)
        {
            string sql = $"select * from _groups where idgroup = {id}";
            DataTable tmp = MySqlWrapper.Select(sql);
            if (tmp.Rows.Count == 0) return false;

            FillFromDataRow(tmp.Rows[0]);

            return true;
        }

        #endregion
    }
}
