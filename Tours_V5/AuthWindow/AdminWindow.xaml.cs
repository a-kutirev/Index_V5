using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Tours_V5.AuthWindow
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        #region Constructor
        public AdminWindow()
        {
            InitializeComponent();

            string sql = "select * from users";
            DataTable tmp = DBWrapper.MySqlWrapper.Select(sql);
            UsersGrid.ItemsSource = tmp.DefaultView;
        }
        #endregion


        #region Action
        private void NewUser_Click(object sender, RoutedEventArgs e)
        {
            EditUserWindow euw = new EditUserWindow();
            euw.WinMode = WinMode.New;

            if ((bool)euw.ShowDialog())
            {
                string sql = "select * from users";
                DataTable tmp = DBWrapper.MySqlWrapper.Select(sql);
                UsersGrid.ItemsSource = tmp.DefaultView;
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ModifyUser_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)(sender as Button).CommandParameter;
            EditUserWindow euw = new EditUserWindow();
            euw.WinMode = WinMode.Edit;
            euw.Id = id;

            if ((bool)euw.ShowDialog())
            {
                string sql = "select * from users";
                DataTable tmp = DBWrapper.MySqlWrapper.Select(sql);
                UsersGrid.ItemsSource = tmp.DefaultView;
            }
        }
        #endregion
    }
}
