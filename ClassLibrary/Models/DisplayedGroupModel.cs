using DBWrapper;
using System.ComponentModel;
using System.Data;

namespace ClassLibrary
{
    public class DisplayedGroupModel : GroupModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Поля таблицы

        private string m_organizationname = "";
        private string m_geoname = "";
        private string m_tourname = "";
        private string m_guidfullname = "";
        private string m_guidshortname = "";

        public string Organizationname
        {
            get => m_organizationname;
            set { 
                m_organizationname = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Organizationname"));
            }
        }
        public string Geoname
        {
            get => m_geoname; set
            {
                m_geoname = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Geoname"));
            }
        }
        public string Tourname { get => m_tourname; set
            {
                m_tourname = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Tourname"));
            }
        }
        public string Guidfullname
        {
            get => m_guidfullname; set
            {
                m_guidfullname = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Guidfullname"));
            }
        }

        public string Guidshortname
        {
            get => m_guidshortname;
            set
            {
                m_guidshortname = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Guidshortname"));
            }
        }
        #endregion

        #region Конструкторы
        public DisplayedGroupModel() : base()
        {

        }
        public DisplayedGroupModel(int id)
        {
            string sql = $"select * from allgroup where idgroup = {id}";
            DataTable tmp = MySqlWrapper.Select(sql);

            if (tmp.Rows.Count > 0)
            {
                FillFromDataRow(tmp.Rows[0]);
                Organizationname = tmp.Rows[0]["organizationname"].ToString();
                Geoname = tmp.Rows[0]["geoname"].ToString();
                Tourname = tmp.Rows[0]["tourname"].ToString();
                Guidfullname = tmp.Rows[0]["guidfullname"].ToString();
                Guidshortname = tmp.Rows[0]["guidshortname"].ToString();
            }
            else
                Idgroup = 0;
        }
        #endregion
    }
}
