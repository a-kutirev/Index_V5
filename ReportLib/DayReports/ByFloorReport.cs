using ClassLibrary;
using ClassLibrary.Models;
using DBWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using WpfControlLibrary.ReportControls;

namespace ReportLib.DayReports
{
    public class ByFloorReport
    {
        #region Members
        private FixedDocument m_document;
        private DateTime m_selectedDate;
        private bool m_showNotes;
        private bool m_showEvents;

        private List<string> monthStr = new List<string> { "Января", "Февраля", "Марта", "Апреля", "Мая",
            "Июня", "Июля", "Августа", "Сентября", "Октября", "Ноября", "Декабря" };
        private List<string> monthStr_2 = new List<string> { "январь", "февраль", "март", "апрель", "май",
            "июнь", "июль", "август", "сентябрь", "октябрь", "ноябрь", "декабрь" };
        private List<string> dayStr = new List<string> { "воскресенье", "понедельник", "вторник", "среда", "четверг", "пятница",
            "суббота"};

        public DateTime SelectedDate { get => m_selectedDate; set => m_selectedDate = value; }
        public bool ShowNotes { get => m_showNotes; set => m_showNotes = value; }
        public FixedDocument Document { get => m_document; set => m_document = value; }
        #endregion

        #region Constructor
        public ByFloorReport(DateTime t, bool sn, bool se)
        {
            m_selectedDate = t;
            m_showNotes = sn;
            m_showEvents = se;

            StackPanel container = new StackPanel();
            container.Orientation = Orientation.Vertical;
            container.HorizontalAlignment = HorizontalAlignment.Left;

            #region Заголовок
            int year = m_selectedDate.Year; ;
            int month = m_selectedDate.Month;
            int day = m_selectedDate.Day;
            string dayWeek = dayStr[(int)m_selectedDate.DayOfWeek];
            string dateString = day.ToString() + " " + monthStr[month - 1] + ", " + dayWeek;

            TextBlock tb = new TextBlock();
            tb.Text = dateString;
            tb.FontSize = 20;
            tb.FontFamily = new FontFamily("Times New Roman");
            tb.FontWeight = FontWeights.Bold;
            tb.HorizontalAlignment = HorizontalAlignment.Center;
            container.Children.Add(tb);
            #endregion

            #region Заметки
            if (m_showNotes)
            {
                string sql2 = $"CALL GetNotesByDate('{m_selectedDate.ToString("yyyy-MM-dd")}')";
                List<NoteModel> notes = (List<NoteModel>)MySqlWrapper.Select(sql2).ToList<NoteModel>();

                container.Children.Add(new NoteControl(notes));
            }
            #endregion

            #region Этажи

            string sql = "select * from floors";
            List<FloorModel> floors = (List<FloorModel>)MySqlWrapper.Select(sql).ToList<FloorModel>();
            Dictionary<int, string> floorName = new Dictionary<int, string>();
            for (int i = 0; i < floors.Count; i++)
                floorName.Add(floors[i].Idfloor, floors[i].Floorname);
            #endregion

            SortedDictionary<int, List<ITimed>> grpListByFloor =
                new SortedDictionary<int, List<ITimed>>();

            List<DisplayedGroupModel> grpList =
                (List<DisplayedGroupModel>)MySqlWrapper.SelectGroupByDate(m_selectedDate).ToList<DisplayedGroupModel>();

            for(int  i = 0; i < grpList.Count; i++)
            {
                if ((grpList[i].Groupstatus & 1) > 0) continue;
                int idFloor = Options.GetFloorIdByTourId(grpList[i].Idtour);

                if (!grpListByFloor.ContainsKey(idFloor))
                    grpListByFloor.Add(idFloor, new List<ITimed>());

                grpListByFloor[idFloor].Add(new SimpleGroupControl(grpList[i]));
            }

            if(m_showEvents)
            {
                List<EventGroupModel> evtList =
                    (List<EventGroupModel>)MySqlWrapper.SelectEventByDate(m_selectedDate).ToList<EventGroupModel>();
                for(int i = 0; i < evtList.Count; i++)
                {
                    if ((evtList[i].Eventgroupstatus & 1) > 0) continue;
                    EventModel em = new EventModel(evtList[i].Idevent);
                    Expo_zonesModel ezm = new Expo_zonesModel(em.Idexpo_zone);
                    int idFloor = ezm.Idfloor;

                    if (!grpListByFloor.ContainsKey(idFloor))
                        grpListByFloor.Add(idFloor, new List<ITimed>());

                    grpListByFloor[idFloor].Add(new SimpleEventControl(evtList[i]));
                }

                for (int i = 0; i < grpListByFloor.Count; i++)
                {
                    List<ITimed> tmp = Sort(grpListByFloor.ElementAt(i).Value);
                    grpListByFloor[grpListByFloor.ElementAt(i).Key] = tmp;
                }
            }

            for (int i = 0; i < grpListByFloor.Count; i++)
            {
                int idFloor = grpListByFloor.ElementAt(i).Key;
                List<ITimed> grpListControls = grpListByFloor.ElementAt(i).Value;

                TextBlock textBlock = new TextBlock();
                textBlock.Text = floorName[idFloor] + "\n";
                textBlock.FontSize = 20;
                textBlock.FontWeight = FontWeights.Bold;
                textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                textBlock.Margin = new Thickness(0,10,0,0);
                textBlock.FontFamily = new FontFamily("Times New Roman");

                container.Children.Add(textBlock);

                for(int j = 0; j < grpListControls.Count; j++)
                {
                    container.Children.Add(grpListControls[j] as UIElement);
                }
            }

            m_document = GetFixedDocument(container, new PrintDialog());
        }

        private List<ITimed> Sort(List<ITimed> value)
        {
            List<ITimed> result = new List<ITimed>();

            int count = value.Count;

            while(result.Count != count)
            {
                int minIndex = 0;
                TimeSpan minTs = (value[0] as ITimed).Time;

                for(int i = 0; i < value.Count; i++)
                {
                    TimeSpan current = (value[i] as ITimed).Time;
                    if(minTs > current)
                    {
                        minIndex = i;
                        minTs = current;
                    }
                }

                ITimed obj = value[minIndex];
                value.RemoveAt(minIndex);
                result.Add(obj);
            }

            return result;
        }
        #endregion

        #region Create Report

        private FixedDocument GetFixedDocument(FrameworkElement toPrint, PrintDialog printDialog)
        {
            printDialog.PrintTicket.PageOrientation = PageOrientation.Portrait;
            PrintCapabilities capabilities = printDialog.PrintQueue.GetPrintCapabilities(printDialog.PrintTicket);
            Size pageSize = new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);
            Size visibleSize = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);
            FixedDocument result = new FixedDocument();

            StackPanel container = new StackPanel();
            container.Orientation = Orientation.Vertical;
            container.HorizontalAlignment = HorizontalAlignment.Left;

            int c = (toPrint as StackPanel).Children.Count;

            for (int i = 0; i < c; i++)
            {
                UIElement element = (toPrint as StackPanel).Children[0];
                (toPrint as StackPanel).Children.Remove(element);
                container.Children.Add(element);
                container.Width = pageSize.Width;

                container.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                container.Arrange(new Rect(new Point(0, 0), container.DesiredSize));

                if (container.DesiredSize.Height > pageSize.Height)
                {
                    container.Children.Remove(element);

                    PageContent pageContent = new PageContent();
                    FixedPage page = new FixedPage();
                    ((IAddChild)pageContent).AddChild(page);
                    result.Pages.Add(pageContent);
                    page.Width = pageSize.Width;
                    page.Height = pageSize.Height;
                    page.Children.Add(container);

                    FixedPage.SetLeft(container, capabilities.PageImageableArea.OriginWidth);
                    FixedPage.SetTop(container, capabilities.PageImageableArea.OriginHeight);

                    container = new StackPanel();
                    container.Orientation = Orientation.Vertical;
                    container.HorizontalAlignment = HorizontalAlignment.Left;

                    container.Children.Add(element);
                }
                else if (i == (c - 1))
                {
                    PageContent pageContent = new PageContent();
                    FixedPage page = new FixedPage();
                    ((IAddChild)pageContent).AddChild(page);
                    page.Children.Add(container);
                    result.Pages.Add(pageContent);
                    page.Width = pageSize.Width;
                    page.Height = pageSize.Height;
                    FixedPage.SetLeft(container, capabilities.PageImageableArea.OriginWidth);
                    FixedPage.SetTop(container, capabilities.PageImageableArea.OriginHeight);
                }
            }
            return result;
        }

        #endregion
    }
}
