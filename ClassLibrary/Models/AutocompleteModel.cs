using System.ComponentModel;
using System.Data;

namespace ClassLibrary.Models
{
    public class AutocompleteModel: INotifyPropertyChanged
    {
        // Привет 20.09.2020 14.42

        #region Members
        private int m_idautocomplete = 0;
        private string m_autocompleteword;
        private string m_autocompletetype;

        public int Idautocomplete
        {
            get => m_idautocomplete;
            set
            {
                m_idautocomplete = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Idautocomplete"));
            }
        }
        public string Autocompleteword
        {
            get => m_autocompleteword;
            set
            {
                m_autocompleteword = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Autocompleteword"));
            }
        }
        public string Autocompletetype
        {
            get => m_autocompletetype;
            set
            {
                m_autocompletetype = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Autocompleteword"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Ctor
        public AutocompleteModel()
        {
        }
        public AutocompleteModel(int id)
        {
            string sql = $"select * from autocompletes where idautocomplete = {id}";
            DataTable dt = DBWrapper.MySqlWrapper.Select(sql);
            if(dt.Rows.Count > 0)
            {
                Idautocomplete = id;
                Autocompleteword = dt.Rows[0]["autocompleteword"].ToString();
                Autocompletetype = dt.Rows[0]["autocompletetype"].ToString();
            }
        }
        #endregion

        #region Insert
        public int Insert()
        {
            string sql = $"insert into autocompletes(autocompleteword, autocompletetype) " +
                $"values(\"{m_autocompleteword.Replace("\"","\"\"")}\",\"{m_autocompletetype.Replace("\"", "\"\"")}\")";
            return DBWrapper.MySqlWrapper.Execute(sql);
        }
        #endregion

        #region Update
        public void Update()
        {
            string sql = $"update autocompletes set " +
                $"autocompleteword = \"{m_autocompleteword.Replace("\"", "\"\"")}\", " +
                $"autocompletetype = \"{m_autocompletetype.Replace("\"", "\"\"")}\" " +
                $"where idautocomplete = {m_idautocomplete}";
            DBWrapper.MySqlWrapper.Execute(sql);
        }
        #endregion
    }
}
