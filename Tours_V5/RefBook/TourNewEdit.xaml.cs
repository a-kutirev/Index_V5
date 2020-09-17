using ClassLibrary;
using System;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Tours_V5.RefBook
{
    /// <summary>
    /// Логика взаимодействия для TourNewEdit.xaml
    /// </summary>
    public partial class TourNewEdit : Window, INotifyPropertyChanged
    {
        #region Members

        public event PropertyChangedEventHandler PropertyChanged;

        private TourModel m_Model;
        private DataView m_ZonesView;
        private DataView m_TourTypeView;

        public TourModel Model
        {
            get => m_Model;
            set
            {
                m_Model = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Model"));
            }
        }

        public DataView ZonesView
        {
            get => m_ZonesView;
            set
            {
                m_ZonesView = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ZonesView"));
            }
        }
        public DataView TourTypeView
        {
            get => m_TourTypeView;
            set
            {
                m_TourTypeView = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TourTypeView"));
            }
        }
        #endregion

        #region Constructor
        public TourNewEdit()
        {
            InitializeComponent();
            this.DataContext = this;
            ZonesView = GetZonesView();
            TourTypeView = GetTourTypeView();
            Model = new TourModel();
        }
        public TourNewEdit(int id)
        {
            InitializeComponent();
            this.DataContext = this;
            ZonesView = GetZonesView();
            TourTypeView = GetTourTypeView();
            Model = new TourModel(id);
            SaveBt.Content = "Сохранить";
        }

        private DataView GetZonesView()
        {
            string sql2 = "select * from expo_zones";
            return DBWrapper.MySqlWrapper.Select(sql2).DefaultView;
        }
        private DataView GetTourTypeView()
        {
            string sql2 = "select * from tourtypes";
            return DBWrapper.MySqlWrapper.Select(sql2).DefaultView;
        }
        #endregion

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            bool val = (bool)(sender as CheckBox).IsChecked;
            if(val)
            {
                StartPicker.Visibility = Visibility.Collapsed;
                EndPicker.Visibility = Visibility.Collapsed;
            }
            else
            {
                StartPicker.Visibility = Visibility.Visible;
                EndPicker.Visibility = Visibility.Visible;
            }
        }

        private void SaveBt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Model.Idtour == 0)
                {
                    Model.Insert();
                }
                else
                {
                    Model.Update();                
                }
                MessageBox.Show("Данные сохранены");
                DialogResult = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка сохранения");
                DialogResult = false;
            }


        }
    }
}
