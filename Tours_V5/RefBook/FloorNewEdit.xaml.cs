using ClassLibrary.Models;
using System.ComponentModel;
using System.Windows;

namespace Tours_V5.RefBook
{
    /// <summary>
    /// Логика взаимодействия для FloorNewEdit.xaml
    /// </summary>
    public partial class FloorNewEdit : Window, INotifyPropertyChanged
    {
        #region Members
        public event PropertyChangedEventHandler PropertyChanged;

        private FloorModel m_model;

        public FloorModel Model
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
        public FloorNewEdit()
        {
            InitializeComponent();
            this.DataContext = this;
            Model = new FloorModel();
        }
        public FloorNewEdit(int id)
        {
            InitializeComponent();
            this.DataContext = this;
            Model = new FloorModel(id);
            SaveBt.Content = "Изменить";
        }
        
        #endregion

        private void SaveBt_Click(object sender, RoutedEventArgs e)
        {
            if (Model.Idfloor == 0) Model.Insert();
            else Model.Update();
            DialogResult = true;
        }
    }
}
