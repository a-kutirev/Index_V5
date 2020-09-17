using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfControlLibrary.CommentControl
{
    /// <summary>
    /// Логика взаимодействия для SelectAutocompleteWindow.xaml
    /// </summary>
    public partial class SelectAutocompleteWindow : Window
    {
        public string sel = "";
        private DataTable m_data;
        private DataView m_dv;

        public SelectAutocompleteWindow(DataTable adata)
        {
            m_data = adata;
            InitializeComponent();
            m_dv = new DataView(m_data);
            for (int i = 0; i < m_dv.Count; i++)
                VariantsLB.Items.Add(m_dv[i][0].ToString());
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            sel = VariantsLB.SelectedItem.ToString();
            DialogResult = true;
        }

        private void CommandBinding_Executed_1(object sender, ExecutedRoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void filter_TextChanged(object sender, TextChangedEventArgs e)
        {
            m_dv.RowFilter = $"autocompleteword like '%{filter.Text}%'";
            VariantsLB.Items.Clear();
            for (int i = 0; i < m_dv.Count; i++)
                VariantsLB.Items.Add(m_dv[i][0].ToString());
        }
    }
}
