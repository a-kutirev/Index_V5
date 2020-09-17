using ClassLibrary;
using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UtilsLib.RegroupNew
{
    /// <summary>
    /// Логика взаимодействия для GroupHeaderControl.xaml
    /// </summary>
    public partial class GroupHeaderControl : UserControl, INotifyPropertyChanged
    {
        private GroupHeaderModel m_model;
        private int m_GroupNumber = -1;
        private string m_header = "";

        

        public event PropertyChangedEventHandler PropertyChanged;

        public int GroupNumber { get => m_GroupNumber; set => m_GroupNumber = value; }
        public string Header
        {
            get => m_header;
            set
            {
                m_header = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Header"));
            }
        }

        public GroupHeaderModel Model { get => m_model; set => m_model = value; }

        public void SetEmpty()
        {
            Header = "+ Новая группа";
        }

        public GroupHeaderControl()
        {
            InitializeComponent();
            this.DataContext = this;

            GroupItemPanel.Children.Clear();
        }

        public void Add(GroupItemControl item)
        {
            item.Tag = this;
            item.MouseDown += Item_MouseDown;
            GroupItemPanel.Children.Add(item);
        }

        public void Remove(GroupItemControl item)
        {
            if(GroupItemPanel.Children.Contains(item))
                GroupItemPanel.Children.Remove(item);
        }


        private GroupItemControl SelectedItem;
        private void Item_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SelectedItem = sender as GroupItemControl;
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (SelectedItem == null) return;

            base.OnMouseMove(e);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DataObject data = new DataObject(typeof(GroupItemControl), SelectedItem);
                DragDrop.DoDragDrop(this, data, DragDropEffects.Move | DragDropEffects.Copy);
            }
        }

        public int GetItemCount()
        {
            return GroupItemPanel.Children.Count;
        }

        private void UserControl_Drop(object sender, DragEventArgs e)
        {
            GroupItemControl groupItemControl = e.Data.GetData(typeof(GroupItemControl)) as GroupItemControl;

            if (groupItemControl.Tag == this) return;

            if (m_GroupNumber == -1)
            {
                this.Margin = new Thickness(10, 20, 10, 20);
                this.Header = groupItemControl.Model.Organizationname;
                this.GroupNumber = CalculateNewNumber();                
                this.Model = new GroupHeaderModel(groupItemControl.Model.Idcommongroup);

                GroupHeaderControl emptyghc = new GroupHeaderControl();
                emptyghc.Tag = this.Tag;
                emptyghc.SetEmpty();
                (this.Tag as WrapPanel).Children.Add(emptyghc);
            }
            else
            {

                bool CheckHeadersAndContacts = CheckHeadAndCont(groupItemControl.Model.Idcommongroup);

                if (!CheckHeadersAndContacts)
                {
                    SelectHeaderDataWindow shdw = new SelectHeaderDataWindow(m_model.Idgroupheader, groupItemControl.Model.Idcommongroup);

                    if ((bool)shdw.ShowDialog())
                    {

                    }
                }
            }

            GroupHeaderControl ghc_parent = groupItemControl.Tag as GroupHeaderControl;
            ghc_parent.Remove(groupItemControl);
            if(ghc_parent.GetItemCount() == 0)
            {
                (ghc_parent.Tag as WrapPanel).Children.Remove(ghc_parent);
            }

            groupItemControl.Tag = this;
            Add(groupItemControl);            

            Sort();
        }

        private int CalculateNewNumber()
        {
            return 111;
        }

        private bool CheckHeadAndCont(int idCommonGroup)
        {
            bool result = true;

            GroupHeaderModel newModel = new GroupHeaderModel(idCommonGroup);

            if ((newModel.Idorganization != m_model.Idorganization) || (newModel.Idgeo != m_model.Idgeo))
                result = false;

            string sql1 = $"select * from commongroup_contacts where idcommongroup = {m_model.Idgroupheader}";
            string sql2 = $"select * from commongroup_contacts where idcommongroup = {idCommonGroup}";

            List<commongroup_contactModel> list1 = 
                (List<commongroup_contactModel>)DBWrapper.MySqlWrapper.Select(sql1).ToList<commongroup_contactModel>();
            List<commongroup_contactModel> list2 = 
                (List<commongroup_contactModel>)DBWrapper.MySqlWrapper.Select(sql2).ToList<commongroup_contactModel>();

            if (list1.Count != list2.Count) result = false;

            return result;   
        }

        private void Sort()
        {
            SortedDictionary<TimeSpan, DisplayedGroupModel> modlist = new SortedDictionary<TimeSpan, DisplayedGroupModel>();
            for(int i = 0; i < GroupItemPanel.Children.Count; i++)
            {
                TimeSpan key = ((GroupItemControl)GroupItemPanel.Children[i]).Model.Grouptime;

                while (modlist.ContainsKey(key))
                    key += new TimeSpan(0, 0, 1);

                modlist.Add( key, ((GroupItemControl)GroupItemPanel.Children[i]).Model);
            }

            GroupItemPanel.Children.Clear();

            for(int i = 0; i < modlist.Count; i++)
            {
                GroupItemControl gic = new GroupItemControl();
                gic.Model = modlist.ElementAt(i).Value;
                gic.Tag = this;
                gic.Margin = new Thickness(5, 5, 5, 5);
                gic.MouseDown += Item_MouseDown;
                GroupItemPanel.Children.Add(gic);
            }
        }
    }
}
