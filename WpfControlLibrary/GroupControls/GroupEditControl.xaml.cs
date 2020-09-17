using ClassLibrary;
using DBWrapper;
using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace WpfControlLibrary.GroupControls
{
    /// <summary>
    /// Логика взаимодействия для GroupEditControl.xaml
    /// </summary>
    public partial class GroupEditControl : UserControl, INotifyPropertyChanged
    {
        #region Members Fields

        public event EventHandler<AddGroupEventArgs> AddGroupOnClick;
        public event EventHandler RemoveGroupOnClick;

        private string m_Amount = "";
        private string m_Age = "";
        private DataView m_HourData;
        private DataView m_MinuteData;
        private DataView m_ToursView;
        private int m_hours;
        private int m_minute;

        private bool loaded = false;
        private bool m_DisableRemoveBt = true;
        private bool m_EnableAddBt = true;
        private GroupModel m_model;

        private BitmapImage error, ok;

        public int GroupNum { get; set; }

        public DataView HourData
        {
            get => m_HourData;
            set
            {
                m_HourData = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HourData"));
            }
        }

        public DataView MinuteData
        {
            get => m_MinuteData;
            set
            {
                m_MinuteData = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MinuteData"));
            }
        }

        public DataView ToursView
        {
            get => m_ToursView;
            set
            {
                m_ToursView = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ToursView"));
            }
        }

        public bool EnableRemoveBt
        {
            get => m_DisableRemoveBt;
            set
            {
                m_DisableRemoveBt = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnableRemoveBt"));
            }
        }

        public bool EnableAddBt
        {
            get => m_EnableAddBt;
            set
            {
                m_EnableAddBt = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnableAddBt"));
            }
        }

        public int Hour
        {
            get => m_hours;
            set
            {
                m_hours = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Hour"));
                m_model.Grouptime = new TimeSpan(m_hours, m_minute, 0);
                if (loaded)  CheckAndAdd();
            }
        }
        public int Minute
        {
            get => m_minute;
            set
            {
                m_minute = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Minute"));
                m_model.Grouptime = new TimeSpan(m_hours, m_minute, 0);
                if (loaded) CheckAndAdd();
            }
        }

        public GroupModel Model
        {
            get => m_model;
            set
            {
                m_model = value;
            }
        }

        public string Amount
        {
            get => m_Amount;
            set
            {
                m_Amount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Amount"));
            }
        }

        public string Age
        {
            get => m_Age;
            set
            {
                m_Age = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Age"));
            }
        }

        #endregion

        #region Constructor

        public GroupEditControl(DisplayedGroupModel _gm)
        {
            InitializeComponent();
            this.DataContext = this;

            error = new BitmapImage(new Uri(@"/WpfControlLibrary;component/Resource/check_error.png", UriKind.Relative));
            ok = new BitmapImage(new Uri(@"/WpfControlLibrary;component/Resource/check_ok.png", UriKind.Relative));

            m_model = _gm;

            int startHour = DayOptions.StartHour;
            HourData = GetHourView(startHour);
            MinuteData = GetMinuteView();
            ToursView = GetToursView();
            int h = _gm.Grouptime.Hours;      
            int m = _gm.Grouptime.Minutes;    
            Hour = h;
            Minute = m;
            ExpoCombo.SelectedValue = m_model.Idtour;
            Age = m_model.Groupage;
            Amount = m_model.Groupamount;
            AcompCombo.Text = m_model.Groupaddition.ToString();
            SetComment(m_model.Groupcomment);

            ConstructAutoCompletionSource();            
        }

        public GroupEditControl(int tmpId)
        {
            InitializeComponent();
            this.DataContext = this;

            error = new BitmapImage(new Uri(@"/WpfControlLibrary;component/Resource/check_error.png", UriKind.Relative));
            ok = new BitmapImage(new Uri(@"/WpfControlLibrary;component/Resource/check_ok.png", UriKind.Relative));

            m_model = new GroupModel();
            m_model.Idgroup = tmpId;

            int startHour = DayOptions.StartHour;
            HourData = GetHourView(startHour);
            MinuteData = GetMinuteView();
            ToursView = GetToursView();
            Hour = DayOptions.StartHour;
            Minute = 0;

            ConstructAutoCompletionSource();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private DataView GetHourView(int sh)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("hour", typeof(int)));            
            for(int i = 0; i < 8; i++)
            {
                DataRow dr = dt.NewRow();
                dr["hour"] = sh + i;
                dt.Rows.Add(dr);
            }

            return dt.DefaultView;
        }
        private DataView GetMinuteView()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("minute", typeof(int)));
            dt.Columns.Add(new DataColumn("minutestring", typeof(string)));
            for (int i = 0; i < 4; i++)
            {
                DataRow dr = dt.NewRow();
                dr["minute"] = i * 15;
                dr["minutestring"] = ((int)dr["minute"]).ToString("D2");
                dt.Rows.Add(dr);
            }

            return dt.DefaultView;
        }
        private DataView GetToursView()
        {
            string sql = $"call GetToursOnDate(\"{DayOptions.Date.ToString("yyyy-MM-dd")}\")";
            DataTable tmp = DBWrapper.MySqlWrapper.Select(sql);
            return tmp.DefaultView;
        }
        public void SetArguments(AddGroupEventArgs args)
        {
            TimeSpan ts = new TimeSpan(0, 15, 0); ;
            TimeSpan newTs = args.time.Add(ts);
            Hour = newTs.Hours;
            Minute = newTs.Minutes;
            ExpoCombo.SelectedValue = args.idtour;
        }
        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            loaded = true;
            CheckAndAdd();
        }

        #region Edit comment click
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CommentWindow cw = new CommentWindow();
            cw.Rtf = this.GetRtfComment();
            if((bool)cw.ShowDialog())
            {
                this.SetComment(cw.Rtf);
            }
        }
        #endregion

        #region Events
        private void RemoveGroupBt_Click(object sender, RoutedEventArgs e)
        {
            DayOptions.ExpoCheckerMain.RemoveGroup(m_model);
            RemoveGroupOnClick?.Invoke(this, EventArgs.Empty);
        }

        private void AddGroupBt_Click(object sender, RoutedEventArgs e)
        {
            AddGroupEventArgs args = new AddGroupEventArgs();
            args.time = new TimeSpan((int)HourCombo.SelectedValue, (int)MinuteCombo.SelectedValue, 0);
            args.idtour = (int)ExpoCombo.SelectedValue;
            AddGroupOnClick?.Invoke(this, args);
        }
        #endregion

        #region Подготовка автозаполнения
        private void ConstructAutoCompletionSource()
        {
            AgeTextBox.AutoSuggestionList.Clear();
            string sql = "select autocompleteword from autocompletes where autocompletetype = \"age\"";
            DataTable dt = MySqlWrapper.Select(sql);
            for (int i = 0; i < dt.Rows.Count; i++)
                AgeTextBox.AutoSuggestionList.Add(dt.Rows[i]["autocompleteword"].ToString());
        }
        #endregion

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
                if (((richText == "\r\n") || (richText == "\r\n\r\n")) && (bool)Options.HideEmptyComments)
                {
                    //brd.Height = 40;
                    //this.Height = 40;
                    //CommentRTB.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception)
            {

            }
        }
        #endregion

        #region Insert & Update
        public void Update()
        {
            Model.Grouptime = new TimeSpan(Hour, Minute, 0);
            Model.Idtour = (int)ExpoCombo.SelectedValue;
            Model.Groupage = AgeTextBox.Text;
            Model.Groupamount = Amount;
            bool res = int.TryParse(AcompCombo.Text, out int add);
            Model.Groupaddition = res ? add : 0;
            Model.Groupcomment = GetRtfComment();
            Model.Update();
        }

        public void Insert(int idGroupHeader)
        {
            Model.Idcommongroup = idGroupHeader;
            Model.Groupdate = DayOptions.Date;
            Model.Grouptime = new TimeSpan(Hour, Minute, 0);
            Model.Idtour = (int)ExpoCombo.SelectedValue;
            Model.Groupage = AgeTextBox.Text;
            Model.Groupamount = Amount;
            try
            {
                Model.Groupaddition = int.Parse(AcompCombo.Text);
            }
            catch
            {
                Model.Groupaddition = 0;
            }
            Model.Groupcomment = GetRtfComment();
            Model.Idcategory = 1;
            Model.Insert();
        }
        #endregion

        public void ReCheck()
        {
            ExpoCheckResult res = DayOptions.ExpoCheckerMain.Check(m_model);

            switch (res)
            {
                case ExpoCheckResult.RES_OK:
                    TimeResultImage.Source = ok;
                    ExpoResultImage.Source = ok;
                    DayOptions.ExpoCheckerMain.AddGroup(m_model);
                    break;
                case ExpoCheckResult.RES_NO_FREE_GUIDS:
                    TimeResultImage.Source = ok;
                    ExpoResultImage.Source = error;
                    break;
                case ExpoCheckResult.RES_BAD_TIME:
                    TimeResultImage.Source = error;
                    ExpoResultImage.Source = ok;
                    break;
            }
        }

        private void ExpoCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRow dr = (e.AddedItems[0] as DataRowView).Row;
            m_model.Idtour = (int)dr["idtour"];
            if (loaded) CheckAndAdd();
        }

        private void CheckAndAdd()
        {
            ExpoCheckResult res = DayOptions.ExpoCheckerMain.Check(m_model);
            switch (res)
            {
                case ExpoCheckResult.RES_OK:
                    TimeResultImage.Source = ok;
                    ExpoResultImage.Source = ok;
                    DayOptions.ExpoCheckerMain.AddGroup(m_model);
                    break;
                case ExpoCheckResult.RES_NO_FREE_GUIDS:
                    TimeResultImage.Source = ok;
                    ExpoResultImage.Source = error;
                    break;
                case ExpoCheckResult.RES_BAD_TIME:
                    TimeResultImage.Source = error;
                    ExpoResultImage.Source = ok;
                    break;
            }
        }
    }

    public class AddGroupEventArgs: EventArgs
    {
        public TimeSpan time;
        public int idtour;
    }
}
