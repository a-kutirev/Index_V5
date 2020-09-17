using System;
using System.Collections.Generic;

namespace UtilsLib.StatWizard
{
    public struct Period
    {
        public DateTime StartDate;
        public DateTime EndDate;
    }

    public static class DataForStatWizard
    {
        public static List<Period> periods;
        public static List<int> categoryList;
        public static int PeriodType;
        public static int ReportType;
    }
}
