using ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

        #region Построение схемы
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
                    ghc.OnGroupItemDrop += Ghc_OnGroupItemDrop;
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
            emptyghc.OnGroupItemDrop += Ghc_OnGroupItemDrop;
            emptyghc.SetEmpty();
            GroupListPanel.Children.Add(emptyghc);
        }
        #endregion

        #region Events
        private void Ghc_OnGroupItemDrop(object sender, DragEventArgs e)
        {
            GroupItemControl groupItemControl = e.Data.GetData(typeof(GroupItemControl)) as GroupItemControl;
            GroupHeaderControl ghc = sender as GroupHeaderControl;

            if (groupItemControl.Tag == ghc) return;

            if (ghc.GroupNumber == -1)
            {
                ghc.Margin = new Thickness(10, 20, 10, 20);
                ghc.Header = groupItemControl.Model.Organizationname;
                ghc.GroupNumber = CalculateNewNumber();                
                ghc.Model = new GroupHeaderModel(groupItemControl.Model.Idcommongroup);

                GroupHeaderControl emptyghc = new GroupHeaderControl();
                emptyghc.OnGroupItemDrop += Ghc_OnGroupItemDrop;
                emptyghc.Margin = new Thickness(10, 20, 10, 20);
                emptyghc.Tag = ghc.Tag;
                emptyghc.SetEmpty();
                GroupListPanel.Children.Add(emptyghc);

                GroupHeaderControl ghc_parent = groupItemControl.Tag as GroupHeaderControl;
                ghc_parent.Remove(groupItemControl);
                if (ghc_parent.GetItemCount() == 0)
                {
                    (ghc_parent.Tag as WrapPanel).Children.Remove(ghc_parent);
                }

                groupItemControl.Tag = this;
                groupItemControl.Model.Groupnum = ghc.GroupNumber;
                ghc.Add(groupItemControl);
            }
            else
            {
                bool CheckHeadersAndContacts = ghc.CheckHeadAndCont(groupItemControl.Model.Idcommongroup);
                groupItemControl.Model.Groupnum = ghc.GroupNumber;


                if (!CheckHeadersAndContacts)
                {
                    SelectHeaderDataWindow shdw = new SelectHeaderDataWindow(ghc.Model.Idgroupheader, groupItemControl.Model.Idcommongroup);

                    if ((bool)shdw.ShowDialog())
                    {
                        //groupItemControl.Model.Groupnum = ghc.GroupNumber;

                        if (shdw.UseCur)
                        {                            
                            groupItemControl.Model.Idcommongroup = ghc.Model.Idgroupheader;
                            groupItemControl.Model.Organizationname = ghc.GetItem(0).Model.Organizationname ;
                            groupItemControl.Model.Geoname = ghc.GetItem(0).Model.Geoname;
                        }
                        else
                        {
                            ghc.SetNewIdCommongroup(groupItemControl.Model.Idcommongroup);
                        }

                        GroupHeaderControl ghc_parent = groupItemControl.Tag as GroupHeaderControl;
                        ghc_parent.Remove(groupItemControl);
                        if (ghc_parent.GetItemCount() == 0)
                        {
                            (ghc_parent.Tag as WrapPanel).Children.Remove(ghc_parent);
                        }

                        groupItemControl.Tag = ghc;
                        ghc.Add(groupItemControl);
                    }
                }
                else
                {
                    GroupHeaderControl ghc_parent = groupItemControl.Tag as GroupHeaderControl;
                    ghc_parent.Remove(groupItemControl);
                    if (ghc_parent.GetItemCount() == 0)
                    {
                        (ghc_parent.Tag as WrapPanel).Children.Remove(ghc_parent);
                    }

                    groupItemControl.Tag = ghc;
                    ghc.Add(groupItemControl);
                }
            }   
        }

        private int CalculateNewNumber()
        {
            int result = -1;

            List<int> numList = new List<int>();
            for(int  i= 0; i < GroupListPanel.Children.Count; i++)
            {
                GroupHeaderControl ghc = GroupListPanel.Children[i] as GroupHeaderControl;
                numList.Add(ghc.GroupNumber);
            }

            for(int i = 0; i < 20; i++)
            {
                if(!numList.Contains(i))
                {
                    result = i;
                    break;
                }
            }

            return result;
        }
        #endregion

        
        private void CancBt_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void SaveBt_Click(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < GroupListPanel.Children.Count; i++)
            {
                GroupHeaderControl ghc = GroupListPanel.Children[i] as GroupHeaderControl;
                for(int j = 0; j < ghc.GetItemCount(); j++)
                {
                    GroupItemControl gic = ghc.GetItem(j);

                    gic.Model.Update();
                }
            }

            MessageBox.Show("Данные сохранены");
            DialogResult = true;
        }
    }
}
