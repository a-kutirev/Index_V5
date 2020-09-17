using ClassLibrary;
using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Controls;

namespace WpfControlLibrary.TimePickerControl
{
    /// <summary>
    /// Логика взаимодействия для TimePickerControl.xaml
    /// </summary>
    public partial class TimePickerControl : UserControl, INotifyPropertyChanged
    {
        #region Members
        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler TimeValueChanged;

        private TimeSpan m_time;
        private DateTime m_date;
        private DataView m_hourView;
        private DataView m_minuteView;
        private int m_HourVal;
        private int m_MinuteVal;

        public TimeSpan Time
        {
            get
            {
                m_time = new TimeSpan(m_HourVal, m_MinuteVal, 0);
                return m_time;
            }
            set
            {                
                m_time = value;
                TimeSpan copy = m_time;
                HourVal = copy.Hours;
                MinuteVal = copy.Minutes;
            }
        }
        public DateTime Date
        {
            get => m_date;
            set
            {
                m_date = value;
                DateTime tmp = DayOptions.Date;
                DayOptions.Date = m_date;
                HourView = GetHourView(DayOptions.StartHour);
                MinuteView = GetMinuteView();
                HourVal = DayOptions.StartHour;
                DayOptions.Date = tmp;
            }
        }
        public DataView HourView
        {
            get => m_hourView;
            set
            {
                m_hourView = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HourView"));
            }
        }
        public DataView MinuteView
        {
            get => m_minuteView;
            set
            {
                m_minuteView = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MinuteView"));
            }
        }
        public int HourVal
        {
            get => m_HourVal;
            set
            {
                m_HourVal = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HourVal"));
                m_time = new TimeSpan(HourVal, MinuteVal, 0);
            }
        }
        public int MinuteVal
        {
            get => m_MinuteVal;
            set
            {
                m_MinuteVal = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MinuteVal"));
                m_time = new TimeSpan(HourVal, MinuteVal, 0);
            }
        }
        #endregion

        #region Constructor
        public TimePickerControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        #endregion

        #region Refill Hour-Minute comboboxes
        private DataView GetHourView(int sh)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("hourvalue", typeof(int)));
            dt.Columns.Add(new DataColumn("hourstring", typeof(string)));
            for (int i = 0; i < 9; i++)
            {
                DataRow dr = dt.NewRow();
                dr["hourvalue"] = sh + i;
                dr["hourstring"] = ((int)dr["hourvalue"]).ToString("D2");
                dt.Rows.Add(dr);
            }

            return dt.DefaultView;
        }
        private DataView GetMinuteView()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("minutevalue", typeof(int)));
            dt.Columns.Add(new DataColumn("minutestring", typeof(string)));
            for (int i = 0; i < 4; i++)
            {
                DataRow dr = dt.NewRow();
                dr["minutevalue"] = i * 15;
                dr["minutestring"] = ((int)dr["minutevalue"]).ToString("D2");
                dt.Rows.Add(dr);
            }

            return dt.DefaultView;
        }
        #endregion
    }
}
