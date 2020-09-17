using System.ComponentModel;
using System.Data;

namespace ClassLibrary.Models
{
    public class Expo_zonesModel : INotifyPropertyChanged
    {
        #region Members
        public event PropertyChangedEventHandler PropertyChanged;

        private int m_idexpo_zone = 0;
        private int m_idfloor = 1;
        private string m_expo_zonename = "";

        public int Idexpo_zone
        {
            get => m_idexpo_zone;
            set
            {
                m_idexpo_zone = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Idexpo_zone"));
            }
        }
        public int Idfloor
        {
            get => m_idfloor;
            set
            {
                m_idfloor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Idfloor")) ;
            }
        }
        public string Expo_zonename
        {
            get => m_expo_zonename;
            set
            {
                m_expo_zonename = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Expo_zonename"));
            }
        }

        #endregion

        #region Constructor
        public Expo_zonesModel()
        {

        }
        public Expo_zonesModel(int id)
        {
            string sql = $"select * from expo_zones where idexpo_zone = {id}";
            DataTable dt = DBWrapper.MySqlWrapper.Select(sql);
            if(dt.Rows.Count > 0)
            {
                Idexpo_zone = id;
                Idfloor = (int)dt.Rows[0]["idfloor"];
                Expo_zonename = dt.Rows[0]["expo_zonename"].ToString();
            }
        }
        #endregion

        #region Insert
        public int Insert()
        {
            string sql = $"insert into expo_zones(idfloor, expo_zonename)values({m_idfloor}, \"{m_expo_zonename.Replace("\"","\"\"")}\")";
            return DBWrapper.MySqlWrapper.Execute(sql);
        }
        #endregion

        #region Update
        public void Update()
        {
            string sql = $"update expo_zones set " +
                $"idfloor = {m_idfloor}, " +
                $"expo_zonename = \"{m_expo_zonename.Replace("\"", "\"\"")}\" " +
                $"where idexpo_zone = {m_idexpo_zone}";
            DBWrapper.MySqlWrapper.Execute(sql);
        }
        #endregion
    }
}
