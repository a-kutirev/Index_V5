using System.ComponentModel;
using System.Data;

namespace ClassLibrary.Models
{
    public class OrganizationModel : INotifyPropertyChanged
    {
        #region Members
        public event PropertyChangedEventHandler PropertyChanged;

        private int m_idorganization;
        private string m_organizationname;

        public int Idorganization
        {
            get => m_idorganization;
            set
            {
                m_idorganization = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Idorganization"));
            }
        }
        public string Organizationname
        {
            get => m_organizationname;
            set
            {
                m_organizationname = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Organizationname"));
            }
        }

        #endregion
        #region Constructor
        public OrganizationModel() { }
        public OrganizationModel(int id)
        {
            string sql = $"select * from organizations where idorganization = {id}";
            DataTable tmp = DBWrapper.MySqlWrapper.Select(sql);
            if(tmp.Rows.Count > 0)
            {
                Idorganization = id;
                Organizationname = tmp.Rows[0]["organizationname"].ToString();
            }
        }
        #endregion

        #region Insert
        public int Insert()
        {
            string sql = $"insert into organizations(organizationname)values(\"{m_organizationname.Replace("\"", "\"\"")}\")";
            Idorganization = DBWrapper.MySqlWrapper.Execute(sql);
            return Idorganization;
        }
        #endregion

        #region Update
        public void Update()
        {
            string sql = $"update organizations set organizationname = \"{m_organizationname.Replace("\"", "\"\"")}\" where idorganization = {m_idorganization}";
            DBWrapper.MySqlWrapper.Execute(sql);
        }
        #endregion 
    }
}
