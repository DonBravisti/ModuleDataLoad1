using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace ScheduleEditorClassLibrary
{
    public class Group
    {
        public string Title;
        public List<AcademicClass> Classes { get; set; } // ScheduleAcademicClass

        public Group(string title)
        {
            Classes = new List<AcademicClass>();
            Title = title;
        }
        [JsonConstructor]
        public Group(string title, List<AcademicClass> classes)
        {
            Classes = classes;
            Title = title;
        }
        public void Add(AcademicClass academicClass)
        {
            Classes.Add(academicClass);
        }

        internal void FillAcademicClasses(MySqlConnection connection, string databaseName)
        {
            int groupId = GetGroupId(Title, databaseName, connection);

            string queryGetLoads = $"SELECT * FROM {databaseName}.empl_loads WHERE group_id = {groupId}";
            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(queryGetLoads, connection);
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);

            DataTable emplLoads = dataSet.Tables[0];
            foreach (DataRow row in emplLoads.Rows)
            {
                string classTitle = row[6].ToString();
                string teacherName = GetTeacherName(row[3].ToString(), databaseName, connection);
                int hours = (int)double.Parse(row[10].ToString());
                string classTypeString = GetClassType(row[8].ToString(), databaseName, connection);
                ClassTypes classType = classTypeString == "Лекция" ? ClassTypes.Lecture : ClassTypes.Practice;

                AcademicClass @class = 
                    new AcademicClass(classTitle, new Teacher(teacherName), hours, classType, SubGroups.First);

                Classes.Add(@class);
            }
        }

        private int GetGroupId(string title, string databaseName, MySqlConnection connection)
        {
            string queryGetGroupId = $"SELECT id FROM {databaseName}.groups WHERE title='{title}' LIMIT 1";
            MySqlCommand command = new MySqlCommand(queryGetGroupId, connection);
            return (int)command.ExecuteScalar();
        }

        private string GetClassType(string id, string databaseName, MySqlConnection connection)
        {
            string queryGetClassType = $"SELECT title FROM {databaseName}.subject_forms where id={id}";
            MySqlCommand command = new MySqlCommand(queryGetClassType, connection);
            return (string)command.ExecuteScalar();
        }

        private string GetTeacherName(string id, string databaseName, MySqlConnection connection)
        {
            string query = $"SELECT surname, name, patronimyc FROM {databaseName}.employees Where id={id}";
            MySqlCommand command = new MySqlCommand(query, connection);
            var reader = command.ExecuteReader();

            string teacherName = "";
            while (reader.Read())
            {
                var surname = reader.GetValue(0);
                var name = reader.GetValue(1);
                var patronimyc = reader.GetValue(2);

                teacherName = string.Format("{0} {1} {2}", surname, name, patronimyc);
            }
            reader.Close();

            return teacherName;
        }
    }
}