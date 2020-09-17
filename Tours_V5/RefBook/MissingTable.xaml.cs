using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Tours_V5.RefBook
{
    /// <summary>
    /// Логика взаимодействия для MissingTable.xaml
    /// </summary>
    public partial class MissingTable : Window, INotifyPropertyChanged
    {
        #region Members
        public event PropertyChangedEventHandler PropertyChanged;

        private bool m_ShowFade;
        private string sql = "call GetWorkedMissings()";

        public bool ShowFade
        {
            get => m_ShowFade;
            set
            {
                m_ShowFade = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Showfade"));
            }
        }
        #endregion

        #region Constructor
        public MissingTable()
        {
            InitializeComponent();
            this.DataContext = this;
            ShowFade = false;
            DataTable dt = DBWrapper.MySqlWrapper.Select(sql);
            MissingTb.AddColumn("Id", "idmissing", 35, true);
            MissingTb.AddColumn("ФИО", "guidfullname", 200, false);
            MissingTb.AddColumn("Начало", "missingbegin", 100, false, WpfControlLibrary.PagedTableControl.DateTimeFormat.DateFormat);
            MissingTb.AddColumn("Окончание", "missingend", 100, false, WpfControlLibrary.PagedTableControl.DateTimeFormat.DateFormat);
            MissingTb.Buttons = WpfControlLibrary.PagedTableControl.AddedButton.EditAndDelete;
            MissingTb.Source = dt;
            MissingTb.FilterRow = "guidfullname";
            MissingTb.EditBtClick += MissingTb_EditBtClick;
            MissingTb.DeleteBtClick += MissingTb_DeleteBtClick;

        }

        private void MissingTb_DeleteBtClick(object sender, WpfControlLibrary.PagedTableControl.GridButtonEventArgs e)
        {
            if(MessageBox.Show("Вы уверены что хотите удалить запись?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question)== MessageBoxResult.Yes)
            {
                int id = e.id;
                string sql = $"delete from missings where idmissing = {id}";
                DBWrapper.MySqlWrapper.Execute(sql);
            }
            DialogResult = true;
        }

        private void MissingTb_EditBtClick(object sender, WpfControlLibrary.PagedTableControl.GridButtonEventArgs e)
        {
            MissingEdit me = new MissingEdit(e.id);
            ShowFade = true;
            me.ShowDialog();
            ShowFade = false;
        }

        #endregion

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) =>
            MissingTb.FilterTemplate = (sender as TextBox).Text;

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
