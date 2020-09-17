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

namespace UtilsLib.RegroupNew
{
    /// <summary>
    /// Логика взаимодействия для SelectHeaderDataWindow.xaml
    /// </summary>
    public partial class SelectHeaderDataWindow : Window, INotifyPropertyChanged
    {
        private string m_Header1;
        private string m_Header2;

        private string m_Con1 = "";
        private string m_Con2 = "";

        public string Header1
        {
            get => m_Header1;
            set
            {
                m_Header1 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Header1"));
            }
        }
        public string Header2
        {
            get => m_Header2;
            set
            {
                m_Header2 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Header2"));
            }
        }

        public string Con1
        {
            get => m_Con1;
            set
            {
                m_Con1 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Con1"));
            }
        }
        public string Con2
        {
            get => m_Con2;
            set
            {
                m_Con2 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Con2"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        public SelectHeaderDataWindow(int curId, int newId)
        {
            InitializeComponent();
            this.DataContext = this;



            string curGeo = new GeoModel(new GroupHeaderModel(curId).Idgeo).Geoname;
            string curOrg = new OrganizationModel(new GroupHeaderModel(curId).Idorganization).Organizationname;

            string newGeo = new GeoModel(new GroupHeaderModel(newId).Idgeo).Geoname;
            string newOrg = new OrganizationModel(new GroupHeaderModel(newId).Idorganization).Organizationname;

            Header1 = $"{curOrg} ({curGeo})";
            Header2 = $"{newOrg} ({newGeo})";

            string sql1 = $"SELECT distinct commongroup_contacts.idcontact FROM commongroup_contacts where idcommongroup = {curId}";
            string sql2 = $"SELECT distinct commongroup_contacts.idcontact FROM commongroup_contacts where idcommongroup = {newId}";

            DataTable list1 = DBWrapper.MySqlWrapper.Select(sql1);
            DataTable list2 = DBWrapper.MySqlWrapper.Select(sql2);

            for (int i = 0; i < list1.Rows.Count; i++)
            {
                ContactModel cm = new ContactModel((int)list1.Rows[i]["idcontact"]);
                Con1 += $"{cm.Contactname} ({cm.Contactpost}) {cm.Contactphone};" + Environment.NewLine;
            }

            for (int i = 0; i < list2.Rows.Count; i++)
            {
                ContactModel cm = new ContactModel((int)list2.Rows[i]["idcontact"]);
                Con2 += $"{cm.Contactname} ({cm.Contactpost}) {cm.Contactphone};" + Environment.NewLine;
            }
        }

        private void CancelBt_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void SaveBt_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
