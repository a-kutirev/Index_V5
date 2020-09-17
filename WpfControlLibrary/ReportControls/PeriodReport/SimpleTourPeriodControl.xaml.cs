using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfControlLibrary.ReportControls
{
    /// <summary>
    /// Логика взаимодействия для SimpleTourPeriodControl.xaml
    /// </summary>
    public partial class SimpleTourPeriodControl : UserControl, INotifyPropertyChanged
    {
        #region members
        private string m_tourName;
        private string m_tourCount;
        private SolidColorBrush m_Back;

        public string TourName
        {
            get => m_tourName;
            set
            {
                m_tourName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TourName"));
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
        public SimpleTourPeriodControl(string tn, string tc)
        {
            InitializeComponent();
            this.DataContext = this;

            TourName = tn;
            TourCount = tc;
        }
        #endregion
    }
}
