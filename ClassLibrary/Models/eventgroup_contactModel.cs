using DBWrapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models
{
    public class eventgroup_contactModel : INotifyPropertyChanged
    {
        #region Members
        private int m_ideventgroup_contact = 0;
        private int m_ideventgroup;
        private int m_idcontact;

        public int Ideventgroup_contact
        {
            get => m_ideventgroup_contact;
            set
            {
                m_ideventgroup_contact = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Ideventgroup_contact"));
            }
        }
        public int Ideventgroup
        {
            get => m_ideventgroup;
            set
            {
                m_ideventgroup = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Ideventgroup"));
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

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructor
        public eventgroup_contactModel()
        {

        }
        #endregion

        #region Insert
        public int Insert()
        {
            string sql = $"insert into eventgroup_contacts(ideventgroup, idcontact) values ({m_ideventgroup},{m_idcontact})";
            return MySqlWrapper.Execute(sql);
        }
        #endregion

        #region Update
        public void Update()
        {
            string sql = $"update eventgroup_contacts set " +
                $"ideventgroup = {m_ideventgroup}, " +
                $"idcontact = {m_idcontact} where " +
                $"ideventgroup_contact = {m_ideventgroup_contact}";
        }
        #endregion
    }
}
