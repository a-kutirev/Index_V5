using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Tours_V5.RefBook
{
    /// <summary>
    /// Логика взаимодействия для ContactTable.xaml
    /// </summary>
    public partial class ContactTable : Window, INotifyPropertyChanged
    {
        #region Members
        public event PropertyChangedEventHandler PropertyChanged;
        private bool m_showFade;
        private string sql = "select * from contacts";

        public bool ShowFade
        {
            get => m_showFade;
            set
            {
                m_showFade = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowFade"));
            }
        }
        #endregion

        #region Constructor
        public ContactTable()
        {
            InitializeComponent();
            this.DataContext = this;
            ShowFade = false;

            DataTable dt = DBWrapper.MySqlWrapper.Select(sql);
            ContactTb.AddColumn("Id", "idcontact", 35, true);
            ContactTb.AddColumn("ФИО", "contactname", 200, false);
            ContactTb.AddColumn("Должность", "contactpost", 200, false);
            ContactTb.AddColumn("Телефон", "contactphone", 150, false);
            ContactTb.Buttons = WpfControlLibrary.PagedTableControl.AddedButton.Edit;
            ContactTb.Source = dt;
            ContactTb.FilterRow = "contactname";
            ContactTb.EditBtClick += ContactTb_EditBtClick;
        }
        #endregion

        private void ContactTb_EditBtClick(object sender, WpfControlLibrary.PagedTableControl.GridButtonEventArgs e)
        {
            AddContactWindow ane = new AddContactWindow(e.id);
            ShowFade = true;
            ane.ShowDialog();
            ShowFade = false;
            DataTable dt = DBWrapper.MySqlWrapper.Select(sql);
            ContactTb.Source = dt;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddContactWindow acw = new AddContactWindow();
            ShowFade = true;
            acw.ShowDialog();
            ShowFade = false;
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) =>
            ContactTb.FilterTemplate = (sender as TextBox).Text;

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
