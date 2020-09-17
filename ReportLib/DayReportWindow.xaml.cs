using ClassLibrary;
using ReportLib.DayReports;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ReportLib
{
    /// <summary>
    /// Логика взаимодействия для DayReportWindow.xaml
    /// </summary>
    public partial class DayReportWindow : Window, INotifyPropertyChanged
    {
        #region Members
        private bool loaded = false;
        private DateTime m_selectedDate;
        private bool m_showNotes;
        private bool m_showEvents;
        private int m_currentReport; // 1 - by time, 2 - by floor

        public DateTime SelectedDate
        {
            get => m_selectedDate;
            set
            {
                m_selectedDate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedDate")) ;

                if (m_currentReport == 1)
                {
                    ByTimeReport brp = new ByTimeReport(m_selectedDate, m_showNotes, m_showEvents);
                    ReportDocument.Document = brp.Document;
                }
                if (m_currentReport == 2)
                {
                    ByFloorReport brp = new ByFloorReport(m_selectedDate, m_showNotes, m_showEvents);
                    ReportDocument.Document = brp.Document;
                }
            }
        }
        public bool ShowNotes
        {
            get => m_showNotes;
            set
            {
                m_showNotes = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowNotes"));

                if (m_currentReport == 1)
                {
                    ByTimeReport brp = new ByTimeReport(m_selectedDate, m_showNotes, m_showEvents);
                    ReportDocument.Document = brp.Document;
                }
                if (m_currentReport == 2)
                {
                    ByFloorReport brp = new ByFloorReport(m_selectedDate, m_showNotes, m_showEvents);
                    ReportDocument.Document = brp.Document;
                }
            }
        }
        public bool ShowEvents
        {
            get => m_showEvents;
            set
            {
                m_showEvents = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowEvents"));

                if (m_currentReport == 1)
                {
                    ByTimeReport brp = new ByTimeReport(m_selectedDate, m_showNotes, m_showEvents);
                    ReportDocument.Document = brp.Document;
                }
                if (m_currentReport == 2)
                {
                    ByFloorReport brp = new ByFloorReport(m_selectedDate, m_showNotes, m_showEvents);
                    ReportDocument.Document = brp.Document;
                }

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructor
        public DayReportWindow(DateTime d, int curr_rep)
        {
            ShowNotes = true;
            m_selectedDate = d;

            InitializeComponent();
            this.DataContext = this;

            ShowEvents = true;

            m_currentReport = curr_rep;

            if (m_currentReport == 1)
            {
                ByTimeReport brp = new ByTimeReport(m_selectedDate, m_showNotes, m_showEvents);
                ReportDocument.Document = brp.Document;
            }
            if (m_currentReport == 2)
            {
                ByFloorReport brp = new ByFloorReport(m_selectedDate, m_showNotes, m_showEvents);
                ReportDocument.Document = brp.Document;
            }
        }        
        #endregion

        #region Events
        private void CloseBt_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!loaded) return;

            ByFloor.Checked -= ToggleButton_Checked;
            ByFloor.Unchecked -= ToggleButton_Checked;
            ByTime.Checked -= ToggleButton_Checked;
            ByTime.Unchecked -= ToggleButton_Checked;

            ByFloor.IsChecked = false;
            ByTime.IsChecked = false;

            switch((sender as ToggleButton).Name)
            {
                case "ByTime":
                    ByTime.IsChecked = true;
                    ByTimeReport brp = new ByTimeReport(m_selectedDate, m_showNotes, m_showEvents);
                    ReportDocument.Document = brp.Document;
                    break;
                case "ByFloor":
                    ByFloor.IsChecked = true;
                    ByFloorReport bfr = new ByFloorReport(m_selectedDate, m_showNotes, m_showEvents);
                    ReportDocument.Document = bfr.Document;
                    break;
            }

            ByFloor.Checked += ToggleButton_Checked;
            ByFloor.Unchecked += ToggleButton_Checked;
            ByTime.Checked += ToggleButton_Checked;
            ByTime.Unchecked += ToggleButton_Checked;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loaded = true;
            if (m_currentReport == 1)
                ByTime.IsChecked = true;
            if (m_currentReport == 2)
                ByFloor.IsChecked = true;
        }
        #endregion

        private void FontSize_Click(object sender, RoutedEventArgs e)
        {
            string n = (sender as Button).Name;
            switch(n)
            {
                case "IncreaseFontSize":
                    Options.FontSize++;
                    break;
                case "DecreaseFontSize":
                    Options.FontSize--;
                    break;
            }

            if (m_currentReport == 1)
            {
                ByTimeReport brp = new ByTimeReport(m_selectedDate, m_showNotes, m_showEvents);
                ReportDocument.Document = brp.Document;
            }
            if (m_currentReport == 2)
            {
                ByFloorReport brp = new ByFloorReport(m_selectedDate, m_showNotes, m_showEvents);
                ReportDocument.Document = brp.Document;
            }

        }
    }
}
