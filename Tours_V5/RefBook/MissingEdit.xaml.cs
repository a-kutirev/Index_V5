using ClassLibrary;
using System;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Tours_V5.RefBook
{
    /// <summary>
    /// Логика взаимодействия для MissingEdit.xaml
    /// </summary>
    public partial class MissingEdit : Window, INotifyPropertyChanged
    {
        #region Members
        private bool win_loaded = false;
        private bool m_fullDay;
        private bool m_notFullDay;

        private DataView m_guidView;
        private MissingModel m_model;

        public MissingModel Model
        {
            get => m_model;
            set
            {
                m_model = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Model"));
                StartTimePicker.HourVal = Model.Missingbegin.Hour;
                StartTimePicker.MinuteVal = Model.Missingbegin.Minute;
                EndTimePicker.HourVal = Model.Missingend.Hour;
                EndTimePicker.MinuteVal = Model.Missingend.Minute;
                if (m_model.Missingnotfullday == 1)
                {
                    NotFullDay = true;
                    FullDay = false;
                }
                else
                {
                    FullDay = true;
                    NotFullDay = false;
                }
            }
        }
        public bool FullDay
        {
            get => m_fullDay;
            set
            {
                m_fullDay = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FullDay"));
                if (!value) return;
                End_Date_Label.Visibility = Visibility.Visible;
                ToPicker.Visibility = Visibility.Visible;

                StartTimePicker.Visibility = Visibility.Collapsed;
                EndTimePicker.Visibility = Visibility.Collapsed;
                Start_Time_Label.Visibility = Visibility.Collapsed;
                End_Time_Label.Visibility = Visibility.Collapsed;
            }
        }
        public bool NotFullDay
        {
            get => m_notFullDay;
            set
            {
                m_notFullDay = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NotFullDay"));
                if (!value) return;
                End_Date_Label.Visibility = Visibility.Collapsed;
                ToPicker.Visibility = Visibility.Collapsed;

                StartTimePicker.Visibility = Visibility.Visible;
                EndTimePicker.Visibility = Visibility.Visible;
                Start_Time_Label.Visibility = Visibility.Visible;
                End_Time_Label.Visibility = Visibility.Visible;
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

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructor
        public MissingEdit(int id)
        {
            InitializeComponent();
            this.DataContext = this;

            m_model = new MissingModel(id);

            DateTime date = m_model.Missingbegin;

            string sql = $"select * from guids where (\"{date.ToMySqlDateString()}\" > date(guidaccept) " +
                $"and \"{date.ToMySqlDateString()}\" < date(guidend)) or guidend is null";
            GuidView = DBWrapper.MySqlWrapper.Select(sql).DefaultView;


            StartTimePicker.Date = date;
            EndTimePicker.Date = date;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            win_loaded = true;
            Model = m_model;
        }
        #endregion

        private void SaveBt_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)b1.IsChecked)
            {
                Model.Missingbegin = FromPicker.SelectedDate.Value;
                Model.Missingend = ToPicker.SelectedDate.Value;
                Model.Update();
                DialogResult = true;
            }
            if ((bool)b2.IsChecked)
            {
                Model.Missingbegin = DateTime.Parse($"{FromPicker.SelectedDate.Value.ToMySqlDateString()} {StartTimePicker.HourVal}:{StartTimePicker.MinuteVal}:00");
                Model.Missingend = DateTime.Parse($"{FromPicker.SelectedDate.Value.ToMySqlDateString()} {EndTimePicker.HourVal}:{EndTimePicker.MinuteVal}:00");
                Model.Update();
                DialogResult = true;
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!win_loaded) return;
            string name = (sender as RadioButton).Name;
            bool val = (bool)(sender as RadioButton).IsChecked;
            switch (name)
            {
                case "b1":
                    End_Date_Label.Visibility = Visibility.Visible;
                    ToPicker.Visibility = Visibility.Visible;

                    StartTimePicker.Visibility = Visibility.Collapsed;
                    EndTimePicker.Visibility = Visibility.Collapsed;
                    Start_Time_Label.Visibility = Visibility.Collapsed;
                    End_Time_Label.Visibility = Visibility.Collapsed;
                    break;
                case "b2":
                    End_Date_Label.Visibility = Visibility.Collapsed;
                    ToPicker.Visibility = Visibility.Collapsed;

                    StartTimePicker.Visibility = Visibility.Visible;
                    EndTimePicker.Visibility = Visibility.Visible;
                    Start_Time_Label.Visibility = Visibility.Visible;
                    End_Time_Label.Visibility = Visibility.Visible;
                    break;
            }

        }

        #region Refill Hour-Minute comboboxes
        private void FromPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            StartTimePicker.Date = (sender as DatePicker).SelectedDate.Value;
            EndTimePicker.Date = (sender as DatePicker).SelectedDate.Value;
        }
        #endregion
    }
}
