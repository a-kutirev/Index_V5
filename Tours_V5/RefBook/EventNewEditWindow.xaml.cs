using ClassLibrary.Models;
using System.ComponentModel;
using System.Data;
using System.Windows;

namespace Tours_V5.RefBook
{
    /// <summary>
    /// Логика взаимодействия для EventNewEditWindow.xaml
    /// </summary>
    public partial class EventNewEditWindow : Window, INotifyPropertyChanged
    {
        #region Members

        private EventModel m_model;
        private DataView m_ZonesView;

        public EventModel Model
        {
            get => m_model;
            set
            {
                m_model = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Model"));
            }
        }

        public DataView ZonesView
        {
            get => m_ZonesView;
            set
            {
                m_ZonesView = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ZonesView"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructor
        public EventNewEditWindow(int id)
        {
            InitializeComponent();

            ZonesView = GetZonesView();

            this.DataContext = this;

            if (id == 0)
            {
                Model = new EventModel();
                SaveBt.Content = "Добавить";
            }
            else
            {
                Model = new EventModel(id);
                SaveBt.Content = "Изменить";
            }
            
        }

        private DataView GetZonesView()
        {
            string sql2 = "select * from expo_zones";
            return DBWrapper.MySqlWrapper.Select(sql2).DefaultView;
        }
        #endregion

        #region Events
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void SaveBt_Click(object sender, RoutedEventArgs e)
        {
            if (Model.Idevent == 0)
                Model.Insert();
            else
                Model.Update();

            DialogResult = true;
        }
        #endregion
    }
}
