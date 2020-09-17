using ClassLibrary;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfControlLibrary.ReportControls
{
    /// <summary>
    /// Логика взаимодействия для FullDayReportSimpleGroup.xaml
    /// </summary>
    public partial class FullDayReportSimpleGroup : UserControl
    {
        #region Members
        private List<DisplayedGroupModel> m_models;
        #endregion

        #region Constructor
        public FullDayReportSimpleGroup(List<DisplayedGroupModel> models)
        {
            m_models = models;

            InitializeComponent();

            for(int i = 0; i < m_models.Count; i++)
            {
                RowDefinition rd = new RowDefinition();                
                MainGrid.RowDefinitions.Add(rd);
            }

            for(int i = 0; i < 11; i++)
            {
                Border brd = new Border();
                Thickness t = (i == 10) ? new Thickness(1,0,1,1) : new Thickness(0,0,1,0) ;
                brd.BorderThickness = t;
                brd.BorderBrush = Brushes.Black;
                Grid.SetRowSpan(brd, m_models.Count);
                Grid.SetColumnSpan(brd, i+1);
                MainGrid.Children.Add(brd);
            }

            for(int i = 0; i < m_models.Count; i++)
            {
                #region Время
                string time = m_models[i].Grouptime.ToString(@"hh\:mm");
                TextBlock l = new TextBlock();
                l.Margin = new Thickness(3,0,0,0);
                l.Text = time;
                l.FontSize = 12;
                Grid.SetRow(l, i);
                Grid.SetColumn(l, 0);
                MainGrid.Children.Add(l);
                #endregion

                #region Кол-во человек
                string amount = m_models[i].Groupamount;
                l = new TextBlock();
                l.Margin = new Thickness(3, 0, 0, 0);
                l.Text = amount;
                l.FontSize = 12;
                Grid.SetRow(l, i);
                Grid.SetColumn(l, 4);
                MainGrid.Children.Add(l);
                #endregion

                #region Возраст
                string age = m_models[i].Groupage;
                TextBlock tb = new TextBlock();
                tb.Margin = new Thickness(3,0,0,0);
                tb.Text = age;
                tb.TextWrapping = TextWrapping.Wrap;
                tb.FontSize = 12;
                Grid.SetRow(tb, i);
                Grid.SetColumn(tb, 5);
                MainGrid.Children.Add(tb);
                #endregion

                #region Экспозиция
                string expo = m_models[i].Tourname;
                tb = new TextBlock();
                tb.Margin = new Thickness(3, 0, 0, 0);
                tb.Text = expo;
                tb.TextWrapping = TextWrapping.Wrap;
                tb.FontSize = 12;
                Grid.SetRow(tb, i);
                Grid.SetColumn(tb, 6);
                MainGrid.Children.Add(tb);
                #endregion

                #region Экскурсовод
                string guid = m_models[i].Guidfullname;
                tb = new TextBlock();
                tb.Margin = new Thickness(3, 0, 0, 0);
                tb.Text = guid;
                tb.TextWrapping = TextWrapping.Wrap;
                tb.FontSize = 12;
                Grid.SetRow(tb, i);
                Grid.SetColumn(tb, 7);
                MainGrid.Children.Add(tb);
                #endregion

                if ((m_models[i].Groupstatus & 1) > 0)
                {
                    #region Причина отказа
                    string comment = RichTextStripper.StripRichTextFormat(m_models[i].Groupdeletereason.Replace('#','\\'));
                    tb = new TextBlock();
                    tb.Margin = new Thickness(3, 0, 0, 0);
                    tb.Text = comment;
                    tb.TextWrapping = TextWrapping.Wrap;
                    tb.FontSize = 12;
                    Grid.SetRow(tb, i);
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
                    Grid.SetRow(tb, i);
                    Grid.SetColumn(tb, 10);
                    MainGrid.Children.Add(tb);
                    #endregion
                }
                else
                {
                    #region Комментарии
                    string comment = RichTextStripper.StripRichTextFormat(m_models[i].Groupcomment.Replace('#', '\\'));
                    tb = new TextBlock();
                    tb.Margin = new Thickness(3, 0, 0, 0);
                    tb.Text = comment;
                    tb.TextWrapping = TextWrapping.Wrap;
                    tb.FontSize = 12;
                    Grid.SetRow(tb, i);
                    Grid.SetColumn(tb, 9);
                    MainGrid.Children.Add(tb);
                    #endregion

                    if ((m_models[i].Groupstatus & 4) > 0)
                    {
                        #region Отметка
                        string otm = "Проведена";
                        tb = new TextBlock();
                        tb.Margin = new Thickness(3, 0, 0, 0);
                        tb.Text = otm;
                        tb.TextWrapping = TextWrapping.Wrap;
                        tb.FontSize = 12;
                        Grid.SetRow(tb, i);
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
                        Grid.SetRow(tb, i);
                        Grid.SetColumn(tb, 10);
                        MainGrid.Children.Add(tb);
                        #endregion
                    }
                }

                if(i == 0)
                {
                    #region Кол-во групп
                    string cnt = m_models.Count.ToString();
                    tb = new TextBlock();
                    tb.Margin = new Thickness(3, 0, 0, 0);
                    tb.Text = cnt;
                    tb.TextWrapping = TextWrapping.Wrap;
                    tb.FontSize = 12;
                    Grid.SetRow(tb, i);
                    Grid.SetRowSpan(tb,m_models.Count);
                    Grid.SetColumn(tb, 1);
                    MainGrid.Children.Add(tb);
                    #endregion

                    #region Организация
                    string org = m_models[i].Organizationname;
                    tb = new TextBlock();
                    tb.Margin = new Thickness(3, 0, 0, 0);
                    tb.Text = org;
                    tb.TextWrapping = TextWrapping.Wrap;
                    tb.FontSize = 12;
                    Grid.SetRow(tb, i);
                    Grid.SetRowSpan(tb, m_models.Count);
                    Grid.SetColumn(tb, 2);
                    MainGrid.Children.Add(tb);
                    #endregion

                    #region География
                    string geo = m_models[i].Geoname;
                    tb = new TextBlock();
                    tb.Margin = new Thickness(3, 0, 0, 0);
                    tb.Text = geo;
                    tb.TextWrapping = TextWrapping.Wrap;
                    tb.FontSize = 12;
                    Grid.SetRow(tb, i);
                    Grid.SetRowSpan(tb, m_models.Count);
                    Grid.SetColumn(tb, 3);
                    MainGrid.Children.Add(tb);
                    #endregion

                    #region Контакты
                    string contacts = Options.GetContactsByCommogGroupId(m_models[i].Idcommongroup);
                    tb = new TextBlock();
                    tb.Margin = new Thickness(3, 0, 0, 0);
                    tb.Text = contacts;
                    tb.TextWrapping = TextWrapping.Wrap;
                    tb.FontSize = 12;
                    Grid.SetRow(tb, i);
                    Grid.SetRowSpan(tb, m_models.Count);
                    Grid.SetColumn(tb, 8);
                    MainGrid.Children.Add(tb);
                    #endregion

                }
            }
        }
        #endregion
    }
}
