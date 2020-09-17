using ClassLibrary.Models;
using System.ComponentModel;
using System.Windows;

namespace Tours_V5
{
    /// <summary>
    /// Логика взаимодействия для EventDeleteWindow.xaml
    /// </summary>
    public partial class EventDeleteWindow : Window, INotifyPropertyChanged
    {
        #region Members
        public event PropertyChangedEventHandler PropertyChanged;

        private EventGroupModel m_model;

        public EventGroupModel Model
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
        public EventDeleteWindow()
        {
            InitializeComponent();
            this.DataContext = this;            
        }

        public EventDeleteWindow(int id)
        {
            InitializeComponent();
            this.DataContext = this;

            Model = new EventGroupModel(id);
        }
        #endregion

        private void SaveBt_Click(object sender, RoutedEventArgs e)
        {
            Model.Eventgroupstatus |= 1;
            Model.Update();
            DialogResult = true;
        }
    }
}
