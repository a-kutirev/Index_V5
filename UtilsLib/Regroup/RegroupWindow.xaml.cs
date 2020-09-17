using ClassLibrary;
using System.Collections.Generic;
using System.Windows;

namespace UtilsLib.Regroup
{
    /// <summary>
    /// Логика взаимодействия для RegroupWindow.xaml
    /// </summary>
    public partial class RegroupWindow : Window
    {
        #region members
        Dictionary<int, GroupHead> groupsByNum = new Dictionary<int, GroupHead>();
        List<DisplayedGroupModel> m_groups;
        #endregion

        #region Constructor
        public RegroupWindow(List<DisplayedGroupModel> allgroups)
        {
            m_groups = allgroups;

            InitializeComponent();

            BuildSchema();
        }

        private void BuildSchema()
        {
            GroupList.Children.Clear();
            groupsByNum.Clear();
            for (int i = 0; i < m_groups.Count; i++)
            {
                int num = (m_groups[i] as DisplayedGroupModel).Groupnum;
                if (!groupsByNum.ContainsKey(num))
                {
                    GroupHead gh = new GroupHead(num);
                    gh.NumGroupChanged += Gh_NumGroupChanged;
                    gh.Organization = (m_groups[i] as DisplayedGroupModel).Organizationname;
                    gh.OrganizationId = (m_groups[i] as DisplayedGroupModel).Idcommongroup;
                    groupsByNum.Add(num, gh);
                    GroupList.Children.Add(gh);
                }

                GroupHead group = groupsByNum[num];
                group.AddGroup(m_groups[i] as DisplayedGroupModel);
            }
        }
        #endregion

        private void SaveBt_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < GroupList.Children.Count; i++)
            {
                GroupHead gh = GroupList.Children[i] as GroupHead;
                int IdHead = gh.SelectedIdHeader;
                for (int j = 0; j < gh.ModelCount; j++)
                {
                    GroupModel gm = gh.GetModel(j);
                    if (gm.Idcommongroup == IdHead) continue;

                    gm.Idcommongroup = IdHead;
                    gm.Update();
                }
            }
            DialogResult = true;
        }

        private void Gh_NumGroupChanged(object sender, NumGroupChangedArgs e)
        {
            int id = e.Id;
            int num = e.GroupNum;
            for (int i = 0; i < m_groups.Count; i++)
                if (m_groups[i].Idgroup == id)
                {
                    m_groups[i].Groupnum = num;
                    (m_groups[i] as GroupModel).Update();
                    break;
                }

            BuildSchema();
        }
    }
}
