using ClassLibrary;
using MySql.Data.MySqlClient;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Tours_V5.Setup
{
    /// <summary>
    /// Логика взаимодействия для NetOptionsWindow.xaml
    /// </summary>
    public partial class NetOptionsWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region members
        private string m_server = "";
        private string m_database = "";
        private string m_username = "";
        private string m_password = "";
        private bool tested = false;

        public string Server { get => m_server;
            set
            {
                m_server = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Server"));
            }
        }
        public string Database
        {
            get => m_database;
            set
            {
                m_database = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Database"));
            }
        }
        public string Username
        {
            get => m_username;
            set
            {
                m_username = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Username"));
            }
        }
        public string Password
        {
            get => m_password;
            set
            {
                m_password = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Password"));
            }
        }
        #endregion

        public NetOptionsWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            Server = Options.Server;
            Database = Options.Database;
            Username = Options.User;
            Password = Options.Password;
        }

        private void AcceptBt_Click(object sender, RoutedEventArgs e)
        {
            string connectionTemplate = "SERVER={0};DATABASE={1};UID={2};PASSWORD={3};convert zero datetime=True";
            string ConnectionString = string.Format(connectionTemplate, Server, Database, Username, Password);

            MySqlConnection conn = new MySqlConnection(ConnectionString);

            bool r = true;
            try { conn.Open(); }
            catch (Exception ex) { r = false; }

            if (r)
            {
                Options.Server = Server;
                Options.Database = Database;
                Options.User = Username;
                Options.Password = Password;

                DialogResult = true;
            }
            else MessageBox.Show("Соединение не установлено");            
        }

        private void TestBt_Click(object sender, RoutedEventArgs e)
        {
            string connectionTemplate = "SERVER={0};DATABASE={1};UID={2};PASSWORD={3};convert zero datetime=True";
            string ConnectionString = string.Format(connectionTemplate, Server, Database, Username, Password);

            MySqlConnection conn = new MySqlConnection(ConnectionString);

            bool r = true;
            try { conn.Open(); }
            catch (Exception ex) { r = false; }

            if (r) MessageBox.Show("Соединение успешно установлено");
            else MessageBox.Show("Соединение не установлено");            
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            tested = false;
        }
    }
}
