using System;
using System.ComponentModel;
using System.Data;

namespace ClassLibrary.Models
{
    public class NoteModel : INotifyPropertyChanged
    {
        #region Members & Events
        private int m_idnote;
        private DateTime m_notestartperiod;
        private DateTime m_noteendperiod;
        private string m_note;
        private int m_notelimit;

        public int Idnote
        {
            get => m_idnote;
            set
            {
                m_idnote = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Idnote"));
            }
        }
        public DateTime Notestartperiod
        {
            get => m_notestartperiod;
            set
            {
                m_notestartperiod = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Notestartperiod"));
            }
        }
        public DateTime Noteendperiod
        {
            get => m_noteendperiod;
            set
            {
                m_noteendperiod = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Noteendperiod"));
            }
        }
        public string Note
        {
            get => m_note;
            set
            {
                m_note = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Note"));
            }
        }
        public int Notelimit
        {
            get => m_notelimit;
            set
            {
                m_notelimit = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Notelimit"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructor
        public NoteModel()
        {
            m_notestartperiod = DateTime.Now;
            m_noteendperiod = DateTime.Now;
            m_note = "";
            m_notelimit = 0;
            m_idnote = 0;
        }
        public NoteModel(int id)
        {
            string sql = $"select * from notes where idnote = {id}";
            DataTable tmp = DBWrapper.MySqlWrapper.Select(sql);
            if(tmp.Rows.Count > 0)
            {
                Idnote = id;
                Notestartperiod = (DateTime)tmp.Rows[0]["notestartperiod"];
                Noteendperiod = (DateTime)tmp.Rows[0]["noteendperiod"];
                Note = tmp.Rows[0]["note"].ToString();
                Notelimit =  int.Parse(tmp.Rows[0]["notelimit"].ToString());
            }
        }
        #endregion

        #region Insert

        public int Insert()
        {
            string sql = "";

            sql = $"insert into notes(notestartperiod, noteendperiod,note, notelimit)values(" +
                $"\"{m_notestartperiod.ToString("yyyy-MM-dd")}\"," +
                $"\"{m_noteendperiod.ToString("yyyy-MM-dd")}\"," +
                $"\"{m_note.Replace("\"", "\"\"")}\", {m_notelimit})";

            return DBWrapper.MySqlWrapper.Execute(sql);
        }

        #endregion

        #region Update
        public void Update()
        {
            string sql = "";

        sql = $"update notes set " +
            $"notestartperiod = \"{m_notestartperiod.ToString("yyyy-MM-dd")}\", " +
            $"noteendperiod = \"{m_noteendperiod.ToString("yyyy-MM-dd")}\"," +
            $"note = \"{m_note.Replace("\"", "\"\"")}\"," +
            $"notelimit = {m_notelimit} " +
            $"where idnote = {m_idnote}";
            DBWrapper.MySqlWrapper.Execute(sql);
        }
        #endregion
    }
}
