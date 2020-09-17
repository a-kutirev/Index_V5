using DBWrapper;
using System.ComponentModel;
using System.Data;

namespace ClassLibrary
{
    public class GroupHeaderModel : INotifyPropertyChanged
    {

        #region Fields & events
        public event PropertyChangedEventHandler PropertyChanged;

        protected int m_idgroupheader = 0;
        protected int m_idorganization = 0;
        protected int m_idgeo = 0;

        public int Idgroupheader
        {
            get => m_idgroupheader;
            set
            {
                m_idgroupheader = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Idgroupheader"));
            }
        }
        public int Idorganization
        {
            get => m_idorganization;
            set
            {
                m_idorganization = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Idorganization"));

            }
        }
        public int Idgeo
        {
            get => m_idgeo;
            set
            {
                m_idgeo = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Idgeo"));
            }
        }
        #endregion

        #region Constructor

        public GroupHeaderModel()
        {

        }
        public GroupHeaderModel(int id)
        {
            Select(id);
        }

        #endregion

        #region Select
        protected void Select(int id)
        {
            string sql = $"select * from groupheaders where idgroupheader = {id}";
            m_idgroupheader = id;
            DataTable tmp = MySqlWrapper.Select(sql);
            if (tmp.Rows.Count > 0)
            {
                m_idorganization = (int)tmp.Rows[0]["idorganization"];
                m_idgeo = (int)tmp.Rows[0]["idgeo"];
            }
        }
        #endregion

        #region Insert
        public int Insert()
        {
            string sql = $"insert into groupheaders(idorganization, idgeo)values({Idorganization},{Idgeo})";
            return MySqlWrapper.Execute(sql);
        }
        #endregion
    }
}
