using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfControlLibrary.MissingControl
{
    /// <summary>
    /// Логика взаимодействия для ButtonsPanel.xaml
    /// </summary>
    public partial class ButtonsPanel : UserControl
    {
        public event EventHandler OnEditClick;
        public event EventHandler OnDeleteClick;

        private int m_idMissing = 0;

        public int IdMissing { get => m_idMissing; set => m_idMissing = value; }

        public ButtonsPanel(int id)
        {
            InitializeComponent();

            m_idMissing = id;
            EditBt.CommandParameter = id;
            DeleteBt.CommandParameter = id;
        }

        private void EditBt_Click(object sender, RoutedEventArgs e)
        {
            OnEditClick?.Invoke(sender, e);
        }

        private void DeleteBt_Click(object sender, RoutedEventArgs e)
        {
            OnDeleteClick?.Invoke(sender, e);
        }
    }
}
