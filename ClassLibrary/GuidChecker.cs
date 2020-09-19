using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Media.Imaging;

namespace ClassLibrary
{
    public class GuidChecker
    {
        #region members fields
        private DateTime m_date;
        private Dictionary<int, OneGuid> guids_checker = new Dictionary<int, OneGuid>();
        private List<GuidModel> guids = new List<GuidModel>();

        public DateTime Date
        {
            get => m_date;
            set
            {
                m_date = value;
                guids_checker.Clear();
                guids.Clear();

                string sql = $"call GetGuidsByDate(\"{m_date.Year}-{m_date.Month}-{m_date.Day}\")";
                DataTable tmp = DBWrapper.MySqlWrapper.Select(sql);
                guids = (List<GuidModel>)tmp.ToList<GuidModel>();
                for(int i = 0; i < guids.Count; i++)
                    guids_checker.Add(guids[i].Idguid, new OneGuid(guids[i].Idguid));
            }
        }
        #endregion

        #region constructor
        public GuidChecker() { }
        #endregion

        #region Add/Remove
        public void Add(int idguid, TimeSpan t, int dur)
        {
            if (!guids_checker.ContainsKey(idguid)) return;
            guids_checker[idguid].Add(t, dur);
        }
        public void Remove(int idguid, TimeSpan t)
        {
            if (!guids_checker.ContainsKey(idguid)) return;
            guids_checker[idguid].Remove(t);
        }
        #endregion

        #region Check
        public StateGuid Check(int idguid, TimeSpan time, int duration)
        {
            if (!guids_checker.ContainsKey(idguid)) return StateGuid.free;
            return guids_checker[idguid].Check(time, duration);
        }
        #endregion 

        public List<GuidSelectItem> GetGuidItems(TimeSpan time, int duration)
        {
            List<GuidSelectItem> result = new List<GuidSelectItem>();

            // TODO Проверка отсутствующих неполный день

            DateTime currDateTime = new DateTime(
                DayOptions.Date.Year, DayOptions.Date.Month, DayOptions.Date.Day, time.Hours, time.Minutes, 0);

            DataTable dtMiss = DBWrapper.MySqlWrapper.SelectMissingOnDatetime(currDateTime);
            List<int> MissAdd = new List<int>();
            for(int i = 0; i < dtMiss.Rows.Count; i++)
            {
                int idMiss = int.Parse(dtMiss.Rows[i]["idguid"].ToString());
                MissAdd.Add(idMiss);
            }

            for (int i = 0; i < guids.Count; i++)
            {
                int id_ = guids[i].Idguid;
                int count_ = (id_ == 1) ? 0 : guids_checker[id_].Count;
                string fullname_ = Options.GetGuidName(id_);
                StateGuid state_ = (id_ == 1) ? StateGuid.free : guids_checker[id_].Check(time, duration);
                if (MissAdd.Contains(id_))
                    state_ = StateGuid.busy;
                result.Add(new GuidSelectItem(id_, fullname_, count_, state_));
            }            

            return result;
        }
    }
    public enum StateGuid
    {
        busy,
        relax,
        free,
        self
    }
    public class TourTime
    {
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public TimeSpan StartRelax { get; set; }
        public TimeSpan EndRelax { get; set; }
        public int Duration { get; set; }

        public TourTime(TimeSpan start_time, int dur_time)
        {
            Start = start_time;
            End = start_time.Add(new TimeSpan(0, dur_time, 0));
            StartRelax = End;
            EndRelax = End.Add(new TimeSpan(0, 15, 0));
            Duration = dur_time;
        }

        public StateGuid Check(TimeSpan time)
        {
            StateGuid result = StateGuid.free;

            if (time >= Start && time <= End) result = StateGuid.busy;
            if (time >= StartRelax && time <= EndRelax) result = StateGuid.relax;

            return result;
        }
    }
    public class OneGuid
    {
        #region Member Fields
        public int Id { get; set; }
        public int Count { get => tourTimes.Count; }
        private List<TourTime> tourTimes = new List<TourTime>();
        #endregion

        #region Constructor
        public OneGuid(int idguid)
        {
            Id = idguid;
        }
        #endregion

        #region Add/Remove
        public void Add(TimeSpan start_time, int duration)
        {
            TourTime t = new TourTime(start_time, duration);
            tourTimes.Add(t);
        }

        public void Remove(TimeSpan start_time)
        {
            for (int i = 0; i < tourTimes.Count; i++)
                if (tourTimes[i].Start == start_time) tourTimes.Remove(tourTimes[i]);
        }
        #endregion

        #region Check

        public StateGuid Check(TimeSpan time, int duration)
        {
            TimeSpan start = time;
            StateGuid result = StateGuid.free;
            int counter = duration / 15;

            for (int j = 0; j < counter; j++)
            {
                time = start.Add(new TimeSpan(0, j * 15, 0));
                for (int i = 0; i < tourTimes.Count; i++)
                    if ((result = tourTimes[i].Check(time)) != StateGuid.free) break;
                if (result != StateGuid.free) break;
            }
            return result;
        }
        #endregion
    }
    public class GuidSelectItem : INotifyPropertyChanged
    {

        #region members
        private BitmapImage white, red, green, yellow;
        private BitmapImage m_StateImage;
        private StateGuid stateGuid;
        private string m_GuidFullName;
        private int m_GuidId;
        private int m_Count;

        public BitmapImage StateImage
        {
            get => m_StateImage;
            set
            {
                m_StateImage = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StateImage"));
            }
        }
        public StateGuid StateGuid { get => stateGuid; set => stateGuid = value; }
        public string GuidFullName { get => m_GuidFullName; set => m_GuidFullName = value; }
        public int GuidId { get => m_GuidId; set => m_GuidId = value; }
        public int Count
        {
            get => m_Count;
            set
            {
                m_Count = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        public GuidSelectItem(int id, string name, int ct, StateGuid state)
        {
            white = new BitmapImage(new Uri("/WpfControlLibrary;component/Resource/white.png", UriKind.Relative));
            red = new BitmapImage(new Uri("/WpfControlLibrary;component/Resource/red.png", UriKind.Relative));
            green = new BitmapImage(new Uri("/WpfControlLibrary;component/Resource/green.png", UriKind.Relative));
            yellow = new BitmapImage(new Uri("/WpfControlLibrary;component/Resource/yellow.png", UriKind.Relative));

            GuidFullName = name;
            Count = ct;
            GuidId = id;

            switch(state)
            {
                case StateGuid.busy:
                    StateImage = red;
                    break;
                case StateGuid.relax:
                    StateImage = yellow;
                    break;
                case StateGuid.free:
                    StateImage = green;
                    break;
            }
        }

    }
}
