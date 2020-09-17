using System.ComponentModel;
using System.Windows;

namespace Tours_V5.RefBook
{
    /// <summary>
    /// Логика взаимодействия для GroupEditWindow.xaml
    /// </summary>
    public partial class GroupEditWindow : Window, INotifyPropertyChanged
    {
        #region Members
        private string m_id = "";

        public event PropertyChangedEventHandler PropertyChanged;
        public int GId;

        public string Id
        {
            get => m_id;
            set
            {
                m_id = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Id"));
            }
        }
        #endregion

        #region Contructor
        public GroupEditWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            idText.Focus();
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int groupid;
            bool res = int.TryParse(Id, out groupid);
            if (res)
            {
                GId = groupid;
                DialogResult = true;
            }
            else MessageBox.Show("Необходимо ввести числовое значение");
        }
    }
}
