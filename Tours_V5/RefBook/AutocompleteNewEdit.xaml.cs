using ClassLibrary.Models;
using System.ComponentModel;
using System.Data;
using System.Windows;

namespace Tours_V5.RefBook
{
    /// <summary>
    /// Логика взаимодействия для AutocompleteNewEdit.xaml
    /// </summary>
    public partial class AutocompleteNewEdit : Window, INotifyPropertyChanged
    {
        #region Members
        public event PropertyChangedEventHandler PropertyChanged;

        private AutocompleteModel m_model;
        private DataView m_catView;

        public AutocompleteModel Model
        {
            get => m_model;
            set
            {
                m_model = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Model"));
            }
        }

        public DataView CatView
        {
            get => m_catView;
            set
            {
                m_catView = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CatView"));
            }
        }

        #endregion

        #region Constructor
        public AutocompleteNewEdit()
        {
            InitializeComponent();
            this.DataContext = this;
            Model = new AutocompleteModel();
            CatView = CategoryTable().DefaultView;
        }
        public AutocompleteNewEdit(AutocompleteModel autocompleteModel)
        {
            InitializeComponent();
            this.DataContext = this;
            Model = autocompleteModel;
            CatView = CategoryTable().DefaultView;
            SaveBt.Content = "Изменить";
        }

        private DataTable CategoryTable()
        {
            DataTable tmp = new DataTable();
            tmp.Columns.Add("category", typeof(string));
            tmp.Columns.Add("categorycode", typeof(string));
            tmp.Rows.Add("Должность","post");
            tmp.Rows.Add("Комментарии", "comment");
            tmp.Rows.Add("Возраст", "age");
            return tmp;
        }
        #endregion

        private void SaveBt_Click(object sender, RoutedEventArgs e)
        {
            if (Model.Idautocomplete == 0)      Model.Insert();
            else                                Model.Update();
            DialogResult = true;
        }
    }
}
