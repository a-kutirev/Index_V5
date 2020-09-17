using ClassLibrary;
using System.ComponentModel;
using System.Windows;

namespace Tours_V5
{
    /// <summary>
    /// Логика взаимодействия для GroupDeleteWindow.xaml
    /// </summary>
    public partial class GroupDeleteWindow : Window, INotifyPropertyChanged
    {
        #region Members
        public event PropertyChangedEventHandler PropertyChanged;

        private GroupModel m_model;

        public GroupModel Model
        {
            get => m_model;
            set
            {
                m_model = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Model"));
            }
        }
        #endregion

        #region Constructor
        public GroupDeleteWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public GroupDeleteWindow(int id)
        {
            InitializeComponent();
            this.DataContext = this;

            Model = new GroupModel(id);            
        }
        #endregion

        private void SaveBt_Click(object sender, RoutedEventArgs e)
        {
            Model.Groupstatus |= 1;
            Model.Update();
            DialogResult = true;
        }
    }
}
