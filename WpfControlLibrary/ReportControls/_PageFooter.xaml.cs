using System.Windows.Controls;

namespace WpfControlLibrary.ReportControls
{
    /// <summary>
    /// Логика взаимодействия для _PageFooter.xaml
    /// </summary>
    public partial class _PageFooter : UserControl
    {
        public _PageFooter(int current, int total, string title)
        {
            InitializeComponent();
            footerText.Text = $"Страница {current} из {total} \t\t\t {title}";
        }
    }
}
