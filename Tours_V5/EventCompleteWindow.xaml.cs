using ClassLibrary.Models;
using DBWrapper;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;

namespace Tours_V5
{
    /// <summary>
    /// Логика взаимодействия для EventCompleteWindow.xaml
    /// </summary>
    public partial class EventCompleteWindow : Window, INotifyPropertyChanged
    {
        #region Members

        private EventGroupModel m_model;
        private EventModel m_evModel;
        private string m_tourDate;
        private string m_guids;
        private DataView m_CategView;

        private bool m_b1 = false;
        private bool m_b2 = false;
        private bool m_b3 = false;
        private bool m_b4 = false;
        private bool m_b5 = false;
        private bool m_b6 = false;
        private bool m_b7 = false;
        private bool m_b8 = false;
        private bool m_b9 = false;

        public EventGroupModel Model
        {
            get => m_model;
            set
            {
                m_model = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Model"));
            }
        }

        public string TourDate
        {
            get => m_tourDate;
            set
            {
                m_tourDate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TourDate"));
            }
        }

        public EventModel EvModel
        {
            get => m_evModel;
            set
            {
                m_evModel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EvModel"));
            }
        }

        public DataView CategView
        {
            get => m_CategView;
            set
            {
                m_CategView = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CategView"));
            }
        }

        public string Guids
        {
            get => m_guids;
            set
            {
                m_guids = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Guids"));
            }
        }

        public bool B1
        {
            get => m_b1;
            set
            {
                m_b1 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("B1"));
            }
        }
        public bool B2
        {
            get => m_b2;
            set
            {
                m_b2 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("B2"));
            }
        }
        public bool B3
        {
            get => m_b3;
            set
            {
                m_b3 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("B3"));
            }
        }
        public bool B4
        {
            get => m_b4;
            set
            {
                m_b4 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("B4"));
            }
        }
        public bool B5
        {
            get => m_b5;
            set
            {
                m_b5 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("B5"));
            }
        }
        public bool B6
        {
            get => m_b6;
            set
            {
                m_b6 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("B6"));
            }
        }
        public bool B7
        {
            get => m_b7;
            set
            {
                m_b7 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("B7"));
            }
        }
        public bool B8
        {
            get => m_b8;
            set
            {
                m_b8 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("B8"));
            }
        }

        public bool B9
        {
            get => m_b9;
            set
            {
                m_b9 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("B9"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructor
        public EventCompleteWindow(int id)
        {
            InitializeComponent();
            this.DataContext = this;

            Model = new EventGroupModel(id);
            TourDate = Model.Eventgroupdate.ToString("dd.MM.yyyy");
            TimePicker.Date = Model.Eventgroupdate;
            TimePicker.Time = Model.Eventgrouptime;

            EvModel = new EventModel(Model.Idevent);

            string sql = "select * from categories";
            CategView = DBWrapper.MySqlWrapper.Select(sql).DefaultView;

            List<int> guidsId = Model.GetListMasters();
            for(int i = 0; i < guidsId.Count; i++)
            {
                Guids += ClassLibrary.Options.GetGuidName(guidsId[i]);
                if (i != (guidsId.Count - 1)) Guids += ", ";
            }

            ConstructAutoCompletionSource();
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {            
            int statistics = 0;
            if (B1) statistics |= 1;
            if (B2) statistics |= 2;
            if (B3) statistics |= 4;
            if (B4) statistics |= 8;
            if (B5) statistics |= 16;
            if (B6) statistics |= 32;
            if (B7) statistics |= 64;
            if (B8) statistics |= 128;
            if (B9) statistics |= 256;
            Model.Eventgroupstat = statistics;

            Model.Eventgroupstatus |= 4;

            Model.Update();

            DialogResult = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            List<int> guidsId = Model.GetListMasters();            
            Dictionary<int, string> guidsDictionary = new Dictionary<int, string>();
            for(int i = 0; i < guidsId.Count; i++)
                guidsDictionary.Add(guidsId[i], ClassLibrary.Options.GetGuidName(guidsId[i]));
            SelectEventMasters sem = new SelectEventMasters(Model.Eventgroupdate, guidsDictionary);
            if((bool)sem.ShowDialog())
            {
                Dictionary<int, string> tmp = sem.MastersLists;

                Model.Eventgroupmaster = "";
                Guids = "";
                for(int i = 0; i < tmp.Count; i++)
                {
                    Model.Eventgroupmaster += tmp.ElementAt(i).Key.ToString();
                    Guids += tmp.ElementAt(i).Value;
                    if(i!=(tmp.Count - 1))
                    {
                        Model.Eventgroupmaster += "#";
                        Guids += ", ";
                    }
                }
            }
        }

        #region Подготовка автозаполнения
        private void ConstructAutoCompletionSource()
        {
            AgeTextBox.AutoSuggestionList.Clear();
            string sql = "select autocompleteword from autocompletes where autocompletetype = \"age\"";
            DataTable dt = MySqlWrapper.Select(sql);
            for (int i = 0; i < dt.Rows.Count; i++)
                AgeTextBox.AutoSuggestionList.Add(dt.Rows[i]["autocompleteword"].ToString());
        }
        #endregion
    }
}
