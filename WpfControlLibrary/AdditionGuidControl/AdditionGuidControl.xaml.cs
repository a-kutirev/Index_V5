using ClassLibrary;
using System.ComponentModel;
using System.Windows.Controls;

namespace WpfControlLibrary.AdditionGuidControl
{
    /// <summary>
    /// Логика взаимодействия для AdditionGuidControl.xaml
    /// </summary>
    public partial class AdditionGuidControl : UserControl, INotifyPropertyChanged
    {
        #region Members
        public event PropertyChangedEventHandler PropertyChanged;

        private GuidModel m_model;
        private bool m_GuidChecked = false;

        public GuidModel Model
        {
            get => m_model;
            set
            {
                m_model = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Model"));
            }
        }

        public bool GuidChecked
        {
            get => m_GuidChecked;
            set
            {
                m_GuidChecked = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GuidChecked"));
            }
        }
        #endregion

        #region Constructor
        public AdditionGuidControl(GuidModel guid)
        {
            InitializeComponent();
            this.DataContext = this;

            Model = guid;
            GuidChecked = false;            
        }
        #endregion
    }
}
