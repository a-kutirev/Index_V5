using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfControlLibrary.CommentControl
{
    /// <summary>
    /// Логика взаимодействия для RTBControl.xaml
    /// </summary>
    public partial class RTBControl : UserControl
    {
        private DataTable m_autocompleteData;
        public DataTable AutocompleteData { get => m_autocompleteData; set => m_autocompleteData = value; }


        public RTBControl()
        {
            InitializeComponent();
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SelectAutocompleteWindow saw = new SelectAutocompleteWindow(m_autocompleteData);
            if (saw.ShowDialog() == true)
            {
                string sss = saw.sel;
                rttext.CaretPosition.InsertTextInRun(sss);
                rttext.CaretPosition = rttext.CaretPosition.DocumentEnd;
            }
        }
        private void Color_bt_Click(object sender, RoutedEventArgs e)
        {
            string name = (sender as ToggleButton).Name;
            TextRange tr = new TextRange(rttext.Selection.Start, rttext.Selection.End);
            Brush tmp;
            switch (name)
            {
                case "red_bt":
                    tmp = Brushes.Red;
                    break;
                case "green_bt":
                    tmp = Brushes.Green;
                    break;
                case "blue_bt":
                    tmp = Brushes.Blue;
                    break;
                case "black_bt":
                    tmp = Brushes.Black;
                    break;
                default:
                    tmp = Brushes.Black;
                    break;
            }
            if ((sender as ToggleButton).IsChecked == true)
                tr.ApplyPropertyValue(RichTextBox.ForegroundProperty, tmp);
            else
                tr.ApplyPropertyValue(RichTextBox.ForegroundProperty, Brushes.Black);
        }
        private void rttext_SelectionChanged(object sender, RoutedEventArgs e)
        {
            TextRange tr = new TextRange(rttext.Selection.Start, rttext.Selection.End);
            b_bt.IsChecked = (tr.GetPropertyValue(RichTextBox.FontWeightProperty).ToString() == "Bold");
            i_bt.IsChecked = (tr.GetPropertyValue(RichTextBox.FontStyleProperty).ToString() == "Italic");
            red_bt.IsChecked = (tr.GetPropertyValue(RichTextBox.ForegroundProperty).ToString() == "#FFFF0000");
            green_bt.IsChecked = (tr.GetPropertyValue(RichTextBox.ForegroundProperty).ToString() == "#FF00FF00");
            blue_bt.IsChecked = (tr.GetPropertyValue(RichTextBox.ForegroundProperty).ToString() == "#FF0000FF");
        }
        private void b_bt_Click(object sender, RoutedEventArgs e)
        {
            TextRange tr = new TextRange(rttext.Selection.Start, rttext.Selection.End);
            if ((sender as ToggleButton).IsChecked == true)
                tr.ApplyPropertyValue(RichTextBox.FontWeightProperty, FontWeights.Bold);
            else
                tr.ApplyPropertyValue(RichTextBox.FontWeightProperty, FontWeights.Normal);
        }
        private void i_bt_Click(object sender, RoutedEventArgs e)
        {
            TextRange tr = new TextRange(rttext.Selection.Start, rttext.Selection.End);
            if ((sender as ToggleButton).IsChecked == true)
                tr.ApplyPropertyValue(RichTextBox.FontStyleProperty, FontStyles.Italic);
            else
                tr.ApplyPropertyValue(RichTextBox.FontStyleProperty, FontStyles.Normal);
        }
        public string GetRtfReplaced()
        {
            return GetRtf().Replace("\\", "#");
        }

        public string GetRtf()
        {
            string rtfText; //string to save to db
            TextRange tr = new TextRange(rttext.Document.ContentStart, rttext.Document.ContentEnd);
            using (MemoryStream ms = new MemoryStream())
            {
                tr.Save(ms, DataFormats.Rtf);
                rtfText = Encoding.ASCII.GetString(ms.ToArray());
            }
            return rtfText.Replace("\"", "\"");
        }

        public void SetRtfReplaced(string document)
        {
            document = document.Replace("#", "\\");
            SetRtf(document);
        }

        public void SetRtf(string document)
        {
            var documentBytes = Encoding.UTF8.GetBytes(document);
            using (var reader = new MemoryStream(documentBytes))
            {
                reader.Position = 0;
                rttext.SelectAll();
                rttext.Selection.Load(reader, DataFormats.Rtf);
            }
        }
    }
}
