using ClassLibrary;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace WpfControlLibrary.ReportControls
{
    /// <summary>
    /// Логика взаимодействия для SimpleGroupControl.xaml
    /// </summary>
    public partial class SimpleGroupControl : UserControl, INotifyPropertyChanged, ITimed
    {
        #region Members
        private DisplayedGroupModel m_model;

        public event PropertyChangedEventHandler PropertyChanged;
        public DisplayedGroupModel Model { get => m_model; set => m_model = value; }

        public TimeSpan Time
        {
            get
            {
                return m_model.Grouptime;
            }
        }

        #endregion

        #region Constructor
        public SimpleGroupControl(DisplayedGroupModel m)
        {
            InitializeComponent();
            this.DataContext = this;

            Model = m;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            string time_tar = m_model.Grouptime.ToString(@"hh\:mm");

            timeText.Text = time_tar;
            timeText.FontSize = Options.FontSize;
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            string expo_tar = " --- " + m_model.Tourname + " --- ";
            Run expo_run = new Run(expo_tar);
            expo_run.FontSize = Options.FontSize;
            expo_run.FontWeight = FontWeights.Bold;
            expo_run.FontFamily = new FontFamily("Times New Roman");

            string main_tar = 
                m_model.Organizationname + " (" + m_model.Geoname + "). " +
                m_model.Groupamount + " человек (" + m_model.Groupage;

            if (m_model.Groupaddition > 0)
                main_tar += ", сопровождающих-" + m_model.Groupaddition + "). ";
            else
                main_tar += "). ";

            Run main_run = new Run(main_tar);
            main_run.FontSize = Options.FontSize;
            main_run.FontWeight = FontWeights.Normal;
            main_run.FontFamily = new FontFamily("Times New Roman");

            /////////////////////////////////////////////////////////////////////////////////////////////////////           
            string guid_tar = " Экскурсовод - " + m_model.Guidshortname + ". ";

            Run guid_run = new Run(guid_tar);
            guid_run.FontSize = Options.FontSize;
            guid_run.FontWeight = FontWeights.Bold;
            guid_run.FontFamily = new FontFamily("Times New Roman");

            /////////////////////////////////////////////////////////////////////////////////////////////////////
            string comment_tar = RichTextStripper.StripRichTextFormat(m_model.Groupcomment.Replace('#','\\'));

            Run comment_run = new Run(comment_tar);
            comment_run.FontSize = Options.FontSize;
            comment_run.FontWeight = FontWeights.Bold;
            comment_run.FontFamily = new FontFamily("Times New Roman");

            Paragraph p = GroupDesc.Document.Blocks.FirstBlock as Paragraph;
            p.Inlines.Add(expo_run);
            p.Inlines.Add(main_run);
            p.Inlines.Add(guid_run);
            if (comment_tar == "")
                p.Inlines.Add(new Run(""));
            else
                p.Inlines.Add(comment_run);
            GroupDesc.Document.Blocks.Add(p);           
                
        }
        #endregion
    }
}
