using ClassLibrary;
using ClassLibrary.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfControlLibrary.ReportControls.DayReport
{
    /// <summary>
    /// Логика взаимодействия для FullDayReportSimpleEvent.xaml
    /// </summary>
    public partial class FullDayReportSimpleEvent : UserControl
    {

        private EventGroupModel m_model;
        public FullDayReportSimpleEvent(EventGroupModel m)
        {
            m_model = m;

            InitializeComponent();

            for (int i = 0; i < 11; i++)
            {
                Border brd = new Border();
                Thickness t = (i == 10) ? new Thickness(1, 0, 1, 1) : new Thickness(0, 0, 1, 0);
                brd.BorderThickness = t;
                brd.BorderBrush = Brushes.Black;
                Grid.SetColumnSpan(brd, i + 1);
                MainGrid.Children.Add(brd);
            }

            #region Время
            string time = m_model.Eventgrouptime.ToString(@"hh\:mm");
            TextBlock l = new TextBlock();
            l.Margin = new Thickness(3, 0, 0, 0);
            l.Text = time;
            l.FontSize = 12;
            Grid.SetColumn(l, 0);
            MainGrid.Children.Add(l);
            #endregion

            #region Мероприятие
            int idEv = m_model.Idevent;
            EventModel em = new EventModel(idEv);
            string eventName = "";
            if (em.Eventtype == "КВ") eventName += "Квест: ";
            if (em.Eventtype == "МК") eventName += "Мастер класс: ";
            if (em.Eventtype == "Л") eventName += "Лекция: ";
            if (em.Eventtype == "СМ") eventName += "Семинар: ";
            eventName += em.Eventname;
            TextBlock tb = new TextBlock();
            tb.Margin = new Thickness(3, 0, 0, 0);
            tb.Text = eventName;
            tb.TextWrapping = TextWrapping.Wrap;
            tb.FontSize = 12;
            Grid.SetColumn(tb, 6);
            MainGrid.Children.Add(tb);
            #endregion

            #region Ведущие
            string guid = Options.GetGuidStringByIds(m_model.GetListMasters());
            tb = new TextBlock();
            tb.Margin = new Thickness(3, 0, 0, 0);
            tb.Text = guid;
            tb.TextWrapping = TextWrapping.Wrap;
            tb.FontSize = 12;
            Grid.SetColumn(tb, 7);
            MainGrid.Children.Add(tb);
            #endregion

            if ((m_model.Eventgroupstatus & 1) > 0)
            {
                #region Причина отказа
                string comment = RichTextStripper.StripRichTextFormat(m_model.Eventgroupdeletereason.Replace('#', '\\'));
                tb = new TextBlock();
                tb.Margin = new Thickness(3, 0, 0, 0);
                tb.Text = comment;
                tb.TextWrapping = TextWrapping.Wrap;
                tb.FontSize = 12;
                Grid.SetColumn(tb, 9);
                MainGrid.Children.Add(tb);
                #endregion

                #region Отметка
                string otm = "Отменена";
                tb = new TextBlock();
                tb.Margin = new Thickness(3, 0, 0, 0);
                tb.Text = otm;
                tb.TextWrapping = TextWrapping.Wrap;
                tb.FontSize = 12;
                Grid.SetColumn(tb, 10);
                MainGrid.Children.Add(tb);
                #endregion
            }
            else
            {
                #region Комментарии
                string comment = RichTextStripper.StripRichTextFormat(m_model.Eventgroupcomment.Replace('#', '\\'));
                tb = new TextBlock();
                tb.Margin = new Thickness(3, 0, 0, 0);
                tb.Text = comment;
                tb.TextWrapping = TextWrapping.Wrap;
                tb.FontSize = 12;
                Grid.SetColumn(tb, 9);
                MainGrid.Children.Add(tb);
                #endregion

                if ((m_model.Eventgroupstatus & 4) > 0)
                {
                    #region Отметка
                    string otm = "Проведена";
                    tb = new TextBlock();
                    tb.Margin = new Thickness(3, 0, 0, 0);
                    tb.Text = otm;
                    tb.TextWrapping = TextWrapping.Wrap;
                    tb.FontSize = 12;
                    Grid.SetColumn(tb, 10);
                    MainGrid.Children.Add(tb);
                    #endregion
                }
                else
                {
                    #region Отметка
                    string otm = "";
                    tb = new TextBlock();
                    tb.Margin = new Thickness(3, 0, 0, 0);
                    tb.Text = otm;
                    tb.TextWrapping = TextWrapping.Wrap;
                    tb.FontSize = 12;
                    Grid.SetColumn(tb, 10);
                    MainGrid.Children.Add(tb);
                    #endregion
                }
            }
        }
    }
}