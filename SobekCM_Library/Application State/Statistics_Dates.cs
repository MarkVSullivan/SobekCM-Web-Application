#region Using directives

using System;
using System.Data;

#endregion

namespace SobekCM.Library.Application_State
{
    /// <summary> Stores the range of dates for which usage statistical information exists in the database for querying </summary>
    public class Statistics_Dates
    {
        /// <summary> Constructor for a new instance of the Statistics_Dates object </summary>
        public Statistics_Dates()
        {
            Clear();
        }

        /// <summary> Constructor for a new instance of the Statistics_Dates object </summary>
        /// <param name="dateRangeTable"> Table which lists every date statistical information exists </param>
        public Statistics_Dates(DataTable dateRangeTable)
        {
            Clear();

            Set_Statistics_Dates(dateRangeTable);
        }

        /// <summary> Month portion of earliest date for usage stats </summary>
        /// <remarks> 1 = January, 2 = February, .. 12 = December </remarks>
        public int Earliest_Month { get; private set; }

        /// <summary> Year portion of earliest date for usage stats </summary>
        public int Earliest_Year { get; private set; }

        /// <summary> Month portion of last date for usage stats </summary>
        /// <remarks> 1 = January, 2 = February, .. 12 = December </remarks>
        public int Latest_Month { get; private set; }

        /// <summary> Year portion of last date for usage stats </summary>
        public int Latest_Year { get; private set; }

        /// <summary> Sets the date range from the table of statistical months available </summary>
        /// <param name="dateRangeTable"> Table which lists every date statistical information exists</param>
        internal void Set_Statistics_Dates( DataTable dateRangeTable )
        {
            if ((dateRangeTable != null) && (dateRangeTable.Rows.Count > 0))
            {
                Earliest_Year = Convert.ToInt32(dateRangeTable.Rows[0][0]);
                Earliest_Month = Convert.ToInt32(dateRangeTable.Rows[0][1]);

                Latest_Year = Convert.ToInt32(dateRangeTable.Rows[dateRangeTable.Rows.Count - 1][0]);
                Latest_Month = Convert.ToInt32(dateRangeTable.Rows[dateRangeTable.Rows.Count - 1][1]);
            }
        }

        /// <summary> Sets the statistics dates to the default </summary>
        internal void Clear()
        {
            Earliest_Month = 3;
            Earliest_Year = 2006;
            Latest_Month = DateTime.Now.Month;
            Latest_Year = DateTime.Now.Year;
        }
    }
}
