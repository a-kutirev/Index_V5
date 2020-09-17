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
    public class ByTimeReport
    {
        #region Members

        private DateTime m_selectedDate;
        private FixedDocument m_document;
        private bool m_showNotes = true;
        private bool m_showEvents = true;

        public FixedDocument Document { get => m_document; set => m_document = value; }
        public bool ShowNotes { get => m_showNotes; set => m_showNotes = value; }

        enum Month { Января, Февраля, Марта, Апреля, Мая, Июня, Июля, Августа, Сентября, Октября, Ноября, Декабря };
        private DateTime date;
        private bool ShowNonpersist = false;
        private int m_year, m_period;
        private bool m_check = false;
        private string m_type = "";
        private List<string> monthStr = new List<string> { "Января", "Февраля", "Марта", "Апреля", "Мая",
            "Июня", "Июля", "Августа", "Сентября", "Октября", "Ноября", "Декабря" };
        private List<string> monthStr_2 = new List<string> { "январь", "февраль", "март", "апрель", "май",
            "июнь", "июль", "август", "сентябрь", "октябрь", "ноябрь", "декабрь" };
        private List<string> dayStr = new List<string> { "воскресенье", "понедельник", "вторник", "среда", "четверг", "пятница",
            "суббота"};
        private bool nonpersExist = false;

        #endregion

        #region  Constructor
        public ByTimeReport(DateTime date, bool sn, bool se)
        {
            m_selectedDate = date;
            ShowNotes = sn;
            m_showEvents = se;

            StackPanel container = new StackPanel();            
            container.Orientation = Orientation.Vertical;
            container.HorizontalAlignment = HorizontalAlignment.Left;

            int year = date.Year; ;
            int month = date.Month;
            int day = date.Day;
            string dayWeek = dayStr[(int)date.DayOfWeek];
            string dateString = day.ToString() + " " + monthStr[month - 1] + ", " + dayWeek;

            TextBlock tb = new TextBlock();
            tb.Text = dateString;
            tb.FontSize = 20;
            tb.FontFamily = new FontFamily("Times New Roman");
            tb.FontWeight = FontWeights.Bold;
            tb.HorizontalAlignment = HorizontalAlignment.Center;
            container.Children.Add(tb);

            if (ShowNotes)
            {
                string sql2 = $"CALL GetNotesByDate('{m_selectedDate.ToString("yyyy-MM-dd")}')";
                List<NoteModel> notes = (List<NoteModel>)MySqlWrapper.Select(sql2).ToList<NoteModel>();

                container.Children.Add(new NoteControl(notes));                
            }

            List<DisplayedGroupModel> grpList =
                (List<DisplayedGroupModel>)MySqlWrapper.SelectGroupByDate(m_selectedDate).ToList<DisplayedGroupModel>();

            for (int i = 0; i < grpList.Count; i++)
            {
                if ((grpList[i].Groupstatus & 1) > 0) continue;

                SimpleGroupControl sgc = new SimpleGroupControl(grpList[i]);
                container.Children.Add(sgc);
            }
            if(m_showEvents)
            {
                List<EventGroupModel> evtList =
                    (List<EventGroupModel>)MySqlWrapper.SelectEventByDate(m_selectedDate).ToList<EventGroupModel>();
                for(int i = 0; i < evtList.Count; i++)
                {
                    if ((evtList[i].Eventgroupstatus & 1) > 0) continue;
                    SimpleEventControl sec = new SimpleEventControl(evtList[i]);
                    container.Children.Add(sec);
                }

                container = Sort(container);
            }
            m_document = GetFixedDocument(container, new PrintDialog());
        }

        private StackPanel Sort(StackPanel container)
        {
            StackPanel result = new StackPanel();
            result.Orientation = Orientation.Vertical;
            result.HorizontalAlignment = HorizontalAlignment.Left;

            int c = container.Children.Count;
            int startedIndex = 0;

            while (container.Children.Count != 0)
            {
                while((container.Children[0] as ITimed) == null)
                {
                    UIElement uIElement = container.Children[0];
                    container.Children.RemoveAt(0);
                    result.Children.Add(uIElement);
                    if (container.Children.Count == 0) break;
                }
                if (container.Children.Count == 0) break;
                int minIndex = 0;
                TimeSpan minTs = (container.Children[0] as ITimed).Time;

                for (int i = 0;  i < container.Children.Count; i++)
                {
                    TimeSpan current = (container.Children[i] as ITimed).Time;
                    if (minTs > current)
                    {
                        minIndex = i;
                        minTs = current;
                    }
                }

                UIElement e = container.Children[minIndex];
                container.Children.RemoveAt(minIndex);
                result.Children.Add(e);
            }

            return result;
        }

        #endregion

        #region Create Document

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

                if(container.DesiredSize.Height > pageSize.Height)
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
                else if(i == (c - 1))
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


        //private FixedDocument GetFixedDocument(FrameworkElement toPrint, PrintDialog printDialog)
        //{
        //    //printDialog.PrintTicket.PageOrientation = PageOrientation.Landscape;
        //    PrintCapabilities capabilities = printDialog.PrintQueue.GetPrintCapabilities(printDialog.PrintTicket);
        //    Size pageSize = new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);
        //    Size visibleSize = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);
        //    FixedDocument result = new FixedDocument();

        //    (toPrint as StackPanel).Width = pageSize.Width;

        //    // If the toPrint visual is not displayed on screen we neeed to measure and arrange it.
        //    toPrint.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        //    toPrint.Arrange(new Rect(new Point(0, 0), toPrint.DesiredSize));

        //    Size size = toPrint.DesiredSize;

        //    // Will assume for simplicity the control fits horizontally on the page.
        //    double yOffset = 0;
        //    while (yOffset < size.Height)
        //    {
        //        VisualBrush vb = new VisualBrush(toPrint);
        //        vb.Stretch = Stretch.None;
        //        vb.AlignmentX = AlignmentX.Left;
        //        vb.AlignmentY = AlignmentY.Top;
        //        vb.ViewboxUnits = BrushMappingMode.Absolute;
        //        vb.TileMode = TileMode.None;
        //        vb.Viewbox = new Rect(0, yOffset, visibleSize.Width, visibleSize.Height);

        //        PageContent pageContent = new PageContent();
        //        FixedPage page = new FixedPage();
        //        ((IAddChild)pageContent).AddChild(page);
        //        result.Pages.Add(pageContent);
        //        page.Width = pageSize.Width;
        //        page.Height = pageSize.Height;

        //        Canvas canvas = new Canvas();
        //        FixedPage.SetLeft(canvas, capabilities.PageImageableArea.OriginWidth);
        //        FixedPage.SetTop(canvas, capabilities.PageImageableArea.OriginHeight);
        //        canvas.Width = visibleSize.Width;
        //        canvas.Height = visibleSize.Height;
        //        canvas.Background = vb;
        //        page.Children.Add(canvas);

        //        yOffset += visibleSize.Height;
        //    }


        //    return result;
        //}

        #endregion
    }
}
