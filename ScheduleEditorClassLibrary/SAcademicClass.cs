using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleEditorClassLibrary
{
    /// <summary>
    /// Пара которая имеет в расписании конкретное место
    /// </summary>
    public class SAcademicClass : AcademicClass
    {
        public int Audience { get; set; }
        public DayOfWeek WeekDay { get; set; }
        public int ClassNumber { get; set; } // номер пары

        public SAcademicClass(int audience, DayOfWeek weekDay, int classNumber, AcademicClass academicClass) : base(academicClass)
        {
            Audience = audience;
            WeekDay = weekDay;
            ClassNumber = classNumber;
        }
        public string GetTitleAndTeacher()
        {
            return $"{this.ClassTitle} {this.Teacher}";
        }
        public string GetAudience()
        {
            return Audience != 0 ? Audience.ToString() : "";
        }
    }
}
