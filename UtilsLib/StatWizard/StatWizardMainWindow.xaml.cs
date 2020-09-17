using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using WpfControlLibrary.PeriodSelector;
using DBWrapper;
using UtilsLib.ResultWindow;

namespace UtilsLib.StatWizard
{
    /// <summary>
    /// Логика взаимодействия для StatWizardMainWindow.xaml
    /// </summary>
    public partial class StatWizardMainWindow : Window
    {

        #region Members

        private bool loaded = false;

        private int currentIndex = 0;
        private int currentPage = 0;

        private static List<int> type1 = new List<int>() { 0,1,2 };
        private static List<int> type2 = new List<int>() { 0,2 };
        private static List<List<int>> MasterSequence = new List<List<int>>() { type1, type2 };

        #endregion

        #region Constructor
        public StatWizardMainWindow()
        {
            InitializeComponent();

            psel.Mode = 0;

            #region Categories
            string sql = "select * from categories";
            DataTable dt = MySqlWrapper.Select(sql);

            for(int i = 0; i < dt.Rows.Count; i++)
            {
                int id = int.Parse(dt.Rows[i]["idcategorie"].ToString());
                string cat = dt.Rows[i]["categoryname"].ToString();
                CheckBox cb = new CheckBox();
                cb.Tag = id;
                cb.Content = cat;
                CategoryListBox.Items.Add(cb);
            }
            #endregion

            Style s = new Style();
            s.Setters.Add(new Setter(UIElement.VisibilityProperty, Visibility.Collapsed));
            tc.ItemContainerStyle = s;

            CheckButtons();
        }
        #endregion

        #region Select period type
        private void Step3_1_Checked(object sender, RoutedEventArgs e)
        {
            if (!loaded) return;
            string n = (sender as RadioButton).Name;
            switch(n)
            {
                case "Step3_1":
                    psel.Visibility = Visibility.Visible;
                    PeriodListBox.Visibility = Visibility.Collapsed;
                    break;
                case "Step3_2":
                    psel.Visibility = Visibility.Collapsed;
                    PeriodListBox.Visibility = Visibility.Visible;

                    PeriodListBox.Items.Clear();
                    PeriodSelector ps = new PeriodSelector();
                    ps.AddPeriodSelectorClick += Ps_AddPeriodSelectorClick;
                    ps.Mode = 1;
                    ps.Margin = new Thickness(3, 0, 3, 0);
                    PeriodListBox.Items.Add(ps);

                    break;
            }
        }

        private void Ps_AddPeriodSelectorClick(object sender, EventArgs e)
        {
            PeriodSelector ps = new PeriodSelector();
            ps.AddPeriodSelectorClick += Ps_AddPeriodSelectorClick;
            ps.Mode = 1;
            ps.Margin = new Thickness(3, 0, 3, 0);
            PeriodListBox.Items.Add(ps);
        }
        #endregion

        #region Events
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loaded = true;
            psel.Visibility = Visibility.Visible;
            PeriodListBox.Visibility = Visibility.Collapsed;
        }

        private void rb_Checked(object sender, RoutedEventArgs e)
        {
            string rb_name = (sender as RadioButton).Name;
            switch(rb_name)
            {
                case "rb_0":
                    if ((bool)rb_0.IsChecked)
                    {
                        currentIndex = 0;
                    }
                    break;
                case "rb_1":
                    if ((bool)rb_1.IsChecked)
                    {
                        currentIndex = 1;
                    }
                    break;
            }
        }

        private void PrevPageBt_Click(object sender, RoutedEventArgs e)
        {
            List<int> tmpSeq = MasterSequence[currentIndex];
            if (currentPage != 0)
            {
                currentPage--;
                tc.SelectedIndex = tmpSeq[currentPage];
                TabItem ti = tc.Items[currentPage] as TabItem;
                ti.IsSelected = true;
            }
            CheckButtons();
        }

        private void NextPageBt_Click(object sender, RoutedEventArgs e)
        {
            List<int> tmpSeq = MasterSequence[currentIndex];
            if (currentPage != (tmpSeq.Count - 1))
            {
                currentPage++;
                tc.SelectedIndex = tmpSeq[currentPage];
            }
            CheckButtons();
        }

        private void CheckButtons()
        {
            NextPageBt.IsEnabled = false;
            PrevPageBt.IsEnabled = false;
            FinishBt.IsEnabled = false;

            int counterPage = MasterSequence[currentIndex].Count;
            if (currentPage == 0)
            {
                NextPageBt.IsEnabled = true;
            }
            if (currentPage == (counterPage - 1))
            {
                PrevPageBt.IsEnabled = true;
                FinishBt.IsEnabled = true;
            }
            if ((currentPage > 0) && (currentPage < (counterPage - 1)))
            {
                NextPageBt.IsEnabled = true;
                PrevPageBt.IsEnabled = true;
            }
        }

        private void FinishBt_Click(object sender, RoutedEventArgs e)
        {
            DataForStatWizard.periods = CalculatePeriods();
            DataForStatWizard.categoryList = CalculateCategory();
            DataForStatWizard.PeriodType = CalculatePeriodType();
            DataForStatWizard.ReportType = CalculateReportType();

            StatGridWindow sgw = new StatGridWindow();
            sgw.ShowDialog();
            
            DialogResult = true;
        }

        private int CalculateReportType()
        {
            int result = 0;

            if ((bool)rb_0.IsChecked)
                result = 1;
            if ((bool)rb_1.IsChecked)
                result = 2;

            return result;
        }
        private int CalculatePeriodType()
        {
            int result = 0;

            if ((bool)Step3_1.IsChecked)
                result = 1;
            if ((bool)Step3_2.IsChecked)
                result = 2;

            return result;
        }
        private List<int> CalculateCategory()
        {
            List<int> result = new List<int>();

            for(int i = 0; i < CategoryListBox.Items.Count; i++)
            {
                CheckBox cb = CategoryListBox.Items[i] as CheckBox;
                int idcat = (int)cb.Tag;
                if((bool)cb.IsChecked)
                    result.Add(idcat);
            }

            return result;
        }
        private List<Period> CalculatePeriods()
        {
            List<Period> result = new List<Period>();

            if ((bool)Step3_1.IsChecked)
            {
                Period p = new Period();
                p.StartDate = psel.BeginDate;
                p.EndDate = psel.EndDate;
                result.Add(p);
            }
            if ((bool)Step3_2.IsChecked)
            {
                for(int i = 0; i < PeriodListBox.Items.Count; i++)
                {
                    PeriodSelector ps = PeriodListBox.Items[i] as PeriodSelector;
                    Period p = new Period();
                    p.StartDate = ps.BeginDate;
                    p.EndDate = ps.EndDate;

                    result.Add(p);
                }
            }

            return result;
        }

        #endregion
    }
}
