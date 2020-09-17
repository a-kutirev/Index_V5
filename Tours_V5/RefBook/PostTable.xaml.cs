using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Tours_V5.RefBook
{
    /// <summary>
    /// Логика взаимодействия для PostTable.xaml
    /// </summary>
    public partial class PostTable : Window, INotifyPropertyChanged
    {
        #region Members
        public event PropertyChangedEventHandler PropertyChanged;
        private bool m_showFade;
        private string sql = "select * from posts";

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
        public PostTable()
        {
            InitializeComponent();
            this.DataContext = this;
            ShowFade = false;

            DataTable dt = DBWrapper.MySqlWrapper.Select(sql);
            PostTb.AddColumn("Id", "idpost", 35, true);
            PostTb.AddColumn("Должность","postname", 200, false);
            PostTb.Buttons = WpfControlLibrary.PagedTableControl.AddedButton.Edit;
            PostTb.Source = dt;
            PostTb.FilterRow = "postname";
            PostTb.EditBtClick += PostTb_EditBtClick;

        }

        private void PostTb_EditBtClick(object sender, WpfControlLibrary.PagedTableControl.GridButtonEventArgs e)
        {
            PostNewEdit pne = new PostNewEdit(e.id);
            ShowFade = true;
            pne.ShowDialog();
            ShowFade = false;
            DataTable dt = DBWrapper.MySqlWrapper.Select(sql);
            PostTb.Source = dt;
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PostNewEdit pne = new PostNewEdit();
            ShowFade = true;
            pne.ShowDialog();
            ShowFade = false;
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) =>
            PostTb.FilterTemplate = (sender as TextBox).Text;

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
