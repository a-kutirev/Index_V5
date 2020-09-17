using ClassLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UtilsLib.RegroupNew
{
    /// <summary>
    /// Логика взаимодействия для GroupItemControl.xaml
    /// </summary>
    public partial class GroupItemControl : UserControl, INotifyPropertyChanged
    {
        private string m_time;
        private string m_expoName;

        private DisplayedGroupModel m_model;

        public string Time
        {
            get => m_time;
            set
            {
                m_time = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Time"));
            }
        }

        public string ExpoName
        {
            get => m_expoName;
            set
            {
                m_expoName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ExpoName"));
            }
        }

        public DisplayedGroupModel Model
        {
            get => m_model;
            set
            {
                m_model = value;

                Time = m_model.Grouptime.ToString(@"hh\:mm");
                ExpoName = m_model.Tourname;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public GroupItemControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
