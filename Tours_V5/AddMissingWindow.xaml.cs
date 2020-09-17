using ClassLibrary;
using System;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Tours_V5
{
    /// <summary>
    /// Логика взаимодействия для AddMissingWindow.xaml
    /// </summary>
    public partial class AddMissingWindow : Window, INotifyPropertyChanged
    {

        #region Members
        private bool win_loaded = false;

        public event PropertyChangedEventHandler PropertyChanged;

        private MissingModel m_Model;
        private DataView m_HourView;
        private DataView m_MinuteView;
        private DataView m_guidView;

        public MissingModel Model
        {
            get => m_Model;
            set
            {
                m_Model = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Model"));
            }
        }

        public DataView HourView
        {
            get => m_HourView;
            set
            {
                m_HourView = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HourView"));
            }
        }
        public DataView MinuteView
        {
            get => m_MinuteView;
            set
            {
                m_MinuteView = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MinuteView"));
            }
        }

        public DataView GuidView
        {
            get => m_guidView;
            set
            {
                m_guidView = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GuidView"));
            }
        }
        #endregion

        #region Constructor
        public AddMissingWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            string sql = "select * from guids where guidend is null";
            GuidView = DBWrapper.MySqlWrapper.Select(sql).DefaultView;

            FromPicker.SelectedDate = DateTime.Now;
            ToPicker.SelectedDate = DateTime.Now;

            Model = new MissingModel();

            End_Date_Label.Visibility = Visibility.Visible;
            ToPicker.Visibility = Visibility.Visible;

            StartTimePicker.Visibility = Visibility.Collapsed;
            EndTimePicker.Visibility = Visibility.Collapsed;
            Start_Time_Label.Visibility = Visibility.Collapsed;
            End_Time_Label.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Настройка окна в зависимости от типа отсутствия
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!win_loaded) return;
            string name = (sender as RadioButton).Name;
            bool val = (bool)(sender as RadioButton).IsChecked;
            switch(name)
            {
                case "b1":
                    Model.Missingnotfullday = 0;
                    End_Date_Label.Visibility = Visibility.Visible;
                    ToPicker.Visibility = Visibility.Visible;

                    StartTimePicker.Visibility = Visibility.Collapsed;
                    EndTimePicker.Visibility = Visibility.Collapsed;
                    Start_Time_Label.Visibility = Visibility.Collapsed;
                    End_Time_Label.Visibility = Visibility.Collapsed;
                    break;
                case "b2":
                    Model.Missingnotfullday = 1;
                    End_Date_Label.Visibility = Visibility.Collapsed;
                    ToPicker.Visibility = Visibility.Collapsed;

                    StartTimePicker.Visibility = Visibility.Visible;
                    EndTimePicker.Visibility = Visibility.Visible;
                    Start_Time_Label.Visibility = Visibility.Visible;
                    End_Time_Label.Visibility = Visibility.Visible;
                    break;
                case "b3":
                    Model.Missingnotfullday = 0;
                    End_Date_Label.Visibility = Visibility.Collapsed;
                    ToPicker.Visibility = Visibility.Collapsed;

                    StartTimePicker.Visibility = Visibility.Collapsed;
                    EndTimePicker.Visibility = Visibility.Collapsed;
                    Start_Time_Label.Visibility = Visibility.Collapsed;
                    End_Time_Label.Visibility = Visibility.Collapsed;
                    break;
            }
        }
        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            win_loaded = true;
        }

        #region Refill Hour-Minute comboboxes
        private void FromPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            StartTimePicker.Date = (sender as DatePicker).SelectedDate.Value;
            EndTimePicker.Date = (sender as DatePicker).SelectedDate.Value;
        }
        #endregion

        private void SaveBt_Click(object sender, RoutedEventArgs e)
        {
            if((bool)b1.IsChecked)
            {
                Model.Missingbegin = FromPicker.SelectedDate.Value;
                Model.Missingend = ToPicker.SelectedDate.Value;
                Model.Insert();
                DialogResult = true;
            }
            if ((bool)b2.IsChecked)
            {
                Model.Missingbegin = DateTime.Parse($"{FromPicker.SelectedDate.Value.ToMySqlDateString()} {StartTimePicker.HourVal}:{StartTimePicker.MinuteVal}:00");
                Model.Missingend = DateTime.Parse($"{FromPicker.SelectedDate.Value.ToMySqlDateString()} {EndTimePicker.HourVal}:{EndTimePicker.MinuteVal}:00");
                Model.Insert();
                DialogResult = true;
            }
            if ((bool)b3.IsChecked)
            {
                string sql = $"update guids set guidend = \"{FromPicker.SelectedDate.Value.ToMySqlDateString()}\" where idguid = {Model.Idguid}";
                DBWrapper.MySqlWrapper.Execute(sql);
                DialogResult = true;
            }
        }
    }
}
