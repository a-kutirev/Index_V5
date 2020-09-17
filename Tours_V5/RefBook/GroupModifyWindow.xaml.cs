using ClassLibrary;
using System;
using System.ComponentModel;
using System.Data;
using System.Windows;

namespace Tours_V5.RefBook
{
    /// <summary>
    /// Логика взаимодействия для GroupModifyWindow.xaml
    /// </summary>
    public partial class GroupModifyWindow : Window, INotifyPropertyChanged
    {
        #region Members

        public event PropertyChangedEventHandler PropertyChanged;

        private DisplayedGroupModel m_Model;
        private string m_TourDate;
        private bool m_Status_Completed;
        private bool m_Status_Confirmed;
        private bool m_Status_Deleted;

        private bool m_b1 = false;
        private bool m_b2 = false;
        private bool m_b3 = false;
        private bool m_b4 = false;
        private bool m_b5 = false;
        private bool m_b6 = false;
        private bool m_b7 = false;
        private bool m_b8 = false;
        private bool m_b9 = false;

        private DataView m_GuidView;
        private DataView m_CategView;
        private DataView m_AcompView;

        public DataView GuidView
        {
            get => m_GuidView;
            set
            {
                m_GuidView = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GuidView"));
            }
        }
        public DataView CategView
        {
            get => m_CategView;
            set
            {
                m_CategView = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CategView"));
            }
        }
        public DataView AcompView
        {
            get => m_AcompView;
            set
            {
                m_AcompView = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AcompView"));
            }
        }

        public DisplayedGroupModel Model
        {
            get => m_Model;
            set
            {
                m_Model = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Model"));
            }
        }

        public string TourDate
        {
            get => m_TourDate;
            set
            {
                m_TourDate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TourDate"));
            }
        }

        public bool Status_Completed
        {
            get => m_Status_Completed;
            set
            {
                m_Status_Completed = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Status_Completed"));
            }
        }
        public bool Status_Confirmed
        {
            get => m_Status_Confirmed;
            set
            {
                m_Status_Confirmed = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Status_Confirmed"));
            }
        }
        public bool Status_Deleted
        {
            get => m_Status_Deleted;
            set
            {
                m_Status_Deleted = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Status_Deleted"));
            }
        }

        public bool B1
        {
            get => m_b1;
            set
            {
                m_b1 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("B1"));
            }
        }
        public bool B2
        {
            get => m_b2;
            set
            {
                m_b2 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("B2"));
            }
        }
        public bool B3
        {
            get => m_b3;
            set
            {
                m_b3 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("B3"));
            }
        }
        public bool B4
        {
            get => m_b4;
            set
            {
                m_b4 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("B4"));
            }
        }
        public bool B5
        {
            get => m_b5;
            set
            {
                m_b5 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("B5"));
            }
        }
        public bool B6
        {
            get => m_b6;
            set
            {
                m_b6 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("B6"));
            }
        }
        public bool B7
        {
            get => m_b7;
            set
            {
                m_b7 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("B7"));
            }
        }
        public bool B8
        {
            get => m_b8;
            set
            {
                m_b8 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("B8"));
            }
        }

        public bool B9
        {
            get => m_b9;
            set
            {
                m_b9 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("B9"));
            }
        }

        #endregion

        #region Constructor
        public GroupModifyWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        public GroupModifyWindow(int id)
        {
            InitializeComponent();
            this.DataContext = this;

            Model = new DisplayedGroupModel(id);

            #region Create Views for ComboBoxes
            DateTime date = Model.Groupdate;
            string sql = $"call GetGuidsByDate('{date.ToMySqlDateString()}')";
            GuidView = DBWrapper.MySqlWrapper.Select(sql).DefaultView;
            sql = "select * from categories";
            CategView = DBWrapper.MySqlWrapper.Select(sql).DefaultView;
            DataTable acompTb = new DataTable();
            acompTb.Columns.Add("int", typeof(int));
            acompTb.Columns.Add("string", typeof(string));
            DataRow dr = acompTb.NewRow();
            dr["string"] = "0"; dr["int"] = 0;
            acompTb.Rows.Add(dr);
            dr = acompTb.NewRow();
            dr["string"] = "1"; dr["int"] = 1;
            acompTb.Rows.Add(dr);
            dr = acompTb.NewRow();
            dr["string"] = "2"; dr["int"] = 2;
            acompTb.Rows.Add(dr);
            dr = acompTb.NewRow();
            dr["string"] = "3"; dr["int"] = 3;
            acompTb.Rows.Add(dr);
            AcompView = acompTb.DefaultView;
            #endregion

            TimePicker.Date = Model.Groupdate;
            TimePicker.HourVal = Model.Grouptime.Hours;
            TimePicker.MinuteVal = Model.Grouptime.Minutes;
            TourDate = Model.Groupdate.ToString("dd.MM.yyyy");

            #region Set Status
            Status_Deleted = (Model.Groupstatus & 1) == 1;
            Status_Confirmed = (Model.Groupstatus & 2) == 2;
            Status_Completed = (Model.Groupstatus & 4) == 4;
            #endregion

            #region Set Statististics
                int stat = Model.Groupstatistic;
                B1 = (stat & 1) >= 1;
                B2 = (stat & 2) >= 1;
                B3 = (stat & 4) >= 1;
                B4 = (stat & 8) >= 1;
                B5 = (stat & 16) >= 1;
                B6 = (stat & 32) >= 1;
                B7 = (stat & 64) >= 1;
                B8 = (stat & 128) >= 1;
                B9 = (stat & 256) >= 1;
            #endregion
        }
        #endregion

        #region Save Modified
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Model.Groupdate = new DateTime(
                Model.Groupdate.Year,
                Model.Groupdate.Month,
                Model.Groupdate.Day,
                TimePicker.HourVal,
                TimePicker.MinuteVal, 0);

            Model.Grouptime = new TimeSpan(TimePicker.HourVal, TimePicker.MinuteVal, 0);

            int status = 0;
            if (Status_Completed)   status |= 4;
            if (Status_Confirmed)   status |= 2;
            if (Status_Deleted)     status |= 1;
            Model.Groupstatus = status;

            int statistics = 0;
            if (B1) statistics |= 1;
            if (B2) statistics |= 2;
            if (B3) statistics |= 4;
            if (B4) statistics |= 8;
            if (B5) statistics |= 16;
            if (B6) statistics |= 32;
            if (B7) statistics |= 64;
            if (B8) statistics |= 128;
            if (B9) statistics |= 256;
            Model.Groupstatistic = statistics;

            Model.Update();

            DialogResult = true;
        }
        #endregion
    }
}