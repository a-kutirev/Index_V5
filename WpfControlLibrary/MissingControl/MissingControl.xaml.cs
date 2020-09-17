using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ClassLibrary;

namespace WpfControlLibrary.MissingControl
{
    /// <summary>
    /// Логика взаимодействия для MissingControl.xaml
    /// </summary>
    public partial class MissingControl : UserControl, INotifyPropertyChanged
    {       

        #region Members
        private MissingModel m_missingModel;
        private string m_begin;
        private string m_end;
        private string m_fullname;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<MissingControlArguments> MissingClick;

        public MissingModel Model
        {
            get => m_missingModel;
            set
            {
                m_missingModel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Model"));
                Fullname = Options.GetGuidName(m_missingModel.Idguid);
                if(m_missingModel.Missingnotfullday == 1)
                {
                    Begin = m_missingModel.Missingbegin.ToString("HH:mm");
                    End = m_missingModel.Missingend.ToString("HH:mm");
                }
                else
                {
                    Begin = m_missingModel.Missingbegin.ToString("yyyy-MM-dd");
                    End = m_missingModel.Missingend.ToString("yyyy-MM-dd");
                }
            }
        }

        public string Begin
        {
            get => m_begin;
            set
            {
                m_begin = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Begin"));
            }
        }
        public string End
        {
            get => m_end;
            set
            {
                m_end = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("End"));
            }
        }
        public string Fullname
        {
            get => m_fullname;
            set
            {
                m_fullname = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Fullname"));
            }
        }
        #endregion

        #region Constructor
        public MissingControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        public MissingControl(MissingModel missingModel)
        {
            InitializeComponent();
            this.DataContext = this;
            Model = missingModel;
        }
        #endregion


        private void DeleteBt_Click(object sender, RoutedEventArgs e)
        {
            MissingControlArguments mca = new MissingControlArguments();
            mca.Id = Model.Idmissing;
            mca.EventType = "delete";
            MissingClick?.Invoke(this, mca);
        }

        private void EditBt_Click(object sender, RoutedEventArgs e)
        {
            MissingControlArguments mca = new MissingControlArguments();
            mca.Id = Model.Idmissing;
            mca.EventType = "edit";
            MissingClick?.Invoke(this, mca);
        }
    }

    public class MissingControlArguments : EventArgs
    {
        public int Id;
        public string EventType;
    }
}
