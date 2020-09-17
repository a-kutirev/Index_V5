using System;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace WpfControlLibrary.PeriodSelector
{
    /// <summary>
    /// Логика взаимодействия для PeriodSelector.xaml
    /// </summary>
    public partial class PeriodSelector : UserControl, INotifyPropertyChanged
    {
        #region Members

        public event EventHandler AddPeriodSelectorClick;

        private DateTime m_beginDate;
        private DateTime m_endDate;

        private int m_mode = 0;
        private bool EndDateChanged = false;
        private bool loaded = false;

        private DataView m_yearDataView;
        private DataView m_monthDataView;

        private int m_selectedBeginYear;
        private int m_selectedEndYear;
        private int m_selectedBeginMonth;
        private int m_selectedEndMonth;

        public DataView YearDataView
        {
            get => m_yearDataView;
            set
            {
                m_yearDataView = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("YearDataView"));
            }
        }
        public DataView MonthDataView
        {
            get => m_monthDataView;
            set
            {
                m_monthDataView = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MonthDataView"));
            }
        }
        public int SelectedBeginYear
        {
            get => m_selectedBeginYear;
            set
            {
                m_selectedBeginYear = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedBeginYear"));
            }
        }
        public int SelectedBeginMonth
        {
            get => m_selectedBeginMonth;
            set
            {
                m_selectedBeginMonth = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedBeginMonth"));
            }
        }
        public int SelectedEndYear
        {
            get => m_selectedEndYear;
            set
            {
                m_selectedEndYear = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedEndYear"));
            }
        }
        public int SelectedEndMonth
        {
            get => m_selectedEndMonth;
            set
            {
                m_selectedEndMonth = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedEndMonth"));
            }
        }

        public int Mode
        {
            get => m_mode;
            set
            {
                m_mode = value;

                if (m_mode == 0)
                {
                    MainGrid.RowDefinitions[0].Height = new GridLength(40);
                    MainGrid.RowDefinitions[1].Height = new GridLength(0);
                }
                if (m_mode == 1)
                {
                    MainGrid.RowDefinitions[0].Height = new GridLength(0);
                    MainGrid.RowDefinitions[1].Height = new GridLength(40);
                }
            }
        }

        public DateTime BeginDate
        {
            get
            {
                if(m_mode == 0)
                    m_beginDate = new DateTime(m_selectedBeginYear, m_selectedBeginMonth, 1);
                else
                    m_beginDate = (DateTime)BeginDatePicker.SelectedDate;
                return m_beginDate;
            }
        }
        public DateTime EndDate
        {
            get
            {
                if (m_mode == 0)
                    m_endDate = new DateTime(m_selectedEndYear, m_selectedEndMonth, DateTime.DaysInMonth(m_selectedEndYear, m_selectedEndMonth));
                else
                    m_endDate = (DateTime)EndDatePicker.SelectedDate;
                return m_endDate;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructor
        public PeriodSelector()
        {
            InitializeComponent();
            this.DataContext = this;

            BeginDatePicker.SelectedDate = DateTime.Now;
            EndDatePicker.SelectedDate = DateTime.Now;

            #region YearDataView

            DataTable dt = new DataTable();
            dt.Columns.Add("yearDisplay", typeof(string));
            dt.Columns.Add("yearValue", typeof(int));

            for(int i = 0; i < 10; i++)
            {
                DataRow drow = dt.NewRow();
                int v = 2018 + i;
                drow["yearDisplay"] = v.ToString();
                drow["yearValue"] = v;

                dt.Rows.Add(drow);
            }

            YearDataView = dt.DefaultView;
            SelectedBeginYear = DateTime.Now.Year;
            SelectedEndYear = DateTime.Now.Year;
            #endregion

            #region MonthDataView

            dt = new DataTable();
            dt.Columns.Add("monthDisplay", typeof(string));
            dt.Columns.Add("monthValue", typeof(int));

            DataRow dr = dt.NewRow();
            dr["monthDisplay"] = "Январь";
            dr["monthValue"] = 1;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["monthDisplay"] = "Февраль";
            dr["monthValue"] = 2;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["monthDisplay"] = "Март";
            dr["monthValue"] = 3;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["monthDisplay"] = "Апрель";
            dr["monthValue"] = 4;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["monthDisplay"] = "Май";
            dr["monthValue"] = 5;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["monthDisplay"] = "Июнь";
            dr["monthValue"] = 6;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["monthDisplay"] = "Июль";
            dr["monthValue"] = 7;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["monthDisplay"] = "Август";
            dr["monthValue"] = 8;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["monthDisplay"] = "Сентябрь";
            dr["monthValue"] = 9;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["monthDisplay"] = "Октябрь";
            dr["monthValue"] = 10;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["monthDisplay"] = "Ноябрь";
            dr["monthValue"] = 11;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["monthDisplay"] = "Декабрь";
            dr["monthValue"] = 12;
            dt.Rows.Add(dr);

            MonthDataView = dt.DefaultView;

            SelectedBeginMonth = DateTime.Now.Month;
            SelectedEndMonth = DateTime.Now.Month;
            #endregion
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddPeriodSelectorClick?.Invoke(this, e);
        }
        private void BeginDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {            
            if(!EndDateChanged & loaded)
                EndDatePicker.SelectedDate = BeginDatePicker.SelectedDate;
        }
        private void EndDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!loaded) return;
            EndDateChanged = true;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            loaded = true;
        }
    }
}
