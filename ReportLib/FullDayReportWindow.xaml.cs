using ReportLib.DayReports;
using System;
using System.ComponentModel;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ReportLib
{
    /// <summary>
    /// Логика взаимодействия для FullDayReportWindow.xaml
    /// </summary>
    public partial class FullDayReportWindow : Window, INotifyPropertyChanged
    {
        #region Members
        private DateTime m_selectedDate;
        private bool m_showTours;
        private bool m_showEvents;

        public DateTime SelectedDate
        {
            get => m_selectedDate;
            set
            {
                m_selectedDate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedDate"));
                try
                {
                    FullDayReport fdr = new FullDayReport(m_selectedDate, m_showTours, m_showEvents);
                    ReportDocument.Document = fdr.Document;
                }
                catch { }
            }
        }

        public bool ShowTours
        {
            get => m_showTours;
            set
            {
                m_showTours = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowTours"));
            }
        }
        public bool ShowEvents
        {
            get => m_showEvents;
            set
            {
                m_showEvents = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowEvents"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructor
        public FullDayReportWindow(DateTime dateTime)
        {
            SelectedDate = dateTime;
            InitializeComponent();
            this.DataContext = this;

            ShowTours = true;
            ShowEvents = true;
            
            FullDayReport fdr = new FullDayReport(m_selectedDate, m_showTours, m_showEvents);
            ReportDocument.Document = fdr.Document;
        }
        #endregion

        #region Events
        private void CloseBt_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            printDialog.PrintQueue = LocalPrintServer.GetDefaultPrintQueue();
            printDialog.PrintTicket = printDialog.PrintQueue.DefaultPrintTicket;           
            

            PrintTicket pt = printDialog.PrintTicket;
            pt.PageOrientation = PageOrientation.Landscape;
            printDialog.PrintTicket = pt;

            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintDocument(ReportDocument.Document.DocumentPaginator, "Document");
                this.Activate();
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            FullDayReport fdr = new FullDayReport(m_selectedDate, m_showTours, m_showEvents);
            ReportDocument.Document = fdr.Document;
        }
    }
}
