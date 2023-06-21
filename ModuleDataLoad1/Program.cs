using ScheduleEditorClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleDataLoad1
{
    internal class Program
    {
        static string connectionString = "server=localhost;database=db;user id=root;password=";

        static void Main(string[] args)
        {
            FacultyGroups faculty = new FacultyGroups();
            faculty.Fill(connectionString);

            List<SGroup> groups = new List<SGroup>();

            foreach (var group in faculty.Groups)
            {
                List<ScheduleRow> rows = new List<ScheduleRow>();

                Console.WriteLine(group.Title);
                foreach (var @class in group.Classes)
                {
                    Console.WriteLine("\t{0}\t{1}\t{2}\t{3}",
                        @class.ClassTitle, @class.Teacher.Name, @class.Type.ToString(), @class.Hours);

                    ScheduleRow row = new ScheduleRow(DayOfWeek.Friday, 3);
                    row.Group1week1 = new SAcademicClass(609, DayOfWeek.Friday, 2, @class);
                    rows.Add(row);
                }
                SGroup group1 = new SGroup(group.Title, rows);
                groups.Add(group1);
            }

            Schedule schedule = new Schedule(groups);
            schedule.SendToDB(connectionString);

            Console.WriteLine("sent succesfully...");
        }
    }
}
