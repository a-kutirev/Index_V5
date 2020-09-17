using ClassLibrary;
using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace UtilsLib.Regroup
{
    /// <summary>
    /// Логика взаимодействия для GroupBody.xaml
    /// </summary>
    public partial class GroupBody : UserControl
    {
        #region Members
        public event EventHandler<NumGroupChangedArgs> NumGroupChanged;
        private DisplayedGroupModel m_model;

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        #endregion

        #region Constructor
        public GroupBody(DisplayedGroupModel m)
        {
            InitializeComponent();

            m_model = m;

            TimeLabel.Content = $"{m.Grouptime.Hours}:{m.Grouptime.Minutes.ToString("D2")}";
            ExpoTextBlock.Text = m.Tourname;
            NumTextBox.Text = m.Groupnum.ToString();
        }
        #endregion

        #region Events
        private void NumTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                NumGroupChangedArgs ev = new NumGroupChangedArgs();
                ev.Id = m_model.Idgroup;
                ev.GroupNum = int.Parse(NumTextBox.Text);
                NumGroupChanged?.Invoke(this, ev);
            }
        }

        private void NumTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!IsTextAllowed(e.Text))
            {
                e.Handled = true;
            }
        }
        #endregion
    }

    public class NumGroupChangedArgs : EventArgs
    {
        public int Id;
        public int GroupNum;

        public NumGroupChangedArgs()
        {

        }
    }
}
