using ClassLibrary;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfControlLibrary.ReportControls
{
    /// <summary>
    /// Логика взаимодействия для ListGroupPeriodSimple.xaml
    /// </summary>
    public partial class ListGroupPeriodSimple : UserControl, INotifyPropertyChanged
    {
        #region Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructor
        public ListGroupPeriodSimple(DisplayedGroupModel model)
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
            tb.Text = model.Groupdate.ToString("dd.MM yyyy");
            tb.TextWrapping = TextWrapping.Wrap;
            tb.Margin = new Thickness(5);
            tb.FontSize = 12;
            tb.TextAlignment = TextAlignment.Center;
            MainGrid.Children.Add(tb);
            Grid.SetColumn(tb, 0);

            tb = new TextBlock();
            tb.Text = model.Grouptime.ToString(@"hh\:mm");
            tb.TextWrapping = TextWrapping.Wrap;
            tb.Margin = new Thickness(5);
            tb.FontSize = 12;
            tb.TextAlignment = TextAlignment.Center;
            MainGrid.Children.Add(tb);
            Grid.SetColumn(tb, 1);

            tb = new TextBlock();
            tb.Text = model.Organizationname;
            tb.Margin = new Thickness(5);
            tb.TextWrapping = TextWrapping.Wrap;
            tb.FontSize = 12;
            MainGrid.Children.Add(tb);
            Grid.SetColumn(tb, 2);

            tb = new TextBlock();
            tb.Text = model.Geoname;
            tb.Margin = new Thickness(5);
            tb.TextWrapping = TextWrapping.Wrap;
            tb.FontSize = 12;
            MainGrid.Children.Add(tb);
            Grid.SetColumn(tb, 3);

            tb = new TextBlock();
            tb.Text = model.Groupamount;
            tb.TextWrapping = TextWrapping.Wrap;
            tb.Margin = new Thickness(5);
            tb.FontSize = 12;
            tb.TextAlignment = TextAlignment.Center;
            MainGrid.Children.Add(tb);
            Grid.SetColumn(tb, 4);

            tb = new TextBlock();
            tb.Text = model.Groupage;
            tb.TextWrapping = TextWrapping.Wrap;
            tb.Margin = new Thickness(5);
            tb.FontSize = 12;
            MainGrid.Children.Add(tb);
            Grid.SetColumn(tb, 5);

            tb = new TextBlock();
            tb.Text = model.Tourname;
            tb.TextWrapping = TextWrapping.Wrap;
            tb.Margin = new Thickness(5);
            tb.FontSize = 12;
            MainGrid.Children.Add(tb);
            Grid.SetColumn(tb, 6);

            tb = new TextBlock();
            tb.Text = model.Guidfullname;
            tb.TextWrapping = TextWrapping.Wrap;
            tb.Margin = new Thickness(5);
            tb.FontSize = 12;
            MainGrid.Children.Add(tb);
            Grid.SetColumn(tb, 7);

            tb = new TextBlock();
            tb.Text = Options.GetContactsByCommogGroupId(model.Idcommongroup);
            tb.TextWrapping = TextWrapping.Wrap;
            tb.Margin = new Thickness(5);
            tb.FontSize = 12;
            MainGrid.Children.Add(tb);
            Grid.SetColumn(tb, 8);

            if((model.Groupstatus & 1) > 0)
            {
                tb = new TextBlock();
                tb.Text = RichTextStripper.StripRichTextFormat(model.Groupdeletereason.Replace('#', '\\'));
                tb.TextWrapping = TextWrapping.Wrap;
                tb.Margin = new Thickness(5);
                tb.FontSize = 12;
                MainGrid.Children.Add(tb);
                Grid.SetColumn(tb, 9);

                tb = new TextBlock();
                tb.Text = "Отменена";
                tb.TextWrapping = TextWrapping.Wrap;
                tb.Margin = new Thickness(5);
                tb.FontSize = 12;
                MainGrid.Children.Add(tb);
                Grid.SetColumn(tb, 10);
            }
            else
            {
                tb = new TextBlock();
                tb.Text = RichTextStripper.StripRichTextFormat(model.Groupcomment.Replace('#', '\\'));
                tb.TextWrapping = TextWrapping.Wrap;
                tb.Margin = new Thickness(5);
                tb.FontSize = 12;
                MainGrid.Children.Add(tb);
                Grid.SetColumn(tb, 9);

                if((model.Groupstatus & 4) > 0)
                {
                    tb = new TextBlock();
                    tb.Text = "Проведена";
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
