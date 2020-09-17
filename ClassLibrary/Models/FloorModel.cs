using System.ComponentModel;
using System.Data;

namespace ClassLibrary.Models
{
    public class FloorModel : INotifyPropertyChanged
    {
        #region Members
        public event PropertyChangedEventHandler PropertyChanged;

        private int m_idfloor = 0;
        private string m_floorname = "";

        public int Idfloor
        {
            get => m_idfloor;
            set
            {
                m_idfloor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Idfloor"));
            }
        }
        public string Floorname
        {
            get => m_floorname;
            set
            {
                m_floorname = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Floorname"));
            }
        }
        #endregion

        #region Contructor
        public FloorModel()
        {

        }
        public FloorModel(int id)
        {
            string sql = $"select * from floors where idfloor = {id}";
            DataTable dt = DBWrapper.MySqlWrapper.Select(sql);
            if(dt.Rows.Count > 0)
            {
                Idfloor = id;
                Floorname = dt.Rows[0]["floorname"].ToString();
            }
        }
        #endregion

        #region Insert
        public int Insert()
        {
            string sql = $"insert into floors(floorname) values (\"{m_floorname}\")";
            return DBWrapper.MySqlWrapper.Execute(sql);
        }
        #endregion

        #region Update
        public void Update()
        {
            string sql = $"update floors set " +
                $"floorname = \"{m_floorname.Replace("\"","\"\"")}\" " +
                $"where idfloor = {m_idfloor}";
            DBWrapper.MySqlWrapper.Execute(sql);
        }
        #endregion
    }
}
