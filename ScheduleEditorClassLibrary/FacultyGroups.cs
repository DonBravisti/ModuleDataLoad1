using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ScheduleEditorClassLibrary
{
    public class FacultyGroups
    {
        public List<Group> Groups { get; set; }

        public FacultyGroups()
        {
            Groups = new List<Group>();
        }

        [JsonConstructor]
        public FacultyGroups(List<Group> groups)
        {
            Groups = groups;
        }

        public void Add(Group group)
        {
            Groups.Add(group);
        }

        public void Fill(string connectionString)
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder(connectionString);
            string databaseName = builder.Database;

            MySqlConnection connection = new MySqlConnection(connectionString);
            try
            {                
                connection.Open();
                FillGroupes(connection, databaseName);                
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
            }            
        }

        private void FillGroupes(MySqlConnection connection, string databaseName)
        {
            string queryGetSchedule = $"SELECT * FROM {databaseName}.groups";
            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(queryGetSchedule, connection);
            DataSet dataSetSchedule = new DataSet();
            dataAdapter.Fill(dataSetSchedule);

            DataTable tableSchedule = dataSetSchedule.Tables[0];
            foreach (DataRow row in tableSchedule.Rows)
            {
                string groupTitle = row[1].ToString();
                Group group = new Group(groupTitle);
                group.FillAcademicClasses(connection, databaseName);
                Groups.Add(group);
            }
        }

        public void SendToDB(string connectionString)
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder(connectionString);

            string databaseName = builder.Database;

            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            foreach (var group in Groups)
            {
                int groupId = GetGroupId(group.Title, databaseName, connection);

                foreach (var @class in group.Classes)
                {
                    int employeeId = GetEmployeeId(@class.Teacher.Name, databaseName, connection);
                    int subgroup = (int)@class.SubGroup + 1;
                    int type = (int)@class.Type + 1;

                    string query = $@"INSERT INTO `{databaseName}`.`schedule` 
                                    (`group_id`, `subgroup_num`, `subject_form_id`, `employee_id`)
                                    VALUES ('{groupId}', '{subgroup}', '{type}', '{employeeId}');";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.ExecuteNonQuery();
                }
            }

            connection.Close();
        }

        internal static int GetEmployeeId(string name, string databaseName, MySqlConnection connection)
        {
            string surname = name.Split()[0];
            string queryGetGroupId = $"SELECT id FROM {databaseName}.employees WHERE surname='{surname}' LIMIT 1";
            MySqlCommand command = new MySqlCommand(queryGetGroupId, connection);
            return (int)command.ExecuteScalar();
        }

        internal static int GetGroupId(string title, string databaseName, MySqlConnection connection)
        {
            string queryGetGroupId = $"SELECT id FROM {databaseName}.groups WHERE title='{title}' LIMIT 1";
            MySqlCommand command = new MySqlCommand(queryGetGroupId, connection);
            return (int)command.ExecuteScalar();
        }
    }
}