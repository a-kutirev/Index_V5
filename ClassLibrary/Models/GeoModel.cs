using System.ComponentModel;
using System.Data;

namespace ClassLibrary.Models
{
    public class GeoModel : INotifyPropertyChanged
    {
        #region Members
        public event PropertyChangedEventHandler PropertyChanged;

        private int m_idgeo;
        private string m_geoname;

        public int Idgeo
        {
            get => m_idgeo;
            set
            {
                m_idgeo = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Idgeo"));
            }
        }
        public string Geoname
        {
            get => m_geoname;
            set
            {
                m_geoname = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Geoname"));
            }
        }

        #endregion

        #region Constructor
        public GeoModel() { }
        public GeoModel(int id)
        {
            string sql = $"select * from geos where idgeo = {id}";
            DataTable tmp = DBWrapper.MySqlWrapper.Select(sql);
            if (tmp.Rows.Count > 0)
            {
                Idgeo = id;
                Geoname = tmp.Rows[0]["geoname"].ToString();
            }
        }
        #endregion

        #region Insert
        public int Insert()
        {
            string sql = $"insert into geos(geoname)values(\"{m_geoname.Replace("\"", "\"\"")}\")";
            Idgeo = DBWrapper.MySqlWrapper.Execute(sql);
            return Idgeo;
        }
        #endregion

        #region Update
        public void Update()
        {
            string sql = $"update geos set geoname = \"{m_geoname.Replace("\"", "\"\"")}\" where idgeo = {m_idgeo}";
            DBWrapper.MySqlWrapper.Execute(sql);
        }
        #endregion 
    }
}
