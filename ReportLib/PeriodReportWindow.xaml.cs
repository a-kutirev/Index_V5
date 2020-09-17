using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ReportLib.PeriodReport;

namespace ReportLib
{
    /// <summary>
    /// Логика взаимодействия для PeriodReportWindow.xaml
    /// </summary>
    public partial class PeriodReportWindow : Window, INotifyPropertyChanged
    {
        #region Members

        private DateTime m_startPeriod;
        private DateTime m_endPeriod;
        private bool m_calcEvents;

        List<string> yearCombo = new List<string>() { "2017","2018","2019","2020","2021","2022","2023","2024","2025"};
        List<string> monthCombo = new List<string>() { "Январь", "Февраль", "Март", "Апрель", "Май",
            "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"};
        List<string> quartCombo = new List<string>() { "1 квартал","2 квартал","3 квартал","4 квартал"};

        private int m_reportType;
        private bool m_loaded = false;

        public DateTime StartPeriod
        {
            get => m_startPeriod;
            set
            {
                m_startPeriod = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StartPeriod"));
            }
        }
        public DateTime EndPeriod
        {
            get => m_endPeriod;
            set
            {
                m_endPeriod = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EndPeriod"));
            }
        }

        public bool CalcEvents
        {
            get => m_calcEvents;
            set
            {
                m_calcEvents = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CalcEvents"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructor
        public PeriodReportWindow(int ind)
        {
            InitializeComponent();
            this.DataContext = this;

            StartPeriod = DateTime.Now;
            EndPeriod = DateTime.Now;
            CalcEvents = true;

            YearCombo.ItemsSource = yearCombo;
            string curYear = DateTime.Now.Year.ToString();
            for(int i = 0; i < yearCombo.Count; i++)
            {
                YearCombo.SelectedIndex = i;
                if (YearCombo.Text == curYear) break;
            }

            m_reportType = ind;
        }
        #endregion

        #region Events
        private void CloseBt_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            m_loaded = true;

            rb1_Checked(rb1, null);
        }

        private void rb1_Checked(object sender, RoutedEventArgs e)
        {
            if (!m_loaded) return;

            switch((sender as RadioButton).Name)
            {
                case "rb1":
                    PeriodGroup.Header = "Месяц";
                    PeriodGroup.Visibility = Visibility.Visible;
                    DiscPeriodGroup.Visibility = Visibility.Collapsed;
                    PeriodCombo.ItemsSource = monthCombo;
                    int curMonth = DateTime.Now.Month;
                    PeriodCombo.SelectedIndex = curMonth - 1;
                    break;
                case "rb2":
                    PeriodGroup.Header = "Квартал";
                    PeriodGroup.Visibility = Visibility.Visible;
                    DiscPeriodGroup.Visibility = Visibility.Collapsed;
                    PeriodCombo.ItemsSource = quartCombo;
                    PeriodCombo.SelectedIndex = 0;
                    break;
                case "rb3":
                    PeriodGroup.Visibility = Visibility.Collapsed;
                    DiscPeriodGroup.Visibility = Visibility.Visible;
                    break;
            }
        }
        #endregion

        #region Запрос на формирование отчета
        private void ShowBt_Click(object sender, RoutedEventArgs e)
        {

            DateTime sp = new DateTime(2019, 12, 1);
            DateTime ep = new DateTime(2019, 12, 31);

            if ((bool)rb1.IsChecked)
            {
                int year = int.Parse(YearCombo.Text);
                int monthNum = PeriodCombo.SelectedIndex + 1;
                int lastDay = DateTime.DaysInMonth(year, monthNum);

                sp = new DateTime(year, monthNum, 1);
                ep = new DateTime(year, monthNum, lastDay);
            }
            else if((bool)rb2.IsChecked)
            {
                int year = int.Parse(YearCombo.Text);
                int l_day = 1;
                int quartNum = PeriodCombo.SelectedIndex;
                switch(quartNum)
                {
                    case 0:
                        sp = new DateTime(year, 1, 1);
                        l_day = DateTime.DaysInMonth(year, 3);
                        ep = new DateTime(year,3,l_day);
                        break;
                    case 1:
                        sp = new DateTime(year, 4, 1);
                        l_day = DateTime.DaysInMonth(year, 6);
                        ep = new DateTime(year, 6, l_day);
                        break;
                    case 2:
                        sp = new DateTime(year, 7, 1);
                        l_day = DateTime.DaysInMonth(year, 9);
                        ep = new DateTime(year, 9, l_day);
                        break;
                    case 3:
                        sp = new DateTime(year, 10, 1);
                        l_day = DateTime.DaysInMonth(year, 12);
                        ep = new DateTime(year, 12, l_day);
                        break;
                }
            }
            else
            {
                sp = StartPeriod;
                ep = EndPeriod;
            }
            PeriodReport_ pr = new PeriodReport_(sp, ep, CalcEvents);
            ReportDocument.Document = pr.Document;
        }
        #endregion
    }
}
