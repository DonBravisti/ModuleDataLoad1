using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleEditorClassLibrary
{
    public class Teacher
    {
        public string Name { get; set; }
        public List<TeacherPreference> Preferences { get; set; }

        public Teacher(string name)
        {
            Name = name;
            Preferences = new List<TeacherPreference>();
        }
        public override string ToString()
        {
            return Name;
        }
        public static bool operator ==(Teacher teacher1, Teacher teacher2)
        {
            if (object.ReferenceEquals(teacher1, teacher2))
            {
                return true;
            }

            if (object.ReferenceEquals(teacher1, null) || object.ReferenceEquals(teacher2, null))
            {
                return false;
            }

            return teacher1.Name == teacher2.Name;
        }

        public static bool operator !=(Teacher person1, Teacher person2)
        {
            return !(person1 == person2);
        }
   


    }
}
