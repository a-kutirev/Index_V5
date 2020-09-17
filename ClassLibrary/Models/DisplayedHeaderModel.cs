using DBWrapper;
using System.ComponentModel;
using System.Data;

namespace ClassLibrary
{
    public class DisplayedHeaderModel : GroupHeaderModel, INotifyPropertyChanged
    {
        #region Fields & Events
        public event PropertyChangedEventHandler PropertyChanged;

        private string m_organizationname = "";
        private string m_geoname = "";        

        public string Organizationname
        {
            get => m_organizationname;
            set
            {
                m_organizationname = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Organizationname"));
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

        public DisplayedHeaderModel() : base() {}
        public DisplayedHeaderModel(int id) : base(id) 
        {
            string sql = $"select * from allheaders where idgroupheader = {id}";
            DataTable tmp = MySqlWrapper.Select(sql);
            if(tmp.Rows.Count > 0)
            {
                m_organizationname = tmp.Rows[0]["organizationname"].ToString();
                m_geoname = tmp.Rows[0]["geoname"].ToString();
            }            
        }

        #endregion
    }
}
