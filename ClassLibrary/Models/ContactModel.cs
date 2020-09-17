using DBWrapper;
using System.ComponentModel;
using System.Data;

namespace ClassLibrary
{
    public class ContactModel: INotifyPropertyChanged
    {

        #region Fields & Events
        public event PropertyChangedEventHandler PropertyChanged;

        private int m_idcontact = 0;
        private string m_contactname = "";
        private string m_contactpost = "";
        private string m_contactphone = "";

        public string Contactname { get => m_contactname;
            set
            {
                m_contactname = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Contactname"));
            }
        }
        public string Contactpost
        {
            get => m_contactpost;
            set
            {
                m_contactpost = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Contactpost"));
            }
        }
        public string Contactphone
        {
            get => m_contactphone;
            set
            {
                m_contactphone = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Contactphone"));
            }
        }
        public int Idcontact
        {
            get => m_idcontact;
            set
            {
                m_idcontact = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Idcontact"));
            }
        }
        #endregion

        #region Constructor
        public ContactModel()
        {

        }

        public ContactModel(int id)
        {
            Select(id);
        }

        public ContactModel(string name, string post, string phone)
        {
            m_contactname = name;
            m_contactpost = post;
            m_contactphone = phone;
        }
        #endregion

        #region CRUD
        public int Insert()
        {
            string sql = $"insert into contacts(contactname, contactpost, contactphone) " +
                $"values(\"{m_contactname.Replace("\"", "\"\"")}\",\"{m_contactpost.Replace("\"", "\"\"")}\",\"{m_contactphone.Replace("\"", "\"\"")}\")";
            Idcontact = MySqlWrapper.Execute(sql);
            return m_idcontact;
        }
    
        public void Update()
        {
            string sql = $"update contacts set " +
                $"contactname = \"{m_contactname.Replace("\"", "\"\"")}\", " +
                $"contactpost = \"{m_contactpost.Replace("\"", "\"\"")}\", " +
                $"contactphone = \"{m_contactphone.Replace("\"", "\"\"")}\" " +
                $"where idcontact = {m_idcontact}";
            MySqlWrapper.Execute(sql);
        }
        #endregion

        #region Select
        private bool Select(int id)
        {
            string sql = $"select * from contacts where idcontact = {id}";
            DataTable tmp = MySqlWrapper.Select(sql);
            if(tmp.Rows.Count > 0)
            {
                Contactname = tmp.Rows[0]["contactname"].ToString();
                Contactpost = tmp.Rows[0]["contactpost"].ToString();
                Contactphone = tmp.Rows[0]["contactphone"].ToString();
                Idcontact = id;
                return true;
            }
            return false;
        }
        #endregion
    }
}
