using ClassLibrary;
using ClassLibrary.Models;
using DBWrapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using WpfControlLibrary.ReportControls;

namespace ReportLib.PeriodReport
{
    public class PeriodReport_
    {
        #region Members

        private FixedDocument m_document;
        private bool m_calcEvents;

        public FixedDocument Document { get => m_document; set => m_document = value; }
        #endregion

        #region Constructor
        public PeriodReport_(DateTime sp, DateTime ep, bool ce)
        {
            m_calcEvents = ce;
            string sql_guid = $"call GetGuidSum('{sp.ToMySqlDateString()}','{ep.ToMySqlDateString()}')";
            string sql_expo = $"call GetExpoSum('{sp.ToMySqlDateString()}','{ep.ToMySqlDateString()}')";

            DataTable guid = MySqlWrapper.Select(sql_guid);
            DataTable expo = MySqlWrapper.Select(sql_expo);

            if (m_calcEvents)
            {
                Dictionary<int, int> guidEventDict = Options.GetEventGuidCounter(sp, ep);
                Dictionary<int, int> evenEventDict = Options.GetEventCounter(sp, ep);

                for(int i = 0; i < evenEventDict.Count; i++)
                {
                    EventModel em = new EventModel(evenEventDict.ElementAt(i).Key);
                    DataRow dr = expo.NewRow();
                    if (em.Eventtype == "КВ")
                        dr["tourname"] = "Квест: " + em.Eventname;
                    if (em.Eventtype == "МК")
                        dr["tourname"] = "Мастер класс: " + em.Eventname;
                    if (em.Eventtype == "Л")
                        dr["tourname"] = "Лекция: " + em.Eventname;
                    if (em.Eventtype == "СМ")
                        dr["tourname"] = "Семинар: " + em.Eventname;
                    dr["count"] = evenEventDict[evenEventDict.ElementAt(i).Key];
                    expo.Rows.Add(dr);
                }

                for(int i = 0; i < guid.Rows.Count; i++)
                {
                    DataRow dr = guid.Rows[i];
                    int idguid = int.Parse(dr["idguid"].ToString());
                    if(guidEventDict.ContainsKey(idguid))
                    {
                        int sumGuid = int.Parse(dr["sum"].ToString());
                        int total = sumGuid + guidEventDict[idguid];
                        dr["sum"] = total;
                        guidEventDict.Remove(idguid);
                    }
                }

                for(int i = 0; i < guidEventDict.Count; i++)
                {
                    DataRow dr = guid.NewRow();
                    dr["idguid"] = guidEventDict.ElementAt(i).Key;
                    dr["sum"] = guidEventDict.ElementAt(i).Value;
                    dr["guidfullname"] = Options.GetGuidName(guidEventDict.ElementAt(i).Key);
                    guid.Rows.Add(dr);
                }
            }

            StackPanel container = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            string header = $"\nОтчет о проведенных экскурсиях за период" +
                $" с {sp.ToString("dd.MM.yyyy")} по {ep.ToString("dd.MM.yyyy")} .\n";

            TextBlock tb = new TextBlock()
            {
                Width = 600,
                TextWrapping = TextWrapping.Wrap,
                Text = header,
                FontSize = 22,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center
            };
            container.Children.Add(tb);

            int sum = 0;

            for(int i = 0; i < guid.Rows.Count; i++)
            {
                string n = guid.Rows[i]["guidfullname"].ToString();
                int cnt = int.Parse(guid.Rows[i]["sum"].ToString());
                string c = cnt.ToString();
                sum += cnt;

                SimpleGuidPeriodControl sgpc = new SimpleGuidPeriodControl(n,c);
                if ((i % 2) == 0) sgpc.Back = Brushes.White;
                else sgpc.Back = Brushes.LightGray;
                container.Children.Add(sgpc);
            }

            SimpleItogPeriodControl sipc = new SimpleItogPeriodControl(sum);
            container.Children.Add(sipc);

            sum = 0;

            for (int i = 0; i < expo.Rows.Count; i++)
            {
                string n = expo.Rows[i]["tourname"].ToString();
                int cnt = int.Parse(expo.Rows[i]["count"].ToString());
                string c = cnt.ToString();
                sum += cnt;

                SimpleTourPeriodControl stpc = new SimpleTourPeriodControl(n, c);
                if ((i % 2) == 0) stpc.Back = Brushes.White;
                else stpc.Back = Brushes.LightGray;
                container.Children.Add(stpc);
            }

            sipc = new SimpleItogPeriodControl(sum);
            container.Children.Add(sipc);

            m_document = GetFixedDocument(container, new PrintDialog());
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

            StackPanel container = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Left
            };

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
                    FixedPage page = new FixedPage()
                    {
                        Width = pageSize.Width,
                        Height = pageSize.Height
                    };
                    ((IAddChild)pageContent).AddChild(page);
                    result.Pages.Add(pageContent);
                    page.Children.Add(container);

                    FixedPage.SetLeft(container, capabilities.PageImageableArea.OriginWidth);
                    FixedPage.SetTop(container, capabilities.PageImageableArea.OriginHeight);

                    container = new StackPanel()
                    {
                        Orientation = Orientation.Vertical,
                        HorizontalAlignment = HorizontalAlignment.Left
                    };

                    FullDayReportHeader fdrh = new FullDayReportHeader();
                    container.Children.Add(fdrh);
                    container.Children.Add(element);
                }
                else if (i == (c - 1))
                {
                    PageContent pageContent = new PageContent();
                    FixedPage page = new FixedPage()
                    {
                        Width = pageSize.Width,
                        Height = pageSize.Height
                    };
                    ((IAddChild)pageContent).AddChild(page);
                    page.Children.Add(container);
                    result.Pages.Add(pageContent);

                    FixedPage.SetLeft(container, capabilities.PageImageableArea.OriginWidth);
                    FixedPage.SetTop(container, capabilities.PageImageableArea.OriginHeight);
                }
            }

            return result;
        }

        #endregion
    }
}
