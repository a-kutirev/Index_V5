using ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UtilsLib.RegroupNew
{
    /// <summary>
    /// Логика взаимодействия для RegroupNewWindow.xaml
    /// </summary>
    public partial class RegroupNewWindow : Window
    {
        #region members
            Dictionary<int, GroupHeaderControl> groupsByNum = new Dictionary<int, GroupHeaderControl>();
            List<DisplayedGroupModel> m_groups;
        #endregion

        #region Ctor
        public RegroupNewWindow(List<DisplayedGroupModel> grp)
        {
            m_groups = grp;

            InitializeComponent();

            BuildSchema();
        }
        #endregion

        private void BuildSchema()
        {
            GroupListPanel.Children.Clear();

            for(int i = 0; i < m_groups.Count; i++)
            {
                int gnum = m_groups[i].Groupnum;
                if(!groupsByNum.ContainsKey(gnum))
                {
                    GroupHeaderControl ghc = new GroupHeaderControl();
                    ghc.Margin = new Thickness(10,20,10,20);
                    ghc.Header = m_groups[i].Organizationname;
                    ghc.GroupNumber = gnum;
                    ghc.Tag = GroupListPanel;                   
                    ghc.Model = new GroupHeaderModel(m_groups[i].Idcommongroup);
                    groupsByNum.Add(gnum, ghc);
                    GroupListPanel.Children.Add(ghc);
                }

                GroupItemControl gic = new GroupItemControl();
                gic.Model = m_groups[i];
                gic.Margin = new Thickness(5,5,5,5);

                gic.Time = m_groups[i].Grouptime.ToString(@"hh\:mm");
                gic.ExpoName = m_groups[i].Tourname;

                groupsByNum[gnum].Add(gic);
            }

            GroupHeaderControl emptyghc = new GroupHeaderControl();
            emptyghc.Tag = GroupListPanel;
            emptyghc.SetEmpty();
            GroupListPanel.Children.Add(emptyghc);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
