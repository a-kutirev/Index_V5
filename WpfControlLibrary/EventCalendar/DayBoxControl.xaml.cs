using System.Windows;
using System.Windows.Controls;

namespace WpfControlLibrary.EventCalendar
{
    public enum StatusDay
    {
        Weekend,
        Work
    }
    /// <summary>
    /// Логика взаимодействия для DayBoxControl.xaml
    /// </summary>
    public partial class DayBoxControl : UserControl
    {
        private StatusDay m_dayStatus;

        public DayBoxControl()
        {
            InitializeComponent();
        }

        public StatusDay DayStatus
        {
            get => m_dayStatus;
            set
            {
                m_dayStatus = value;
                if (m_dayStatus == StatusDay.Work)
                    DayAppointmentsStack.SetResourceReference(
                        StackPanel.BackgroundProperty, "WorkDay");
                else
                    DayAppointmentsStack.SetResourceReference(
                        StackPanel.BackgroundProperty, "WeekendDay");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
