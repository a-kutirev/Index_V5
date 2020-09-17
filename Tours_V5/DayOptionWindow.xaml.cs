using ClassLibrary;
using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfControlLibrary.AdditionGuidControl;

namespace Tours_V5
{
    /// <summary>
    /// Логика взаимодействия для DayOptionWindow.xaml
    /// </summary>
    public partial class DayOptionWindow : Window, INotifyPropertyChanged
    {
        #region Members
        public event PropertyChangedEventHandler PropertyChanged;

        private DaysOptionModel m_model;
        private DateTime m_date;
        private DataView m_StartHourView;
        private DataView m_DayLengthView;
        private string m_StrDate;
        private bool loaded = false;        

        private bool m_offControl = false;
        private bool m_obzorTour = false;
        private bool m_useStartHour = false;
        private bool m_additionGuid = false;

        private int m_StartHour = 10;

        public DaysOptionModel Model
        {
            get => m_model;
            set
            {
                m_model = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Model"));
            }
        }
        public DateTime Date
        {
            get => m_date;
            set
            {
                m_date = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Date"));
            }
        }
        public string StrDate
        {
            get => m_StrDate;
            set
            {
                m_StrDate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StrDate"));
            }
        }        
        public DataView StartHourView
        {
            get => m_StartHourView;
            set
            {
                m_StartHourView = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StartHourView"));
            }
        }
        public DataView DayLengthView
        {
            get => m_DayLengthView;
            set
            {
                m_DayLengthView = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DayLengthView"));
            }
        }

        public bool OffControl
        {
            get => m_offControl;
            set
            {
                m_offControl = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OffControl"));
                Model.Offcontrol = m_offControl ? 1 : 0;
            }
        }
        public bool ObzorTour
        {
            get => m_obzorTour;
            set
            {
                m_obzorTour = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ObzorTour"));
                Model.Obzor = m_obzorTour ? 1 : 0;
            }
        }
        public bool UseStartHour
        {
            get => m_useStartHour;
            set
            {
                m_useStartHour = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("UseStartHour"));
                Model.Usestarthour = m_useStartHour ? 1 : 0;
            }
        }
        public bool AdditionGuid
        {
            get => m_additionGuid;
            set
            {
                m_additionGuid = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AdditionGuid"));
                Model.Addguid = m_additionGuid ? 1 : 0;
            }
        }
        public int StartHour
        {
            get => m_StartHour;
            set
            {
                m_StartHour = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StartHour"));
            }
        }
        #endregion

        #region Constructor
        public DayOptionWindow(DateTime dt)
        {
            InitializeComponent();
            this.DataContext = this;

            List<GuidModel> guids = (List<GuidModel>)DBWrapper.MySqlWrapper.Select($"call GetAdditionalGuidsOnDate('{dt.ToMySqlDateString()}')").ToList<GuidModel>();            

            if (dt.DayOfWeek == DayOfWeek.Monday || dt.DayOfWeek == DayOfWeek.Tuesday)
                WeekCheck.IsChecked = true;
            else
                WorkCheck.IsChecked = true;

            for (int i = 0; i < guids.Count; i++)
                GuidList.Items.Add(new AdditionGuidControl(guids[i]));

            StartHourView = GetStartHourView();
            DayLengthView = GetDayLengthView();

            Model = new DaysOptionModel(dt);            
            if (Model.Iddaysoption != 0)
            {
                OffControl =    Model.Offcontrol == 1;
                ObzorTour =     Model.Obzor == 1;
                UseStartHour =  Model.Usestarthour == 1;
                AdditionGuid =  Model.Addguid == 1;
                if(m_additionGuid)
                {
                    string sql = $"select * from addition_guids where iddaysoptions = {Model.Iddaysoption}";
                    DataTable tmp = DBWrapper.MySqlWrapper.Select(sql);
                    for(int i = 0; i < tmp.Rows.Count; i++)
                    {
                        int guidId = (int)tmp.Rows[i]["idguid"];
                        for (int j = 0; j < GuidList.Items.Count; j++)
                        {                            
                            AdditionGuidControl m = GuidList.Items[j] as AdditionGuidControl;
                            if (m.Model.Idguid == guidId) m.GuidChecked = true;                            
                        }
                    }
                }
                if(UseStartHour) StartHour = Model.Starthour;
            }
            else
            {
                Model.Daysoptiondate = dt;
                OffControl = false;
                ObzorTour = false;
                UseStartHour = false;
                AdditionGuid = false;
            }

            Date = dt;
            StrDate = $"{dt.ToString("dd MMMM yyyy")} г.";
        }

        private DataView GetStartHourView()
        {
            DataTable tmp = new DataTable();
            tmp.Columns.Add("hourstr", typeof(string));
            tmp.Columns.Add("hour", typeof(int));

            for (int i = 0; i< 24; i++)
            {
                DataRow dr = tmp.NewRow();
                dr["hour"] = i;
                dr["hourstr"] = i.ToString();
                tmp.Rows.Add(dr);
            }

            return tmp.DefaultView;
        }

        private DataView GetDayLengthView()
        {
            DataTable tmp = new DataTable();
            tmp.Columns.Add("hourstr", typeof(string));
            tmp.Columns.Add("hour", typeof(int));

            for (int i = 1; i < 13; i++)
            {
                DataRow dr = tmp.NewRow();
                dr["hour"] = i;
                dr["hourstr"] = i.ToString();
                tmp.Rows.Add(dr);
            }

            return tmp.DefaultView;
        }
        #endregion

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!loaded) return;
            page1.Visibility = Visibility.Collapsed;
            page2.Visibility = Visibility.Collapsed;
            page3.Visibility = Visibility.Collapsed;
            page4.Visibility = Visibility.Collapsed;
            page5.Visibility = Visibility.Collapsed;

            int index = (sender as ListBox).SelectedIndex;
            switch(index)
            {
                case 0:
                    page1.Visibility = Visibility.Visible;
                    break;
                case 1:
                    page2.Visibility = Visibility.Visible;
                    break;
                case 2:
                    page3.Visibility = Visibility.Visible;
                    break;
                case 3:
                    page4.Visibility = Visibility.Visible;
                    break;
                case 4:
                    page5.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loaded = true;
            page1.Visibility = Visibility.Visible;
        }

        #region Сохранение и сброс
        private void SaveBt_Click(object sender, RoutedEventArgs e)
        {
            Model.Workday = (bool)WorkCheck.IsChecked ? 1 : 0;            

            if (Model.Iddaysoption == 0)
                Model.Insert();
            else
                Model.Update();

            if(Model.Addguid == 1)
            {
                int id = Model.Iddaysoption;
                string delSql = $"delete from addition_guids where iddaysoptions = {id}";
                DBWrapper.MySqlWrapper.Execute(delSql);
                for (int i = 0; i < GuidList.Items.Count; i++)
                {
                    AdditionGuidControl agc = GuidList.Items[i] as AdditionGuidControl;
                    if(agc.GuidChecked)
                    {
                        string sqlTmp = $"insert into addition_guids(iddaysoptions, idguid)values({id},{agc.Model.Idguid})";
                        DBWrapper.MySqlWrapper.Execute(sqlTmp);
                    }
                }
            }

                DialogResult = true;
        }

        private void ResetBt_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Выуверены что хотите удалить текущие опции?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question)
                == MessageBoxResult.Yes)
            {
                if (Model.Iddaysoption != 0)
                {
                    string sql = $"delete from addition_guids where iddaysoptions = {Model.Iddaysoption}";
                    DBWrapper.MySqlWrapper.Execute(sql);
                    sql = $"delete from daysoptions where iddaysoption = {Model.Iddaysoption}";
                    DBWrapper.MySqlWrapper.Execute(sql);
                }
                DialogResult = true;
            }            
        }
        #endregion
    }
}
