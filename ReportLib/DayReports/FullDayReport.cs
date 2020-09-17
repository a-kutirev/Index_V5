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
using WpfControlLibrary.ReportControls.DayReport;

namespace ReportLib.DayReports
{
    public class FullDayReport
    {
        #region Members

        private bool showTours, showEvents;

        private List<string> monthStr = new List<string> { "Января", "Февраля", "Марта", "Апреля", "Мая", "Июня", "Июля", "Августа", "Сентября", "Октября", "Ноября", "Декабря" };
        private List<string> monthStr_2 = new List<string> { "январь", "февраль", "март", "апрель", "май","июнь", "июль", "август", "сентябрь", "октябрь", "ноябрь", "декабрь" };
        private List<string> dayStr = new List<string> { "воскресенье", "понедельник", "вторник", "среда", "четверг", "пятница", "суббота"};

        private DateTime m_selectedDate;
        private FixedDocument m_document;
        private string title;

        public FixedDocument Document { get => m_document; set => m_document = value; }
        #endregion

        #region Constructor
        public FullDayReport(DateTime d, bool st, bool se)
        {
            m_selectedDate = d;
            showTours = st;
            showEvents = se;

            StackPanel container = new StackPanel();
            container.Orientation = Orientation.Vertical;
            container.HorizontalAlignment = HorizontalAlignment.Left;

            int year = m_selectedDate.Year; ;
            int month = m_selectedDate.Month;
            int day = m_selectedDate.Day;
            string dayWeek = dayStr[(int)m_selectedDate.DayOfWeek];
            title = day.ToString() + " " + monthStr[month - 1] + ", " + dayWeek + "\n";

            TextBlock tb = new TextBlock();
            tb.Text = title;
            tb.FontSize = 20;
            tb.FontFamily = new FontFamily("Times New Roman");
            tb.FontWeight = FontWeights.Bold;
            tb.HorizontalAlignment = HorizontalAlignment.Center;
            container.Children.Add(tb);

            FullDayReportHeader fdrh = new FullDayReportHeader();

            container.Children.Add(fdrh);

            List<DisplayedGroupModel> grpList =
                (List<DisplayedGroupModel>)MySqlWrapper.SelectGroupByDate(m_selectedDate).ToList<DisplayedGroupModel>();

            Dictionary<int, List<DisplayedGroupModel>> allGroups =
                new Dictionary<int, List<DisplayedGroupModel>>();

            for(int i = 0; i < grpList.Count; i++)
            {
                int numGrp = grpList[i].Groupnum;
                if(!allGroups.ContainsKey(numGrp))
                    allGroups.Add(numGrp, new List<DisplayedGroupModel>());

                allGroups[numGrp].Add(grpList[i]);
            }

            if(showTours)
                for(int i = 0; i < allGroups.Count; i++)
                {
                    FullDayReportSimpleGroup sg = new FullDayReportSimpleGroup(allGroups.ElementAt(i).Value);
                    container.Children.Add(sg);
                }

            List<EventGroupModel> allEvents =
                (List<EventGroupModel>)DBWrapper.MySqlWrapper.SelectEventByDate(m_selectedDate).ToList<EventGroupModel>();

            if(showEvents)
                for(int i = 0; i < allEvents.Count; i++)
                {
                    FullDayReportSimpleEvent sevent = new FullDayReportSimpleEvent(allEvents[i]);
                    container.Children.Add(sevent);
                }

            Document = GetFixedDocumentNew(container, new PrintDialog());
        }
        #endregion

        #region Create Document

        private FixedDocument GetFixedDocumentNew(FrameworkElement toPrint, PrintDialog printDialog)
        {
            printDialog.PrintTicket.PageOrientation = PageOrientation.Landscape;
            PrintCapabilities capabilities = printDialog.PrintQueue.GetPrintCapabilities(printDialog.PrintTicket);
            Size pageSize = new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);
            Size visibleSize = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);
            FixedDocument result = new FixedDocument();

            StackPanel container = new StackPanel();
            container.Orientation = Orientation.Vertical;
            container.HorizontalAlignment = HorizontalAlignment.Left;

            int c = (toPrint as StackPanel).Children.Count;
            Dictionary<int, StackPanel> pages = new Dictionary<int, StackPanel>();
            int currentPage = 0;

            for (int i = 0; i < c; i++)
            {
                UIElement element = (toPrint as StackPanel).Children[0];
                (toPrint as StackPanel).Children.Remove(element);
                container.Children.Add(element);
                container.Width = pageSize.Width;

                container.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                container.Arrange(new Rect(new Point(0, 0), container.DesiredSize));

                if (container.DesiredSize.Height > (visibleSize.Height - 27))
                {
                    container.Children.Remove(element);

                    pages.Add(currentPage++, container);

                    container = new StackPanel();
                    container.Orientation = Orientation.Vertical;
                    container.HorizontalAlignment = HorizontalAlignment.Left;

                    ListGroupPeriodHeader fdrh = new ListGroupPeriodHeader();
                    container.Children.Add(fdrh);

                    container.Children.Add(element);
                }
                else if (i == (c - 1))
                {
                    pages.Add(currentPage++, container);
                }
            }

            double fullHeight = visibleSize.Height;

            for (int i = 0; i < pages.Count; i++)
            {
                pages[i].Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                pages[i].Arrange(new Rect(new Point(0, 0), container.DesiredSize));

                double deltaHeight = fullHeight - pages[i].DesiredSize.Height;
                int TopMargin = (int)(deltaHeight - 27);
                Thickness margin = new Thickness(35, TopMargin, 0, 0);
                _PageFooter pf = new _PageFooter(i + 1, pages.Count, title);
                pf.Margin = margin;
                pages[i].Children.Add(pf);
            }

            for (int i = 0; i < pages.Count; i++)
            {
                PageContent pageContent = new PageContent();
                FixedPage page = new FixedPage();
                ((IAddChild)pageContent).AddChild(page);
                result.Pages.Add(pageContent);
                page.Width = pageSize.Width;
                page.Height = pageSize.Height;
                page.Children.Add(pages[i]);

                FixedPage.SetLeft(pages[i], capabilities.PageImageableArea.OriginWidth);
                FixedPage.SetTop(pages[i], capabilities.PageImageableArea.OriginHeight);
            }

            return result;
        }


        private FixedDocument GetFixedDocument(FrameworkElement toPrint, PrintDialog printDialog)
        {
            printDialog.PrintTicket.PageOrientation = PageOrientation.Landscape;
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

                if (container.DesiredSize.Height > visibleSize.Height)
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

                    FullDayReportHeader fdrh = new FullDayReportHeader();
                    container.Children.Add(fdrh);

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
