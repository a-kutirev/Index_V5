using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows;
using WpfControlLibrary.EventCalendar;

namespace Tours_V5
{
    /// <summary>
    /// Логика взаимодействия для CalendarWindow.xaml
    /// </summary>
    public partial class CalendarWindow : Window, INotifyPropertyChanged
    {

        #region Members
        private List<Appointment> myAppointsmentList = new List<Appointment>();
        private int m_monthNum, m_yearNum;
        private DataTable monthCountTable;
        private DataTable optionsCalendarTable;
        private bool m_showFade = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool ShowFade
        {
            get => m_showFade;
            set
            {
                m_showFade = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowFade"));
            }
        }
        #endregion

        #region Constructor
        public CalendarWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            m_monthNum = AptCalendar.DisplayStartDate.Month;
            m_yearNum = AptCalendar.DisplayStartDate.Year;
            SetApointment();
            ShowFade = false;
        }
        #endregion

        private void MonthView_DisplayMonthChanged(object sender, WpfControlLibrary.EventCalendar.MonthChangedEventArgs e)
        {
            m_monthNum = AptCalendar.DisplayStartDate.Month;
            m_yearNum = AptCalendar.DisplayStartDate.Year;
            SetApointment();
        }

        private void SetApointment()
        {
            string sql = $"select groupdate, count(groupdate) as count from (select groupdate from _groups " +
                $"where year(groupdate) = {m_yearNum} and month(groupdate) = {m_monthNum} and (groupstatus & 1) = 0) a group by groupdate;";
            monthCountTable = DBWrapper.MySqlWrapper.Select(sql);
            sql = $"select * from daysoptions where year(daysoptiondate) = {m_yearNum} and month(daysoptiondate) = {m_monthNum}";
            optionsCalendarTable = DBWrapper.MySqlWrapper.Select(sql);

            myAppointsmentList.Clear();

            int aptID = 0;

            for (int i = 0; i < monthCountTable.Rows.Count; i++)
            {
                Appointment apt = new Appointment
                {
                    AppointmentID = aptID++,
                    Subject = $"Всего: {monthCountTable.Rows[i]["count"]}",
                    StartTime = DateTime.Parse(monthCountTable.Rows[i]["groupdate"].ToString()),
                    EndTime = DateTime.Parse(monthCountTable.Rows[i]["groupdate"].ToString())
                };
                myAppointsmentList.Add(apt);
            }
            for(int i = 0; i < optionsCalendarTable.Rows.Count; i++)
            {

                DateTime m_StartTime = DateTime.Parse(optionsCalendarTable.Rows[i]["daysoptiondate"].ToString());
                DateTime m_EndTime = DateTime.Parse(optionsCalendarTable.Rows[i]["daysoptiondate"].ToString());

                bool addguid = optionsCalendarTable.Rows[i]["addguid"].ToString() == "1";
                if (addguid) // Доп экскурсовод
                {
                    Appointment apt = new Appointment()
                    {
                        AppointmentID = aptID++,
                        Subject = "Доп. экскурсовод",
                        StartTime = m_StartTime,
                        EndTime = m_EndTime
                    };
                    myAppointsmentList.Add(apt);
                }
                bool obzor = optionsCalendarTable.Rows[i]["obzor"].ToString() == "1";
                if (obzor) // Обзорная экскурсия
                {
                    Appointment apt = new Appointment()
                    {
                        AppointmentID = aptID++,
                        Subject = "Обз. экскурсия",
                        StartTime = m_StartTime,
                        EndTime = m_EndTime
                    };
                    myAppointsmentList.Add(apt);
                }
                bool usesh = optionsCalendarTable.Rows[i]["usestarthour"].ToString() == "1";
                if(usesh)
                {
                    Appointment apt = new Appointment()
                    {
                        AppointmentID = aptID++,
                        Subject = $"Нач. раб. дня - {optionsCalendarTable.Rows[i]["starthour"].ToString()}",
                        StartTime = m_StartTime,
                        EndTime = m_EndTime
                    };
                    myAppointsmentList.Add(apt);
                }
            }

            AptCalendar.MonthAppointments = myAppointsmentList;
        }

        private void AddNoteBt_Click(object sender, RoutedEventArgs e)
        {
            AddNoteWindow anw = new AddNoteWindow();
            anw.ShowDialog();
        }

        private void MonthView_DayBoxDoubleClicked(object sender, WpfControlLibrary.EventCalendar.NewAppointmentEventArgs e)
        {           
            DayOptionWindow dow = new DayOptionWindow(e.StartDate);
            ShowFade = true;
            dow.ShowDialog();
            ShowFade = false;
            SetApointment();
        }
    }
}
