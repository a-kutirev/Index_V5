using ClassLibrary.Models;
using System.ComponentModel;
using System.Windows;

namespace Tours_V5.RefBook
{
    /// <summary>
    /// Логика взаимодействия для PostNewEdit.xaml
    /// </summary>
    public partial class PostNewEdit : Window, INotifyPropertyChanged
    {
        #region Members 
        public event PropertyChangedEventHandler PropertyChanged;

        private PostModel m_Model;

        public PostModel Model
        {
            get => m_Model;
            set
            {
                m_Model = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Model"));
            }
        }
        #endregion

        #region Constructor
        public PostNewEdit()
        {
            InitializeComponent();
            this.DataContext = this;
            Model = new PostModel();
        }
        public PostNewEdit(int id)
        {
            InitializeComponent();
            this.DataContext = this;
            Model = new PostModel(id);
            SaveBt.Content = "Изменить";
        }
        #endregion

        private void SaveBt_Click(object sender, RoutedEventArgs e)
        {
            if(Model.Idpost == 0) Model.Insert();
            else                  Model.Update();
            DialogResult = true;
        }
    }
}
