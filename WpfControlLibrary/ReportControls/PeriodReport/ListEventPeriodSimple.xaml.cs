using ClassLibrary;
using ClassLibrary.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfControlLibrary.ReportControls.PeriodReport
{
    /// <summary>
    /// Логика взаимодействия для ListEventPeriodSimple.xaml
    /// </summary>
    public partial class ListEventPeriodSimple : UserControl
    { 
        #region Constructor
        public ListEventPeriodSimple(EventGroupModel model)
        {
            InitializeComponent();
            this.DataContext = this;

            for (int i = 0; i < 11; i++)
            {
                Border brd = new Border();
                Thickness t = (i == 10) ? new Thickness(1, 0, 1, 1) : new Thickness(0, 0, 1, 0);
                brd.BorderThickness = t;
                brd.BorderBrush = Brushes.Black;
                Grid.SetColumnSpan(brd, i + 1);
                MainGrid.Children.Add(brd);
            }

            TextBlock tb = new TextBlock();
            tb.Text = model.Eventgroupdate.ToString("dd.MM yyyy");
            tb.TextWrapping = TextWrapping.Wrap;
            tb.Margin = new Thickness(5);
            tb.FontSize = 12;
            tb.TextAlignment = TextAlignment.Center;
            MainGrid.Children.Add(tb);
            Grid.SetColumn(tb, 0);

            tb = new TextBlock();
            tb.Text = model.Eventgrouptime.ToString(@"hh\:mm");
            tb.TextWrapping = TextWrapping.Wrap;
            tb.Margin = new Thickness(5);
            tb.FontSize = 12;
            tb.TextAlignment = TextAlignment.Center;
            MainGrid.Children.Add(tb);
            Grid.SetColumn(tb, 1);

            tb = new TextBlock();
            tb.Text = (new OrganizationModel(model.Idorganization)).Organizationname;
            tb.Margin = new Thickness(5);
            tb.TextWrapping = TextWrapping.Wrap;
            tb.FontSize = 12;
            MainGrid.Children.Add(tb);
            Grid.SetColumn(tb, 2);

            tb = new TextBlock();
            tb.Text = (new GeoModel(model.Idgeo)).Geoname;
            tb.Margin = new Thickness(5);
            tb.TextWrapping = TextWrapping.Wrap;
            tb.FontSize = 12;
            MainGrid.Children.Add(tb);
            Grid.SetColumn(tb, 3);

            tb = new TextBlock();
            tb.Text = model.Eventgroupamount.ToString();
            tb.TextWrapping = TextWrapping.Wrap;
            tb.Margin = new Thickness(5);
            tb.FontSize = 12;
            tb.TextAlignment = TextAlignment.Center;
            MainGrid.Children.Add(tb);
            Grid.SetColumn(tb, 4);

            tb = new TextBlock();
            tb.Text = model.Eventgroupage;
            tb.TextWrapping = TextWrapping.Wrap;
            tb.Margin = new Thickness(5);
            tb.FontSize = 12;
            MainGrid.Children.Add(tb);
            Grid.SetColumn(tb, 5);

            tb = new TextBlock();
            int id = model.Idevent;
            EventModel em = new EventModel(id);
            string tt = "";
            switch(em.Eventtype)
            {
                case "КВ":      tt += "Квест: ";        break;
                case "МК":      tt += "Мастеркласс: ";  break;
                case "Л":       tt += "Лекция: ";       break;
            }
            tt += em.Eventname;
            tb.Text = tt;
            tb.TextWrapping = TextWrapping.Wrap;
            tb.Margin = new Thickness(5);
            tb.FontSize = 12;
            MainGrid.Children.Add(tb);
            Grid.SetColumn(tb, 6);

            tb = new TextBlock();
            tb.Text = Options.GetGuidStringByIds(model.GetListMasters());
            tb.TextWrapping = TextWrapping.Wrap;
            tb.Margin = new Thickness(5);
            tb.FontSize = 12;
            MainGrid.Children.Add(tb);
            Grid.SetColumn(tb, 7);

            if ((model.Eventgroupstatus & 1) > 0)
            {
                tb = new TextBlock();
                tb.Text = "Отменено";
                tb.TextWrapping = TextWrapping.Wrap;
                tb.Margin = new Thickness(5);
                tb.FontSize = 12;
                MainGrid.Children.Add(tb);
                Grid.SetColumn(tb, 10);
            }
            else
            {
                if ((model.Eventgroupstatus & 4) > 0)
                {
                    tb = new TextBlock();
                    tb.Text = "Проведено";
                    tb.TextWrapping = TextWrapping.Wrap;
                    tb.Margin = new Thickness(5);
                    tb.FontSize = 12;
                    MainGrid.Children.Add(tb);
                    Grid.SetColumn(tb, 10);
                }
            }
        }
        #endregion
    }
}
