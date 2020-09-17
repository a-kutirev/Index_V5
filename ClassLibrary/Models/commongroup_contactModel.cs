namespace ClassLibrary.Models
{
    public class commongroup_contactModel
    {
        #region Members
        private int m_idcommongroup_contact;
        private int m_idcommongroup;
        private int m_idcontact;

        public int Idcommongroup { get => m_idcommongroup; set => m_idcommongroup = value; }
        public int Idcontact { get => m_idcontact; set => m_idcontact = value; }
        public int Idcommongroup_contact { get => m_idcommongroup_contact; set => m_idcommongroup_contact = value; }
        #endregion
        #region Constructor
        public commongroup_contactModel() { }
        public commongroup_contactModel(int idheader, int idcontact) {
            m_idcommongroup = idheader;
            m_idcontact = idcontact;
        }
        #endregion
        #region Insert
        public int Insert()
        {
            string sql = $"insert into commongroup_contacts(idcommongroup, idcontact) " +
                $"values({m_idcommongroup},{m_idcontact})";
            m_idcommongroup_contact = DBWrapper.MySqlWrapper.Execute(sql);
            return m_idcommongroup_contact;
        }
        #endregion

        #region Update
        public void Update()
        {
            string sql = $"update commongroup_contacts set " +
                $"idcommongroup = {m_idcommongroup}, " +
                $"idcontact = {m_idcontact} " +
                $"where idcommongroup_contact = {m_idcommongroup_contact}";

            DBWrapper.MySqlWrapper.Execute(sql);
        }
        #endregion
    }
}
