using ClassLibrary;
using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
using WpfControlLibrary.PagedTableControl;

namespace Tours_V5
{
    /// <summary>
    /// Логика взаимодействия для AddEventContactWindow.xaml
    /// </summary>
    public partial class AddEventContactWindow : Window, INotifyPropertyChanged
    {
        #region Members
        private DataTable CurContacts;
        private DataTable selContacts;
        private bool m_ShowAllContacts = false;
        private EventGroupModel m_model;
        private ContactModel curSelectedContact;
        private bool contactsChanged = false;

        public bool ShowAllContacts
        {
            get => m_ShowAllContacts;
            set
            {
                m_ShowAllContacts = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowAllContacts"));

                string sql = "";
                if (!m_ShowAllContacts) sql = $"GetContactsByOrganization({m_model.Idorganization})";
                else sql = "select * from contacts";
                selContacts = DBWrapper.MySqlWrapper.Select(sql);
                contactsTable.Source = selContacts;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructor
        public AddEventContactWindow(EventGroupModel model)
        {
            m_model = model;

            InitializeComponent();
            this.DataContext = this;            

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

            string sql = $"GetContactsByOrganization({m_model.Idorganization})";
            selContacts = DBWrapper.MySqlWrapper.Select(sql);
            contactsTable.Source = selContacts;
            ShowAllContacts = false;

            if(model.ideventgroup != 0)
            {
                sql = $"select * from eventgroup_contacts where ideventgroup = {model.ideventgroup}";
                DataTable tmp = DBWrapper.MySqlWrapper.Select(sql);

                CurContacts.Rows.Clear();
                for(int i = 0; i < tmp.Rows.Count; i++)
                {
                    DataRow dr = CurContacts.NewRow();

                    ContactModel cm = new ContactModel(int.Parse(tmp.Rows[i]["idcontact"].ToString()));

                    dr["id"] = cm.Idcontact;
                    dr["name"] = cm.Contactname;
                    dr["post"] = cm.Contactpost;
                    dr["phone"] = cm.Contactphone;

                    CurContacts.Rows.Add(dr);
                }

                selectedContactsGrid.ItemsSource = CurContacts.DefaultView;
            }
        }
        #endregion

        #region Events
        private void ContactsTable_SelectionGridChange(object sender, GridButtonEventArgs e)
        {
            int idContact = e.id;
            curSelectedContact = new ContactModel(idContact);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ShowAllContacts)
            {
                string sql = $"select * from contacts where contactname like '%{(sender as TextBox).Text}%'";
                selContacts = DBWrapper.MySqlWrapper.Select(sql);
                contactsTable.Source = selContacts;
            }
        }

        private void SaveBt_Click(object sender, RoutedEventArgs e)
        {
            if(contactsChanged)
            {
                string delsql = $"delete from eventgroup_contacts where ideventgroup = {m_model.ideventgroup}";
                DBWrapper.MySqlWrapper.Execute(delsql);
                for(int i = 0; i < CurContacts.Rows.Count; i++)
                {
                    eventgroup_contactModel m = new eventgroup_contactModel();
                    m.Ideventgroup = m_model.ideventgroup;
                    m.Idcontact = int.Parse(CurContacts.Rows[i]["Id"].ToString());
                    m.Insert();
                }
            }

            DialogResult = true;
        }

        private void AddContactToList_Click(object sender, RoutedEventArgs e)
        {
            contactsChanged = true;

            DataRow dr = CurContacts.NewRow();
            dr["id"] = curSelectedContact.Idcontact;
            dr["name"] = curSelectedContact.Contactname;
            dr["phone"] = curSelectedContact.Contactphone;
            dr["post"] = curSelectedContact.Contactpost;
            CurContacts.Rows.Add(dr);
            selectedContactsGrid.ItemsSource = CurContacts.DefaultView;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            contactsChanged = true;
            AddContactWindow acw = new AddContactWindow();
            if ((bool)acw.ShowDialog())
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


        //delete from selected contacts
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            contactsChanged = true;
            int id = (int)((sender as Button).CommandParameter);

            for(int i = 0; i < CurContacts.Rows.Count; i++)
            {
                int idcontact = int.Parse(CurContacts.Rows[i]["Id"].ToString());
                if (idcontact == id)
                {
                    CurContacts.Rows.Remove(CurContacts.Rows[i]);
                    break;
                }

                selectedContactsGrid.ItemsSource = CurContacts.DefaultView;
            }
        }
    }
}
