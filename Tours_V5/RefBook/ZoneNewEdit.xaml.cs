using ClassLibrary.Models;
using System.ComponentModel;
using System.Data;
using System.Windows;

namespace Tours_V5.RefBook
{
    /// <summary>
    /// Логика взаимодействия для ZoneNewEdit.xaml
    /// </summary>
    public partial class ZoneNewEdit : Window, INotifyPropertyChanged
    {
        #region Members

        public event PropertyChangedEventHandler PropertyChanged;

        private Expo_zonesModel m_model;
        private DataView m_FloorView;

        public Expo_zonesModel Model
        {
            get => m_model;
            set
            {
                m_model = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Model")) ;
            }
        }

        public DataView FloorView
        {
            get => m_FloorView;
            set
            {
                m_FloorView = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FloorView"));
            }
        }
        #endregion

        #region Constructor
        public ZoneNewEdit()
        {
            InitializeComponent();
            this.DataContext = this;
            Model = new Expo_zonesModel();
            FloorView = GetFloorView();
        }
        public ZoneNewEdit(int id)
        {
            InitializeComponent();
            this.DataContext = this;
            Model = new Expo_zonesModel(id);
            FloorView = GetFloorView();
            SaveBt.Content = "Сохранить";
        }

        private DataView GetFloorView()
        {
            string sql = "select * from floors";
            return DBWrapper.MySqlWrapper.Select(sql).DefaultView;
        }
        #endregion

        private void SaveBt_Click(object sender, RoutedEventArgs e)
        {
            if (Model.Idexpo_zone == 0) Model.Insert();
            else Model.Update();
            DialogResult = true;
        }
    }
}
