using ClassLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace WpfControlLibrary.GroupControls
{
    /// <summary>
    /// Логика взаимодействия для GroupControl.xaml
    /// </summary>
    public partial class GroupControl : UserControl, INotifyPropertyChanged
    {

        #region Fields & Events
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler GuidComboReload;
        public event EventHandler<EventGroup> DeleteGroupClick;
        public event EventHandler<EventGroup> EditGroupClick;
        public event EventHandler<EventGroup> CompleteGroupClick;

        private string m_toolTip = "";
        private bool m_dontChangeGuid = false;
        private bool m_confirmed = false;
        private bool enableGuidComboUpdate = false;
        private bool m_last;
        private bool m_enableControls = true;
        private bool loaded = false;
        private List<GuidSelectItem> m_guidSelectList;
        private DisplayedGroupModel m_model;

        public bool Last
        {
            set
            {
                {
                    m_last = value;
                    if (m_last)
                        brd.CornerRadius = new CornerRadius(0, 0, 10, 10);
                    else
                        brd.CornerRadius = new CornerRadius(0, 0, 0, 0);
                }
            }
        }

        public string Ttime
        {
            get
            {
                return $"{Model.Grouptime.Hours}:{Model.Grouptime.Minutes.ToString("D2")}";
            }
        }

        public DisplayedGroupModel Model
        {
            get => m_model;
            set
            {
                m_model = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Model"));
            }
        }

        public List<GuidSelectItem> GuidSelectList
        {
            get => m_guidSelectList;
            set
            {
                m_guidSelectList = value;                
                for(int i = 0; i < m_guidSelectList.Count; i++)
                    if(Model.Idguid == m_guidSelectList[i].GuidId)
                    {                        
                        m_guidSelectList[i].StateImage = new BitmapImage(
                            new Uri("/WpfControlLibrary;component/Resource/white.png", UriKind.Relative));
                        GuidCOmbo.SelectedIndex = i;                        
                        break;
                    }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GuidSelectList"));
                GuidCOmbo.SelectionChanged += GuidCOmbo_SelectionChanged;
            }
        }

        public bool EnableControls
        {
            get => m_enableControls;
            set
            {
                m_enableControls = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnableControls"));
            }
        }

        public bool DontChangeGuid
        {
            get => m_dontChangeGuid;
            set
            {
                m_dontChangeGuid = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DontChangeGuid"));
                GuidCOmbo.IsEnabled = !(((Model.Groupstatus & 1) > 0) || ((Model.Groupstatus & 4) > 0) || DontChangeGuid);
                if (m_dontChangeGuid) Model.Groupstatus |= 8;
                else Model.Groupstatus &= 7;
                if (!loaded) return;
                (Model as GroupModel).Update();
            }
        }

        public bool Confirmed
        {
            get => m_confirmed;
            set
            {
                m_confirmed = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Confirmed"));
                if (m_confirmed) Model.Groupstatus |= 2;
                else Model.Groupstatus &= 65533;
                if (!loaded) return;
                Model.Update();
            }
        }

        public string ToolTip1
        {
            get => m_toolTip;
            set
            {
                m_toolTip = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ToolTip1"));
            }
        }
        #endregion

        #region Constructor
        public GroupControl()
        {
            InitializeComponent();
        }
        public GroupControl(DisplayedGroupModel dgm)
        {
            InitializeComponent();
            this.DataContext = this;
            Model = dgm;            

            SetComment(m_model.Groupcomment);

            #region Цветовое выделение проведенных / удаленных экскурсий
            if((dgm.Groupstatus & 8) > 0)
            {
                DontChangeGuid = true;
            }
            ToolTip1 = "id = " + Model.Idgroup.ToString();
            if ((dgm.Groupstatus & 4) > 0)
            {
                if ((bool)Options.HideCompleted) this.Visibility = Visibility.Collapsed;
                else this.Visibility = Visibility.Visible;
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
                brd.Background = lgb;
                ToolTip1 = $"id = {Model.Idgroup}{Environment.NewLine}{Options.GetCategName(Model.Idcategory)}";
                List<string> opt = Options.GetStatisticsByCode(Model.Groupstatistic);
                for(int i = 0; i < opt.Count;i++)
                {
                    ToolTip1 = ToolTip1 + Environment.NewLine + opt[i];
                }
            }
            if ((dgm.Groupstatus & 1) > 0)
            {
                if ((bool)Options.HideDeleted) this.Visibility = Visibility.Collapsed;
                else this.Visibility = Visibility.Visible;
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
                brd.Background = lgb;

                ToolTip1 = $"id = {Model.Idgroup}{Environment.NewLine}{Model.Groupdeletereason}";
            }
            EnableControls = !(((Model.Groupstatus & 1) > 0) || ((Model.Groupstatus & 4) > 0));

            GuidCOmbo.IsEnabled = !(((Model.Groupstatus & 1) > 0) || ((Model.Groupstatus & 4) > 0) || DontChangeGuid);
            #endregion

            GuidSelectList = DayOptions.GuidCheckerMain.GetGuidItems(dgm.Grouptime, Options.GetDuration(Model.Idtour));            
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

                if (((formattedText == "\r\n") || (formattedText == "\r\n\r\n") || (formattedText == "")) && (bool)Options.HideEmptyComments)
                    CommentColumn.Height = new GridLength(0);
                else
                    CommentColumn.Height = new GridLength(45);

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
                if (((range.Text == "\r\n") || (range.Text == "\r\n\r\n") || (range.Text == "")) && (bool)Options.HideEmptyComments)
                    CommentColumn.Height = new GridLength(0);
                else
                    CommentColumn.Height = new GridLength(45);
                ToolTip tt = new ToolTip();
                tt.Content = tip;
                CommentRTB.ToolTip = tt;                
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region Guid changed
        private void GuidCOmbo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!enableGuidComboUpdate) return;
            DayOptions.GuidCheckerMain.Remove(Model.Idguid, Model.Grouptime);
            int newIdGuid = (int)(((ComboBox)sender).SelectedValue as GuidSelectItem).GuidId;
            Model.Idguid = newIdGuid;
            DayOptions.GuidCheckerMain.Add(newIdGuid, Model.Grouptime, Options.GetDuration(Model.Idtour));
            (Model as GroupModel).Update();
            GuidComboReload?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Group Events Handlers
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            m_confirmed = (Model.Groupstatus & 2) > 0;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Confirmed"));
            loaded = true;            
            enableGuidComboUpdate = true;
        }

        private void EditBt_Click(object sender, RoutedEventArgs e)
        {
            EventGroup eg = new EventGroup();
            eg.idgroup = Model.Idgroup;
            EditGroupClick?.Invoke(this, eg);
        }

        private void DeleteBt_Click(object sender, RoutedEventArgs e)
        {
            EventGroup eg = new EventGroup();
            eg.idgroup = Model.Idgroup;
            DeleteGroupClick?.Invoke(this, eg);
        }

        private void CompleteBt_Click(object sender, RoutedEventArgs e)
        {
            EventGroup eg = new EventGroup();
            eg.idgroup = Model.Idgroup;
            CompleteGroupClick?.Invoke(this, eg);
        }
        #endregion

        private void CommentBt_Click(object sender, RoutedEventArgs e)
        {
            CommentWindow cw = new CommentWindow();
            cw.Rtf = this.GetRtfComment();
            if ((bool)cw.ShowDialog())
            {
                this.SetComment(cw.Rtf);
                Model.Groupcomment = cw.Rtf;
                Model.Update();
            }
        }
    }

    public class EventGroup : EventArgs
    {
        public int idgroup;
    }
}
