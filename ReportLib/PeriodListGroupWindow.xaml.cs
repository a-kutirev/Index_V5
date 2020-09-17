using ReportLib.PeriodReport;
using System;
using System.ComponentModel;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ReportLib
{
    /// <summary>
    /// Логика взаимодействия для PeriodListGroupWindow.xaml
    /// </summary>
    public partial class PeriodListGroupWindow : Window, INotifyPropertyChanged
    {
        #region Members

        private bool m_loaded = false;

        private DateTime m_startTime;
        private DateTime m_endTime;
        private bool m_showCompleted = false;
        private bool m_showTours;
        private bool m_showEvents;

        public DateTime StartTime
        {
            get => m_startTime;
            set
            {
                m_startTime = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StartTime"));
                if (!m_loaded) return;
            }
        }
        public DateTime EndTime
        {
            get => m_endTime;
            set
            {
                m_endTime = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EndTime"));
                if (!m_loaded) return;
            }
        }

        public bool ShowCompleted
        {
            get => m_showCompleted;
            set
            {
                m_showCompleted = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowCompleted"));
                if (!m_loaded) return;
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
        public PeriodListGroupWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            StartTime = DateTime.Now;
            EndTime = DateTime.Now;
            ShowCompleted = false;
            ShowTours = true;
            ShowEvents = true;
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
        }

        private void ShowBt_Click(object sender, RoutedEventArgs e)
        {
            ListGroupReport lgr = new ListGroupReport(m_startTime, m_endTime, m_showCompleted, m_showTours, m_showEvents);
            ReportDocument.Document = lgr.Document;
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            printDialog.PrintQueue = LocalPrintServer.GetDefaultPrintQueue();
            printDialog.PrintTicket = printDialog.PrintQueue.DefaultPrintTicket;


            PrintTicket pt = printDialog.PrintTicket;
            pt.PageOrientation = PageOrientation.Landscape;
            printDialog.PrintTicket = pt;

            //printDialog.PageRange = new PageRange(1, 1);
            //printDialog.PageRangeSelection = PageRangeSelection.UserPages;
            //printDialog.UserPageRangeEnabled = true;

            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintDocument(ReportDocument.Document.DocumentPaginator, "Document");
                this.Activate();
            }
        }
        #endregion
    }
}
