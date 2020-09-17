using ClassLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UtilsLib.StatWizard;

namespace UtilsLib.ResultWindow
{
    /// <summary>
    /// Логика взаимодействия для StatGridWindow.xaml
    /// </summary>
    public partial class StatGridWindow : Window
    {
        #region Members

        private int m_colCount;

        string sql_type_1_period_1 =
            "select year(groupdate) as year, month(groupdate) as month, idcategory, sum(groupamount) as sum from " +
            "(select * from _groups where (groupdate between '{0}' and '{1}') and ((groupstatus & 4) = 4)) a " +
            "group by year(groupdate), idcategory, month(groupdate) order by year(groupdate), month(groupdate), idcategory";

        string sql_type_1_period_2 =
            "select idcategory, sum(groupamount) as sum from " +
            "(select* from _groups where (groupdate between '{0}' and '{1}') and((groupstatus & 4) = 4)) a " +
            "group by idcategory order by idcategory";

        string sql_type_2_period_1 =
            "select  year(groupdate) as year, month(groupdate) as month, sum(groupamount) as sum from " +
            "(select* from _groups where (groupdate between '{0}' and '{1}') and((groupstatus & 4) = 4)) a " +
            "group by year(groupdate), month(groupdate) order by year(groupdate), month(groupdate) ";

        string sql_type_2_period_2 =
            "select sum(groupamount) as sum from _groups where (groupdate between '{0}' and '{1}') and ((groupstatus & 4) = 4)";        

        #endregion

        #region Constructor
        public StatGridWindow()
        {
            InitializeComponent();

            if (DataForStatWizard.ReportType == 1)
                Type1_report();
            if (DataForStatWizard.ReportType == 2)
                Type2_report();
        }
        #endregion

        #region Type 1 report
        private void Type1_report()
        {           
            int PeriodType = DataForStatWizard.PeriodType;
            LegendStackPanel.Children.Clear();

            #region По месяцам
            if (PeriodType == 1)
            {
                DataTable resultTable = CalculateResultTable();
                string sqlString = String.Format(sql_type_1_period_1,
                    DataForStatWizard.periods[0].StartDate.ToString("yyyy-MM-dd"),
                    DataForStatWizard.periods[0].EndDate.ToString("yyyy-MM-dd"));

                DataTable tmp = DBWrapper.MySqlWrapper.Select(sqlString);
                Dictionary<string, Dictionary<int, int>> tmpDictionary =
                    new Dictionary<string, Dictionary<int, int>>();
                for(int i = 0; i < tmp.Rows.Count; i++)
                {
                    int year =      int.Parse(tmp.Rows[i]["year"].ToString());
                    int month =     int.Parse(tmp.Rows[i]["month"].ToString());
                    string key =    $"{year}_{month}";
                    int idcat =     int.Parse(tmp.Rows[i]["idcategory"].ToString());
                    int sum = int.Parse(tmp.Rows[i]["sum"].ToString());                    
                    if (DataForStatWizard.categoryList.Contains(idcat))
                    {
                        if (!tmpDictionary.ContainsKey(key))
                            tmpDictionary.Add(key, new Dictionary<int, int>());

                        tmpDictionary[key].Add(idcat, sum);
                    }
                }

                for(int i = 0; i < tmpDictionary.Count; i++)
                {
                    DataRow dr = resultTable.NewRow();

                    int monthNum = int.Parse(tmpDictionary.ElementAt(i).Key.Split('_')[1]);
                    int yearNum = int.Parse(tmpDictionary.ElementAt(i).Key.Split('_')[0]);
                    string monthName = new DateTime(200, monthNum, 1)
                            .ToString("MMMM", CultureInfo.CreateSpecificCulture("ru")); 
                    dr["period"] = $"{monthName} {yearNum} г.";
                    Dictionary<int, int> categ = tmpDictionary[tmpDictionary.ElementAt(i).Key];
                    for(int j = 0; j < categ.Count; j++)
                    {
                        string catname = $"cat{categ.ElementAt(j).Key}";
                        dr[catname] = categ[categ.ElementAt(j).Key].ToString();
                    }
                    resultTable.Rows.Add(dr);
                }
                MainGrid.ItemsSource = resultTable.DefaultView;
                CreateFooter();
            }
            #endregion

            #region Произвольный период

            if (PeriodType == 2)
            {
                DataTable resultTable = CalculateResultTable();
                Dictionary<string, Dictionary<int, int>> tmpDictionary =
                  new Dictionary<string, Dictionary<int, int>>();

                for(int per = 0; per < DataForStatWizard.periods.Count; per++)
                {
                    string sqlString = String.Format(sql_type_1_period_2,
                        DataForStatWizard.periods[per].StartDate.ToString("yyyy-MM-dd"),
                        DataForStatWizard.periods[per].EndDate.ToString("yyyy-MM-dd"));
                    DataTable tmp = DBWrapper.MySqlWrapper.Select(sqlString);
                    tmpDictionary.Add($"Период {per + 1}", new Dictionary<int, int>());
                    for (int cat = 0; cat < tmp.Rows.Count; cat++)
                    {
                        if(DataForStatWizard.categoryList.Contains(int.Parse(tmp.Rows[cat]["idcategory"].ToString())))
                        tmpDictionary[$"Период {per + 1}"].Add(
                            int.Parse(tmp.Rows[cat]["idcategory"].ToString()),
                            int.Parse(tmp.Rows[cat]["sum"].ToString()));
                    }
                }

                for (int i = 0; i < tmpDictionary.Count; i++)
                {
                    DataRow dr = resultTable.NewRow();

                    dr["period"] = tmpDictionary.ElementAt(i).Key;
                    Dictionary<int, int> categ = tmpDictionary[tmpDictionary.ElementAt(i).Key];
                    for (int j = 0; j < categ.Count; j++)
                    {
                        string catname = $"cat{categ.ElementAt(j).Key}";
                        dr[catname] = categ[categ.ElementAt(j).Key].ToString();
                    }
                    resultTable.Rows.Add(dr);
                }

                for (int i = 0; i < DataForStatWizard.periods.Count; i ++)
                {
                    TextBlock tb = new TextBlock();

                    string text = $"Период {i+1}{Environment.NewLine}";
                    text += $"     C {DataForStatWizard.periods[i].StartDate.ToString("yyyy.MM.dd")} по " +
                        $"{DataForStatWizard.periods[i].EndDate.ToString("yyyy.MM.dd")}";
                    tb.TextWrapping = TextWrapping.Wrap;
                    tb.Text = text;
                    tb.Margin = new Thickness(3);

                    LegendStackPanel.Children.Add(tb);
                }

                CreateFooter();

                MainGrid.ItemsSource = resultTable.DefaultView;
            }
            #endregion            
        }
        #endregion

        #region Type 2 report
        private void Type2_report()
        {
            int PeriodType = DataForStatWizard.PeriodType;
            LegendStackPanel.Children.Clear();

            #region По месяцам
            if (PeriodType == 1)
            {
                DataTable resultTable = CalculateResultTable_2();
                string sqlString = String.Format(sql_type_2_period_1,
                    DataForStatWizard.periods[0].StartDate.ToString("yyyy-MM-dd"),
                    DataForStatWizard.periods[0].EndDate.ToString("yyyy-MM-dd"));

                DataTable tmp = DBWrapper.MySqlWrapper.Select(sqlString);
                for(int i = 0; i < tmp.Rows.Count; i ++)
                {
                    int year =      int.Parse(tmp.Rows[i]["year"].ToString());
                    int month =     int.Parse(tmp.Rows[i]["month"].ToString());
                    string monthName = new DateTime(200, month, 1)
                        .ToString("MMMM", CultureInfo.CreateSpecificCulture("ru"));
                    int sum = int.Parse(tmp.Rows[i]["sum"].ToString());
                    DataRow dr = resultTable.NewRow();
                    dr["period"] = $"{monthName} {year} г.";
                    dr["sum"] = sum.ToString();
                    resultTable.Rows.Add(dr);                    
                }

                MainGrid.ItemsSource = resultTable.DefaultView;

                CreateFooter();
            }
            #endregion

            #region Произвольный период
            if (PeriodType == 2)
            {
                DataTable resultTable = CalculateResultTable_2();

                for(int i = 0; i < DataForStatWizard.periods.Count; i++)
                {
                    string start = DataForStatWizard.periods[i].StartDate.ToString("yyyy-MM-dd");
                    string end = DataForStatWizard.periods[i].EndDate.ToString("yyyy-MM-dd");
                    string sqlString = String.Format(sql_type_2_period_2, start, end);

                    DataTable tmp = DBWrapper.MySqlWrapper.Select(sqlString);
                    int sum = 0;
                    if (tmp.Rows.Count > 0)
                        sum = int.Parse(tmp.Rows[0]["sum"].ToString());

                    DataRow dr = resultTable.NewRow();
                    dr["period"] = $"Период {i + 1}";
                    dr["sum"] = sum.ToString();
                    resultTable.Rows.Add(dr);
                }

                for (int i = 0; i < DataForStatWizard.periods.Count; i++)
                {
                    TextBlock tb = new TextBlock();

                    string text = $"Период {i + 1}{Environment.NewLine}";
                    text += $"     C {DataForStatWizard.periods[i].StartDate.ToString("yyyy.MM.dd")} по " +
                        $"{DataForStatWizard.periods[i].EndDate.ToString("yyyy.MM.dd")}";
                    tb.TextWrapping = TextWrapping.Wrap;
                    tb.Text = text;
                    tb.Margin = new Thickness(3);

                    LegendStackPanel.Children.Add(tb);
                }

                MainGrid.ItemsSource = resultTable.DefaultView;
                CreateFooter();
            }
            #endregion

        }
        #endregion

        #region CalculateResultTable For Type 1 Report
        private DataTable CalculateResultTable()
        {
            DataTable result = new DataTable();

            int categoryCount = DataForStatWizard.categoryList.Count;
            m_colCount = categoryCount + 1;

            DataColumn dc = new DataColumn();
            dc.ColumnName = "period";
            dc.DataType = typeof(string);
            result.Columns.Add(dc);

            DataGridTextColumn dtColumn = new DataGridTextColumn()
            {
                Header = "Период",
                Binding = new Binding("period"),
                Width = 100
            };

            MainGrid.Columns.Add(dtColumn);

            for (int i = 0; i < categoryCount; i++)
            {
                DataColumn catColumn = new DataColumn();
                catColumn.DataType = typeof(string);
                catColumn.ColumnName = $"cat{DataForStatWizard.categoryList[i]}";
                result.Columns.Add(catColumn);

                DataGridTextColumn dtCol = new DataGridTextColumn()
                {
                    Header = Options.GetCategName(DataForStatWizard.categoryList[i]),
                    Binding = new Binding($"cat{DataForStatWizard.categoryList[i]}"),
                    Width = 80
                };
                MainGrid.Columns.Add(dtCol);
            }

            return result;
        }
        #endregion

        #region CalculateResultTable For Type 2 Report
        private DataTable CalculateResultTable_2()
        {
            DataTable result = new DataTable();

            m_colCount = 2;
            DataColumn dc = new DataColumn();
            dc.ColumnName = "period";
            dc.DataType = typeof(string);
            result.Columns.Add(dc);

            DataGridTextColumn dtColumn = new DataGridTextColumn()
            {
                Header = "Период",
                Binding = new Binding("period"),
                Width = 100
            };
            MainGrid.Columns.Add(dtColumn);

            dc = new DataColumn();
            dc.ColumnName = "sum";
            dc.DataType = typeof(int);
            result.Columns.Add(dc);

            dtColumn = new DataGridTextColumn()
            {
                Header = "Всего за период",
                Binding = new Binding("sum"),
                Width = 100
            };
            MainGrid.Columns.Add(dtColumn);

            return result;
        }
        #endregion

        private void CreateFooter()
        {
            Dictionary<int, int> counter = new Dictionary<int, int>();

            for(int i = 0; i < MainGrid.Items.Count; i++)
            {
                DataRowView drv = MainGrid.Items[i] as DataRowView;
                for(int j = 1; j < m_colCount; j++)
                {
                    if (!counter.ContainsKey(j))
                        counter.Add(j, 0);

                    int value = 0;
                    bool existNum = int.TryParse(drv[j].ToString(), out value);
                    if (existNum)
                        counter[j] += value;
                }
            }

            TextBlock tb = new TextBlock();
            tb.FontWeight = FontWeights.Bold;
            tb.FontSize = 14;
            tb.Width = 100;
            tb.Text = "Итого: ";

            ItogoSP.Children.Add(tb);

            for(int i = 0; i < counter.Count; i++)
            {
                tb = new TextBlock();
                tb.FontWeight = FontWeights.Bold;
                tb.FontSize = 14;
                tb.Width = 80;
                tb.Text = counter.ElementAt(i).Value.ToString();
                ItogoSP.Children.Add(tb);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
