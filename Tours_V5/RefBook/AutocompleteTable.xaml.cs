using ClassLibrary.Models;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Tours_V5.RefBook
{
    /// <summary>
    /// Логика взаимодействия для AutocompleteTable.xaml
    /// </summary>
    public partial class AutocompleteTable : Window, INotifyPropertyChanged
    {
        #region Members

        public event PropertyChangedEventHandler PropertyChanged;

        private bool m_showFade;
        private string sql = "select * from autocompletes";

        public bool ShowFade
        {
            get => m_showFade;
            set
            {
                m_showFade = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowFade"));
            }
        }

        #endregion

        #region Constructor
        public AutocompleteTable()
        {
            InitializeComponent();
            this.DataContext = this;
            ShowFade = false;

            DataTable dt = DBWrapper.MySqlWrapper.Select(sql);            
            AutocompleteTb.AddColumn("", "idautocomplete", 50, true);
            AutocompleteTb.AddColumn("", "autocompleteword", 300,  false);
            AutocompleteTb.AddColumn("", "autocompletetype", 65, false);
            AutocompleteTb.Buttons = WpfControlLibrary.PagedTableControl.AddedButton.EditAndDelete;
            AutocompleteTb.Source = dt;
            AutocompleteTb.FilterRow = "autocompleteword";
            AutocompleteTb.DeleteBtClick += AutocompleteTb_DeleteBtClick;
            AutocompleteTb.EditBtClick += AutocompleteTb_EditBtClick;
        }
        #endregion

        #region Window Events
        private void AutocompleteTb_EditBtClick(object sender, WpfControlLibrary.PagedTableControl.GridButtonEventArgs e)
        {
            AutocompleteModel m = new AutocompleteModel(e.id);
            AutocompleteNewEdit ane = new AutocompleteNewEdit(m);
            ShowFade = true;
            ane.ShowDialog();
            ShowFade = false;
            DataTable dt = DBWrapper.MySqlWrapper.Select(sql);
            AutocompleteTb.Source = dt;
        }

        private void AutocompleteTb_DeleteBtClick(object sender, WpfControlLibrary.PagedTableControl.GridButtonEventArgs e)
        {
            if(MessageBox.Show("Вы уверены хотите удалить запись?", "Подтверждение", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                string sql1 = $"delete from autocompletes where idautocomplete = {e.id}";
                DBWrapper.MySqlWrapper.Execute(sql1);
                DataTable dt = DBWrapper.MySqlWrapper.Select(sql);
                AutocompleteTb.Source = dt;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AutocompleteNewEdit ane = new AutocompleteNewEdit();
            ShowFade = true;
            ane.ShowDialog();
            ShowFade = false;
            DataTable dt = DBWrapper.MySqlWrapper.Select(sql);
            AutocompleteTb.Source = dt;
        }
        #endregion

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) =>
            AutocompleteTb.FilterTemplate = (sender as TextBox).Text;

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
