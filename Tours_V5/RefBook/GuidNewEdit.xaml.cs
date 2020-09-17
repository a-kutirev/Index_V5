using ClassLibrary;
using System;
using System.ComponentModel;
using System.Data;
using System.Windows;

namespace Tours_V5.RefBook
{
    /// <summary>
    /// Логика взаимодействия для GuidNewEdit.xaml
    /// </summary>
    public partial class GuidNewEdit : Window, INotifyPropertyChanged
    {
        #region Members
        public event PropertyChangedEventHandler PropertyChanged;

        private GuidModel m_model;
        private DataView m_postsView;

        public GuidModel Model
        {
            get => m_model;
            set
            {
                m_model = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Model"));
            }
        }

        public DataView PostsView
        {
            get => m_postsView;
            set
            {
                m_postsView = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PostsView"));
            }
        }

        #endregion

        #region Constructor
        public GuidNewEdit()
        {
            InitializeComponent();
            this.DataContext = this;

            Model = new GuidModel();
            PostsView = GetPosts();
        }
        public GuidNewEdit(int id)
        {
            InitializeComponent();
            this.DataContext = this;

            Model = new GuidModel(id);
            PostsView = GetPosts();
            SaveBt.Content = "Изменить";

            if(Model.Guidend == (new DateTime(1,1,1)))
            {
                GuidEndLabel.Visibility = Visibility.Collapsed;
                GuidEndPicker.Visibility = Visibility.Collapsed;
            }
            else
            {
                GuidEndLabel.Visibility = Visibility.Visible;
                GuidEndPicker.Visibility = Visibility.Visible;
            }


        }

        private DataView GetPosts()
        {
            string postssql = "select * from posts";
            return DBWrapper.MySqlWrapper.Select(postssql).DefaultView;
        }
        #endregion

        private void SaveBt_Click(object sender, RoutedEventArgs e)
        {
            if (Model.Idguid == 0)  Model.Insert();
            else                    Model.Update();
            DialogResult = true;
        }
    }
}
