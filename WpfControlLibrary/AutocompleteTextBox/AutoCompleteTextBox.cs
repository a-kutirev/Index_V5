using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace WpfControlLibrary.AutocompleteTextBox
{
    public class AutoCompleteTextBox : ComboBox
    {
        private Label m_Watermark;

        //The autosuggestionlist source.
        private ObservableCollection<string> autoSuggestionList = new ObservableCollection<string>();

        /// <summary>
        /// Gets or sets the auto suggestion list.
        /// </summary>
        /// <value>The auto suggestion list.</value>
        public ObservableCollection<string> AutoSuggestionList
        {
            get { return autoSuggestionList; }
            set { autoSuggestionList = value; }
        }

        public AutoCompleteTextBox()
        {
            //load and apply style to the ComboBox.
            ResourceDictionary rd = new ResourceDictionary
            {
                Source = new Uri("/" + this.GetType().Assembly.GetName().Name +
                ";component/AutocompleteTextbox/AutoCompleteComboBoxStyle.xaml",
                 UriKind.Relative)
            };
            this.Resources = rd;
            //disable default Text Search Function
            this.IsTextSearchEnabled = false;
        }

        public Label Watermark { get => m_Watermark; set => m_Watermark = value; }

        /// <summary>
        /// Override OnApplyTemplate method 
        /// Get TextBox control out of Combobox control, and hook up TextChanged event.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            //get the textbox control in the ComboBox control
            if (this.Template.FindName("PART_EditableTextBox", this) is TextBox textBox)
            {
                //disable Autoword selection of the TextBox
                textBox.AutoWordSelection = true;
                //handle TextChanged event to dynamically add Combobox items.\
                textBox.FontSize = 14;
                textBox.FontFamily = new FontFamily("Tahoma");
                textBox.TextChanged += new TextChangedEventHandler(textBox_TextChanged);
                textBox.IsTabStop = true;
                textBox.TabIndex = TabIndex;
            }
        }

        public void SetIndex(int value)
        {
            string tmp = Text;
            SelectedIndex = value;
            Text = tmp;
        }

        public int GetIndex()
        {
            return SelectedIndex;
        }

        /// <summary>
        /// main logic to generate auto suggestion list.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.TextChangedEventArgs"/> 
        /// instance containing the event data.</param>
        void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (Watermark != null)
            {
                if (textBox.Text != "")
                    Watermark.Visibility = Visibility.Collapsed;
                else
                    Watermark.Visibility = Visibility.Visible;
            }

            if (textBox.Text.Length > textBox.SelectionStart)
            {
                return;
            }

            if (textBox.Text.Length > 0 && SelectedIndex != -1)
            {
                SetIndex(-1);
                textBox.CaretIndex = textBox.Text.Length;
                return;
            }

            textBox.AutoWordSelection = false;


            // if the word in the textbox is selected, then don't change item collection

            if ((textBox.SelectionStart != 0 || textBox.Text.Length == 0))
            {
                this.Items.Clear();
                //add new filtered items according the current TextBox input
                if (!string.IsNullOrEmpty(textBox.Text))
                {
                    foreach (string s in this.autoSuggestionList)
                    {
                        if (textBox.Text.Length < s.Length)
                        {
                            bool contain = s.Contains(textBox.Text, StringComparison.OrdinalIgnoreCase);
                            if (contain)
                            {
                                int ind1 = s.IndexOf(textBox.Text, StringComparison.OrdinalIgnoreCase);
                                string unboldpart_1 = s.Substring(0, ind1);
                                string boldpart = textBox.Text;
                                string unboldpart_2 = s.Substring(
                                    unboldpart_1.Length + boldpart.Length);
                                //construct AutoCompleteEntry and add to the ComboBox
                                AutoCompleteEntry entry = new AutoCompleteEntry(
                                    s, unboldpart_1, boldpart, unboldpart_2);
                                this.Items.Add(entry);
                            }
                        }
                    }
                }
            }
            // open or close dropdown of the ComboBox according to whether there are items in the 
            // fitlered result.
            this.IsDropDownOpen = this.HasItems;

            //avoid auto selection
            textBox.Focus();
            textBox.SelectionStart = textBox.Text.Length;

            SetIndex(-1);
        }

    }

    public static class StringExtension
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }
    }

    /// <summary>
    /// Extended ComboBox Item
    /// </summary>
    public class AutoCompleteEntry : ComboBoxItem
    {
        private TextBlock tbEntry;

        //text of the item
        private string text;

        /// <summary>
        /// Contrutor of AutoCompleteEntry class
        /// </summary>
        /// <param name="text">All the Text of the item </param>
        /// <param name="bold">The already entered part of the Text</param>
        /// <param name="unbold">The remained part of the Text</param>
        public AutoCompleteEntry(string text, string unbold_1, string bold, string unbold_2)
        {
            this.text = text;
            tbEntry = new TextBlock();
            //highlight the current input Text
            tbEntry.Inlines.Add(new Run { Text = unbold_1 });
            string bold_main = text.Substring(unbold_1.Length, bold.Length);

            tbEntry.Inlines.Add(new Run
            {
                Text = bold_main,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.RoyalBlue)
            });
            tbEntry.Inlines.Add(new Run { Text = unbold_2 });
            this.Content = tbEntry;
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        public string Text
        {
            get { return this.text; }
        }
    }

}
