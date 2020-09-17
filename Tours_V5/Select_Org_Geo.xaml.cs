using ClassLibrary.Models;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Tours_V5
{
    /// <summary>
    /// Логика взаимодействия для Select_Org_Geo.xaml
    /// </summary>
    public partial class Select_Org_Geo : Window
    {
        #region members
        private DataTable dt;
        private DataView view;
        private SelectWindowType type;
        private int m_id;
        private string m_name;

        public int Id { get => m_id; set => m_id = value; }
        public string Name1 { get => m_name; set => m_name = value; }
        #endregion

        #region Constructor
        public Select_Org_Geo(SelectWindowType wintype)
        {
            InitializeComponent();

            type = wintype;

            if(wintype == SelectWindowType.Organization)
            {
                dt = DBWrapper.MySqlWrapper.SelectAllOrganizations();
                DataGridTextColumn intcol = new DataGridTextColumn();
                intcol.Binding = new Binding("idorganization");
                intcol.Visibility = Visibility.Collapsed;
                grid.Columns.Add(intcol);
                DataGridTextColumn textcol = new DataGridTextColumn();
                textcol.Binding = new Binding("organizationname");
                textcol.Width = 400;
                grid.Columns.Add(textcol);
            }
            else
            {
                dt = DBWrapper.MySqlWrapper.SelectAllGeos();
                DataGridTextColumn intcol = new DataGridTextColumn();
                intcol.Binding = new Binding("idgeo");
                intcol.Visibility = Visibility.Collapsed;
                grid.Columns.Add(intcol);

                DataGridTextColumn textcol = new DataGridTextColumn();
                textcol.Binding = new Binding("geoname");
                textcol.Width = 400;
                grid.Columns.Add(textcol);
            }
            view = dt.DefaultView;
            grid.ItemsSource = view;

            EditableTextBox.Focus();
        }
        #endregion

        private void AddBt_Click(object sender, RoutedEventArgs e)
        {
            Name1 = EditableTextBox.Text;
            if(type == SelectWindowType.Organization)
            {
                OrganizationModel om = new OrganizationModel();
                om.Organizationname = Name1;
                Id = om.Insert();
            }
            else
            {
                GeoModel gm = new GeoModel();
                gm.Geoname = Name1;
                Id = gm.Insert();
            }
            DialogResult = true;
        }

        private void EditableTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string t = (sender as TextBox).Text;
            if(type == SelectWindowType.Organization)
                view.RowFilter = $"organizationname LIKE '%{t}%'";
            else
                view.RowFilter = $"geoname LIKE '%{t}%'";
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            Id = int.Parse((row.Item as DataRowView).Row[0].ToString());
            Name1 = (row.Item as DataRowView).Row[1].ToString();
            DialogResult = true;
        }
    }

    public enum SelectWindowType
    {
        Organization,
        Geography
    }
}
