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

            foreach (var group in faculty.Groups)
            {
                Console.WriteLine(group.Title);
                foreach (var @class in group.Classes)
                {
                    Console.WriteLine("\t{0}\t{1}\t{2}\t{3}",
                        @class.ClassTitle, @class.Teacher.Name, @class.Type.ToString(), @class.Hours);
                }
            }

            faculty.SendToDB(connectionString);
        }
    }
}
