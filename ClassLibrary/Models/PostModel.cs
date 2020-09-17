using System.ComponentModel;
using System.Data;

namespace ClassLibrary.Models
{
    public class PostModel: INotifyPropertyChanged
    {
        #region Members

        public event PropertyChangedEventHandler PropertyChanged;

        private int m_idpost = 0;
        private string m_postname = "";
        private bool m_post_guidadd = false;

        public int Idpost
        {
            get => m_idpost;
            set
            {
                m_idpost = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Idpost"));
            }
        }
        public string Postname
        {
            get => m_postname;
            set
            {
                m_postname = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Postname"));
            }
        }
        public bool Post_guidadd
        {
            get => m_post_guidadd;
            set
            {
                m_post_guidadd = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Post_guidadd"));
            }
        }

        #endregion

        #region Constructor
        public PostModel()
        {

        }

        public PostModel(int id)
        {
            string sql = $"select * from posts where idpost = {id}";
            DataTable dt = DBWrapper.MySqlWrapper.Select(sql);
            if (dt.Rows.Count > 0)
            {
                Idpost = id;
                Postname = dt.Rows[0]["postname"].ToString();
                Post_guidadd = dt.Rows[0]["post_guidadd"].ToString() == "1";
            }
        }
        #endregion

        #region Insert
        public int Insert()
        {
            string sql = $"insert into posts(postname, post_guidadd)values(\"{m_postname}\", {(m_post_guidadd ? 1 : 0)})";
            return DBWrapper.MySqlWrapper.Execute(sql);
        }
        #endregion

        #region Update
        public void Update()
        {
            string sql = $"update posts set " +
                $"postname = \"{m_postname}\", " +
                $"post_guidadd = {(m_post_guidadd ? 1 : 0)} " +
                $"where idpost = {m_idpost}";
            DBWrapper.MySqlWrapper.Execute(sql);
        }
        #endregion
    }
}
