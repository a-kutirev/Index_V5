using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Tours_V5.RefBook
{
    /// <summary>
    /// Логика взаимодействия для ZoneTable.xaml
    /// </summary>
    public partial class ZoneTable : Window, INotifyPropertyChanged
    {
        #region Members
        public event PropertyChangedEventHandler PropertyChanged;
        private bool m_showFade;

        string sql = "select * from expo_zones";

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
        public ZoneTable()
        {
            InitializeComponent();
            this.DataContext = this;
            ShowFade = false;

            DataTable dt = DBWrapper.MySqlWrapper.Select(sql);
            ZoneTb.AddColumn("Id", "idexpo_zone", 35, true);
            ZoneTb.AddColumn("Наименование", "expo_zonename", 250, false);
            ZoneTb.Buttons = WpfControlLibrary.PagedTableControl.AddedButton.Edit;
            ZoneTb.FilterRow = "expo_zonename";
            ZoneTb.Source = dt;
            ZoneTb.EditBtClick += ZoneTb_EditBtClick;
        }

        private void ZoneTb_EditBtClick(object sender, WpfControlLibrary.PagedTableControl.GridButtonEventArgs e)
        {
            ZoneNewEdit zne = new ZoneNewEdit(e.id);
            ShowFade = true;
            zne.ShowDialog();
            ShowFade = false;
            DataTable dt = DBWrapper.MySqlWrapper.Select(sql);
            ZoneTb.Source = dt;
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ZoneNewEdit zne = new ZoneNewEdit();
            ShowFade = true;
            zne.ShowDialog();
            ShowFade = false;
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) =>
            ZoneTb.FilterTemplate = (sender as TextBox).Text;

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
