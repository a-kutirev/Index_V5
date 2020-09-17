using ClassLibrary.Models;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Tours_V5
{
    /// <summary>
    /// Логика взаимодействия для SelectEventWindow.xaml
    /// </summary>
    public partial class SelectEventWindow : Window
    {
        private DataTable dt;
        private DataView dv;

        private int m_id;
        private string m_event;

        private string sql = "select * from events";


        public SelectEventWindow()
        {
            InitializeComponent();

            dt = DBWrapper.MySqlWrapper.Select(sql);            

            DataTable tmp = new DataTable();
            tmp.Columns.Add("idevent");
            tmp.Columns.Add("eventname");

            for(int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = tmp.NewRow();
                
                dr["idevent"] = (int)(dt.Rows[i]["idevent"]);
                int idEZ = (int)(dt.Rows[i]["idexpo_zone"]);
                string nn = $"{dt.Rows[i]["eventname"]} ({(new Expo_zonesModel(idEZ)).Expo_zonename})";
                dr["eventname"] = nn;

                tmp.Rows.Add(dr);
            }

            DataGridTextColumn intcol = new DataGridTextColumn();
            intcol.Binding = new Binding("idevent");
            intcol.Visibility = Visibility.Collapsed;
            grid.Columns.Add(intcol);
            DataGridTextColumn textcol = new DataGridTextColumn();
            textcol.Binding = new Binding("eventname");
            textcol.Width = 400;
            grid.Columns.Add(textcol);

            dv = tmp.DefaultView;
            grid.ItemsSource = dv;
        }

        public int Id { get => m_id; set => m_id = value; }
        public string Event { get => m_event; set => m_event = value; }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            Id = int.Parse((row.Item as DataRowView).Row[0].ToString());
            Event = (row.Item as DataRowView).Row[1].ToString();
            DialogResult = true;
        }

        private void AddBt_Click(object sender, RoutedEventArgs e)
        {
            if(grid.SelectedItem == null)
            {
                MessageBox.Show("Не выбрана ни одна строка");
                return;
            }

            DataRowView drv = grid.SelectedItem as DataRowView;
            Id = int.Parse(drv.Row[0].ToString());
            Event = drv.Row[1].ToString();
            DialogResult = true;
        }

        private void EditableTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
