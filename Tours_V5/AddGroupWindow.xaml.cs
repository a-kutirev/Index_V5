using ClassLibrary;
using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using WpfControlLibrary.GroupControls;
using WpfControlLibrary.PagedTableControl;

namespace Tours_V5
{
    /// <summary>
    /// Логика взаимодействия для AddGroupWindow.xaml
    /// </summary>
    public partial class AddGroupWindow : Window, INotifyPropertyChanged
    {        
        #region Members
        public event PropertyChangedEventHandler PropertyChanged;

        public int GroupNum { get; set; }

        private GroupHeaderModel oldGroupHeader = new GroupHeaderModel();
        private Dictionary<int, int> oldContacts = new Dictionary<int, int>();

        private int m_idorg = 0;
        private int m_idgeo = 0;
        private string m_org;
        private string m_geo;
        private DataTable CurContacts;
        private DataTable selContacts;
        private int tmpId = 1;

        private bool m_ShowAllContacts = false;
        private bool m_edit = false;

        private GroupHeaderModel groupHeaderModel;

        public string Org
        {
            get => m_org;
            set
            {
                m_org = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Org"));
            }
        }
        public string Geo
        {
            get => m_geo;
            set
            {
                m_geo = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Geo"));
            }
        }

        public bool ShowAllContacts
        {
            get => m_ShowAllContacts;
            set
            {
                m_ShowAllContacts = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowAllContacts"));
                string sql = "";
                if (!m_ShowAllContacts) sql = $"GetContactsByOrganization({m_idorg})";
                else sql = "select * from contacts";
                selContacts = DBWrapper.MySqlWrapper.Select(sql);
                contactsTable.Source = selContacts;
            }
        }
        #endregion

        private int m_gn = -1;

        #region Constructor
        public AddGroupWindow(List<DisplayedGroupModel> groups)
        {
            InitializeComponent();
            this.DataContext = this;
            m_edit = true;

            groupHeaderModel = new GroupHeaderModel(groups[0].Idcommongroup);
            oldGroupHeader.Idorganization = groupHeaderModel.Idorganization;
            oldGroupHeader.Idgeo = groupHeaderModel.Idgeo;
            oldGroupHeader.Idgroupheader = groupHeaderModel.Idgroupheader;
            m_idorg = groupHeaderModel.Idorganization;
            Org = (new OrganizationModel(m_idorg)).Organizationname;
            m_idgeo = groupHeaderModel.Idgeo;
            Geo = (new GeoModel(m_idgeo)).Geoname;

            for(int i = 0; i < groups.Count; i++)
            {
                GroupEditControl gec = new GroupEditControl(groups[i]);
                gec.AddGroupOnClick += Gec_AddGroupOnClick;
                gec.RemoveGroupOnClick += Gec_RemoveGroupOnClick;
                GroupList.Children.Add(gec);
                if (i == 0)
                    m_gn = gec.Model.Groupnum;
            }

            Page1.Visibility = Visibility.Visible;
            Page2.Visibility = Visibility.Collapsed;

            CurContacts = new DataTable();
            CurContacts.Columns.Add("id", typeof(int));
            CurContacts.Columns.Add("name", typeof(string));
            CurContacts.Columns.Add("post", typeof(string));
            CurContacts.Columns.Add("phone", typeof(string));

            contactsTable.AddColumn("Id", "idcontact", 40, true);
            contactsTable.AddColumn("ФИО контакта", "contactname", 250, false);
            contactsTable.AddColumn("Должность", "contactpost", 200, false);
            contactsTable.AddColumn("Телефон", "contactphone", 150, false);
            contactsTable.Buttons = AddedButton.None;
            contactsTable.SelectionGridChange += ContactsTable_SelectionGridChange;

            string sql = $"GetContactsByOrganization({m_idorg})";
            selContacts = DBWrapper.MySqlWrapper.Select(sql);
            contactsTable.Source = selContacts;
            ShowAllContacts = false;

            sql = $"select * from commongroup_contacts where idcommongroup = {groups[0].Idcommongroup}";
            DataTable tmp = DBWrapper.MySqlWrapper.Select(sql);

            for (int i = 0; i < tmp.Rows.Count; i++)
            {
                DataRow dr = CurContacts.NewRow();

                int idcont = (int)tmp.Rows[i]["idcontact"];
                int idcommongroup_contact = (int)tmp.Rows[i]["idcommongroup_contact"];
                if (!oldContacts.ContainsKey(idcont))
                {
                    oldContacts.Add(idcont, idcommongroup_contact);
                    ContactModel cm = new ContactModel(idcont);

                    dr["id"] = cm.Idcontact;
                    dr["name"] = cm.Contactname;
                    dr["phone"] = cm.Contactpost;
                    dr["post"] = cm.Contactphone;
                    CurContacts.Rows.Add(dr);
                }
            }

            selectedContactsGrid.ItemsSource = CurContacts.DefaultView;

            CheckButton();
            CheckTransitionButton();
        }

        public AddGroupWindow(int gn)
        {
            GroupNum = gn;
            InitializeComponent();
            this.DataContext = this;

            groupHeaderModel = new GroupHeaderModel();

            GroupEditControl gec = new GroupEditControl(tmpId++);
            gec.Model.Groupnum = GroupNum;
            gec.AddGroupOnClick += Gec_AddGroupOnClick;
            gec.RemoveGroupOnClick += Gec_RemoveGroupOnClick;
            GroupList.Children.Add(gec);

            Page1.Visibility = Visibility.Visible;
            Page2.Visibility = Visibility.Collapsed;

            CurContacts = new DataTable();
            CurContacts.Columns.Add("id", typeof(int));
            CurContacts.Columns.Add("name", typeof(string));
            CurContacts.Columns.Add("post", typeof(string));
            CurContacts.Columns.Add("phone", typeof(string));

            contactsTable.AddColumn("Id", "idcontact", 40, true);
            contactsTable.AddColumn("ФИО контакта", "contactname", 250, false);
            contactsTable.AddColumn("Должность", "contactpost", 200, false);
            contactsTable.AddColumn("Телефон", "contactphone", 150, false);
            contactsTable.Buttons = AddedButton.None;
            contactsTable.SelectionGridChange += ContactsTable_SelectionGridChange; ;

            CheckButton();
            CheckTransitionButton();
        }

        ContactModel curSelectedContact;
        private void ContactsTable_SelectionGridChange(object sender, GridButtonEventArgs e)
        {
            int idContact = e.id;
            curSelectedContact = new ContactModel(idContact);
        }
        #endregion

        #region Group add-remove events
        private void Gec_RemoveGroupOnClick(object sender, EventArgs e)
        {

            GroupList.Children.Remove(sender as UIElement);
            for(int i = 0; i < GroupList.Children.Count; i++)
                (GroupList.Children[i] as GroupEditControl).ReCheck();

            CheckButton();
        }

        private void Gec_AddGroupOnClick(object sender, AddGroupEventArgs e)
        {
            GroupEditControl gec = new GroupEditControl(tmpId++);
            gec.Model.Groupnum = GroupNum;
            gec.AddGroupOnClick += Gec_AddGroupOnClick;
            gec.RemoveGroupOnClick += Gec_RemoveGroupOnClick;
            GroupList.Children.Add(gec);
            CheckButton();
            gec.SetArguments(e);
        }

        private void CheckButton()
        {
            if (GroupList.Children.Count > 1)
            {
                for (int i = 0; i < GroupList.Children.Count; i++)
                {
                    (GroupList.Children[i] as GroupEditControl).EnableRemoveBt = true;
                    (GroupList.Children[i] as GroupEditControl).EnableAddBt = false;
                    if (i == (GroupList.Children.Count - 1))
                        (GroupList.Children[i] as GroupEditControl).EnableAddBt = true;
                }

            }
            else
            {
                (GroupList.Children[0] as GroupEditControl).EnableRemoveBt = false;
                (GroupList.Children[0] as GroupEditControl).EnableAddBt = true;
            }
        }
        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        #region Переходы / Сохранение
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(Page1.Visibility == Visibility.Visible)
            {
                Page1.Visibility = Visibility.Collapsed;
                Page2.Visibility = Visibility.Visible;
            }
            CheckTransitionButton();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (Page2.Visibility == Visibility.Visible)
            {
                Page1.Visibility = Visibility.Visible;
                Page2.Visibility = Visibility.Collapsed;                
            }
            CheckTransitionButton();
        }

        private void CheckTransitionButton()
        {
            if (Page1.Visibility == Visibility.Visible)
            {
                BackBt.IsEnabled = false;
                NextBt.IsEnabled = true;
            }
            if (Page2.Visibility == Visibility.Visible)
            {
                BackBt.IsEnabled = true;
                NextBt.IsEnabled = false;
            }
        }

        private void SaveBt_Click(object sender, RoutedEventArgs e)
        {
            if (m_idorg == 0)
            {
                MessageBox.Show("Не выбрана организация");
                return;
            }
            if (m_idgeo == 0)
            {
                MessageBox.Show("Не выбрано географическое предложение");
                return;
            }

            if (!m_edit)
            {
                // 1. Сохранение в groupheader
                //int idgroupheader = 0;
                int idgroupheader = groupHeaderModel.Insert();
                // 2. Сохранение групп
                for (int i = 0; i < GroupList.Children.Count; i++)
                {
                    GroupEditControl gec = (GroupEditControl)GroupList.Children[i];
                    gec.Insert(idgroupheader);
                }
                // 3. Сохранение контактных данных
                for (int i = 0; i < CurContacts.Rows.Count; i++)
                {
                    int idContact = (int)CurContacts.Rows[i]["id"];
                    commongroup_contactModel m = new commongroup_contactModel(idgroupheader, idContact);
                    m.Insert();
                }
            }
            else
            {
                int idgroupheader = 0;
                if ((groupHeaderModel.Idgeo != oldGroupHeader.Idgeo) || 
                    (groupHeaderModel.Idorganization != oldGroupHeader.Idorganization))
                {
                    idgroupheader = groupHeaderModel.Insert();

                    for (int i = 0; i < CurContacts.Rows.Count; i++)
                    {
                        int idContact = (int)CurContacts.Rows[i]["id"];
                        commongroup_contactModel m = new commongroup_contactModel(idgroupheader, idContact);
                        m.Insert();
                    }
                }
                else
                {
                    idgroupheader = oldGroupHeader.Idgroupheader;
                }

                for (int i = 0; i < GroupList.Children.Count; i++)
                {
                    GroupEditControl gec = (GroupEditControl)GroupList.Children[i];
                    gec.Model.Idcommongroup = idgroupheader;
                    if (gec.Model.Idgroup < 100)
                    {
                        gec.Model.Groupnum = m_gn;
                        gec.Insert(idgroupheader);
                    }
                    else
                    {
                        gec.Update();
                    }
                }
                for (int i = 0; i < CurContacts.Rows.Count; i++)
                {
                    int idContact = (int)CurContacts.Rows[i]["id"];
                    commongroup_contactModel m = new commongroup_contactModel(idgroupheader, idContact);
                    m.Insert();
                }
            }

            DialogResult = true;
        }
        #endregion

        #region Выборка организации и географии
        private void SelectOrgBt_Click(object sender, RoutedEventArgs e)
        {
            string name = (sender as Button).Name;
            Select_Org_Geo sog;
            switch(name)
            {
                case "SelectOrgBt":
                    sog = new Select_Org_Geo(SelectWindowType.Organization);
                    if((bool)sog.ShowDialog())
                    {
                        m_idorg = sog.Id;
                        Org = sog.Name1;
                        string sql = $"GetContactsByOrganization({m_idorg})";
                        selContacts = DBWrapper.MySqlWrapper.Select(sql);
                        contactsTable.Source = selContacts;
                        groupHeaderModel.Idorganization = m_idorg;
                        ShowAllContacts = false;
                    }
                    break;
                case "SelectGeogBt":
                    sog = new Select_Org_Geo(SelectWindowType.Geography);
                    if ((bool)sog.ShowDialog())
                    {
                        m_idgeo = sog.Id;
                        Geo = sog.Name1;
                        groupHeaderModel.Idgeo = m_idgeo;
                    }
                    break;
                default:
                    sog = new Select_Org_Geo(SelectWindowType.Organization);
                    break;
            }            
        }
        #endregion

        #region Contacts tab
        private void AddContactToList_Click(object sender, RoutedEventArgs e)
        {
            if (curSelectedContact == null) return;
            DataRow dr = CurContacts.NewRow();
            dr["id"] = curSelectedContact.Idcontact;
            dr["name"] = curSelectedContact.Contactname;
            dr["phone"] = curSelectedContact.Contactphone;
            dr["post"] = curSelectedContact.Contactpost;
            CurContacts.Rows.Add(dr);
            selectedContactsGrid.ItemsSource = CurContacts.DefaultView;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            AddContactWindow acw = new AddContactWindow();
            if((bool)acw.ShowDialog())
            {
                ContactModel cm = new ContactModel(acw.Id);
                DataRow dr = CurContacts.NewRow();
                dr["id"] = cm.Idcontact;
                dr["name"] = cm.Contactname;
                dr["phone"] = cm.Contactphone;
                dr["post"] = cm.Contactpost;
                CurContacts.Rows.Add(dr);
            }
        }
        #endregion

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(ShowAllContacts)
            {                
                string sql = $"select * from contacts where contactname like '%{(sender as TextBox).Text}%'";
                selContacts = DBWrapper.MySqlWrapper.Select(sql);
                contactsTable.Source = selContacts;
            }
        }
    }
}
