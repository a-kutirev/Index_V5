using ClassLibrary;
using ClassLibrary.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace WpfControlLibrary.ReportControls
{
    /// <summary>
    /// Логика взаимодействия для SimpleEventControl.xaml
    /// </summary>
    public partial class SimpleEventControl : UserControl, ITimed
    {

        private EventGroupModel m_model;

        public SimpleEventControl(EventGroupModel m)
        {
            InitializeComponent();

            m_model = m;
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            timeText.Text = m_model.Eventgrouptime.ToString(@"hh\:mm");
            timeText.FontSize = Options.FontSize;
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            EventModel m_evModel = new EventModel(m_model.Idevent);

            string text = "";
            if (m_evModel.Eventtype == "МК") text += "Мастер-класс: ";
            if (m_evModel.Eventtype == "КВ") text += "Квест: ";
            if (m_evModel.Eventtype == "Л") text += "Лекция: ";

            text += m_evModel.Eventname;
            text += ". Ведущие - ";

            text += Options.GetGuidStringByIds(m_model.GetListMasters());

            text += ". ";
            text += RichTextStripper.StripRichTextFormat(m_model.Eventgroupcomment.Replace('#', '\\'));

            Run main_run = new Run(text);
            main_run.FontSize = Options.FontSize;
            main_run.FontWeight = FontWeights.Bold;
            main_run.FontFamily = new FontFamily("Times New Roman");

            Paragraph p = EventDesc.Document.Blocks.FirstBlock as Paragraph;
            p.Inlines.Add(main_run);
            EventDesc.Document.Blocks.Add(p);
        }

        public TimeSpan Time 
        {
            get {
                return m_model.Eventgrouptime;
            }
        }
    }
}
