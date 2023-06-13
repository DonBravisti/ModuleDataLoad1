using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleEditorClassLibrary
{
    /// <summary>
    /// хранит в себе ScheduleRow
    /// </summary>
    public class ScheduleFacultyRows
    {
        public List<ScheduleRow> Data { get; set; }

        public ScheduleFacultyRows()
        {
            Data = new List<ScheduleRow>(20); // 5 дней в неделю по 4 пары
            for (int i = 0; i < 20; i++)
            {
                Data.Add(null);    
            }
        }
        public ScheduleRow this [int index]
        {
            get { return Data[index]; }
            set { Data[index] = value; }
        }
        public int Count()
        {
            return Data.Count;
        }
    }
}
