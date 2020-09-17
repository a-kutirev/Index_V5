using ClassLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UtilsLib.Regroup
{
    /// <summary>
    /// Логика взаимодействия для GroupHead.xaml
    /// </summary>
    public partial class GroupHead : UserControl
    {
        #region Members
        public event EventHandler<NumGroupChangedArgs> NumGroupChanged;

        private int m_GroupNum;
        private string m_organization = "";
        private int m_organizationId = 0;
        private List<DisplayedGroupModel> m_listGroup = new List<DisplayedGroupModel>();
        List<int> m_idGroupHeader = new List<int>();
        private DataTable headerList = new DataTable();

        public string Organization
        {
            get => m_organization;
            set
            {
                m_organization = value;
                headComboBox.Items.Clear();
                headComboBox.Items.Add(m_organization);
                headComboBox.SelectedIndex = 0;
            }
        }


        public int OrganizationId
        {
            get => m_organizationId;
            set
            {
                m_organizationId = value;
            }
        }

        public int ModelCount
        {
            get
            {
                return BodyList.Children.Count;
            }
        }

        public int HeadCounter
        {
            get
            {
                return headComboBox.ItemsSource.OfType<object>().Count();
            }
        }

        public int SelectedIdHeader
        {
            get
            {
                return (int)headComboBox.SelectedValue;
            }
        }
        #endregion

        #region Constructor
        public GroupHead(int gNum)
        {
            InitializeComponent();
            m_GroupNum = gNum;

            headerList.Columns.Add("IdHeader", typeof(int));
            headerList.Columns.Add("NameHeader", typeof(string));

        }
        #endregion

        #region AddGroup

        public void AddGroup(DisplayedGroupModel model)
        {
            m_listGroup.Add(model);
            GroupBody gb = new GroupBody(model);
            gb.NumGroupChanged += Gb_NumGroupChanged;
            BodyList.Children.Add(gb);
            int idHeader = model.Idcommongroup;
            if (!m_idGroupHeader.Contains(idHeader))
            {
                m_idGroupHeader.Add(idHeader);
                CreateHeaderList();
            }

            if (m_idGroupHeader.Count > 1)
                MessageBox.Show("Вы объединили в группу несколько экскурсий с разными организациями.\nВыберите в выпадающем списке общую организацию для всех групп.\n(В дальнейшем выбор можно изменить)");
        }

        private void CreateHeaderList()
        {
            headerList.Rows.Clear();

            for (int i = 0; i < m_idGroupHeader.Count; i++)
            {
                DataRow dr = headerList.NewRow();
                dr["IdHeader"] = m_idGroupHeader[i];
                dr["NameHeader"] = Options.GetNameHeaderById(m_idGroupHeader[i]);
                headerList.Rows.Add(dr);
            }
            try { headComboBox.Items.Clear(); }
            catch { }

            headComboBox.ItemsSource = headerList.DefaultView;
            headComboBox.DisplayMemberPath = "NameHeader";
            headComboBox.SelectedValuePath = "IdHeader";
            headComboBox.SelectedIndex = 0;
        }

        private void Gb_NumGroupChanged(object sender, NumGroupChangedArgs e)
        {
            NumGroupChanged?.Invoke(this, e);
        }

        public DisplayedGroupModel GetModel(int i)
        {
            return m_listGroup[i];
        }
        #endregion
    }
}
