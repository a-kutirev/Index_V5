using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Tours_V5
{
    /// <summary>
    /// Логика взаимодействия для SearchGroupByOrganizationWindow.xaml
    /// </summary>
    public partial class SearchGroupByOrganizationWindow : Window
    {

        private bool loaded = false;
        private string sql = "select * from organizations";
        private string sql2 = "select * from allgroupforsearch where idorganization = '{0}'";

        private DataView organizationsView;
        private DataTable organizationsTable;
        public DateTime? date = null;

        public SearchGroupByOrganizationWindow()
        {
            InitializeComponent();

            organizationsTable = DBWrapper.MySqlWrapper.Select(sql);
        }

        private void FilterTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!loaded) return;
            organizationsView = organizationsTable.DefaultView;
            organizationsView.RowFilter = $"organizationname like '%{FilterTxt.Text}%'";
            OrganizationsDataGrid.ItemsSource = organizationsView;
        }

        private void OrganizationsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;

            DataRow dr = (e.AddedItems[0] as DataRowView).Row;
            int idOrg = int.Parse(dr["idorganization"].ToString());

            string sqlGroup = string.Format(sql2, idOrg);
            DataTable tmp = DBWrapper.MySqlWrapper.Select(sqlGroup);
            GroupDataGrid.ItemsSource = tmp.DefaultView;
        }

        private void CloseBt_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loaded = true;
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;

            DataRow dr = (row.Item as DataRowView).Row;
            date = DateTime.Parse(dr["groupdate"].ToString());
            DialogResult = true;            
        }
    }
}
