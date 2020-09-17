using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Tours_V5.RefBook
{
    /// <summary>
    /// Логика взаимодействия для FloorTable.xaml
    /// </summary>
    public partial class FloorTable : Window, INotifyPropertyChanged
    {
        #region Members
        public event PropertyChangedEventHandler PropertyChanged;

        string sql = "select * from floors";
        private bool m_showFade = false;

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
        public FloorTable()
        {
            InitializeComponent();
            this.DataContext = this;
            ShowFade = false;

            DataTable dt = DBWrapper.MySqlWrapper.Select(sql);
            FloorTb.AddColumn("Id", "idfloor", 35, true);
            FloorTb.AddColumn("", "floorname", 150, false);
            FloorTb.Buttons = WpfControlLibrary.PagedTableControl.AddedButton.Edit;
            FloorTb.FilterRow = "floorname";
            FloorTb.EditBtClick += FloorTb_EditBtClick;
            FloorTb.Source = dt;
        }

        private void FloorTb_EditBtClick(object sender, WpfControlLibrary.PagedTableControl.GridButtonEventArgs e)
        {
            FloorNewEdit fne = new FloorNewEdit(e.id);
            ShowFade = true;
            fne.ShowDialog();
            ShowFade = false;
            DataTable dt = DBWrapper.MySqlWrapper.Select(sql);
            FloorTb.Source = dt;
        }
        #endregion

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) =>
            FloorTb.FilterTemplate = (sender as TextBox).Text;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Добавление этажности
        }

        private void SaveBt_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; 
        }
    }
}
