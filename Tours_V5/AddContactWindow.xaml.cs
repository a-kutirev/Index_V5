using ClassLibrary;
using DBWrapper;
using System.ComponentModel;
using System.Data;
using System.Windows;

namespace Tours_V5
{
    /// <summary>
    /// Логика взаимодействия для AddContactWindow.xaml
    /// </summary>
    public partial class AddContactWindow : Window, INotifyPropertyChanged
    {
        #region Members
        private ContactModel m_model;
        private int m_id = 0;
        public ContactModel Model
        {
            get => m_model;
            set
            {
                m_model = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Model"));
            }
        }

        public int Id { get => m_id; set => m_id = value; }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructor
        public AddContactWindow()
        {
            InitializeComponent();
            DataContext = this;
            Model = new ContactModel();
            ConstructAutoCompletionSource();
        }        
        public AddContactWindow(int id)
        {
            InitializeComponent();
            DataContext = this;

            Model = new ContactModel(id);
            SaveBt.Content = "Изменить";
            ConstructAutoCompletionSource();
        }
        #endregion

        #region Save
        private void SaveBt_Click(object sender, RoutedEventArgs e)
        {
            if (Model.Idcontact == 0)           Id = Model.Insert();                
            else                                Model.Update();
            DialogResult = true;
        }
        #endregion

        #region Подготовка автозаполнения
        private void ConstructAutoCompletionSource()
        {
            PostTextBox.AutoSuggestionList.Clear();
            string sql = "select autocompleteword from autocompletes where autocompletetype = \"post\"";
            DataTable dt = MySqlWrapper.Select(sql);
            for (int i = 0; i < dt.Rows.Count; i++)
                PostTextBox.AutoSuggestionList.Add(dt.Rows[i]["autocompleteword"].ToString());
        }
        #endregion
    }
}
