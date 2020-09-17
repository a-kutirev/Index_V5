using ClassLibrary;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Tours_V5
{
    /// <summary>
    /// Логика взаимодействия для SelectContactWindow.xaml
    /// </summary>
    public partial class SelectContactWindow : Window, INotifyPropertyChanged
    {
        #region Members
        private string sql = "select * from contacts";
        private string sql2 = "select * from contacts where idcontact in " +
            "(SELECT distinct idcontact FROM commongroup_contacts where idcommongroup in " +
            "(SELECT idgroupheader FROM groupheaders where idorganization = {0}))";
        private int m_idorg;

        private int m_id;

        private DataView m_contactView;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Id
        {
            get => m_id;
            set
            {
                m_id = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Id"));
            }
        }
        public DataView ContactView
        {
            get => m_contactView;
            set
            {
                m_contactView = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ContactView"));
            }
        }
        #endregion

        #region Constructor
        public SelectContactWindow(int idOrg)
        {
            InitializeComponent();
            this.DataContext = this;

            m_idorg = idOrg;
            string sql3 = string.Format(sql2, idOrg);

            ContactView = DBWrapper.MySqlWrapper.Select(sql3).DefaultView;
        }
        #endregion

        #region Select click        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).CommandParameter != null)
            {
                Id = (int)((sender as Button).CommandParameter);
            }
            else
            {
                DataRowView drv = DGrid.SelectedItems[0] as DataRowView;
                Id = int.Parse(drv[0].ToString());
            }

            DialogResult = true;
        }
        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
        #endregion

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ContactView.RowFilter = $"contactname like '%{(sender as TextBox).Text}%'";
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if((bool)(sender as CheckBox).IsChecked)
                ContactView = DBWrapper.MySqlWrapper.Select(sql).DefaultView;
            else
            {
                string sql3 = string.Format(sql2, m_idorg);

                ContactView = DBWrapper.MySqlWrapper.Select(sql3).DefaultView;
            }
        }
    }
}
