using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleEditorClassLibrary
{
    public class TeacherPreference
    {
        public DayOfWeek WeekDay { get; set; }
        public int LessonNumber { get; set; }
        public TeacherPreference(DayOfWeek weekDay, int lessonNumber)
        {
            WeekDay = weekDay;
            LessonNumber = lessonNumber;
        }
    }
}
