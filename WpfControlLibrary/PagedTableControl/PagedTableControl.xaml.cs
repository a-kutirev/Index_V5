using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfControlLibrary.PagedTableControl
{
    public enum AddedButton
    {
        None,
        Edit,
        Delete,
        EditAndDelete
    }

    public enum DateTimeFormat
    {
        None,
        DateFormat,
        TimeFormat
    }
    /// <summary>
    /// Логика взаимодействия для PagedTableControl.xaml
    /// </summary>
    public partial class PagedTableControl : UserControl
    {
        #region Members
        private DataTable m_source;
        private int[] countValue = new int[] { 10, 25, 50, 100 };
        private int m_currentPage = 1;
        private int m_currentTotalCount = 0;
        private int m_currentTotalPage = 0;
        private int m_currentNumCount = 10;
        private string m_filterRow = "";
        private string m_filterTemplate = "";
        private AddedButton m_Buttons = AddedButton.None;
        private string m_commandParameterField;

        public event EventHandler<GridButtonEventArgs> EditBtClick;
        public event EventHandler<GridButtonEventArgs> DeleteBtClick;
        public event EventHandler<GridButtonEventArgs> SelectionGridChange;

        bool loaded = false;
        public DataView DataView_
        {
            set
            {
                grid.ItemsSource = value;
                UserControl_Loaded(this, null);
            }
        }
        public DataTable Source
        {
            get => m_source;
            set
            {
                m_source = value;
                if (m_source == null) return;
                grid.ItemsSource = m_source.DefaultView;
                UserControl_Loaded(this, null);
            }
        }
        public AddedButton Buttons
        {
            get => m_Buttons;
            set
            {
                m_Buttons = value;

                if (m_Buttons == AddedButton.Edit || m_Buttons == AddedButton.EditAndDelete)
                {
                    DataGridTemplateColumn buttColumn = new DataGridTemplateColumn();

                    FrameworkElementFactory button = new FrameworkElementFactory(typeof(Button));
                    button.SetValue(Button.ContentProperty, "Изменить");
                    button.SetValue(Button.BorderThicknessProperty, new Thickness(0));
                    button.SetValue(Button.CommandParameterProperty, new Binding(m_commandParameterField));
                    button.AddHandler(Button.ClickEvent, new RoutedEventHandler(EditClick));
                    DataTemplate template = new DataTemplate
                    {
                        VisualTree = button
                    };
                    buttColumn.CellTemplate = template;
                    buttColumn.Width = 80;
                    grid.Columns.Add(buttColumn);
                }
                if (m_Buttons == AddedButton.Delete || m_Buttons == AddedButton.EditAndDelete)
                {
                    DataGridTemplateColumn buttColumn = new DataGridTemplateColumn();

                    FrameworkElementFactory button = new FrameworkElementFactory(typeof(Button));
                    button.SetValue(Button.ContentProperty, "Удалить");
                    button.SetValue(Button.BorderThicknessProperty, new Thickness(0));
                    button.SetValue(Button.CommandParameterProperty, new Binding(m_commandParameterField));
                    button.AddHandler(Button.ClickEvent, new RoutedEventHandler(DeleteClick));
                    DataTemplate template = new DataTemplate
                    {
                        VisualTree = button
                    };
                    buttColumn.CellTemplate = template;
                    buttColumn.Width = 80;
                    grid.Columns.Add(buttColumn);
                }
            }
        }

        public string CommandParameterField { get => m_commandParameterField; set => m_commandParameterField = value; }
        public string FilterRow
        {
            get => m_filterRow;
            set
            {
                m_filterRow = value;
            }
        }
        public string FilterTemplate
        {
            get => m_filterTemplate;
            set
            {
                if (m_filterRow == "") return;
                m_filterTemplate = value;
                string filter = $"{m_filterRow} like '%{m_filterTemplate}%'";                
                DataView dv = m_source.DefaultView;
                dv.RowFilter = filter;
                SetGridContent(dv.ToTable());
            }
        }

        #endregion

        public void DeleteClick(object sender, RoutedEventArgs e)
        {
            GridButtonEventArgs ea = new GridButtonEventArgs
            {
                id = int.Parse((sender as Button).CommandParameter.ToString())
            };
            DeleteBtClick?.Invoke(this, ea);
        }

        public void EditClick(object sender, RoutedEventArgs e)
        {
            GridButtonEventArgs ea = new GridButtonEventArgs
            {
                id = int.Parse((sender as Button).CommandParameter.ToString())
            };
            EditBtClick?.Invoke(this, ea);
        }


        public PagedTableControl()
        {
            InitializeComponent();

            grid.AutoGenerateColumns = false;
            grid.IsReadOnly = true;
            grid.HorizontalGridLinesBrush = Brushes.LightGray;
            grid.VerticalGridLinesBrush = Brushes.LightGray;
            grid.AlternatingRowBackground = Brushes.AliceBlue;
        }

        public void Refresh()
        {
            SetGridContent(m_source);
        }

        public void AddColumn(string header, string binding, int width, bool isCommandparameter,
    DateTimeFormat dateTimeFormat = DateTimeFormat.None)
        {
            DataGridTextColumn textColumn = new DataGridTextColumn
            {
                Header = header,
                Binding = new Binding(binding),
                Width = width
            };
            grid.Columns.Add(textColumn);
            if (isCommandparameter)
                m_commandParameterField = binding;
            if (dateTimeFormat == DateTimeFormat.DateFormat)
            {
                textColumn.Binding.StringFormat = "{0: yyyy.MM.dd}";
            }
            if (dateTimeFormat == DateTimeFormat.TimeFormat)
            {
                textColumn.Binding.StringFormat = "{0: HH:mm}";
            }

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            loaded = true;
            if (m_source != null)
            {
                m_currentTotalCount = m_source.Rows.Count;
                m_currentTotalPage = (int)Math.Ceiling((double)m_currentTotalCount / m_currentNumCount);
                SetGridContent(m_source);
            }
        }

        private void SetGridContent(DataTable m_source)
        {
            try
            {
                if (loaded && m_source != null)
                {
                    m_currentTotalCount = m_source.Rows.Count;
                    m_currentNumCount = countValue[numItemsCombo.SelectedIndex];
                    m_currentTotalPage = (int)Math.Ceiling((double)m_currentTotalCount / m_currentNumCount);
                    if (m_currentPage > m_currentTotalPage) m_currentPage = 1;

                    totalCounter.Content = $"Всего: {m_currentTotalCount} записей";
                    counter.Content = $"{m_currentPage} из {m_currentTotalPage}";

                    int skip = (m_currentPage - 1) * m_currentNumCount;
                    if (skip > m_currentTotalCount) skip = 0;
                    DataTable tmp = m_source.AsEnumerable().Skip(skip).Take(m_currentNumCount).CopyToDataTable();
                    grid.ItemsSource = tmp.DefaultView;
                    CheckNavigateBt();
                }
            }
            catch { }
        }

        private void CheckNavigateBt()
        {
            if (m_currentPage == 1)
            {
                firstBt.IsEnabled = false;
                previousBt.IsEnabled = false;
            }
            else
            {
                firstBt.IsEnabled = true;
                previousBt.IsEnabled = true;
            }
            if (m_currentPage == m_currentTotalPage)
            {
                lastBt.IsEnabled = false;
                nextBt.IsEnabled = false;
            }
            else
            {
                lastBt.IsEnabled = true;
                nextBt.IsEnabled = true;
            }
        }

        private void Grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromItem(e.AddedItems[0]);
                if (row != null)
                {
                    DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(row);
                    int m_id = 0;
                    if (presenter.ItemContainerGenerator.ContainerFromIndex(0) is DataGridCell cellId)
                        m_id = int.Parse(((TextBlock)cellId.Content).Text);

                    GridButtonEventArgs ea = new GridButtonEventArgs
                    {
                        id = m_id
                    };
                    SelectionGridChange?.Invoke(this, ea);
                }
            }
        }

        private void Bt_Click(object sender, RoutedEventArgs e)
        {
            string name = (sender as Button).Name;
            switch (name)
            {
                case "firstBt":
                    m_currentPage = 1;
                    break;
                case "lastBt":
                    m_currentPage = m_currentTotalPage;
                    break;
                case "nextBt":
                    if (m_currentPage < m_currentTotalPage) m_currentPage++;
                    break;
                case "previousBt":
                    if (m_currentPage > 1) m_currentPage--;
                    break;
            }

            if (m_filterRow != "")
            {
                string filter = $"{m_filterRow} like '%{m_filterTemplate}%'";
                DataView dv = m_source.DefaultView;
                dv.RowFilter = filter;
                SetGridContent(dv.ToTable());
            }
            else
                SetGridContent(m_source);
        }

        private void NumItemsCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (loaded)
            {
                m_currentNumCount = countValue[(sender as ComboBox).SelectedIndex];
                m_currentTotalPage = (int)Math.Ceiling((double)m_currentTotalCount / m_currentNumCount);
                if (m_currentPage > m_currentTotalPage) m_currentPage = 1;

                if (m_filterRow != "")
                {
                    string filter = $"{m_filterRow} like '%{m_filterTemplate}%'";
                    DataView dv = m_source.DefaultView;
                    dv.RowFilter = filter;
                    SetGridContent(dv.ToTable());
                }
                else
                    SetGridContent(m_source);
            }
        }

        static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null) child = GetVisualChild<T>(v);
                if (child != null) break;
            }
            return child;
        }
    }

    #region Event  Args
    public class GridButtonEventArgs : EventArgs
    {
        public int id;
    }
    #endregion
}
