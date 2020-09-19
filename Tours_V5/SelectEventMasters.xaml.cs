using ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Tours_V5
{
    /// <summary>
    /// Логика взаимодействия для SelectEventMasters.xaml
    /// </summary>
    public partial class SelectEventMasters : Window
    {
        private DateTime m_date;
        private List<GuidModel> guids;
        private Dictionary<int, string> m_MastersLists;


        public SelectEventMasters(DateTime d, Dictionary<int, string> mastersDict = null)
        {
            InitializeComponent();
            m_date = d;

            string sql = $"call GetGuidsByDate('{m_date.ToString("yyyy-MM-dd")}')";
            guids = (List<GuidModel>)DBWrapper.MySqlWrapper.Select(sql).ToList<GuidModel>();

            for(int i = 0; i < guids.Count; i++)
            {
                if (guids[i].Idguid == 1) continue;
                CheckBox cb = new CheckBox();
                cb.Tag = guids[i].Idguid;
                cb.Content = guids[i].Guidfullname;
                mastersListBox.Items.Add(cb);

                if (mastersDict != null)
                    if (mastersDict.ContainsKey(guids[i].Idguid))
                        cb.IsChecked = true;
            }
        }

        public Dictionary<int, string> MastersLists { get => m_MastersLists; set => m_MastersLists = value; }

        private void AddBt_Click(object sender, RoutedEventArgs e)
        {
            m_MastersLists = new Dictionary<int, string>();
            for(int i = 0; i < mastersListBox.Items.Count; i++)
            {
                CheckBox c = mastersListBox.Items[i] as CheckBox;
                if ((bool)(c.IsChecked))
                {
                    int id = (int)c.Tag;
                    string s = (string)c.Content;
                    m_MastersLists.Add(id, s);
                }
            }

            DialogResult = true;
        }
    }
}
