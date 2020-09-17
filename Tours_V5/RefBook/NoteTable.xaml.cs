using System;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Tours_V5.RefBook
{
    /// <summary>
    /// Логика взаимодействия для NoteTable.xaml
    /// </summary>
    public partial class NoteTable : Window, INotifyPropertyChanged
    {
        #region Members
        string sql = "select * from notes";
        private bool m_showFade = false;
        private DataView m_noteView;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool ShowFade
        {
            get => m_showFade;
            set
            {
                m_showFade = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowFade"));
            }
        }

        public DataView NoteView
        {
            get => m_noteView;
            set
            {
                m_noteView = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NoteView"));
            }
        }
        #endregion

        public NoteTable()
        {
            InitializeComponent();
            this.DataContext = this;

            ShowFade = false;
            NoteView = DBWrapper.MySqlWrapper.Select(sql).DefaultView;
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            string date = (sender as DatePicker).SelectedDate.Value.ToString("yyyy-MM-dd");
            string sql2 = $"CALL GetNotesByDate(\"{date}\")";
            NoteView = DBWrapper.MySqlWrapper.Select(sql2).DefaultView;
        }

        private void SaveBt_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void EditBt_Click(object sender, RoutedEventArgs e)
        {
            ShowFade = true;
            AddNoteWindow anw = new AddNoteWindow((int)((sender as Button).CommandParameter));
            if((bool)anw.ShowDialog())
            {

            }
            ShowFade = false;
            NoteView = DBWrapper.MySqlWrapper.Select(sql).DefaultView;
        }

        private void DeleteBt_Click(object sender, RoutedEventArgs e)
        {
            ShowFade = true;
            if(MessageBox.Show("Вы уверены что хотите удалить заметку?","Подтверждение",MessageBoxButton.YesNo, MessageBoxImage.Question)
                == MessageBoxResult.Yes)
            {
                string sql2 = $"delete from notes where idnote = {(sender as Button).CommandParameter.ToString()}";
                DBWrapper.MySqlWrapper.Execute(sql2);
            }
            ShowFade = false;

            string date;
            try
            {
                date = (sender as DatePicker).SelectedDate.Value.ToString("yyyy-MM-dd");
            }
            catch
            {
                date = DateTime.Now.ToString("yyyy-MM-dd");
            }
            string sql3 = $"CALL GetNotesByDate(\"{date}\")";
            NoteView = DBWrapper.MySqlWrapper.Select(sql3).DefaultView;
        }
    }
}
