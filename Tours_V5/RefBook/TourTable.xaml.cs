using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Tours_V5.RefBook
{
    /// <summary>
    /// Логика взаимодействия для TourTable.xaml
    /// </summary>
    public partial class TourTable : Window, INotifyPropertyChanged
    {
        #region Members
        public event PropertyChangedEventHandler PropertyChanged;
        private bool m_showFade;

        private string sql = "select * from tours";

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
        public TourTable()
        {
            InitializeComponent();
            this.DataContext = this;
            
            ShowFade = false;
            DataTable dt = DBWrapper.MySqlWrapper.Select(sql);
            ToursTb.AddColumn("Id", "idtour", 35, true);
            ToursTb.AddColumn("Наименование", "tourname", 400, false);
            ToursTb.Buttons = WpfControlLibrary.PagedTableControl.AddedButton.Edit;
            ToursTb.Source = dt;
            ToursTb.FilterRow = "tourname";
            ToursTb.EditBtClick += ToursTb_EditBtClick;
        }

        private void ToursTb_EditBtClick(object sender, WpfControlLibrary.PagedTableControl.GridButtonEventArgs e)
        {
            TourNewEdit ane = new TourNewEdit(e.id);
            ShowFade = true;
            ane.ShowDialog();
            ShowFade = false;
            DataTable dt = DBWrapper.MySqlWrapper.Select(sql);
            ToursTb.Source = dt;
        }        
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TourNewEdit tne = new TourNewEdit();
            ShowFade = true;
            tne.ShowDialog();
            ShowFade = false;
            DataTable dt = DBWrapper.MySqlWrapper.Select(sql);
            ToursTb.Source = dt;
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) =>
            ToursTb.FilterTemplate = (sender as TextBox).Text;

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
