using DBWrapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassLibrary
{

    public enum ExpoCheckResult
    {
        RES_OK,
        RES_BAD_TIME,
        RES_NO_FREE_GUIDS
    }

    public class ExpoChecker
    {
        #region Member fields
        private DateTime m_date;        
        private int m_startHour;
        private int m_guidCounter = 0;
        private List<DisplayedGroupModel> m_displayedGroupModels;
        private List<GuidLine> m_guidTimeChecker = new List<GuidLine>();
        private Dictionary<int, string> m_startTimechecker = new Dictionary<int, string>();

        private bool CoronaMode = false;
        private TimeSpan maxDif = new TimeSpan(0,45,0);


        public DateTime Date
        {
            get => m_date;
            set
            {
                m_date = value;
                m_startHour = DayOptions.StartHour;

                #region Количество экскурсоводов
                string sql = $"Call GetGuidsByDate('{m_date.ToString("yyyy-MM-dd")}')";
                m_guidCounter = MySqlWrapper.Select(sql).Rows.Count - 1;
                #endregion

                #region Инициализация 'чекеров'
                m_guidTimeChecker.Clear();
                m_startTimechecker.Clear();

                for(int i = 0; i< m_guidCounter; i++)
                    m_guidTimeChecker.Add(new GuidLine());

                m_startTimechecker.Clear();

                DisplayedGroupModels = (List<DisplayedGroupModel>)MySqlWrapper.SelectGroupByDate(m_date).ToList<DisplayedGroupModel>();
                #endregion
            }
        }
        public List<DisplayedGroupModel> DisplayedGroupModels
        {
            get => m_displayedGroupModels;
            set
            {
                m_displayedGroupModels = value;

                #region Заполнение чекеров

                int groupCount = m_displayedGroupModels.Count;

                for (int i = 0; i < groupCount; i++)
                {
                    if ((m_displayedGroupModels[i].Groupstatus & 1) != 0) continue;

                    int zone = Options.GetZoneIdByTourId(m_displayedGroupModels[i].Idtour);
                    string str = $"{m_displayedGroupModels[i].Grouptime.Hours}:{m_displayedGroupModels[i].Grouptime.Minutes}#{zone}";
                    if (m_startTimechecker.ContainsKey(m_displayedGroupModels[i].Idgroup))
                        m_startTimechecker[m_displayedGroupModels[i].Idgroup] = str;
                    else m_startTimechecker.Add(m_displayedGroupModels[i].Idgroup, str);                    

                    for (int j = 0; j < m_guidCounter; j++)
                    {
                        if (m_guidTimeChecker[j].CheckTime(m_displayedGroupModels[i]))
                        {
                            m_guidTimeChecker[j].AddGroup(m_displayedGroupModels[i]);
                            break;
                        }
                    }
                }

                #endregion
            }
        }
        #endregion

        #region Constructor
        public ExpoChecker()
        {

        }
        #endregion

        #region Проверка
        public ExpoCheckResult Check(GroupModel gm)
        {
            ExpoCheckResult result = ExpoCheckResult.RES_OK;

            int zone = Options.GetZoneIdByTourId(gm.Idtour);
            string str = $"{gm.Grouptime.Hours}:{gm.Grouptime.Minutes}#{zone}";

            if (m_startTimechecker.ContainsValue(str))
                if (m_startTimechecker.ContainsKey(gm.Idgroup))
                    if(m_startTimechecker[gm.Idgroup] != str)
                        result = ExpoCheckResult.RES_BAD_TIME;


            // Работа по графику в режиме коронавируса
            if (CoronaMode)
            {
                TimeSpan groupTime = gm.Grouptime;

                for(int i = 0; i < m_startTimechecker.Count; i++)
                {
                    int curIdZone = int.Parse(m_startTimechecker.ElementAt(i).Value.Split('#')[1]);
                    if(curIdZone == zone)
                    {
                        TimeSpan curTimeSpan = TimeSpan.Parse(m_startTimechecker.ElementAt(i).Value.Split('#')[0]);
                        TimeSpan diff;

                        if (curTimeSpan > groupTime)
                            diff = curTimeSpan - groupTime;
                        else
                            diff = groupTime - curTimeSpan;

                        if((diff <= maxDif) && (m_startTimechecker.ElementAt(i).Key != gm.Idgroup))
                        {                           
                            result = ExpoCheckResult.RES_BAD_TIME;
                            break;
                        }
                    }
                }
            }

            bool tmp = false;

            for(int i = 0; i < m_guidCounter; i++)
                if (m_guidTimeChecker[i].CheckTime(gm)) tmp = true; ;                

            result = tmp ? result : ExpoCheckResult.RES_NO_FREE_GUIDS;

            return result;
        }
        #endregion

        #region Добавить группу
        public void AddGroup(GroupModel gm)
        {
            // 1. Удаление если уже добавлена (m_startTimeChecker)

            if (m_startTimechecker.ContainsKey(gm.Idgroup))
                m_startTimechecker.Remove(gm.Idgroup);

            // 2. Проверка возможность добавления и добавление (m_startTimeChecker)

            int zone = Options.GetZoneIdByTourId(gm.Idtour);
            string str = $"{gm.Grouptime.Hours}:{gm.Grouptime.Minutes}#{zone}";
            m_startTimechecker.Add(gm.Idgroup, str);

            // 1. Удаление если уже добавлена (m_guidTimeChecker)

            for (int i = 0; i < m_guidCounter; i++)
                if(m_guidTimeChecker[i].ExistGroup(gm))
                {
                    m_guidTimeChecker[i].RemoveGroup(gm);
                    break;
                }

            // 2. Проверка возможность добавления и добавление (m_guidTimeChecker)

            for (int i = 0; i < m_guidCounter; i++)
                if(m_guidTimeChecker[i].CheckTime(gm))
                {
                    m_guidTimeChecker[i].AddGroup(gm);
                    break;
                }
        }
        #endregion

        #region Удаление группы
        public void RemoveGroup(GroupModel gm)
        {
            // 1. Удаление (m_startTimeChecker)
            if (m_startTimechecker.ContainsKey(gm.Idgroup))
                m_startTimechecker.Remove(gm.Idgroup);

            // 2. Удаление (m_guidTimeChecker)
            for (int i =0; i < m_guidCounter; i++)
                if(m_guidTimeChecker[i].ExistGroup(gm))
                {
                    m_guidTimeChecker[i].RemoveGroup(gm);
                    break;
                }
        }
        #endregion
    }

    public class GuidLine
    {
        #region Members 
        private List<int> m_line = new List<int>();
        private int relax = 0;

        private Dictionary<int, int> TourDuration = new Dictionary<int, int>();
        private Dictionary<int, int> TourZones = new Dictionary<int, int>();
        private List<int> groupsId = new List<int>();

        #endregion

        #region Constructor
        public GuidLine()
        {
            #region Экскурсия - продолжительность, место проведения
            string sql = "select * from tours";
            TourDuration.Clear();
            TourZones.Clear();
            List<TourModel> tmpTM = (List<TourModel>)DBWrapper.MySqlWrapper.Select(sql).ToList<TourModel>();
            for (int i = 0; i < tmpTM.Count; i++)
            {
                TourDuration.Add(tmpTM[i].Idtour, tmpTM[i].Tourduration);
                TourZones.Add(tmpTM[i].Idtour, tmpTM[i].Idexpo_zone);
            }
            #endregion

            for (int i = 0; i < 96; i++)
                m_line.Add(0);

            groupsId.Clear();
        }
        #endregion

        #region Добавить группу
        public void AddGroup(GroupModel model)
        {           
            int index = Conventers.TimeSpanToIndex(model.Grouptime);
            int duration = TourDuration[model.Idtour] + relax;
            int min = 0;
            if (model.Idgroup == 0)
            {
                for (int i = 0; i < 96; i++)
                    if (m_line[i] < min) min = m_line[i];

                model.Idgroup = --min;
            }

            for(int i  = 0; i < (int)(duration / 15); i++)
                m_line[index + i] = model.Idgroup == 0 ? -1 : model.Idgroup;

            groupsId.Add(model.Idgroup);
        }
        #endregion

        #region Проверка есть ли группа в списке
        public bool ExistGroup(GroupModel model)
        {
            return groupsId.Contains(model.Idgroup);
        }
        #endregion

        #region Проверка есть ли время для выбранной экскурсии
        public bool CheckTime(GroupModel gm)
        {
            bool result = true;

            int index = Conventers.TimeSpanToIndex(gm.Grouptime);
            int dur = (int)(TourDuration[gm.Idtour] / 15) + 1;
            for(int i = 0; i < dur; i++)
            {
                if((m_line[index + i] != 0) && (m_line[index + i] != gm.Idgroup))
                {
                    result = false;
                    break;
                }
            }

            return result;
        }
        #endregion

        #region Удалить группу
        public void RemoveGroup(GroupModel model)
        {
            for (int i = 0; i < 96; i++)
                if (m_line[i] == model.Idgroup) m_line[i] = 0;

            if (groupsId.Contains(model.Idgroup)) groupsId.Remove(model.Idgroup);
        }
        #endregion
    }
}
