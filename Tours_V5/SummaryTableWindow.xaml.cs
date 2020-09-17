using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ClassLibrary;
using ClassLibrary.Models;

namespace Tours_V5
{
    /// <summary>
    /// Логика взаимодействия для SummaryTableWindow.xaml
    /// </summary>
    public partial class SummaryTableWindow : Window, INotifyPropertyChanged
    {
        #region Members        

        private DateTime m_StartTime;
        private DateTime m_EndTime;

        List<DisplayedGroupModel> models = null;
        List<EventGroupModel> ev_models = null;

        private bool m_ShowCompleted;
        private bool m_ShowTours;
        private bool m_ShowEvents;

        public DateTime StartTime
        {
            get => m_StartTime;
            set
            {
                m_StartTime = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StartTime"));
            }
        }
        public DateTime EndTime
        {
            get => m_EndTime;
            set
            {
                m_EndTime = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EndTime"));
            }
        }
        public bool ShowCompleted
        {
            get => m_ShowCompleted;
            set
            {
                m_ShowCompleted = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowCompleted"));
            }
        }
        public bool ShowTours
        {
            get => m_ShowTours;
            set
            {
                m_ShowTours = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowTours"));
            }
        }
        public bool ShowEvents
        {
            get => m_ShowEvents;
            set
            {
                m_ShowEvents = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowEvents"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructor
        public SummaryTableWindow()
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

        private void ShowBt_Click(object sender, RoutedEventArgs e)
        {
            GuidListBox.Items.Clear();
            TourDetailTreeView.Items.Clear();
            DetailGuidTreeView.Items.Clear();
            MissTreeView.Items.Clear();

            #region Расчет Экскурсий
            if (ShowTours)
            {
                string sql = $"select * from allgroup where groupdate between '{StartTime.ToMySqlDateString()}' and '{EndTime.ToMySqlDateString()}'";

                models = (List<DisplayedGroupModel>)DBWrapper.MySqlWrapper.Select(sql).ToList<DisplayedGroupModel>();

                int total_count = models.Count;
                int completed_count = 0;
                int deleted_count = 0;
                int noncompleted = 0;

                SortedDictionary<int, TourCounter> TourCounters = new SortedDictionary<int, TourCounter>();

                for (int i = 0; i < total_count; i++)
                {
                    int idTour = models[i].Idtour;
                    if (!TourCounters.ContainsKey(idTour))
                        TourCounters.Add(idTour,
                            new TourCounter()
                            {
                                Name = models[i].Tourname,
                                ID = models[i].Idtour
                            });

                    if ((models[i].Groupstatus & 1) == 1)
                    {
                        deleted_count++;
                        TourCounters[idTour].DeletedCount++;
                    }
                    if ((models[i].Groupstatus & 4) == 4)
                    {
                        completed_count++;
                        TourCounters[idTour].CompletedCount++;
                    }
                    if ((models[i].Groupstatus & 1) == 0 && (models[i].Groupstatus & 4) == 0)
                    {
                        noncompleted++;
                        TourCounters[idTour].NonCompletedCount++;
                    }
                }

                TotalLabel.Content = total_count.ToString();
                CompletedLabel.Content = completed_count.ToString();
                DeletedLabel.Content = deleted_count.ToString();
                NonCompletedLabel.Content = noncompleted.ToString();

                TreeViewItem tviMain = new TreeViewItem();
                tviMain.IsExpanded = true;
                tviMain.Header = "Экскурсии";
                TourDetailTreeView.Items.Add(tviMain);

                for (int i = 0; i < TourCounters.Count; i++)
                {
                    TreeViewItem tviHeader = new TreeViewItem();
                    TourCounter tc = TourCounters.ElementAt(i).Value;
                    tviHeader.FontWeight = FontWeights.SemiBold;

                    TextBlock tbHeaderExpo = new TextBlock();
                    tbHeaderExpo.TextWrapping = TextWrapping.Wrap;
                    tbHeaderExpo.Text = tc.Name + $" (Всего - {tc.CompletedCount + tc.DeletedCount + tc.NonCompletedCount})";
                    tbHeaderExpo.Width = TourDetailTreeView.ActualWidth - 40;
                    tviHeader.Header = tbHeaderExpo;

                    TreeViewItem tmp;

                    if (tc.CompletedCount > 0)
                    {
                        tmp = new TreeViewItem();
                        tmp.Header = $"Проведено: {tc.CompletedCount}";
                        tmp.FontWeight = FontWeights.Normal;
                        tviHeader.Items.Add(tmp);
                    }

                    if (tc.DeletedCount > 0)
                    {
                        tmp = new TreeViewItem();
                        tmp.Header = $"Отменено: {tc.DeletedCount}";
                        tmp.FontWeight = FontWeights.Normal;
                        tviHeader.Items.Add(tmp);
                    }

                    if (tc.NonCompletedCount > 0)
                    {
                        tmp = new TreeViewItem();
                        tmp.Header = $"Не проведено: {tc.NonCompletedCount}";
                        tmp.FontWeight = FontWeights.Normal;
                        tviHeader.Items.Add(tmp);
                    }

                    tviHeader.IsExpanded = false;
                    tviMain.Items.Add(tviHeader);
                }
            }
            else
            {
                TourGroupBox.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region Расчет мероприятий
            if (ShowEvents)
            {
                string sqlEvent = $"select * from eventgroups where eventgroupdate between '{StartTime.ToMySqlDateString()}' and '{EndTime.ToMySqlDateString()}'";
                ev_models = (List<EventGroupModel>)DBWrapper.MySqlWrapper.Select(sqlEvent).ToList<EventGroupModel>();

                int ev_total_count = ev_models.Count;
                int ev_completed_count = 0;
                int ev_deleted_count = 0;
                int ev_noncompleted_count = 0;

                SortedDictionary<int, EventCounter> EventCounters = new SortedDictionary<int, EventCounter>();

                for (int i = 0; i < ev_total_count; i++)
                {
                    int id = ev_models[i].Idevent;
                    if (!EventCounters.ContainsKey(id))
                    {
                        EventCounter ec = new EventCounter();
                        EventModel em = new EventModel(id);
                        ec.ID = id;
                        ec.Name = em.Eventname;
                        switch (em.Eventtype)
                        {
                            case "КВ": ec.Type = "Квест: "; break;
                            case "МК": ec.Type = "Мастер-класс: "; break;
                            case "Л": ec.Type = "Лекция: "; break;
                        }
                        EventCounters.Add(id, ec);
                    }

                    if ((ev_models[i].Eventgroupstatus & 1) == 1)
                    {
                        ev_deleted_count++;
                        EventCounters[id].DeletedCount++;
                    }
                    if ((ev_models[i].Eventgroupstatus & 4) == 4)
                    {
                        ev_completed_count++;
                        EventCounters[id].CompletedCount++;
                    }
                    if ((ev_models[i].Eventgroupstatus & 1) == 0 && (ev_models[i].Eventgroupstatus & 4) == 0)
                    {
                        ev_noncompleted_count++;
                        EventCounters[id].NonCompletedCount++;
                    }
                }

                TotalEventsLabel.Content = ev_total_count;
                CompletedEventsLabel.Content = ev_completed_count;
                DeletedEventsLabel.Content = ev_deleted_count;
                NonCompletedEventsLabel.Content = ev_noncompleted_count;

                TreeViewItem tviMain = new TreeViewItem();
                tviMain.IsExpanded = true;
                tviMain.Header = "Мероприятия";
                TourDetailTreeView.Items.Add(tviMain);

                for (int i = 0; i < EventCounters.Count; i++)
                {
                    TreeViewItem tviHeader = new TreeViewItem();
                    EventCounter tc = EventCounters.ElementAt(i).Value;
                    tviHeader.FontWeight = FontWeights.SemiBold;
                    tviHeader.Header = tc.Type + tc.Name + $" (Всего - {tc.CompletedCount + tc.DeletedCount + tc.NonCompletedCount})";

                    TreeViewItem tmp;
                    if (tc.CompletedCount > 0)
                    {
                        tmp = new TreeViewItem();
                        tmp.Header = $"Проведено: {tc.CompletedCount}";
                        tmp.FontWeight = FontWeights.Normal;
                        tviHeader.Items.Add(tmp);
                    }

                    if (tc.DeletedCount > 0)
                    {
                        tmp = new TreeViewItem();
                        tmp.Header = $"Отменено: {tc.DeletedCount}";
                        tmp.FontWeight = FontWeights.Normal;
                        tviHeader.Items.Add(tmp);
                    }

                    if (tc.NonCompletedCount > 0)
                    {
                        tmp = new TreeViewItem();
                        tmp.Header = $"Не проведено: {tc.NonCompletedCount}";
                        tmp.FontWeight = FontWeights.Normal;
                        tviHeader.Items.Add(tmp);
                    }


                    tviHeader.IsExpanded = false;
                    tviMain.Items.Add(tviHeader);
                }
            }
            else
            {
                EventsGroupBox.Visibility = Visibility.Collapsed;
            }
            #endregion

            List<int> guids = new List<int>();

            for (int i = 0; i < models.Count; i++)
            {
                int id = models[i].Idguid;
                if (!guids.Contains(id))
                {
                    RadioButton rb = new RadioButton()
                    {
                        Content = Options.GetGuidName(id),
                        Tag = id,
                        GroupName = "guids",
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    rb.Click += Rb_Click; ;
                    guids.Add(id);
                    GuidListBox.Items.Add(rb);
                }
            }

            if (ShowEvents)
            {
                for (int i = 0; i < ev_models.Count; i++)
                {
                    List<int> ids = ev_models[i].GetListMasters();
                    for (int j = 0; j < ids.Count; j++)
                    {
                        int iid = ids[j];
                        if (!guids.Contains(iid))
                        {
                            RadioButton rb = new RadioButton()
                            {
                                Content = Options.GetGuidName(iid),
                                Tag = iid,
                                GroupName = "guids",
                                VerticalAlignment = VerticalAlignment.Center
                            };
                            rb.Click += Rb_Click; ;
                            guids.Add(iid);
                            GuidListBox.Items.Add(rb);
                        }
                    }
                }
            }
        }

        private void Rb_Click(object sender, RoutedEventArgs e)
        {
            DetailGuidTreeView.Items.Clear();
            MissTreeView.Items.Clear();
            SortedDictionary<int, TourCounter> TourCounters = new SortedDictionary<int, TourCounter>();
            int id = (int)(sender as RadioButton).Tag;
            
            int completed_count = 0;
            int deleted_count = 0;
            int noncompleted = 0;

            int ev_completed_count = 0;
            int ev_deleted_count = 0;
            int ev_noncompleted_count = 0;

            #region Расчет экскурсий для экскурсовода
            if (ShowTours)
            {
                for (int i = 0; i < models.Count; i++)
                {
                    int idguid = models[i].Idguid;
                    if (idguid != id) continue;

                    int idTour = models[i].Idtour;
                    if (!TourCounters.ContainsKey(idTour))
                        TourCounters.Add(idTour,
                            new TourCounter()
                            {
                                Name = models[i].Tourname,
                                ID = models[i].Idtour
                            });

                    if ((models[i].Groupstatus & 1) == 1)
                    {
                        deleted_count++;
                        TourCounters[idTour].DeletedCount++;
                    }
                    if ((models[i].Groupstatus & 4) == 4)
                    {
                        completed_count++;
                        TourCounters[idTour].CompletedCount++;
                    }
                    if ((models[i].Groupstatus & 1) == 0 && (models[i].Groupstatus & 4) == 0)
                    {
                        noncompleted++;
                        TourCounters[idTour].NonCompletedCount++;
                    }
                }

                TreeViewItem tviMain = new TreeViewItem();
                tviMain.IsExpanded = true;
                tviMain.Header = "Экскурсии";
                DetailGuidTreeView.Items.Add(tviMain);

                for (int i = 0; i < TourCounters.Count; i++)
                {
                    TreeViewItem tviHeader = new TreeViewItem();
                    TourCounter tc = TourCounters.ElementAt(i).Value;
                    tviHeader.FontWeight = FontWeights.SemiBold;
                    tviHeader.Header = tc.Name + $" (Всего - {tc.CompletedCount + tc.DeletedCount + tc.NonCompletedCount})";

                    TreeViewItem tmp;

                    if (tc.CompletedCount > 0)
                    {
                        tmp = new TreeViewItem();
                        tmp.Header = $"Проведено: {tc.CompletedCount}";
                        tmp.FontWeight = FontWeights.Normal;
                        tviHeader.Items.Add(tmp);
                    }

                    if (tc.DeletedCount > 0)
                    {
                        tmp = new TreeViewItem();
                        tmp.Header = $"Отменено: {tc.DeletedCount}";
                        tmp.FontWeight = FontWeights.Normal;
                        tviHeader.Items.Add(tmp);
                    }

                    if (tc.NonCompletedCount > 0)
                    {
                        tmp = new TreeViewItem();
                        tmp.Header = $"Не проведено: {tc.NonCompletedCount}";
                        tmp.FontWeight = FontWeights.Normal;
                        tviHeader.Items.Add(tmp);
                    }

                    tviHeader.IsExpanded = false;
                    tviMain.Items.Add(tviHeader);
                }
            }
            #endregion

            #region Расчет мероприятий для экскурсовода
            if(ShowEvents)
            {
                SortedDictionary<int, EventCounter> EventCounters = new SortedDictionary<int, EventCounter>();

                for (int i = 0; i < ev_models.Count; i++)
                {
                    List<int> ids = ev_models[i].GetListMasters();
                    if (!ids.Contains(id)) continue;

                    int idev = ev_models[i].Idevent;
                    if (!EventCounters.ContainsKey(idev))
                    {
                        EventCounter ec = new EventCounter();
                        EventModel em = new EventModel(idev);
                        ec.ID = idev;
                        ec.Name = em.Eventname;
                        switch (em.Eventtype)
                        {
                            case "КВ": ec.Type = "Квест: "; break;
                            case "МК": ec.Type = "Мастер-класс: "; break;
                            case "Л": ec.Type = "Лекция: "; break;
                        }
                        EventCounters.Add(idev, ec);
                    }

                    if ((ev_models[i].Eventgroupstatus & 1) == 1)
                    {
                        ev_deleted_count++;
                        EventCounters[idev].DeletedCount++;
                    }
                    if ((ev_models[i].Eventgroupstatus & 4) == 4)
                    {
                        ev_completed_count++;
                        EventCounters[idev].CompletedCount++;
                    }
                    if ((ev_models[i].Eventgroupstatus & 1) == 0 && (ev_models[i].Eventgroupstatus & 4) == 0)
                    {
                        ev_noncompleted_count++;
                        EventCounters[idev].NonCompletedCount++;
                    }
                }

                TreeViewItem tviMain = new TreeViewItem();
                tviMain.IsExpanded = true;
                tviMain.Header = "Мероприятия";
                DetailGuidTreeView.Items.Add(tviMain);

                for (int i = 0; i < EventCounters.Count; i++)
                {
                    TreeViewItem tviHeader = new TreeViewItem();
                    EventCounter tc = EventCounters.ElementAt(i).Value;
                    tviHeader.FontWeight = FontWeights.SemiBold;
                    tviHeader.Header = tc.Type + tc.Name + $" (Всего - {tc.CompletedCount + tc.DeletedCount + tc.NonCompletedCount})";

                    TreeViewItem tmp;

                    if (tc.CompletedCount > 0)
                    {
                        tmp = new TreeViewItem();
                        tmp.Header = $"Проведено: {tc.CompletedCount}";
                        tmp.FontWeight = FontWeights.Normal;
                        tviHeader.Items.Add(tmp);
                    }

                    if (tc.DeletedCount > 0)
                    {
                        tmp = new TreeViewItem();
                        tmp.Header = $"Отменено: {tc.DeletedCount}";
                        tmp.FontWeight = FontWeights.Normal;
                        tviHeader.Items.Add(tmp);
                    }

                    if (tc.NonCompletedCount > 0)
                    {
                        tmp = new TreeViewItem();
                        tmp.Header = $"Не проведено: {tc.NonCompletedCount}";
                        tmp.FontWeight = FontWeights.Normal;
                        tviHeader.Items.Add(tmp);
                    }


                    tviHeader.IsExpanded = false;
                    tviMain.Items.Add(tviHeader);
                }
            }
            #endregion

            TotalGuidLabel.Content = completed_count + deleted_count + noncompleted +
                ev_completed_count + ev_deleted_count + ev_noncompleted_count;
            CompletedGuidLabel.Content = completed_count + ev_completed_count;
            DeletedGuidLabel.Content = deleted_count + ev_deleted_count;
            NonCompletedGuidLabel.Content = noncompleted + ev_noncompleted_count;

            #region Отсутствие экскурсовода
            string misssql = $"select * from missings where not missingnotfullday and idguid = {id} and missingbegin " +
                $"between '{StartTime.ToMySqlDateString()}' and '{EndTime.ToMySqlDateString()}'";

            List<MissingModel> miss_models = (List<MissingModel>)DBWrapper.MySqlWrapper.Select(misssql)
                .ToList<MissingModel>();

            for(int i = 0; i < miss_models.Count; i++)
            {
                TreeViewItem tvHeader = new TreeViewItem();
                tvHeader.Header = $"{miss_models[i].Missingbegin.ToString("dd.MM.yyyy")}-{miss_models[i].Missingend.ToString("dd.MM.yyyy")}";
                TreeViewItem tvDetail = new TreeViewItem();
                tvDetail.Header = $"{miss_models[i].Missingcomment}";
                tvHeader.Items.Add(tvDetail);
                MissTreeView.Items.Add(tvHeader);

                if (miss_models[i].Missingcomment.ToUpper().Contains("БОЛЬ"))
                    tvHeader.Foreground = Brushes.Red;
            }
            #endregion
        }
        #endregion
    }
    public class TourCounter
    {
        public int ID { get; set; } = 0;
        public string Name { get; set; } = "";
        public int CompletedCount { get; set; } = 0;
        public int DeletedCount { get; set; } = 0;
        public int NonCompletedCount { get; set; } = 0;

        public TourCounter() { }
    }
    public class EventCounter
    {
        public int ID { get; set; } = 0;
        public string Type { get; set; } = "";
        public string Name { get; set; } = "";
        public int CompletedCount { get; set; } = 0;
        public int DeletedCount { get; set; } = 0;
        public int NonCompletedCount { get; set; } = 0;

        public EventCounter() { }
    }   

}
