using DBWrapper;
using System.Data;
using System.Windows;
using Tours_V5.RefBook;

namespace Tours_V5.OldStyle
{
    /// <summary>
    /// Логика взаимодействия для SelectModifiedWindow.xaml
    /// </summary>
    public partial class SelectModifiedWindow : Window
    {
        public SelectModifiedWindow()
        {
            InitializeComponent();
        }

        private void TourBt_Click(object sender, RoutedEventArgs e)
        {
            GroupEditWindow wnd = new GroupEditWindow();
            if ((bool)wnd.ShowDialog())
            {
                int id = wnd.GId;

                string sql = $"select * from _groups where idgroup = {id}";
                DataTable tmp = MySqlWrapper.Select(sql);
                if (tmp.Rows.Count == 0)
                {
                    MessageBox.Show("Группы с указанным Id не существует");
                    return;
                }

                GroupModifyWindow gmw = new GroupModifyWindow(id);
                gmw.ShowDialog();
            }
            DialogResult = true;
        }

        private void EventBt_Click(object sender, RoutedEventArgs e)
        {
            EventEditWindow eew = new EventEditWindow();

            if ((bool)eew.ShowDialog())
            {
                int id = eew.GId;

                string sql = $"select * from eventgroups where ideventgroup = {id}";
                DataTable tmp = MySqlWrapper.Select(sql);
                if (tmp.Rows.Count == 0)
                {
                    MessageBox.Show("Мероприятия с указанным Id не существует");
                    return;
                }

                EventModifyWindow emw = new EventModifyWindow(id);
                emw.ShowDialog();
            }
            DialogResult = true;
        }
    }
}
