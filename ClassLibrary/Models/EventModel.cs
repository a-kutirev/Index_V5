using System.ComponentModel;
using System.Data;

namespace ClassLibrary.Models
{
    public class EventModel : INotifyPropertyChanged
    {
        #region Members

        private int m_idevent = 0;
        private string m_eventname = "";
        private int m_idexpo_zone = 1;
        private string m_eventtype = "МК";  // МК - мастер класс, КВ - квест, Л - лекция

        public int Idevent
        {
            get => m_idevent;
            set
            {
                m_idevent = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Idevent"));
            }
        }
        public string Eventname
        {
            get => m_eventname;
            set
            {
                m_eventname = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Eventname"));
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
        public string Eventtype
        {
            get => m_eventtype;
            set
            {
                m_eventtype = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Eventtype"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructor
        public EventModel()
        {

        }

        public EventModel(int id)
        {
            string sql = $"select * from events where idevent = {id}";
            DataTable tmp = DBWrapper.MySqlWrapper.Select(sql);

            if(tmp.Rows.Count > 0)
            {
                Idevent = id;
                Eventname = tmp.Rows[0]["eventname"].ToString();
                Idexpo_zone = int.Parse(tmp.Rows[0]["idexpo_zone"].ToString());
                Eventtype = tmp.Rows[0]["eventtype"].ToString();
            }
        }
        #endregion

        #region Insert
        public int Insert()
        {
            string sql = $"insert into events(eventname, idexpo_zone , eventtype)values " +
                $"('{m_eventname}',{m_idexpo_zone},'{m_eventtype}')";
            return DBWrapper.MySqlWrapper.Execute(sql);
        }
        #endregion

        #region Update
        public void Update()
        {
            string sql = $"update events set " +
                $"eventname = '{m_eventname}', " +
                $"idexpo_zone = {m_idexpo_zone}, " +
                $"eventtype = '{m_eventtype}' " +
                $"where idevent = {m_idevent}";
            DBWrapper.MySqlWrapper.Execute(sql);
        }
        #endregion        
    }
}
