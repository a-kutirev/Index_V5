using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Xml;

namespace Updater
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        XmlDocument m_documentCurrent;
        XmlDocument m_documentRemote;
        private string pathCurrent = "";
        private string pathRemote = "";
        private string remote_version = "";
        private string current_version = "";
        private List<string> filelist = new List<string>();

        private string ConfigFileName = "update.xml";

        public MainWindow()
        {
            InitializeComponent();

            pathCurrent = AppDomain.CurrentDomain.BaseDirectory + ConfigFileName;
            m_documentCurrent = new XmlDocument();
            m_documentCurrent.Load(pathCurrent);
            XmlElement element = m_documentCurrent.DocumentElement;

            foreach (XmlNode xnode in element)
            {
                if (xnode.Name == "path") pathRemote = xnode.InnerText;
                if (xnode.Name == "version") current_version = xnode.InnerText;
            }

            m_documentRemote = new XmlDocument();
            m_documentRemote.Load(pathRemote);
            XmlElement elementRemote = m_documentRemote.DocumentElement;
            foreach (XmlNode xnode in elementRemote)
            {                
                if (xnode.Name == "version") remote_version = xnode.InnerText;
                if (xnode.Name == "files")
                    foreach (XmlNode xn in xnode.ChildNodes) filelist.Add(xn.InnerText);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            double cv, rv;

            try
            {
                cv = double.Parse(current_version);
                rv = double.Parse(remote_version);
            }
            catch
            {
                cv = double.Parse(current_version.Replace('.',','));
                rv = double.Parse(remote_version.Replace('.',','));
            }

            if (rv > cv)
            {
                string rempath = Path.GetDirectoryName(pathRemote) + "\\";
                string locpath = AppDomain.CurrentDomain.BaseDirectory;
                for (int i = 0; i < filelist.Count; i++)
                {
                    string rem_file = rempath + filelist[i];
                    string loc_file = locpath + filelist[i];
                    File.Copy(rem_file, loc_file, true);
                }
            }

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = $"{AppDomain.CurrentDomain.BaseDirectory}Tours_V5.exe";
            psi.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
            Process ps = new Process();
            ps.StartInfo = psi;
            ps.Start();

            this.Close();
        }
    }
}
