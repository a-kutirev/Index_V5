using ClassLibrary;
using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using WpfControlLibrary;

namespace Tours_V5
{
    /// <summary>
    /// Логика взаимодействия для AddEventWindow.xaml
    /// </summary>
    public partial class AddEventWindow : Window, INotifyPropertyChanged
    {
        #region Members

        private EventGroupModel m_model;
        private DateTime m_date;
        private string m_orgName;
        private string m_geoName;
        private string m_evName;
        private string masters;

        public EventGroupModel Model
        {
            get => m_model;
            set
            {
                m_model = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Model"));
            }
        }

        public string EvName
        {
            get => m_evName;
            set
            {
                m_evName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EvName"));
            }
        }

        public string OrgName
        {
            get => m_orgName;
            set
            {
                m_orgName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OrgName"));
            }
        }
        public string GeoName
        {
            get => m_geoName;
            set
            {
                m_geoName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GeoName"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructor

        public AddEventWindow(int id)
        {
            InitializeComponent();
            this.DataContext = this;

            Model = new EventGroupModel(id);

            OrgName = (new OrganizationModel(Model.Idorganization)).Organizationname;
            GeoName = (new GeoModel(Model.Idgeo)).Geoname;

            m_date = Model.Eventgroupdate;
            timePicker.Date = Model.Eventgroupdate;
            timePicker.Time = Model.Eventgrouptime;

            EvName = new EventModel(Model.Idevent).Eventname;

            List<int> mastersId = Model.GetListMasters();
            mastersListBox.Items.Clear();
            masters = Model.Eventgroupmaster;
            for (int i = 0; i < mastersId.Count; i++)
            {
                GuidModel gm = new GuidModel(mastersId[i]);
                TextBlock tb = new TextBlock();
                tb.Text = gm.Guidfullname;                
                tb.Tag = gm.Idguid;
                mastersListBox.Items.Add(tb);
                SetComment(Model.Eventgroupcomment);
            }
        }

        public AddEventWindow(DateTime dateTime)
        {
            InitializeComponent();
            this.DataContext = this;

            m_date = dateTime;
            timePicker.Date = dateTime;
            

            Model = new EventGroupModel();
            Model.Idorganization = 181;
            Model.Idgeo = 5;
            Model.Eventgroupdate = dateTime;

            OrgName = (new OrganizationModel(Model.Idorganization)).Organizationname;
            GeoName = (new GeoModel(Model.Idgeo)).Geoname;
        }
        #endregion

        #region Events
        private void SaveBt_Click(object sender, RoutedEventArgs e)
        {
            Model.Eventgrouptime = new TimeSpan(timePicker.HourVal, timePicker.MinuteVal, 0);
            Model.Eventgroupcomment = GetRtfComment();
            Model.Eventgroupmaster = masters;

            if(Model.Idevent == 0)
            {
                MessageBox.Show("Не выбрано мероприятие");
                return;
            }
            if (Model.ideventgroup == 0)
            {
                Model.Insert();
            }
            else
            {
                Model.Update();
            }
            DialogResult = true;
        }
        #endregion

        // SelectEventWindow
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SelectEventWindow sew = new SelectEventWindow();
            if((bool)sew.ShowDialog())
            {
                EvName = sew.Event;
                Model.Idevent = sew.Id;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SelectEventMasters sem;
            if (mastersListBox.Items.Count == 0)
                sem = new SelectEventMasters(m_date);
            else
            {
                Dictionary<int, string> tmp = new Dictionary<int, string>();
                for(int i = 0; i < mastersListBox.Items.Count; i++)
                {
                    TextBlock tb = mastersListBox.Items[i] as TextBlock;
                    int id = (int)tb.Tag;
                    string txt = (string)tb.Text;

                    tmp.Add(id, txt);
                }

                sem = new SelectEventMasters(m_date, tmp);
            }
            if((bool)sem.ShowDialog())
            {
                Dictionary<int, string> tmp = sem.MastersLists;
                mastersListBox.Items.Clear();

                masters = "";
                for (int j = 0; j < tmp.Count; j++)
                {
                    TextBlock tb = new TextBlock();
                    tb.Text = tmp.ElementAt(j).Value.ToString();
                    tb.Tag = (int)tmp.ElementAt(j).Key;
                    masters += tmp.ElementAt(j).Key.ToString();
                    if (j != (tmp.Count - 1)) masters += "#";
                    mastersListBox.Items.Add(tb);
                }
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            CommentWindow cw = new CommentWindow();
            cw.Rtf = this.GetRtfComment();
            if ((bool)cw.ShowDialog())
            {
                this.SetComment(cw.Rtf);
            }
        }

        #region RTF комментарии
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
                RichTextBox tip = new RichTextBox();
                tip.IsEnabled = false;
                tip.FontSize = 16;
                tip.Width = 300;
                range = new TextRange(tip.Document.ContentStart, tip.Document.ContentEnd);
                range.Load(ms, DataFormats.Rtf);
                ToolTip tt = new ToolTip();
                tt.Content = tip;
                CommentRTB.ToolTip = tt;
                string richText = new TextRange(CommentRTB.Document.ContentStart, CommentRTB.Document.ContentEnd).Text;
            }
            catch (Exception)
            {

            }
        }
        #endregion

        #region Выборка организации и географии
        private void SelectOrgBt_Click(object sender, RoutedEventArgs e)
        {
            string name = (sender as Button).Name;
            Select_Org_Geo sog;
            switch (name)
            {
                case "SelectOrgBt":
                    sog = new Select_Org_Geo(SelectWindowType.Organization);
                    if ((bool)sog.ShowDialog())
                    {
                        Model.Idorganization = sog.Id;
                        OrgName = (new OrganizationModel(Model.Idorganization)).Organizationname;
                    }
                    break;
                case "SelectGeogBt":
                    sog = new Select_Org_Geo(SelectWindowType.Geography);
                    if ((bool)sog.ShowDialog())
                    {
                        Model.Idgeo = sog.Id;
                        GeoName = (new GeoModel(Model.Idgeo)).Geoname;                        
                    }
                    break;
                default:
                    sog = new Select_Org_Geo(SelectWindowType.Organization);
                    break;
            }
        }
        #endregion

        private void AddContactBt_Click(object sender, RoutedEventArgs e)
        {
            AddEventContactWindow aecw
                = new AddEventContactWindow(Model);

            aecw.ShowDialog();
        }
    }
}
