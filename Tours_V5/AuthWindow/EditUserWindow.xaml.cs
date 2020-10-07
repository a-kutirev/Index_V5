using DBWrapper;
using System.ComponentModel;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace Tours_V5.AuthWindow
{
    public enum WinMode
    {
        New,
        Edit
    }
    /// <summary>
    /// Логика взаимодействия для EditUserWindow.xaml
    /// </summary>
    public partial class EditUserWindow : Window, INotifyPropertyChanged
    {
        #region Members
        private string m_fio;
        private string m_comment;
        private bool m_inActive;
        private WinMode m_winMode;
        private int m_id;
        private int m_selIndex;

        public WinMode WinMode { get => m_winMode; set => m_winMode = value; }
        public int Id
        {
            get => m_id;
            set
            {
                m_id = value;
                if (m_winMode == WinMode.Edit)
                {
                    string sql = $"select * from users where idusers = {m_id}";
                    DataTable tmp = MySqlWrapper.Select(sql);
                    if (tmp.Rows.Count > 0)
                    {
                        Fio = tmp.Rows[0]["usersname"].ToString();
                        string role = tmp.Rows[0]["usersrole"].ToString();
                        switch (role)
                        {
                            case "admin":
                                SelIndex = 0;
                                break;
                            case "user":
                                SelIndex = 1;
                                break;
                            case "root":
                                SelIndex = 2;
                                break;
                        }
                        Comment = tmp.Rows[0]["userscomment"].ToString();
                        InActive = tmp.Rows[0]["userInactive"].ToString() == "1";
                    }
                }
            }
        }
        public string Fio
        {
            get => m_fio;
            set
            {
                m_fio = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FIO"));
            }
        }
        public int SelIndex
        {
            get => m_selIndex;
            set
            {
                m_selIndex = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelIndex"));
            }
        }
        public string Comment
        {
            get => m_comment; set
            {
                m_comment = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Comment"));
            }
        }
        public bool InActive
        {
            get => m_inActive;
            set
            {
                m_inActive = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InActive"));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructor
        public EditUserWindow()
        {
            InitializeComponent();
            DataContext = this;
            SelIndex = 1;
        }
        #endregion

        #region ACtion
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            string role = "";
            switch (SelIndex)
            {
                case 0:
                    role = "admin";
                    break;
                case 1:
                    role = "user";
                    break;
                case 2:
                    role = "root";
                    break;
            }

            string sql = ""; // Fio, role, Comment, InActive ? 1 : 0, m_id
            if (WinMode == WinMode.Edit)
            {
                sql = $"update users set " +
                    $"usersname = '{Fio}'," +
                    $"usersrole = '{role}'," +
                    $"userscomment = '{Comment}'," +
                    $"userInactive = {(InActive ? 1 : 0)} " +
                    $"where idusers = {m_id}";
            }
            else
            {
                sql = $"insert into users(usersname, usersrole, userscomment, userInactive) values " +
                    $"('{Fio}','P{role}','{Comment}',{(InActive ? 1 : 0)})";
            }

            MySqlWrapper.Execute(sql);
            DialogResult = true;
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            {
                if (MessageBox.Show("Вы уверены что хотите изменить пароль?", "Подтверждение", MessageBoxButton.OKCancel,
                    MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    string MD5_psv = PasswordMd5(Comment);

                    string sql = $"update users set " +
                        $"userspass = '{MD5_psv}' " +
                        $"where idusers = {m_id}";

                    MySqlWrapper.Execute(sql);

                    MessageBox.Show("Пароль сброшен до пароля по умолчанию - \n" + Comment, "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

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

    }
}