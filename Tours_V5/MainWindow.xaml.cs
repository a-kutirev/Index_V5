using ClassLibrary;
using ClassLibrary.CalendarBackground;
using ClassLibrary.Models;
using DBWrapper;
using Microsoft.Windows.Controls.Ribbon;
using ReportLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using Tours_V5.OldStyle;
using Tours_V5.RefBook;
using UtilsLib.Regroup;
using UtilsLib.RegroupNew;
using UtilsLib.StatWizard;
using WpfControlLibrary.GroupControls;
using WpfControlLibrary.MissingControl;

namespace Tours_V5
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Fields & Events
        public event PropertyChangedEventHandler PropertyChanged;
        private CalendarBackground calendarBackground;
        private List<DisplayedGroupModel> grpList;
        private List<EventGroupModel> evtList;
        private DateTime m_selectedDate = DateTime.Now;
        private string m_titleString = "";
        private bool m_useOldInterface = false;
        private bool m_showDeleted;
        private bool m_showCompleted;
        private bool m_showEmptyComments;
        private string m_dayMessage;
        private string m_dayLimit;
        private bool m_loaded = false;
        private bool m_showFade = false;
        private ContextMenu contextMenu = new ContextMenu();
        
        public DateTime SelectedDate
        {
            get => m_selectedDate;
            set
            {
                m_selectedDate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedDate"));
            }
        }
        public bool UseOldInterface
        {
            get => m_useOldInterface;
            set
            {
                m_useOldInterface = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("UseOldInterface"));
                Options.UseOldInterface = m_useOldInterface;
                if (m_useOldInterface)
                {
                    OldInterface.Visibility = Visibility.Visible;
                    NewInterface.Visibility = Visibility.Collapsed;
                }
                else
                {
                    OldInterface.Visibility = Visibility.Collapsed;
                    NewInterface.Visibility = Visibility.Visible;
                }

            }
        }
        public bool ShowDeleted
        {
            get => m_showDeleted;
            set
            {
                m_showDeleted = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowDeleted"));
                Options.HideDeleted = m_showDeleted;
                calendar_DisplayDateChanged(null, null);
            }
        }
        public bool ShowCompleted
        {
            get => m_showCompleted;
            set
            {
                m_showCompleted = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowCompleted"));
                Options.HideCompleted = m_showCompleted;
                calendar_DisplayDateChanged(null, null);
            }
        }
        public bool ShowEmptyComments
        {
            get => m_showEmptyComments;
            set
            {
                m_showEmptyComments = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowEmptyComments"));
                Options.HideEmptyComments = m_showEmptyComments;
                calendar_DisplayDateChanged(null, null);
            }
        }

        public string TitleString
        {
            get => m_titleString;
            set
            {
                m_titleString = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TitleString"));
            }
        }

        public string DayMessage
        {
            get => m_dayMessage;
            set
            {
                m_dayMessage = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DayMessage"));
            }
        }
        public string DayLimit
        {
            get => m_dayLimit;
            set
            {
                m_dayLimit = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DayLimit"));
            }
        }

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
        public MainWindow(int userid)
        {
            try
            {

                InitializeComponent();
                this.DataContext = this;

                string pathCurrent = AppDomain.CurrentDomain.BaseDirectory + "Update.xml";

                if (File.Exists(pathCurrent))
                {
                    XmlDocument m_documentCurrent = new XmlDocument();
                    m_documentCurrent.Load(pathCurrent);
                    XmlElement element = m_documentCurrent.DocumentElement;

                    foreach (XmlNode xnode in element)
                    {
                        if (xnode.Name == "version") VersionLabel.Content = xnode.InnerText;
                    }
                }


                #region Initial table context menu (old bar)
                contextMenu.FontSize = 14;

                MenuItem mi = new MenuItem();
                mi.Name = "GroupItem";
                mi.Header = "Группы";
                mi.Click += Mi_Click1; ; ;
                contextMenu.Items.Add(mi);

                mi = new MenuItem();
                mi.Name = "EventItem";
                mi.Header = "Мероприятия";
                mi.Click += Mi_Click; ; ;
                contextMenu.Items.Add(mi);

                mi = new MenuItem();
                mi.Name = "ContactItem";
                mi.Header = "Контакты";
                mi.Click += Mi_Click; ;
                contextMenu.Items.Add(mi);

                mi = new MenuItem();
                mi.Name = "TourItem";
                mi.Header = "Экскурсии";
                mi.Click += Mi_Click; ;
                contextMenu.Items.Add(mi);

                mi = new MenuItem();
                mi.Name = "GuidItem";
                mi.Header = "Сотрудники";
                mi.Click += Mi_Click; ;
                contextMenu.Items.Add(mi);

                mi = new MenuItem();
                mi.Name = "MissingItem";
                mi.Header = "Отсутствующие";
                mi.Click += Mi_Click; ;
                contextMenu.Items.Add(mi);

                mi = new MenuItem();
                mi.Name = "NoteItem";
                mi.Header = "Заметки";
                mi.Click += Mi_Click;
                contextMenu.Items.Add(mi);

                Separator s = new Separator();
                contextMenu.Items.Add(s);

                mi = new MenuItem();
                mi.Name = "PostItem";
                mi.Header = "Должности";
                mi.Click += Mi_Click; ;
                contextMenu.Items.Add(mi);


                mi = new MenuItem();
                mi.Name = "ZoneItem";
                mi.Header = "Места проведения";
                mi.Click += Mi_Click; ;
                contextMenu.Items.Add(mi);

                mi = new MenuItem();
                mi.Name = "FloorItem";
                mi.Header = "Этажность";
                mi.Click += Mi_Click; ;
                contextMenu.Items.Add(mi);

                mi = new MenuItem();
                mi.Name = "AutocompleteItem";
                mi.Header = "Автозаполнение";
                mi.Click += Mi_Click; ;
                contextMenu.Items.Add(mi);

                s = new Separator();
                contextMenu.Items.Add(s);

                mi = new MenuItem();
                mi.Name = "RegroupItem";
                mi.Header = "Перегруппировка";
                mi.Click += Mi_Click; ;
                contextMenu.Items.Add(mi);


                #endregion

                calendar.DisplayDate = DateTime.Now;
                calendarBackground = new CalendarBackground(calendar);

                UseOldInterface = (bool)Options.UseOldInterface;
                ShowCompleted = (bool)Options.HideCompleted;
                ShowDeleted = (bool)Options.HideDeleted;
                ShowEmptyComments = (bool)Options.HideEmptyComments;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region Calendar Events

        private int currentMonth = 0;
        private void calendar_DisplayDateChanged(object sender, CalendarDateChangedEventArgs e)
        {
            PeriodNotesLabel.Visibility = Visibility.Collapsed;
            DayLimitLabel.Visibility = Visibility.Collapsed;
            PeriodNotesLabel1.Visibility = Visibility.Collapsed;
            DayLimitLabel1.Visibility = Visibility.Collapsed;

            if (!m_loaded) return;
            GroupList.Children.Clear();
            grpList = (List<DisplayedGroupModel>)MySqlWrapper.SelectGroupByDate(m_selectedDate).ToList<DisplayedGroupModel>();
            evtList = (List<EventGroupModel>)MySqlWrapper.SelectEventByDate(m_selectedDate).ToList<EventGroupModel>();

            DayOptions.Date = m_selectedDate;
            DayOptions.GuidCheckerMain.Date = m_selectedDate;
            DayOptions.ExpoCheckerMain.Date = m_selectedDate;                    
            int newMonth = m_selectedDate.Month;

            // Заголовки
            Dictionary<int, HeaderControl> heads = new Dictionary<int, HeaderControl>();
            int m_tourCount = 0;
            for (int i = 0; i < grpList.Count; i++)
            {
                if ((grpList[i].Groupstatus & 1) == 0)
                    DayOptions.GuidCheckerMain.Add(grpList[i].Idguid, grpList[i].Grouptime, Options.GetDuration(grpList[i].Idtour));
            }

            for (int i = 0; i < grpList.Count; i++)
            {
                int idh = grpList[i].Groupnum;
                if (!heads.ContainsKey(idh))
                {
                    HeaderControl hc = new HeaderControl(grpList[i].Idcommongroup);
                    hc.GuidComboUpdate += Hc_GuidComboUpdate;
                    heads.Add(idh, hc);
                    GroupList.Children.Add(hc);
                    hc.GroupEvClick += Hc_GroupEvClick;
                    hc.EditEvClick += Hc_EditEvClick;
                }
                heads[idh].AddGroup(grpList[i]);
                if ((grpList[i].Groupstatus & 1) > 0) continue;
                m_tourCount++;
            }

            if(evtList.Count > 0)
            {
                for (int i = 0; i < evtList.Count; i++)
                {
                    EventControl ec = new EventControl(evtList[i]);
                    ec.EditEventClick += Ec_EditEventClick;
                    ec.DeleteEventClick += Ec_DeleteEventClick;
                    ec.CompleteEventClick += Ec_CompleteEventClick;
                    GroupList.Children.Add(ec);
                }
            }

            TitleString = $"{SelectedDate.ToString("dd MMMM yyyy")} г. (Всего - {m_tourCount} экскурсий)";
            
            UpdateMissingList();

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                UpdateGuidTree();
                UpdateBackground();                
            }).Start();

            UpdateDayMessage();
        }
        private void Hc_GuidComboUpdate(object sender, EventArgs e) 
        { 
            calendar_DisplayDateChanged(this, null); 
        }
        private void calendar_DisplayModeChanged(object sender, CalendarModeChangedEventArgs e)
        {
            if ((e.NewMode == CalendarMode.Year) || (e.NewMode == CalendarMode.Decade))
                calendarBackground.ClearDates();
            calendar.Background = calendarBackground.GetBackground();
        }
        private void UpdateBackground()
        {
            // Первый день + 42 ( 6 х 7 ) дня для полной сетки календаря 

            //DateTime displaytime = calendar.DisplayDate;            
            DateTime displaytime = DateTime.Now;// = calendar.DisplayDate;
            int tmp = 0;
            CalendarMode mode = CalendarMode.Month;

            Application.Current.Dispatcher.Invoke(
                DispatcherPriority.Normal,
                    (ThreadStart)delegate { 
                        displaytime = calendar.DisplayDate;
                        tmp = (int)calendar.FirstDayOfWeek;
                        mode = calendar.DisplayMode;
                    });

            var firstdayofmonth = new DateTime(displaytime.Year, displaytime.Month, 1);
            int dayofweek = (int)firstdayofmonth.DayOfWeek;
            if (dayofweek == 0) dayofweek = 7;

            if (dayofweek == tmp) dayofweek = 8;

            if (tmp == (int)DayOfWeek.Sunday) dayofweek += 1;
            
            DateTime firstdate = firstdayofmonth.AddDays(-(Double)dayofweek + 1);
            calendarBackground.ClearDates();
            List<string> weekends = new List<string>();
            // Выходные дни
            for (int i = 0; i < 42; i++)
            {
                DateTime date = firstdate.AddDays(i);
                if (((int)date.DayOfWeek == 1) || ((int)date.DayOfWeek == 2))
                {
                    calendarBackground.AddDate(date, "cross");
                    weekends.Add(date.ToShortDateString());
                }
            }
            // Отсутствующие - добавленные            

            DateTime lastdate = firstdate.AddDays(42);

            string sql = $"select * from daysoptions where daysoptiondate >= '{firstdate.ToMySqlDateString()}' and daysoptiondate <= '{lastdate.ToMySqlDateString()}'";
            List<DaysOptionModel> monthModel = (List<DaysOptionModel>)DBWrapper.MySqlWrapper.Select(sql).ToList<DaysOptionModel>();

            for(int i = 0; i < monthModel.Count; i++)
            {
                if (monthModel[i].Workday > 0)
                {
                    if (weekends.Contains(monthModel[i].Daysoptiondate.ToShortDateString()))
                        weekends.Remove(monthModel[i].Daysoptiondate.ToShortDateString());

                    calendarBackground.AddDate(monthModel[i].Daysoptiondate, "transp");
                }
                if (monthModel[i].Workday == 0)
                {
                    if (!weekends.Contains(monthModel[i].Daysoptiondate.ToShortDateString()))
                        weekends.Add(monthModel[i].Daysoptiondate.ToShortDateString());

                    calendarBackground.AddDate(monthModel[i].Daysoptiondate, "cross");
                }

                if ((monthModel[i].Addguid == 1) && !weekends.Contains(monthModel[i].Daysoptiondate.ToShortDateString()))
                    calendarBackground.AddDate(monthModel[i].Daysoptiondate, "tjek");
            }

            if (mode == CalendarMode.Month)
                Application.Current.Dispatcher.Invoke(
                    DispatcherPriority.Normal,
                        (ThreadStart)delegate { calendar.Background = calendarBackground.GetBackground(); });
            //calendar.Background = calendarBackground.GetBackground();
        }
        private void UpdateGuidTree()
        {
            GuidTreeView.Dispatcher.Invoke((Action)(() => GuidTreeView.Foreground = Brushes.Red));
            GuidTreeView.Dispatcher.Invoke((Action)(() => GuidTreeView.Items.Clear()));            

            Dictionary<int, SortedDictionary<TimeSpan, string>> guidTreeTemplate =
                new Dictionary<int, SortedDictionary<TimeSpan, string>>();

            for(int i = 0; i < grpList.Count; i++)
            {
                if ((grpList[i].Groupstatus & 1) > 0) continue;
                int id = grpList[i].Idguid;
                if(!guidTreeTemplate.ContainsKey(id))
                {
                    SortedDictionary<TimeSpan, string> guid_tours = new SortedDictionary<TimeSpan, string>();
                    guidTreeTemplate.Add(id, guid_tours);
                }
                try
                {
                    guidTreeTemplate[id].Add(grpList[i].Grouptime, grpList[i].Tourname);
                }
                catch
                {

                }
            }

            for(int i = 0; i < evtList.Count; i++)
            {
                if ((evtList[i].Eventgroupstatus & 1) > 0) continue;
                List<int> ids = evtList[i].GetListMasters();
                string eventname = (new EventModel(evtList[i].Idevent)).Eventname;
                TimeSpan time = evtList[i].Eventgrouptime;
                for(int j = 0; j < ids.Count; j++)
                {
                    int id = ids[j];
                    if (!guidTreeTemplate.ContainsKey(id))
                    {
                        SortedDictionary<TimeSpan, string> guid_tours = new SortedDictionary<TimeSpan, string>();
                        guidTreeTemplate.Add(id, guid_tours);
                    }
                    guidTreeTemplate[id].Add(time, eventname);
                }
            }

            for(int i = 0; i < guidTreeTemplate.Count; i++)
            {
                int id = guidTreeTemplate.ElementAt(i).Key;

                TreeViewItem item = null;
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    item = new TreeViewItem();
                    item.Header = $"{(new GuidModel(id)).Guidfullname} ({guidTreeTemplate[id].Count})";
                    item.IsExpanded = true;
                    GuidTreeView.Items.Add(item);

                    for (int j = 0; j < guidTreeTemplate[id].Count; j++)
                    {
                        TimeSpan time = guidTreeTemplate[id].ElementAt(j).Key;
                        string name = guidTreeTemplate[id].ElementAt(j).Value;
                        TreeViewItem item2 = new TreeViewItem();
                        TextBlock textHeader = new TextBlock();
                        textHeader.Text = $"({time.ToString(@"hh\:mm")}) - {name}";
                        textHeader.TextWrapping = TextWrapping.Wrap;
                        textHeader.Width = GuidTreeView.ActualWidth - 40;
                        item2.Header = textHeader;//$"({time.ToString(@"hh\:mm")}) - {name}";
                        item.Items.Add(item2);
                    }
                }));
            }
        }
        private void UpdateMissingList()
        {
            //MissingPanel.Children.Clear();
            MissingPanel.Items.Clear();
            List<MissingModel> missings = (List<MissingModel>)MySqlWrapper.SelectMissingOnDate(m_selectedDate).ToList<MissingModel>();
            for (int i = 0; i < missings.Count; i++)
            {
                TreeViewItem tvi = new TreeViewItem();
                tvi.IsExpanded = true;
                tvi.Foreground = Brushes.Red;
                tvi.FontWeight = FontWeights.Bold;
                tvi.Header = Options.GetGuidName(missings[i].Idguid);
                TreeViewItem detail = new TreeViewItem();
                if (missings[i].Missingnotfullday == 1)
                    detail.Header = $"{missings[i].Missingbegin.ToLongTimeString()} - {missings[i].Missingend.ToShortTimeString()}";
                else
                    detail.Header = $"{missings[i].Missingbegin.ToShortDateString()} - {missings[i].Missingend.ToShortDateString()}";
                tvi.Items.Add(detail);
                detail = new TreeViewItem();
                TextBlock tb = new TextBlock()
                {
                    Text = missings[i].Missingcomment,
                    TextWrapping = TextWrapping.Wrap,
                    FontWeight = FontWeights.Bold,
                    Width = 250
                };
                detail.Header = tb;
                tvi.Items.Add(detail);

                #region ButtonsPanel
                ButtonsPanel panel = new ButtonsPanel(missings[i].Idmissing);
                panel.OnEditClick += Panel_OnEditClick;
                panel.OnDeleteClick += Panel_OnDeleteClick;
                detail = new TreeViewItem();
                detail.Header = panel;
                tvi.Items.Add(detail);
                #endregion

                MissingPanel.Items.Add(tvi);
            }
        }
        private void UpdateDayMessage()
        {
            List<NoteModel> notes = (List<NoteModel>)DBWrapper.MySqlWrapper.SelectNotesOnDate(m_selectedDate).ToList<NoteModel>();
            m_dayMessage = "";
            m_dayLimit = "";
            for (int i = 0; i < notes.Count; i++)
                if (notes[i].Notelimit == 1)
                {
                    DayLimitLabel.Visibility = Visibility.Visible;
                    DayLimitLabel1.Visibility = Visibility.Visible;
                    DayLimit += notes[i].Note + Environment.NewLine;
                }
                else
                {
                    PeriodNotesLabel.Visibility = Visibility.Visible;
                    PeriodNotesLabel1.Visibility = Visibility.Visible;
                    DayMessage += notes[i].Note + Environment.NewLine;
                }
        }
        #endregion

        #region Загрузка окна
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            calendar.DisplayDateChanged += calendar_DisplayDateChanged;
            calendar.DisplayModeChanged += calendar_DisplayModeChanged;
            m_loaded = true;
            calendar_DisplayDateChanged(null, null);
        }
        #endregion

        #region Bar events

        #region Вкладка дополнительно

        private void SearchGroupBt_Click(object sender, RoutedEventArgs e)
        {
            SearchGroupByOrganizationWindow searchWindow = new SearchGroupByOrganizationWindow();
            ShowFade = true;
            if ((bool)searchWindow.ShowDialog())
            {
                if(searchWindow.date != null)
                {
                    this.SelectedDate = (DateTime)searchWindow.date;
                }
            };
            ShowFade = false;
        }

        private void SearchContactBt_Click(object sender, RoutedEventArgs e)
        {
            SearchContactByOrganizationWindow searchWindow = new SearchContactByOrganizationWindow();
            ShowFade = true;
            searchWindow.ShowDialog();
            ShowFade = false;
        }
        private void EventEditBt_Click(object sender, RoutedEventArgs e)
        {
            EventEditWindow eew = new EventEditWindow();
            ShowFade = true;
            if((bool)eew.ShowDialog())
            {
                int id = eew.GId;

                string sql = $"select * from eventgroups where ideventgroup = {id}";
                DataTable tmp = MySqlWrapper.Select(sql);
                if (tmp.Rows.Count == 0)
                {
                    MessageBox.Show("Мероприятия с указанным Id не существует");
                    ShowFade = false;
                    return;
                }

                EventModifyWindow emw = new EventModifyWindow(id);
                emw.ShowDialog();
            }
            ShowFade = false;
            calendar_DisplayDateChanged(null, null);
        }

        private void DirectoryGroup_Click(object sender, RoutedEventArgs e)
        {
            GroupEditWindow wnd = new GroupEditWindow();
            ShowFade = true;
            if ((bool)wnd.ShowDialog())
            {
                int id = wnd.GId;

                string sql = $"select * from _groups where idgroup = {id}";
                DataTable tmp = MySqlWrapper.Select(sql);
                if (tmp.Rows.Count == 0)
                {
                    MessageBox.Show("Группы с указанным Id не существует");
                    ShowFade = false;
                    return;
                }

                GroupModifyWindow gmw = new GroupModifyWindow(id);
                gmw.ShowDialog();
            }
            ShowFade = false;
            calendar_DisplayDateChanged(sender, null);
        }

        private void Utils_Click(object sender, RoutedEventArgs e)
        {
            string n = (sender as RibbonButton).Name;
            switch (n)
            {
                case "RegroupBt":
                    RegroupNewWindow rw = new RegroupNewWindow(grpList);
                    ShowFade = true;
                    rw.ShowDialog();
                    ShowFade = false;
                    break;
                case "SqlBt":
                    break;
            }

            calendar_DisplayDateChanged(null, null);
        }

        private void Mi_Click1(object sender, RoutedEventArgs e)
        {
            SelectModifiedWindow smw = new SelectModifiedWindow();
            ShowFade = true;
            smw.ShowDialog();
            ShowFade = false;
            calendar_DisplayDateChanged(null, null);
        }
        #endregion

        #region Вкладка добавить

        private void StatReportButton_Click(object sender, RoutedEventArgs e)
        {
            SummaryTableWindow stw = new SummaryTableWindow();
            ShowFade = true;
            stw.ShowDialog();
            ShowFade = false;
        }

        private void Svodka_Click(object sender, RoutedEventArgs e)
        {
            SummaryTableWindow stw = new SummaryTableWindow();
            ShowFade = true;
            stw.ShowDialog();
            ShowFade = false;
        }

        private void CalendarButton_Click(object sender, RoutedEventArgs e)
        {
            CalendarWindow cw = new CalendarWindow();
            ShowFade = true;
            cw.ShowDialog();
            ShowFade = false;
        }
        private void AddEvent_Click(object sender, RoutedEventArgs e)
        {
            AddEventWindow aew = new AddEventWindow(m_selectedDate);
            ShowFade = true;
            aew.ShowDialog();
            ShowFade = false;
            calendar_DisplayDateChanged(this, null);
        }

        private void Calendar_Click(object sender, RoutedEventArgs e)
        {
            CalendarWindow cw = new CalendarWindow();
            ShowFade = true;
            cw.ShowDialog();
            ShowFade = false;
            calendar_DisplayDateChanged(this, null);
        }
        private void AddGroup_Click(object sender, RoutedEventArgs e)
        {
            List<int> groups = new List<int>();
            for (int i = 0; i < GroupList.Children.Count; i++)
            {
                if ((GroupList.Children[i] as HeaderControl) != null)
                    groups.Add(((HeaderControl)GroupList.Children[i]).GetNumGroup());
            }
            groups.Sort();

            AddGroupWindow agw = new AddGroupWindow((groups.Count > 0) ? groups[groups.Count - 1] + 1 : 1);
            ShowFade = true;
            agw.ShowDialog();
            ShowFade = false;

            calendar_DisplayDateChanged(this, null);
        }
        private void AddGroupButton_Click(object sender, RoutedEventArgs e)
        {
            List<int> groups = new List<int>();
            for (int i = 0; i < GroupList.Children.Count; i++)
            {
                if ((GroupList.Children[i] as HeaderControl) != null)
                    groups.Add(((HeaderControl)GroupList.Children[i]).GetNumGroup());
            }
            groups.Sort();

            SelectAddWindow selectAddWindow = new SelectAddWindow((groups.Count > 0) ? groups[groups.Count - 1] + 1 : 1, m_selectedDate);
            ShowFade = true;
            selectAddWindow.ShowDialog();
            ShowFade = false;
            calendar_DisplayDateChanged(this, null);
        }

        private void AddNote_Click(object sender, RoutedEventArgs e)
        {
            AddNoteWindow anw = new AddNoteWindow();
            ShowFade = true;
            anw.ShowDialog();
            ShowFade = false;
        }

        private void AddMissing_Click(object sender, RoutedEventArgs e)
        {
            AddMissingWindow amw = new AddMissingWindow();
            ShowFade = true;
            amw.ShowDialog();
            ShowFade = false;
        }
        #endregion

        #region Вкладка справочники
        private void Directory_Click(object sender, RoutedEventArgs e)
        {
            Window wnd;
            string name_ = (sender as RibbonButton).Name;
            switch(name_)
            {
                case "DirectoryPost":
                    wnd = new PostTable(); 
                    break;
                case "DirectoryZones":
                    wnd = new ZoneTable();
                    break;
                case "DirectoryAutocomplete":
                    wnd = new AutocompleteTable();
                    break;
                case "DirectoryGuid":
                    wnd = new GuidTable();
                    break;
                case "DirectoryExpo":
                    wnd = new TourTable();
                    break;
                case "DirectoryFloor":
                    wnd = new FloorTable();
                    break;
                case "DirectoryMissing":
                    wnd = new MissingTable();
                    break;
                case "DirectoryNote":
                    wnd = new NoteTable();
                    break;
                case "DirectoryEvent":
                    wnd = new EventTable();
                    break;
                default:
                    wnd = new ContactTable();
                    break;
            }

            ShowFade = true;
            wnd.ShowDialog();
            ShowFade = false;
        }
        #endregion

        #region Context Menu Table

        private void TableButton_Click(object sender, RoutedEventArgs e)
        {
            contextMenu.IsOpen = true;
        }


        private void Mi_Click(object sender, RoutedEventArgs e)
        {
            string name = (sender as MenuItem).Name;
            Window wnd = null; ;

            switch (name)
            {
                case "PostItem":
                    wnd = new PostTable();
                    break;
                case "ZoneItem":
                    wnd = new ZoneTable();
                    break;
                case "FloorItem":
                    wnd = new FloorTable();
                    break;
                case "AutocompleteItem":
                    wnd = new AutocompleteTable();
                    break;
                case "GuidItem":
                    wnd = new GuidTable();
                    break;
                case "TourItem":
                    wnd = new TourTable();
                    break;
                case "MissingItem":
                    wnd = new MissingTable();
                    break;
                case "NoteItem":
                    wnd = new NoteTable();
                    break;
                case "EventItem":
                    wnd = new EventTable();
                    break;
                case "RegroupItem":
                    wnd = new RegroupNewWindow(grpList);
                    break;
                default:
                    wnd = new ContactTable();
                    break;
            }

            ShowFade = true;
            wnd.ShowDialog();
            ShowFade = false;

            calendar_DisplayDateChanged(null, null);
        }
        #endregion

        #region Вкладка отчеты

        private void StatWiz_Click(object sender, RoutedEventArgs e)
        {
            StatWizardMainWindow stat = new StatWizardMainWindow();
            ShowFade = true;
            stat.ShowDialog();
            ShowFade = false;
        }
        private void RepFullDay_Click(object sender, RoutedEventArgs e)
        {
            FullDayReportWindow drw = new FullDayReportWindow(m_selectedDate);
            ShowFade = true;
            drw.ShowDialog();
            ShowFade = false;

        }
        private void RepDay_Click(object sender, RoutedEventArgs e)
        {
            string name = (sender as RibbonButton).Name;

            DayReportWindow drw = null; 

            if (name == RepByTime.Name)
                drw = new DayReportWindow(m_selectedDate, 1);
            if (name == RepByFloor.Name)
                drw = new DayReportWindow(m_selectedDate, 2);

            ShowFade = true;
            drw.ShowDialog();
            ShowFade = false;
        }
        private void RepPeriod_Click(object sender, RoutedEventArgs e)
        {
            int index = -1;

            if ((sender as RibbonButton).Name == RepByMonth.Name)   index = 0;
            if ((sender as RibbonButton).Name == RepByQuart.Name)   index = 1;
            if ((sender as RibbonButton).Name == RepByPeriod.Name)  index = 2;

            PeriodReportWindow prw = new PeriodReportWindow(index);
            ShowFade = true;
            prw.ShowDialog();
            ShowFade = false;
        }        
        private void ListGrp_Click(object sender, RoutedEventArgs e)
        {
            PeriodListGroupWindow plgw = new PeriodListGroupWindow();
            ShowFade = true;
            plgw.ShowDialog();
            ShowFade = false;
        }
        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            SelectReportWindow srw = new SelectReportWindow(m_selectedDate);
            ShowFade = true;
            srw.ShowDialog();
            ShowFade = false;
        }
        #endregion

        #endregion

        #region GroupEvents
        private void Hc_EditEvClick(object sender, EditEventClickArgs e)
        {
            int ng = e.NumGroup;
            List<DisplayedGroupModel> newList = new List<DisplayedGroupModel>();
            for(int i = 0; i < grpList.Count; i++)
                if (grpList[i].Groupnum == ng) newList.Add(grpList[i]);

            AddGroupWindow agw = new AddGroupWindow(newList);
            ShowFade = true;
            agw.ShowDialog();
            ShowFade = false;
            calendar_DisplayDateChanged(null, null);
        }
        private void Hc_GroupEvClick(object sender, GroupEventClickArgs e)
        {
            //MessageBox.Show(e.EventType + "  :  " + e.IdGroup.ToString());
            Window wnd = null;
            switch(e.EventType)
            {
                case "delete":
                    wnd = new GroupDeleteWindow(e.IdGroup);
                    break;
                case "complete":
                    wnd = new GroupCompleteWindow(e.IdGroup);
                    break;
                case "edit":
                    wnd = new GroupEditWIndow_Main(e.IdGroup);
                    break;
            }

            ShowFade = true;
            wnd.ShowDialog();
            ShowFade = false;
            calendar_DisplayDateChanged(sender, null);
        }
        private void Ec_CompleteEventClick(object sender, EventEvent e)
        {
            EventCompleteWindow ecw = new EventCompleteWindow(e.idevent);
            ShowFade = true;
            ecw.ShowDialog();
            ShowFade = false;
            calendar_DisplayDateChanged(sender, null);
        }
        private void Ec_DeleteEventClick(object sender, EventEvent e)
        {
            EventDeleteWindow edw = new EventDeleteWindow(e.idevent);
            ShowFade = true;
            edw.ShowDialog();
            ShowFade = false;
            calendar_DisplayDateChanged(sender, null);
        }
        private void Ec_EditEventClick(object sender, EventEvent e)
        {
            AddEventWindow aew = new AddEventWindow(e.idevent);
            ShowFade = true;
            aew.ShowDialog();
            ShowFade = false;
            calendar_DisplayDateChanged(sender, null);
        }
        #endregion

        #region MissingEvents
        private void Panel_OnDeleteClick(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы уверены что хотите удалить запись?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                int id = (int)((sender as Button).CommandParameter);
                string sql = $"delete from missings where idmissing = {id}";
                DBWrapper.MySqlWrapper.Execute(sql);
            }
            calendar_DisplayDateChanged(this, null);
        }

        private void Panel_OnEditClick(object sender, EventArgs e)
        {
            int id = (int)((sender as Button).CommandParameter);
            MissingEdit me = new MissingEdit(id);
            ShowFade = true;
            me.ShowDialog();
            ShowFade = false;
            calendar_DisplayDateChanged(this, null);
        }

        #endregion
    }
}
