using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Tours_V5.RefBook
{
    /// <summary>
    /// Логика взаимодействия для GuidTable.xaml
    /// </summary>
    public partial class GuidTable : Window, INotifyPropertyChanged
    {        
        #region Members
        public event PropertyChangedEventHandler PropertyChanged;
        private bool m_showFade;
        private bool m_ShowAllGuids = false;

        private string sqlAll = "select * from allguids";
        private string sql = "select * from allguids where guidend is null";

        public bool ShowFade
        {
            get => m_showFade;
            set
            {
                m_showFade = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowFade"));
            }
        }

        public bool ShowAllGuids
        {
            get => m_ShowAllGuids;
            set
            {
                m_ShowAllGuids = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowAllGuids"));
                if(m_ShowAllGuids)
                {
                    DataTable dt = DBWrapper.MySqlWrapper.Select(sqlAll);
                    GuidTb.Source = dt;
                }
                else
                {
                    DataTable dt = DBWrapper.MySqlWrapper.Select(sql);
                    GuidTb.Source = dt;
                }
            }
        }
        #endregion

        #region Constructor
        public GuidTable()
        {
            InitializeComponent();
            this.DataContext = this;
            ShowFade = false;

            DataTable dt = DBWrapper.MySqlWrapper.Select(sql);
            GuidTb.AddColumn("Id", "idguid", 35, true);
            GuidTb.AddColumn("ФИО", "guidfullname", 200, false);
            GuidTb.AddColumn("Должность", "postname", 200, false);
            GuidTb.Buttons = WpfControlLibrary.PagedTableControl.AddedButton.Edit;
            GuidTb.Source = dt;
            GuidTb.FilterRow = "guidfullname";
            GuidTb.EditBtClick += GuidTb_EditBtClick;
        }

        private void GuidTb_EditBtClick(object sender, WpfControlLibrary.PagedTableControl.GridButtonEventArgs e)
        {
            GuidNewEdit ane = new GuidNewEdit(e.id);
            ShowFade = true;
            ane.ShowDialog();
            ShowFade = false;
            ShowAllGuids = m_ShowAllGuids;
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GuidNewEdit gne = new GuidNewEdit();
            ShowFade = true;
            gne.ShowDialog();
            ShowFade = false;
            ShowAllGuids = m_ShowAllGuids;
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) =>
            GuidTb.FilterTemplate = (sender as TextBox).Text;

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
