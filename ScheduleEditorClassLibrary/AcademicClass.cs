using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ScheduleEditorClassLibrary
{
    public class AcademicClass
    {
        /// <summary>
        /// название пары
        /// </summary>
        public string ClassTitle { get; set; }
        public Teacher Teacher { get; set; }
        public int Hours { get; set; }
        public ClassTypes Type { get; set; }
        public SubGroups SubGroup { get; set; }
        [JsonConstructor]
        public AcademicClass(string classTitle, Teacher teacher, int hours, ClassTypes type, SubGroups subGroup)
        {
            this.ClassTitle = classTitle;
            this.Teacher = teacher;
            this.Hours = hours;
            this.Type = type;
            this.SubGroup = subGroup;
        }
        public AcademicClass(AcademicClass @class)
        {
            ClassTitle = @class.ClassTitle;
            Teacher = @class.Teacher;
            Hours = @class.Hours;
            Type = @class.Type;
            SubGroup = @class.SubGroup;
        }
    }
}
