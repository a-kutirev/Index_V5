using ClassLibrary;
using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfControlLibrary.EventCalendar
{
    /// <summary>
    /// Логика взаимодействия для MonthView.xaml
    /// </summary>
    public partial class MonthView : UserControl
    {
        private DateTime m_displayStartDate = DateTime.Now;
        private int m_displayMonth = 1;
        private int m_displayYear = 2019;
        private CultureInfo m_cultureInfo;
        private System.Globalization.Calendar sysCal;
        private List<Appointment> m_monthAppointments;

        public event EventHandler<MonthChangedEventArgs> DisplayMonthChanged;
        public event EventHandler<NewAppointmentEventArgs> DayBoxDoubleClicked;
        public event EventHandler<int> AppointmentDblClicked;

        public List<Appointment> MonthAppointments
        {
            get => m_monthAppointments;
            set
            {
                m_monthAppointments = value;
                BuildCalendarUI();
            }
        }
        public DateTime DisplayStartDate
        {
            get => m_displayStartDate;
            set
            {
                m_displayStartDate = value;
                m_displayMonth = m_displayStartDate.Month;
                m_displayYear = m_displayStartDate.Year;
            }

        }

        public MonthView()
        {
            InitializeComponent();

            MonthGoNext.Source = new BitmapImage(new Uri("/WpfControlLibrary;component/Resource/ForwardGreen.png", UriKind.Relative));
            MonthGoPrev.Source = new BitmapImage(new Uri("/WpfControlLibrary;component/Resource/ForwardGreen.png", UriKind.Relative));
            m_displayStartDate = DateTime.Now.AddDays(-1 * (DateTime.Now.Day - 1));
            m_displayMonth = m_displayStartDate.Month;
            m_displayYear = m_displayStartDate.Year;
            m_cultureInfo = new CultureInfo(CultureInfo.CurrentUICulture.LCID);
            sysCal = m_cultureInfo.Calendar;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            BuildCalendarUI();
        }

        private void BuildCalendarUI()
        {
            int iDaysInMonth = sysCal.GetDaysInMonth(m_displayYear, m_displayMonth);
            int iOffsetDays = (int)m_displayStartDate.DayOfWeek;
            iOffsetDays = (iOffsetDays == 0) ? 6 : iOffsetDays - 1;
            int iWeekCount = 0;

            WeekOfDaysControl weekRowControl = new WeekOfDaysControl();
            MonthViewGrid.Children.Clear();
            AddRowsToMonthGrid(iDaysInMonth, iOffsetDays);
            string fullMonthName = new DateTime(2015, m_displayMonth, 1).ToString("MMMM", CultureInfo.CreateSpecificCulture("ru"));
            MonthYearLabel.Content = fullMonthName + "  " + m_displayYear;

            //string sql = $"select * from daysoptions where year(daysoptiondate) = {m_displayYear} and month(daysoptiondate) = {m_displayMonth}";
            //List<DaysOptionModel> tmp = (List<DaysOptionModel>)DBWrapper.MySqlWrapper.Select(sql).ToList<DaysOptionModel>();
            DateTime endDate = m_displayStartDate.AddDays(iDaysInMonth);

            string sql = $"select * from daysoptions where daysoptiondate >= '{m_displayStartDate.ToString("yyyy-MM-dd")}' and daysoptiondate <= '{endDate.ToString("yyyy-MM-dd")}'";
            List<DaysOptionModel> tmp = (List<DaysOptionModel>)DBWrapper.MySqlWrapper.Select(sql).ToList<DaysOptionModel>();

            Dictionary<DateTime, bool> WorkTable = new Dictionary<DateTime, bool>();
            for (int i = 0; i < tmp.Count; i++)
                    WorkTable.Add(tmp[i].Daysoptiondate, tmp[i].Workday == 1);

            for (int i = 1; i < iDaysInMonth + 1; i++)
            {
                if ((i != 1) && (Math.IEEERemainder((i + iOffsetDays - 1), 7) == 0))
                {
                    Grid.SetRow(weekRowControl, iWeekCount);                    
                    MonthViewGrid.Children.Add(weekRowControl);
                    weekRowControl = new WeekOfDaysControl();
                    iWeekCount++;
                }
                DateTime dayBoxDate = new DateTime(m_displayYear, m_displayMonth, i);
                DayBoxControl dayBox = new DayBoxControl();
                int tmpOffset = (int)(new DateTime(m_displayYear, m_displayMonth, i)).DayOfWeek;

                Color ccc;
                if (tmpOffset == 1 || tmpOffset == 2)
                    ccc = (Color)ColorConverter.ConvertFromString("#FFFFA5A5");
                else
                    ccc = (Color)ColorConverter.ConvertFromString("#FFAEAEAE");

                DateTime dt = new DateTime(m_displayYear, m_displayMonth, i);

                if (WorkTable.ContainsKey(dt))
                    if(!WorkTable[dt])
                        ccc = (Color)ColorConverter.ConvertFromString("#FFFFA5A5");
                    else
                        ccc = (Color)ColorConverter.ConvertFromString("#FFAEAEAE");

                dayBox.DayAppointmentsStack.Background = new LinearGradientBrush(Color.FromRgb(255, 255, 255), ccc, 90);

                dayBox.DayNumberLabelTextBox.Text = i.ToString();
                dayBox.Tag = i;
                dayBox.MouseDoubleClick += DayBox_MouseDoubleClick; ;

                if (dayBoxDate == DateTime.Today)
                {
                    dayBox.DayLabelRowBorder.Background = (Brush)TryFindResource("OrangeGradientBrush");
                    dayBox.DayAppointmentsStack.Background = Brushes.Wheat;
                }

                if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                {
                    DayBoxAppointmentControl apt = new DayBoxAppointmentControl();
                    apt.DisplayText.Text = "Apt on " + i + " th";
                    dayBox.DayAppointmentsStack.Children.Add(apt);
                }
                else
                {
                    if (m_monthAppointments != null)
                    {
                        int iDay = i;
                        List<Appointment> aptInDay =
                            m_monthAppointments.FindAll(new Predicate<Appointment>(apt => apt.StartTime.Day == i));
                        foreach (var a in aptInDay)
                        {
                            DayBoxAppointmentControl apt = new DayBoxAppointmentControl();
                            apt.DisplayText.Text = a.Subject;
                            apt.Tag = a.AppointmentID;
                            apt.MouseDoubleClick += Apt_MouseDoubleClick; ;
                            dayBox.DayAppointmentsStack.Children.Add(apt);
                        }
                    }
                }

                Grid.SetColumn(dayBox, (i - (iWeekCount * 7)) + iOffsetDays);
                weekRowControl.WeekRowGrid.Children.Add(dayBox);
            }
            Grid.SetRow(weekRowControl, iWeekCount);
            MonthViewGrid.Children.Add(weekRowControl);
        }

        private void Apt_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void DayBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((e.Source as DayBoxControl) != null)
            {
                NewAppointmentEventArgs ev = new NewAppointmentEventArgs();
                if ((sender as DayBoxControl).Tag != null)
                {
                    ev.StartDate = new DateTime(m_displayYear, m_displayMonth, (int)(e.Source as DayBoxControl).Tag);
                    ev.EndDate = new DateTime(m_displayYear, m_displayMonth, (int)(e.Source as DayBoxControl).Tag);
                }
                e.Handled = true;
                OnDayBoxDoubleClicked(ev);
            }
        }

        private void UpdateMonth(int v)
        {
            MonthChangedEventArgs ev = new MonthChangedEventArgs
            {
                OldDisplayStartDate = m_displayStartDate
            };
            DisplayStartDate = m_displayStartDate.AddMonths(v);
            ev.NewDisplayStartDate = m_displayStartDate;
            OnMonthChanged(ev);
        }

        private void OnMonthChanged(MonthChangedEventArgs ev)
        {
            DisplayMonthChanged?.Invoke(this, ev);
        }
        private void OnDayBoxDoubleClicked(NewAppointmentEventArgs ev)
        {
            DayBoxDoubleClicked?.Invoke(this, ev);
        }
        private void OnAppointmentDblClicked(int i)
        {
            AppointmentDblClicked?.Invoke(this, i);
        }
        private void AddRowsToMonthGrid(int iDaysInMonth, int iOffsetDays)
        {
            MonthViewGrid.RowDefinitions.Clear();
            GridLength rowHeight = new GridLength(60, GridUnitType.Star);
            int EndOffSetDays = 8 - (int)m_displayStartDate.AddDays(iDaysInMonth - 1).DayOfWeek;
            for (int i = 0; i < (int)(iDaysInMonth + EndOffSetDays + iOffsetDays) / 7; i++)
            {
                RowDefinition rowDef = new RowDefinition
                {
                    Height = rowHeight
                };
                MonthViewGrid.RowDefinitions.Add(rowDef);
            }
        }

        private void MonthGoPrev_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {            
            UpdateMonth(-1);
        }

        private void MonthGoNext_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UpdateMonth(1);
        }
    }

    #region EventArgs & Utilities
    public class MonthChangedEventArgs : EventArgs
    {
        public DateTime OldDisplayStartDate;
        public DateTime NewDisplayStartDate;
    }
    public class NewAppointmentEventArgs : EventArgs
    {
        public DateTime StartDate;
        public DateTime EndDate;
        public int CandidateId;
        public int RequirementId;
    }

    public static class Utilities
    {
        public static FrameworkElement FindVisualAncestor(
            Type ancestorType, Visual visual)
        {
            while ((visual != null) && !(ancestorType.IsInstanceOfType(visual)))
            {
                visual = VisualTreeHelper.GetParent(visual) as Visual;
            }
            return visual as FrameworkElement;
        }
    }
    #endregion
}
