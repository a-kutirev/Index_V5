using ClassLibrary;
using ClassLibrary.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace WpfControlLibrary.ReportControls
{
    /// <summary>
    /// Логика взаимодействия для NoteControl.xaml
    /// </summary>
    public partial class NoteControl : UserControl
    {
        #region Members
        private List<NoteModel> m_models;
        #endregion

        #region Constructor
        public NoteControl(List<NoteModel> m)
        {
            InitializeComponent();

            m_models = m;

            for(int i = 0; i < m_models.Count; i++)
            {
                if (m_models[i].Notelimit == 1) continue;
                string note = m_models[i].Note;
                Run note_run = new Run(note);
                note_run.FontSize = Options.FontSize;
                note_run.FontWeight = FontWeights.Bold;
                note_run.FontFamily = new FontFamily("Times New Roman");

                NotesDesc.Document.Blocks.Add(new Paragraph(note_run));
            }

            NotesDesc.Document.Blocks.Add(new Paragraph());
        }
        #endregion
    }
}
