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
using WpfControlLibrary.ReportControls;
using WpfControlLibrary.ReportControls.PeriodReport;

namespace ReportLib.PeriodReport
{
    public class ListGroupReport
    {
        #region Members
        private FixedDocument m_document;
        private bool m_showEvents;
        private bool m_showTours;
        string title = "";
        public FixedDocument Document { get => m_document; set => m_document = value; }
        #endregion

        #region Constructor
        public ListGroupReport(DateTime sp, DateTime ep, bool showCompleted, bool showTours, bool showEvents)
        {
            m_showEvents = showEvents;
            m_showTours = showTours;
            StackPanel container = new StackPanel();
            container.Orientation = Orientation.Vertical;
            container.HorizontalAlignment = HorizontalAlignment.Left;

            title = $"Список экскурсий (мероприятий) за период  с {sp.ToString("dd.MM.yyyy")} по {ep.ToString("dd.MM.yyyy")}\n";
            TextBlock tb = new TextBlock();
            tb.Text = title;
            tb.TextWrapping = TextWrapping.Wrap;
            tb.TextAlignment = TextAlignment.Center;
            tb.FontSize = 20;
            tb.FontWeight = FontWeights.Bold;
            container.Children.Add(tb);

            ListGroupPeriodHeader head = new ListGroupPeriodHeader();
            container.Children.Add(head);

            List<DisplayedGroupModel> groups = (List<DisplayedGroupModel>)
                MySqlWrapper.SelectGroupBetweenDate(sp, ep).ToList<DisplayedGroupModel>();
            List<EventGroupModel> events = (List<EventGroupModel>)
                MySqlWrapper.SelectEventBetweenDate(sp, ep).ToList<EventGroupModel>();


            SortedDictionary<DateTime, List<UIElement>> group_event = new SortedDictionary<DateTime, List<UIElement>>();

            if(showTours)
                for(int i = 0; i < groups.Count; i++)
                {
                    DateTime tmp = groups[i].Groupdate;

                    if (!group_event.ContainsKey(tmp))
                        group_event.Add(tmp, new List<UIElement>());

                    group_event[tmp].Add(new ListGroupPeriodSimple(groups[i]));
                }
            if(showEvents)
                for(int i = 0; i < events.Count; i++)
                {
                    DateTime tmp = events[i].Eventgroupdate;

                    if (!group_event.ContainsKey(tmp))
                        group_event.Add(tmp, new List<UIElement>());

                    group_event[tmp].Add(new ListEventPeriodSimple(events[i]));
                }
            
            for(int i = 0; i < group_event.Count; i++)
            {
                for(int j = 0; j < group_event.ElementAt(i).Value.Count; j++)
                {
                    container.Children.Add(group_event.ElementAt(i).Value[j]);
                }
            }

            //for (int i = 0; i < groups.Count; i++)
            //{
            //    DisplayedGroupModel model = groups[i];

            //    if (showCompleted && ((model.Groupstatus & 1) > 0)) continue;

            //    ListGroupPeriodSimple simple = new ListGroupPeriodSimple(model);
            //    container.Children.Add(simple);
            //}

            m_document = GetFixedDocumentNew(container, new PrintDialog());
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

            for(int i = 0; i < c; i++)
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

            for(int i = 0; i < pages.Count; i++)
            {
                pages[i].Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                pages[i].Arrange(new Rect(new Point(0, 0), container.DesiredSize));

                double deltaHeight = fullHeight - pages[i].DesiredSize.Height;
                int TopMargin = (int)(deltaHeight - 27);
                Thickness margin = new Thickness(35, TopMargin, 0, 0);
                _PageFooter pf = new _PageFooter(i+1, pages.Count, title);
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

                if (container.DesiredSize.Height > (visibleSize.Height))
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

                    ListGroupPeriodHeader fdrh = new ListGroupPeriodHeader();
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
