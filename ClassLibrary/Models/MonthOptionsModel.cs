using System;

namespace ClassLibrary.Models
{
    public class MonthOptionsModel
    {
        #region members
        private int m_idmonth_options;
        private DateTime m_date_;
        private int m_add_;
        private int m_miss_;
        private int m_work_;
        private int m_only_miss;

        public int Idmonth_options { get => m_idmonth_options; set => m_idmonth_options = value; }
        public DateTime Date_ { get => m_date_; set => m_date_ = value; }
        public int Add_ { get => m_add_; set => m_add_ = value; }
        public int Miss_ { get => m_miss_; set => m_miss_ = value; }
        public int Work_ { get => m_work_; set => m_work_ = value; }
        public int Only_miss { get => m_only_miss; set => m_only_miss = value; }
        #endregion

        #region Constructor
        public MonthOptionsModel()
        {

        }
        #endregion
    }
}
