using System.ComponentModel;
using System.Windows.Controls;

namespace WpfControlLibrary.ReportControls
{
    /// <summary>
    /// Логика взаимодействия для SimpleItogPeriodControl.xaml
    /// </summary>
    public partial class SimpleItogPeriodControl : UserControl, INotifyPropertyChanged
    {

        private int m_count;

        public int Count
        {
            get => m_count;
            set
            {
                m_count = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public SimpleItogPeriodControl(int c)
        {
            InitializeComponent();
            this.DataContext = this;

            Count = c;
        }        
    }
}
