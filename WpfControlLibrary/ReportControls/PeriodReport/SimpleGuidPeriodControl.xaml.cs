using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfControlLibrary.ReportControls
{
    /// <summary>
    /// Логика взаимодействия для SimpleGuidPeriodControl.xaml
    /// </summary>
    public partial class SimpleGuidPeriodControl : UserControl, INotifyPropertyChanged
    {
        #region Members

        private string m_guidName;
        private string m_tourCount;
        private SolidColorBrush m_Back;

        public string GuidName
        {
            get => m_guidName;
            set
            {
                m_guidName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GuidName"));
            }
        }
        public string TourCount
        {
            get => m_tourCount;
            set
            {
                m_tourCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TourCount"));
            }
        }

        public SolidColorBrush Back
        {
            get => m_Back;
            set
            {
                m_Back = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Back"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructor
        public SimpleGuidPeriodControl(string gn, string tc)
        {
            InitializeComponent();
            this.DataContext = this;
            GuidName = gn;
            TourCount = tc;
        }
        #endregion
    }
}
