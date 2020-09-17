using ClassLibrary;
using ClassLibrary.Models;
using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using WpfControlLibrary;

namespace Tours_V5
{
    /// <summary>
    /// Логика взаимодействия для GroupEditWIndow.xaml
    /// </summary>
    public partial class GroupEditWIndow_Main : Window, INotifyPropertyChanged
    {
        #region Members

        public event PropertyChangedEventHandler PropertyChanged;

        private DisplayedGroupModel m_model;
        private DataView m_guidView;
        private DataView m_tourView;
        private DataView m_acompView;
        private DataView m_contactsView;
        private DateTime m_tourDate;
        private DateTime m_oldDate;
        private bool m_dontChangeGuid;
        private bool loaded = false;
        private BitmapImage error;
        private BitmapImage ok;

        public DisplayedGroupModel Model
        {
            get => m_model;
            set
            {
                m_model = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Model"));
            }
        }
        public DataView GuidView
        {
            get => m_guidView;
            set
            {
                m_guidView = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GuidView"));
            }
        }
        public DataView AcompView
        {
            get => m_acompView;
            set
            {
                m_acompView = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AcompView"));
            }
        }
        public DataView TourView
        {
            get => m_tourView;
            set
            {
                m_tourView = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TourView"));
            }
        }
        public DateTime TourDate
        {
            get => m_tourDate;
            set
            {
                m_tourDate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TourDate"));
            }
        }
        public bool DontChangeGuid
        {
            get => m_dontChangeGuid;
            set
            {
                m_dontChangeGuid = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DontChangeGuid"));
            }
        }
        public DataView ContactsView
        {
            get => m_contactsView;
            set
            {
                m_contactsView = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ContactsView"));
            }
        }

        #endregion

        #region Constructor
        public GroupEditWIndow_Main()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        public GroupEditWIndow_Main(int id)
        {
            InitializeComponent();
            this.DataContext = this;
            Model = new DisplayedGroupModel(id);

            error = new BitmapImage(new Uri(@"/WpfControlLibrary;component/Resource/check_error.png", UriKind.Relative));
            ok = new BitmapImage(new Uri(@"/WpfControlLibrary;component/Resource/check_ok.png", UriKind.Relative));

            m_oldDate = Model.Groupdate.Date;

            SetComment(Model.Groupcomment);
            DontChangeGuid = (Model.Groupstatus & 8) > 0;
                        
            #region Create Views for ComboBoxes
            DateTime date = Model.Groupdate;
            string sql = $"call GetGuidsByDate('{date.ToMySqlDateString()}')";
            GuidView = DBWrapper.MySqlWrapper.Select(sql).DefaultView;
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
            sql = $"call GetToursOnDate('{date.ToMySqlDateString()}')";
            TourView = DBWrapper.MySqlWrapper.Select(sql).DefaultView;
            #endregion

            #region Set Date-Time
            TimePicker.Date = Model.Groupdate;
            TimePicker.HourVal = Model.Grouptime.Hours;
            TimePicker.MinuteVal = Model.Grouptime.Minutes;
            TourDate = Model.Groupdate;
            #endregion

            #region Contacts

            int headId = Model.Idcommongroup;
            sql = $"call GetContactsByHeaderId({headId})";
            DataTable tmp = DBWrapper.MySqlWrapper.Select(sql);
            ContactsView = tmp.DefaultView;

            #endregion
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
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region Сохранение изменений
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            #region Сохранение контактов

            int headId = Model.Idcommongroup;
            string sql = $"delete from commongroup_contacts where idcommongroup = {headId}";
            DBWrapper.MySqlWrapper.Execute(sql);

            DataTable dt = ContactsView.ToTable();

            for(int i = 0; i < dt.Rows.Count; i++)
            {
                int idContact = int.Parse(dt.Rows[i]["idcontact"].ToString());
                (new commongroup_contactModel(headId, idContact)).Insert();                                
            }

            #endregion

            Model.Groupdate = new DateTime(
                TourDate.Year,
                TourDate.Month,
                TourDate.Day,
                TimePicker.HourVal,
                TimePicker.MinuteVal, 0);

            Model.Grouptime = new TimeSpan(TimePicker.HourVal, TimePicker.MinuteVal, 0);

            Model.Groupcomment = GetRtfComment();
            Model.Update();
            DialogResult = true;
        }
        #endregion

        #region Не изменять экскурсовода
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            bool val = (bool)(sender as CheckBox).IsChecked;
            if (val) Model.Groupstatus |= 8;
            GuidCOmbo.IsEnabled = !val;
        }
        #endregion

        #region Удаление из списка контактов
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены что хотите удалить контакт?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question)
                == MessageBoxResult.Yes)
            {
                int headId = Model.Idcommongroup;
                int idcontact = (int)((sender as Button).CommandParameter);
                string sql = $"delete from commongroup_contacts where (idcommongroup = {headId}) and (idcontact = {idcontact})";
                DBWrapper.MySqlWrapper.Execute(sql);

                sql = $"call GetContactsByHeaderId({headId})";
                DataTable tmp = DBWrapper.MySqlWrapper.Select(sql);
                ContactsView = tmp.DefaultView;
            }
        }
        #endregion

        #region Сохранение комментария
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            CommentWindow cw = new CommentWindow();
            cw.Rtf = this.GetRtfComment();
            if ((bool)cw.ShowDialog())
            {
                this.SetComment(cw.Rtf);
                Model.Groupcomment = cw.Rtf;
            }
        }
        #endregion

        private void AddContact_Click(object sender, RoutedEventArgs e)
        {
            int headId = Model.Idcommongroup;
            string name = (sender as Button).Name;

            GroupHeaderModel org_gr = new GroupHeaderModel(headId);

            switch(name)
            {
                case "AddExistingContact":
                    SelectContactWindow scw = new SelectContactWindow(org_gr.Idorganization);
                    if ((bool)(scw.ShowDialog()))
                        (new commongroup_contactModel(headId, scw.Id)).Insert();
                    break;
                case "AddNewContact":
                    AddContactWindow acw = new AddContactWindow();
                    if((bool)(acw.ShowDialog()))                        
                        (new commongroup_contactModel(headId, acw.Id)).Insert();
                    break;
            }

            #region Contacts

            string sql = $"call GetContactsByHeaderId({headId})";
            DataTable tmp = DBWrapper.MySqlWrapper.Select(sql);
            ContactsView = tmp.DefaultView;

            #endregion
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

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!loaded) return;
            if((sender as DatePicker).SelectedDate.Value.Date != m_oldDate)
            {
                Model.Groupnum += 100;
            }
            else
            {
                if (Model.Groupnum > 100)
                    Model.Groupnum -= 50;
            }
        }

        private void TimePicker_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!loaded) return;
            if (e.PropertyName == "HourVal" || e.PropertyName == "MinuteVal")
            {
                m_model.Grouptime = TimePicker.Time;
                m_model.Groupdate =
                    new DateTime(
                        m_model.Groupdate.Year,
                        m_model.Groupdate.Month,
                        m_model.Groupdate.Day,
                        m_model.Grouptime.Hours,
                        m_model.Grouptime.Minutes,
                        0);
                if (loaded) CheckAndAdd();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loaded = true;
            CheckAndAdd();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!loaded) return;
            m_model.Idtour = (int)(sender as ComboBox).SelectedValue;
            CheckAndAdd();
        }

        private void DataGridRow_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            int id = int.Parse((row.Item as DataRowView).Row[0].ToString());

            AddContactWindow acw = new AddContactWindow(id);
            acw.ShowDialog();

            #region Contacts

            int headId = Model.Idcommongroup;
            string sql = $"call GetContactsByHeaderId({headId})";
            DataTable tmp = DBWrapper.MySqlWrapper.Select(sql);
            ContactsView = tmp.DefaultView;

            #endregion
        }
    }
}
