using System.Data;
using System.Windows;

namespace WpfControlLibrary
{
    /// <summary>
    /// Логика взаимодействия для CommentWindow.xaml
    /// </summary>
    public partial class CommentWindow : Window
    {
        private string m_rtf = "";

        public string Rtf
        {
            get { return CommentRtf.GetRtfReplaced(); }
            set
            {
                m_rtf = value;
                CommentRtf.SetRtfReplaced(m_rtf);
            }
        }

        public CommentWindow()
        {
            InitializeComponent();

            string sql = "select autocompleteword from autocompletes where autocompletetype = \"comment\"";
            DataTable tmp = DBWrapper.MySqlWrapper.Select(sql);
            CommentRtf.AutocompleteData = tmp;
        }

        private void CancelBt_Click(object sender, RoutedEventArgs e) => DialogResult = false;
        private void SaveBt_Click(object sender, RoutedEventArgs e) => DialogResult = true;
    }
}
