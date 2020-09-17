using ClassLibrary.Models;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Tours_V5
{
    /// <summary>
    /// Логика взаимодействия для AddNoteWindow.xaml
    /// </summary>
    public partial class AddNoteWindow : Window, INotifyPropertyChanged
    {
        #region Members
        public event PropertyChangedEventHandler PropertyChanged;

        private NoteModel m_model;
        private bool m_limit;

        public NoteModel Model
        {
            get => m_model;
            set
            {
                m_model = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Model"));
            }
        }

        public bool Limit
        {
            get => m_limit;
            set
            {
                m_limit = value;
            }
        }
        #endregion

        #region Constructor
        public AddNoteWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            Model = new NoteModel();
        }
        public AddNoteWindow(int id)
        {
            InitializeComponent();
            this.DataContext = this;

            Model = new NoteModel(id);
            Limit = Model.Notelimit == 1;
            SaveBt.Content = "Сохранить";
        }
        #endregion

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //bool val = (bool)(sender as CheckBox).IsChecked;
            //if(val)
            //{
            //    StartPeriodLabel.Content = "Дата :";
            //    EndPeriodLabel.Visibility = Visibility.Collapsed;
            //    EndPeriodPicker.Visibility = Visibility.Collapsed;
            //}
            //else
            //{
            //    StartPeriodLabel.Content = "Начало периода :";
            //    EndPeriodLabel.Visibility = Visibility.Visible;
            //    EndPeriodPicker.Visibility = Visibility.Visible;
            //}
        }

        private void SaveBt_Click(object sender, RoutedEventArgs e)
        {
            Model.Notelimit = Limit ? 1 : 0;
            if (Model.Idnote == 0) Model.Insert();
            else Model.Update();
            DialogResult = true;
        }
    }
}
