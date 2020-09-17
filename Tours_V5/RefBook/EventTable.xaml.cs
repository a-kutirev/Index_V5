using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Tours_V5.RefBook
{
    /// <summary>
    /// Логика взаимодействия для EventTable.xaml
    /// </summary>
    public partial class EventTable : Window, INotifyPropertyChanged
    {
        #region Members

        private bool m_showFade = false;

        private string sql = "SELECT events.*, expo_zones.expo_zonename FROM tours_v5.events inner join expo_zones on events.idexpo_zone = expo_zones.idexpo_zone";

        public bool ShowFade
        {
            get => m_showFade;
            set
            {
                m_showFade = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowFade"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructor
        public EventTable()
        {
            InitializeComponent();
            this.DataContext = this;
            ShowFade = false;

            DataTable dt = DBWrapper.MySqlWrapper.Select(sql);

            EventsTb.AddColumn("Id", "idevent", 35, true);
            EventsTb.AddColumn("Мероприятие", "eventname", 300, false);
            EventsTb.AddColumn("Место", "expo_zonename", 150, false);
            EventsTb.AddColumn("Тип", "eventtype", 50, false);
            EventsTb.Buttons = WpfControlLibrary.PagedTableControl.AddedButton.Edit;
            EventsTb.EditBtClick += EventsTb_EditBtClick;
            EventsTb.Source = dt;
            EventsTb.FilterRow = "eventname";

        }
        #endregion

        #region Events

        private void EventsTb_EditBtClick(object sender, WpfControlLibrary.PagedTableControl.GridButtonEventArgs e) //Edit
        {
            EventNewEditWindow enew = new EventNewEditWindow(e.id);
            ShowFade = true;
            enew.ShowDialog();
            ShowFade = false;

            DataTable dt = DBWrapper.MySqlWrapper.Select(sql);
            EventsTb.Source = dt;
        }
        private void Button_Click_1(object sender, RoutedEventArgs e) // Add
        {
            EventNewEditWindow enew = new EventNewEditWindow(0);
            ShowFade = true;
            enew.ShowDialog();
            ShowFade = false;

            DataTable dt = DBWrapper.MySqlWrapper.Select(sql);
            EventsTb.Source = dt;
        }
        private void Button_Click(object sender, RoutedEventArgs e) // Close;
        {
            DialogResult = true;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)=>
            EventsTb.FilterTemplate = (sender as TextBox).Text;
        #endregion
    }
}
