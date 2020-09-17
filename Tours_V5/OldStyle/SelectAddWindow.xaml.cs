using System;
using System.Windows;

namespace Tours_V5.OldStyle
{
    /// <summary>
    /// Логика взаимодействия для SelectAddWindow.xaml
    /// </summary>
    public partial class SelectAddWindow : Window
    {
        private int m_groupn;
        private DateTime m_date;

        public SelectAddWindow(int groupNum, DateTime d)
        {
            InitializeComponent();

            m_groupn = groupNum;
            m_date = d;
        }

        private void TourBt_Click(object sender, RoutedEventArgs e)
        {
            AddGroupWindow agw = new AddGroupWindow(m_groupn);
            agw.ShowDialog();
            DialogResult = true;
        }

        private void EventBt_Click(object sender, RoutedEventArgs e)
        {
            AddEventWindow aew = new AddEventWindow(m_date);
            aew.ShowDialog();
            DialogResult = true;
        }
    }
}
