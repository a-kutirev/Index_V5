using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Tours_V5
{
    /// <summary>
    /// Логика взаимодействия для SearchContactByOrganizationWindow.xaml
    /// </summary>
    public partial class SearchContactByOrganizationWindow : Window
    {
        private bool loaded = false;

        private string sql = "select * from organizations";
        private string sql2 = 
            "select * from contacts where idcontact in " +
            "(SELECT distinct idcontact FROM commongroup_contacts where idcommongroup in (SELECT idgroupheader FROM groupheaders where idorganization = {0}))";
        private DataView organizationsView;
        private DataTable organizationsTable;

        public SearchContactByOrganizationWindow()
        {
            InitializeComponent();

            organizationsTable = DBWrapper.MySqlWrapper.Select(sql);
        }

        private void CloseBt_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void OrganizationDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;
            DataRow dr = (e.AddedItems[0] as DataRowView).Row;
            int idOrg = int.Parse(dr["idorganization"].ToString());

            string sqlContacts = string.Format(sql2, idOrg);
            DataTable tmp = DBWrapper.MySqlWrapper.Select(sqlContacts);
            ContactsDataGrid.ItemsSource = tmp.DefaultView;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loaded = true;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!loaded) return;
            organizationsView = organizationsTable.DefaultView;
            organizationsView.RowFilter = $"organizationname like '%{FilterTxt.Text}%'";
            OrganizationsDataGrid.ItemsSource = organizationsView;
        }
    }
}
