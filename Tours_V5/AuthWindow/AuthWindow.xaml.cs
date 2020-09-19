using ClassLibrary;
using DBWrapper;
using LogLibrary;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Tours_V5.Setup;

namespace Tours_V5.AuthWindow
{
    /// <summary>
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window, INotifyPropertyChanged
    {
        #region private Members

        private DataView m_usersDataView;
        private int m_selectedUserId;
        private Dictionary<int, string> PasswordHash = new Dictionary<int, string>();
        private Dictionary<int, string> Logins = new Dictionary<int, string>();


        public DataView UsersDataView
        {
            get => m_usersDataView;
            set
            {
                m_usersDataView = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("UsersDataView"));
            }
        }

        public int SelectedUserId
        {
            get => m_selectedUserId;
            set
            {
                m_selectedUserId = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedUserId"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructor
        public AuthWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            if (!Options.Check())
                Options.CreateKey();

            bool connect = TestConnection();
            while (!connect)
            {
                NetOptionsWindow now = new NetOptionsWindow();
                now.ShowDialog();
                connect = TestConnection();
            }

            string sql = "select idusers, usersname, userspass, login from users where not userInactive";
            DataTable tmp = MySqlWrapper.Select(sql);

            for(int i = 0; i < tmp.Rows.Count; i++)
            {
                int id = (int)tmp.Rows[i]["idusers"];
                string hash = tmp.Rows[i]["userspass"].ToString();
                string ll = tmp.Rows[i]["login"].ToString();
                PasswordHash.Add(id, hash);
                Logins.Add(id, ll);
            }

            UsersDataView = tmp.DefaultView;
            SelectedUserId = 1;

            PswBox.Focus();
        }
        #endregion


        #region LoginClick
        private void AcceptBt_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordMd5(PswBox.Password) == PasswordHash[SelectedUserId])
            {
                Log.username = Logins[SelectedUserId];

                if (UserNameComboBox.Text == "Администратор")
                {
                    AdminWindow muw = new AdminWindow();
                    muw.Show();
                    Close();
                }
                else
                {
                    MainWindow mw = new MainWindow(SelectedUserId);
                    mw.Show();
                    Close();
                }
            }
            else
            {
                MessageBox.Show("Неверный пароль", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
                PswBox.Password = "";
                PswBox.Focus();
            }
        }
        #endregion

        #region Тест соединения
        private bool TestConnection()
        {
            bool result = true;
            string server = Options.Server;
            string db = Options.Database;
            string user = Options.User;
            string password = Options.Password;

            string connectionTemplate = "SERVER={0};DATABASE={1};UID={2};PASSWORD={3};convert zero datetime=True";
            string ConnectionString = string.Format(connectionTemplate, server, db, user, password);

            MySqlConnection conn = new MySqlConnection(ConnectionString);

            bool r = true;
            try { conn.Open(); }
            catch (Exception) { r = false; }

            if (!r) result = false;
            else MySqlWrapper.connectionString = ConnectionString;

            return result;
        }
        #endregion

        #region Вычисление хеша md5
        private string PasswordMd5(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);
            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
        #endregion

        private void PswBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                AcceptBt_Click(null, null);
        }
    }
}
