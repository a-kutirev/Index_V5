using ClassLibrary;
using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace WpfControlLibrary.GroupControls
{
    /// <summary>
    /// Логика взаимодействия для EventControl.xaml
    /// </summary>
    public partial class EventControl : UserControl, INotifyPropertyChanged
    {
        #region Members
        private string m_guids;
        private string m_eventTime;
        private string m_orgGeo;
        private string m_contacts;
        private EventGroupModel m_model;
        private EventModel m_evModel;

        private string m_time;

        public EventGroupModel Model
        {
            get => m_model;
            set
            {
                m_model = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Model"));
            }
        }

        public string Time
        {
            get => m_time;
            set
            {
                m_time = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Time"));
            }
        }

        public EventModel EvModel
        {
            get => m_evModel;
            set
            {
                m_evModel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EvModel"));
            }
        }

        public string Guids
        {
            get => m_guids;
            set
            {
                m_guids = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Guids"));
            }
        }

        public string EventTime
        {
            get => m_eventTime;
            set
            {
                m_eventTime = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EventTime")) ;
            }
        }

        public string OrgGeo
        {
            get => m_orgGeo;
            set
            {
                m_orgGeo = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OrgGeo"));
            }
        }

        public string Contacts
        {
            get => m_contacts;
            set
            {
                m_contacts = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Contacts"));
            }
        }

        public event EventHandler<EventEvent> DeleteEventClick;
        public event EventHandler<EventEvent> EditEventClick;
        public event EventHandler<EventEvent> CompleteEventClick;

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructor
        public EventControl(EventGroupModel m)
        {
            InitializeComponent();
            Margin = new Thickness(20, 6, 25, 0);
            this.DataContext = this;
            Model = m;
            EventTime = Model.Eventgrouptime.ToString("hh\\:mm");
            EvModel = new EventModel(m_model.Idevent);            
            SetComment(Model.Eventgroupcomment);

            OrgGeo = $"{(new OrganizationModel(m.Idorganization)).Organizationname} ({(new GeoModel(m.Idgeo)).Geoname})";

            Guids = Options.GetGuidStringByIds(Model.GetListMasters());
            ee = new EventEvent();
            ee.idevent = Model.ideventgroup;

            TextBlock tb = new TextBlock();
            tb.FontSize = 14;
            tb.FontWeight = FontWeights.Bold;
            tb.Text = "id = " + Model.ideventgroup.ToString();
            InfoLabel.ToolTip = tb;

            if ((Model.Eventgroupstatus & 1) > 0)
            {
                if ((bool)Options.HideDeleted) this.Visibility = Visibility.Collapsed;
                LinearGradientBrush lgb = new LinearGradientBrush();
                lgb.StartPoint = new Point(0.5, 0);
                lgb.EndPoint = new Point(0.5, 1);
                GradientStop gs = new GradientStop(Colors.Salmon, 0);
                lgb.GradientStops.Add(gs);
                gs = new GradientStop((Color)ColorConverter.ConvertFromString("#FFFFD3D3"), 0.1);
                lgb.GradientStops.Add(gs);
                gs = new GradientStop((Color)ColorConverter.ConvertFromString("#FFFFD3D3"), 0.9);
                lgb.GradientStops.Add(gs);
                gs = new GradientStop(Colors.White, 1);
                lgb.GradientStops.Add(gs);
                brd_head.Background = lgb;
                brd_bodi.Background = lgb;

                string tt = $"id = {Model.ideventgroup}{Environment.NewLine}{Model.Eventgroupdeletereason}";
                tb = new TextBlock();
                tb.FontSize = 14;
                tb.FontWeight = FontWeights.Bold;
                tb.Text = tt;
                InfoLabel.ToolTip = tb;

                DisableButton();
            }

            if ((Model.Eventgroupstatus & 4) > 0)
            {
                if ((bool)Options.HideCompleted) this.Visibility = Visibility.Collapsed;
                LinearGradientBrush lgb = new LinearGradientBrush();
                lgb.StartPoint = new Point(0.5, 0);
                lgb.EndPoint = new Point(0.5, 1);
                GradientStop gs = new GradientStop(Colors.LightGreen, 0);
                lgb.GradientStops.Add(gs);
                gs = new GradientStop((Color)ColorConverter.ConvertFromString("#FFD3FFD5"), 0.1);
                lgb.GradientStops.Add(gs);
                gs = new GradientStop((Color)ColorConverter.ConvertFromString("#FFD3FFD5"), 0.9);
                lgb.GradientStops.Add(gs);
                gs = new GradientStop(Colors.White, 1);
                lgb.GradientStops.Add(gs);
                brd_head.Background = lgb;
                brd_bodi.Background = lgb;

                string tt = $"id = {Model.ideventgroup}{Environment.NewLine}{Options.GetCategName(Model.Idcategory)}";
                List<string> opt = Options.GetStatisticsByCode(Model.Eventgroupstat);
                for (int i = 0; i < opt.Count; i++)
                    tt += Environment.NewLine + opt[i];

                tb = new TextBlock();
                tb.FontSize = 14;
                tb.FontWeight = FontWeights.Bold;
                tb.Text = tt;
                InfoLabel.ToolTip = tb;

                DisableButton();
            }

            string sql = $"select * from eventgroup_contacts where ideventgroup = {Model.ideventgroup}";
            DataTable tmp = DBWrapper.MySqlWrapper.Select(sql);
            Contacts = "";
            for(int i = 0; i < tmp.Rows.Count; i++)
            {
                int idcontact = int.Parse(tmp.Rows[i]["idcontact"].ToString());
                ContactModel cm = new ContactModel(idcontact);

                Contacts += $"{cm.Contactname} ({cm.Contactpost}) {cm.Contactphone}; ";
            }
        }

        private void DisableButton()
        {
            EditBt.IsEnabled = false;
            CompleteBt.IsEnabled = false;
            DeleteBt.IsEnabled = false;
        }

        EventEvent ee;
        #endregion

        #region RTF COmment
        public string GetRtfComment()
        {
            string rtfText; //string to save to db
            TextRange tr = new TextRange(CommentRTB.Document.ContentStart, CommentRTB.Document.ContentEnd);
            using (MemoryStream ms = new MemoryStream())
            {
                tr.Save(ms, DataFormats.Rtf);
                rtfText = Encoding.ASCII.GetString(ms.ToArray());
            }
            string ddd = rtfText;
            //rtfText = rtfText.Replace("\"", "'");
            return rtfText.Replace("\\", "#");
        }

        public void SetComment(string text)
        {
            try
            {
                string formattedText = text
                    .Replace("#fs18#", "#fs22#")
                    .Replace("#fs19#", "#fs22#")
                    .Replace("#fs20#", "#fs22#")
                    .Replace("#fs21#", "#fs22#")
                    .Replace("#fs23#", "#fs22#")
                    .Replace("#fs24#", "#fs22#")
                    .Replace("#fs25#", "#fs22#")
                    .Replace("#fs26#", "#fs22#")
                    .Replace("#fs27#", "#fs22#")
                    .Replace("#fs28#", "#fs22#")
                    .Replace("#fs29#", "#fs22#")
                    .Replace("#fs30#", "#fs22#")
                    .Replace("#fs31#", "#fs22#")
                    .Replace("#fs32#", "#fs22#")
                    .Replace("#fs33#", "#fs22#")
                    .Replace("#fs34#", "#fs22#")
                    .Replace("#fs35#", "#fs22#")
                    .Replace("#fs36#", "#fs22#")
                    .Replace("#fs37#", "#fs22#")
                    .Replace("#fs38#", "#fs22#")
                    .Replace("#fs39#", "#fs22#")
                    .Replace("#", "\\");

                TextRange range = new TextRange(CommentRTB.Document.ContentStart, CommentRTB.Document.ContentEnd);
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(formattedText));
                range.Load(ms, DataFormats.Rtf);
                RichTextBox tip = new RichTextBox()
                {
                    IsEnabled = false,
                    FontSize = 16,
                    Width = 300
                };
                range = new TextRange(tip.Document.ContentStart, tip.Document.ContentEnd);
                range.Load(ms, DataFormats.Rtf);
                ToolTip tt = new ToolTip();
                tt.Content = tip;
                CommentRTB.ToolTip = tt;
            }
            catch (Exception)
            {

            }
        }
        #endregion

        #region Events

        private void EditBt_Click(object sender, RoutedEventArgs e) =>
            EditEventClick?.Invoke(this, ee);

        private void DeleteBt_Click(object sender, RoutedEventArgs e) =>
            DeleteEventClick?.Invoke(this, ee);

        private void CompleteBt_Click(object sender, RoutedEventArgs e)=>
            CompleteEventClick?.Invoke(this, ee);

        #endregion
    }

    public class EventEvent : EventArgs
    {
        public int idevent;
    }
}
