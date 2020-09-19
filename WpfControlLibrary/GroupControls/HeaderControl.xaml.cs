using ClassLibrary;
using ClassLibrary.Models;
using DBWrapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfControlLibrary.GroupControls
{
    /// <summary>
    /// Логика взаимодействия для HeaderControl.xaml
    /// </summary>
    public partial class HeaderControl : UserControl, INotifyPropertyChanged
    {
        #region Fields & events
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler GuidComboUpdate;
        public event EventHandler<GroupEventClickArgs> GroupEvClick;
        public event EventHandler<EditEventClickArgs> EditEvClick;

        private DisplayedHeaderModel m_dhm = null;
        private string m_fullName = "";
        private List<string> m_contactSource = new List<string>();
        private List<GroupControl> m_internalGroups = new List<GroupControl>();
        private bool loaded = false;


        public DisplayedHeaderModel Dhm
        {
            get => m_dhm;
            set
            {
                m_dhm = value;
                FullName = $"{m_dhm.Organizationname} ({m_dhm.Geoname})";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Dhm"));
            }
        }        

        public string FullName
        {
            get => m_fullName;
            set
            {
                m_fullName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs ("FullName"));
            }
        }

        public List<string> ContactSource
        {
            get => m_contactSource;
            set
            {
                m_contactSource = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ContactSource"));
            }
        }
        #endregion

        #region Constructor
        public HeaderControl()
        {
            InitializeComponent();
            Margin = new Thickness(20, 6, 25, 0);
        }        

        public HeaderControl(int id)
        {
            InitializeComponent();
            this.DataContext = this;

            Margin = new Thickness(20, 6, 25, 0);
            Dhm = new DisplayedHeaderModel(id);            

            DataTable tmp = MySqlWrapper.SelectContactsByIdHeader(id);

            for(int i = 0; i < tmp.Rows.Count; i++)
            {
                ContactModel cm = new ContactModel((int)tmp.Rows[i]["idcontact"]);
                ContactSource.Add($"{cm.Contactname} ({cm.Contactpost}) {cm.Contactphone}");
            }
        }
        #endregion

        #region Добавить группу
        public void AddGroup(DisplayedGroupModel dgm)
        {
            GroupControl gc = new GroupControl(dgm);            
            if(m_internalGroups.Count >= 1)
                m_internalGroups[m_internalGroups.Count - 1].Last = false;
            GroupList.Children.Add(gc);
            gc.CompleteGroupClick += Gc_CompleteGroupClick;
            gc.EditGroupClick += Gc_EditGroupClick;
            gc.DeleteGroupClick += Gc_DeleteGroupClick;
            gc.Last = true;
            m_internalGroups.Add(gc);
            gc.GuidComboReload += Gc_GuidComboReload;

            bool allDeleted = true;
            bool existOpened = false;
            bool existCompleted = false;
            bool hide = true;
            for (int i = 0; i < GroupList.Children.Count; i++)
            {
                GroupControl grp = GroupList.Children[i] as GroupControl;
                if (grp.Visibility == Visibility.Visible) hide = false;
                //if ((grp.Model.Groupstatus & 4) != 4) allCompleted = false;
                //else allDeleted = false;
                //if ((grp.Model.Groupstatus & 1) != 1) allDeleted = false;
                //else allCompleted = false;

                if((grp.Model.Groupstatus & 4) == 4)
                {
                    allDeleted = false;
                    existCompleted = true;
                }
                if((grp.Model.Groupstatus & 5) == 0)
                {
                    allDeleted = false;
                    existOpened = true;
                }
            }

            LinearGradientBrush lgb = new LinearGradientBrush();
            lgb.StartPoint = new Point(0.5, 0);
            lgb.EndPoint = new Point(0.5, 1);
            GradientStop gs = new GradientStop(Colors.White, 0);
            lgb.GradientStops.Add(gs);
            gs = new GradientStop((Color)ColorConverter.ConvertFromString("#f0f8ff"), 0.1);
            lgb.GradientStops.Add(gs);
            gs = new GradientStop((Color)ColorConverter.ConvertFromString("#f0f8ff"), 0.9);
            lgb.GradientStops.Add(gs);
            gs = new GradientStop((Color)ColorConverter.ConvertFromString("#b0e2ff"), 1);
            lgb.GradientStops.Add(gs);
            brd.Background = lgb;

            if (existCompleted && !existOpened)
            {
                lgb = new LinearGradientBrush();
                lgb.StartPoint = new Point(0.5, 0);
                lgb.EndPoint = new Point(0.5, 1);
                gs = new GradientStop(Colors.White, 1);
                lgb.GradientStops.Add(gs);
                gs = new GradientStop((Color)ColorConverter.ConvertFromString("#FFD3FFD5"), 0.2);
                lgb.GradientStops.Add(gs);
                gs = new GradientStop((Color)ColorConverter.ConvertFromString("#FFD3FFD5"), 0.8);
                lgb.GradientStops.Add(gs);
                gs = new GradientStop(Colors.LightGreen, 0);
                lgb.GradientStops.Add(gs);
                brd.Background = lgb;
            }

            if (allDeleted && !existOpened)
            {
                lgb = new LinearGradientBrush();
                lgb.StartPoint = new Point(0.5, 0);
                lgb.EndPoint = new Point(0.5, 1);
                gs = new GradientStop(Colors.White, 1);
                lgb.GradientStops.Add(gs);
                gs = new GradientStop((Color)ColorConverter.ConvertFromString("#FFFFD3D3"), 0.2);
                lgb.GradientStops.Add(gs);
                gs = new GradientStop((Color)ColorConverter.ConvertFromString("#FFFFD3D3"), 0.8);
                lgb.GradientStops.Add(gs);
                gs = new GradientStop(Colors.Salmon, 0);
                lgb.GradientStops.Add(gs);
                brd.Background = lgb;
            }

            if (hide) this.Visibility = Visibility.Collapsed;
            else this.Visibility = Visibility.Visible;
        }
        #endregion

        #region Group Events Handler
        private void Gc_GuidComboReload(object sender, EventArgs e)
        {
            GuidComboUpdate?.Invoke(this, EventArgs.Empty);
        }
        private void Gc_DeleteGroupClick(object sender, EventGroup e)
        {
            GroupEventClickArgs gec = new GroupEventClickArgs();
            gec.EventType = "delete";
            gec.IdGroup = e.idgroup;
            GroupEvClick?.Invoke(this, gec);
        }

        private void Gc_EditGroupClick(object sender, EventGroup e)
        {
            GroupEventClickArgs gec = new GroupEventClickArgs();
            gec.EventType = "edit";
            gec.IdGroup = e.idgroup;
            GroupEvClick?.Invoke(this, gec);
        }

        private void Gc_CompleteGroupClick(object sender, EventGroup e)
        {
            GroupEventClickArgs gec = new GroupEventClickArgs();
            gec.EventType = "complete";
            gec.IdGroup = e.idgroup;
            GroupEvClick?.Invoke(this, gec);
        }
        #endregion

        public int GetNumGroup()
        {
            return (GroupList.Children.Count > 0) ? ((GroupControl)GroupList.Children[0]).Model.Groupnum : 0;
        }

        private void EditAllGroup_Click(object sender, RoutedEventArgs e)
        {
            EditEventClickArgs ev = new EditEventClickArgs();
            ev.NumGroup = GetNumGroup();
            EditEvClick?.Invoke(this, ev);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!loaded) return;

            string sql = $"select * from commongroup_contacts where idcommongroup = {m_dhm.Idgroupheader}";
            List<commongroup_contactModel> mods =
                (List<commongroup_contactModel>)DBWrapper.MySqlWrapper.Select(sql).ToList<commongroup_contactModel>();

            int oldval = mods[0].Idcontact;
            int newval = mods[guidComb.SelectedIndex].Idcontact;

            mods[0].Idcontact = newval;
            mods[guidComb.SelectedIndex].Idcontact = oldval;

            mods[0].Update();
            mods[guidComb.SelectedIndex].Update();

            DataTable tmp = MySqlWrapper.SelectContactsByIdHeader(m_dhm.Idgroupheader);

            ContactSource.Clear();
            for (int i = 0; i < tmp.Rows.Count; i++)
            {
                ContactModel cm = new ContactModel((int)tmp.Rows[i]["idcontact"]);
                ContactSource.Add($"{cm.Contactname} ({cm.Contactpost}) {cm.Contactphone}");
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            loaded = true;
        }
    }

    public class GroupEventClickArgs: EventArgs
    {
        public string EventType;
        public int IdGroup;
    }
    public class EditEventClickArgs : EventArgs
    {
        public int NumGroup;
    }
}
