using ReportLib;
using System;
using System.Windows;
using System.Windows.Controls;
using UtilsLib.StatWizard;

namespace Tours_V5.OldStyle
{
    /// <summary>
    /// Логика взаимодействия для SelectReportWindow.xaml
    /// </summary>
    public partial class SelectReportWindow : Window
    {
        private DateTime m_date;

        public SelectReportWindow(DateTime dt)
        {
            InitializeComponent();

            m_date = dt;
        }

        private void OldReportButton_Click(object sender, RoutedEventArgs e)
        {
            string n = (sender as Button).Name;

            Window wnd;

            switch(n)
            {
                case "DayRepTime":
                    wnd = new DayReportWindow(m_date, 1);
                    wnd.ShowDialog();
                    break;
                case "DayRepFloor":
                    wnd = new DayReportWindow(m_date, 2);
                    wnd.ShowDialog();
                    break;
                case "DayRepFull":
                    wnd = new FullDayReportWindow(m_date);
                    wnd.ShowDialog();
                    break;
                case "PriodRepMonth":
                    wnd = new PeriodReportWindow(0);
                    wnd.ShowDialog();
                    break;
                case "PriodRepQart":
                    wnd = new PeriodReportWindow(1);
                    wnd.ShowDialog();
                    break;
                case "PriodRepPeriod":
                    wnd = new PeriodReportWindow(2);
                    wnd.ShowDialog();
                    break;
                case "RepListGroup":
                    wnd = new PeriodListGroupWindow();
                    wnd.ShowDialog();
                    break;
            }

            Close();
        }

        private void StatReport_Click(object sender, RoutedEventArgs e)
        {
            StatWizardMainWindow statWiz = new StatWizardMainWindow();
            statWiz.ShowDialog();
        }
    }
}
