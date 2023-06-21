using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace ScheduleEditorClassLibrary
{
    public class Schedule
    {
        public List<SGroup> Groups { get; set; }

        public Schedule(List<string> groupTitles)
        {
            Groups = groupTitles.Select(title => new SGroup(title)).ToList();
        }

        [JsonConstructor]
        public Schedule(List<SGroup> groups)
        {
            Groups = groups;
        }

        public SGroup this[string groupTitle]
        {
            get { return Groups.Where(group => group.Title == groupTitle).Single(); }
        }
        public Results IsTeacherAvaible(string activeGroup, int row, int col, AcademicClass academicClass)
        {
            if ((academicClass.SubGroup == SubGroups.First && col >= 2 ||
                academicClass.SubGroup == SubGroups.Second && col < 2) && academicClass.Type != ClassTypes.Lecture)
                return Results.TypeMismatch;
            for (int i = 0; i < Groups.Count; i++)
            {
                var sRow = Groups[i][(DayOfWeek)(row / 8 + 1), (row - row / 8 * 8) / 2 + 1];
                if (sRow == null) continue;
                if (Groups[i].Title == activeGroup)
                {
                    if (academicClass.Type == ClassTypes.Practice)
                    {
                        if (academicClass.Hours > 36)
                        {
                            if (sRow.Group2week1.Teacher == academicClass.Teacher || sRow.Group2week2.Teacher == academicClass.Teacher)
                                return Results.TeacherIsBusy;
                        }
                        else // раз в 2 недели
                        {
                            if (row % 2 == 0 && (sRow.Group1week1.Teacher == academicClass.Teacher || sRow.Group2week1.Teacher == academicClass.Teacher) ||
                                row % 2 != 0 && (sRow.Group1week2.Teacher == academicClass.Teacher || sRow.Group2week2.Teacher == academicClass.Teacher))
                                return Results.TeacherIsBusy;
                        }
                    }
                    continue;
                }

                if (academicClass.Type == ClassTypes.Lecture)
                {
                    if (academicClass.Hours > 36)
                    {
                        if (sRow.Group1week1.Teacher == academicClass.Teacher || sRow.Group1week2.Teacher == academicClass.Teacher ||
                            sRow.Group2week1.Teacher == academicClass.Teacher || sRow.Group2week2.Teacher == academicClass.Teacher)
                            return Results.TeacherIsBusy;
                    }
                    else // раз в 2 недели
                    {
                        if (row % 2 == 0 && (sRow.Group1week1.Teacher == academicClass.Teacher || sRow.Group2week1.Teacher == academicClass.Teacher) ||
                            row % 2 != 0 && (sRow.Group1week2.Teacher == academicClass.Teacher || sRow.Group2week2.Teacher == academicClass.Teacher))
                            return Results.TeacherIsBusy;
                    }
                }
                else if (academicClass.Type == ClassTypes.Practice)
                {
                    if (academicClass.Hours > 36)
                    {
                        if (col < 2 && (sRow.Group1week1.Teacher == academicClass.Teacher || sRow.Group1week2.Teacher == academicClass.Teacher) ||
                            col >= 2 && (sRow.Group2week1.Teacher == academicClass.Teacher || sRow.Group2week2.Teacher == academicClass.Teacher))
                            return Results.TeacherIsBusy;
                    }
                    else // раз в 2 недели
                    {
                        if (row % 2 == 0)
                        {
                            if (col < 2 && sRow.Group1week1.Teacher == academicClass.Teacher ||
                                col >= 2 && sRow.Group2week1.Teacher == academicClass.Teacher)
                                return Results.TeacherIsBusy;
                        }
                        else
                        {
                            if (col < 2 && sRow.Group1week2.Teacher == academicClass.Teacher ||
                                col >= 2 && sRow.Group2week2.Teacher == academicClass.Teacher)
                                return Results.TeacherIsBusy;
                        }
                    }
                }
            }
            return Results.Available;
        }
        public Results IsAudienceAvaible(string activeGroup, int row, int col, AcademicClass academicClass, int aud)
        {
            return Results.Available;
            for (int i = 0; i < Groups.Count; i++)
            {
                //if (Groups[i].Title == activeGroup) continue;
                var sRow = Groups[i][(DayOfWeek)(row / 8 + 1), (row - row / 8 * 8) / 2 + 1];
                if (sRow == null) continue;
                RowTypes rowType;
                if (academicClass.Type == ClassTypes.Lecture && academicClass.Hours > 36) rowType = RowTypes.Simple;
                else if (academicClass.Type == ClassTypes.Lecture && academicClass.Hours <= 36) rowType = RowTypes.TwoWeeks;
                else if (academicClass.Type == ClassTypes.Practice && academicClass.Hours > 36) rowType = RowTypes.TwoGroups;
                else if (academicClass.Type == ClassTypes.Practice && academicClass.Hours <= 36) rowType = RowTypes.TwoGroupsAndTwoWeeks;

                bool simple = academicClass.Type == ClassTypes.Lecture && academicClass.Hours > 36 && sRow.Group1week1.Teacher == academicClass.Teacher;
                bool twoWeeks1 = academicClass.Type == ClassTypes.Lecture && academicClass.Hours <= 36 && row % 2 == 0 && sRow.Group1week1.Teacher == academicClass.Teacher;
                bool twoWeeks2 = academicClass.Type == ClassTypes.Lecture && academicClass.Hours <= 36 && row % 2 != 0 && sRow.Group1week2.Teacher == academicClass.Teacher;
                bool twoGroups1 = academicClass.Type == ClassTypes.Practice && academicClass.Hours > 36 && col < 2 && sRow.Group1week1.Teacher == academicClass.Teacher;
                bool twoGroups2 = academicClass.Type == ClassTypes.Practice && academicClass.Hours > 36 && col > 2 && sRow.Group2week1.Teacher == academicClass.Teacher;
                bool twoGroupsAndTwoWeeks1 = academicClass.Type == ClassTypes.Practice && academicClass.Hours <= 36 && col < 2 && row % 2 == 0 && sRow.Group1week1.Teacher == academicClass.Teacher;
                bool twoGroupsAndTwoWeeks2 = academicClass.Type == ClassTypes.Practice && academicClass.Hours <= 36 && col < 2 && row % 2 != 0 && sRow.Group1week2.Teacher == academicClass.Teacher;
                bool twoGroupsAndTwoWeeks3 = academicClass.Type == ClassTypes.Practice && academicClass.Hours <= 36 && col >= 2 && row % 2 == 0 && sRow.Group2week1.Teacher == academicClass.Teacher;
                bool twoGroupsAndTwoWeeks4 = academicClass.Type == ClassTypes.Practice && academicClass.Hours <= 36 && col >= 2 && row % 2 != 0 && sRow.Group2week2.Teacher == academicClass.Teacher;

                if (simple || twoWeeks1 || twoWeeks2 || twoGroups1 || twoGroups2 || twoGroupsAndTwoWeeks1 || twoGroupsAndTwoWeeks2 || twoGroupsAndTwoWeeks3 || twoGroupsAndTwoWeeks4)
                {
                    return Results.TeacherIsBusy;
                }
            }
            return Results.Available;
        }
        public void PutData(string activeGroup, int row, int col, AcademicClass academicClass, int audience)
        {
            var weekDay = (DayOfWeek)(row / 8 + 1);
            var сlassNumber = (row - ((int)weekDay - 1) * 8) / 2 + 1; // [1 - 4]
            var sRow = this[activeGroup][weekDay, сlassNumber];
            if (sRow == null)
            {
                this[activeGroup].Add(new ScheduleRow(weekDay, сlassNumber));
                sRow = this[activeGroup][weekDay, сlassNumber];
            }
            var sAcademicClass = new SAcademicClass(audience, weekDay, сlassNumber, academicClass);
            if (academicClass.Type == ClassTypes.Lecture)
            {
                if (academicClass.Hours > 36)
                {
                    sRow.Group1week1 = sAcademicClass;
                    sRow.Group1week2 = sAcademicClass;
                    sRow.Group2week1 = sAcademicClass;
                    sRow.Group2week2 = sAcademicClass;
                }
                else
                {
                    if (row % 2 == 0)
                    {
                        sRow.Group1week1 = sAcademicClass;
                        sRow.Group2week1 = sAcademicClass;
                    }
                    else
                    {
                        sRow.Group1week2 = sAcademicClass;
                        sRow.Group2week2 = sAcademicClass;
                    }
                }
            }
            else
            {
                if (academicClass.Hours > 36)
                {
                    if (col < 2)
                    {
                        sRow.Group1week1 = sAcademicClass;
                        sRow.Group1week2 = sAcademicClass;
                    }
                    else
                    {
                        sRow.Group2week1 = sAcademicClass;
                        sRow.Group2week2 = sAcademicClass;
                    }
                }
                else
                {
                    if (row % 2 == 0 && col < 2)
                    {
                        sRow.Group1week1 = sAcademicClass;
                    }
                    else if (row % 2 == 0)
                    {
                        sRow.Group2week1 = sAcademicClass;
                    }
                    else if (col < 2)
                    {
                        sRow.Group1week2 = sAcademicClass;
                    }
                    else
                    {
                        sRow.Group2week2 = sAcademicClass;
                    }
                }
            }

            //if (academicClass.Type == ClassTypes.Lecture && (academicClass.Hours <= 36 && row % 2 == 0 || academicClass.Hours > 36) ||
            //academicClass.Type == ClassTypes.Practice && academicClass.Hours > 36 && col < 2)
            //{
            //    sRow.Group1week1 = sAcademicClass;
            //}
            //else if (academicClass.Type == ClassTypes.Lecture)
            //{
            //    if (academicClass.Hours <= 36 && row % 2 != 0) // раз в 2 недели
            //    {
            //        sRow.Group1week2 = sAcademicClass;
            //    }
            //}
            //else
            //{
            //    if (academicClass.Hours <= 36) // раз в 2 недели
            //    {
            //        if (row % 2 == 0 && col < 2)
            //        {
            //            sRow.Group1week1 = sAcademicClass;
            //        }
            //        else if (row % 2 == 0)
            //        {
            //            sRow.Group2week1 = sAcademicClass;
            //        }
            //        else if (col < 2)
            //        {
            //            sRow.Group1week2 = sAcademicClass;
            //        }
            //        else
            //        {
            //            sRow.Group2week2 = sAcademicClass;
            //        }
            //    }
            //    else if (col >= 2)
            //    {
            //        sRow.Group2week1 = sAcademicClass;
            //    }
            //}            
        }



        //Отравка расписания в базу данных
        public void SendToDB(string connectionString)
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder(connectionString);

            string databaseName = builder.Database;

            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            foreach (var group in Groups)
            {
                int groupId = FacultyGroups.GetGroupId(group.Title, databaseName, connection);
                foreach (var row in group.Rows)
                {
                    SendClassToDB(databaseName, connection, groupId, row.Group1week1);
                    //SendClassToDB(databaseName, connection, groupId, row.Group1week2);
                    //SendClassToDB(databaseName, connection, groupId, row.Group2week1);
                    //SendClassToDB(databaseName, connection, groupId, row.Group2week2);
                }
            }

            connection.Close();
        }

        private void SendClassToDB(string databaseName, MySqlConnection connection, int groupId, SAcademicClass academicClass)
        {
            if (academicClass == null) return;

            int employeeId = GetEmployeeId(academicClass.Teacher.Name, databaseName, connection);
            int subgroup = (int)academicClass.SubGroup + 1;
            int type = (int)academicClass.Type + 1;
            int cRoomFundId = SetAndGetCRoomFundId(academicClass, databaseName, connection);
            int eduSemId = GetEduSemId(academicClass.ClassTitle, databaseName, connection);

            string query = $@"INSERT INTO `{databaseName}`.`schedule` 
                                    (`group_id`, `subgroup_num`, `edu_semester_id`, `subject_form_id`, `croom_fund_id`, `employee_id`)
                                    VALUES ('{groupId}', '{subgroup}', '{eduSemId}', '{type}', '{cRoomFundId}', '{employeeId}');";
            MySqlCommand command = new MySqlCommand(query, connection);
            command.ExecuteNonQuery();
        }

        private int GetEduSemId(string classTitle, string databaseName, MySqlConnection connection)
        {
            string query = $@"SELECT id FROM {databaseName}.edu_semesters where edu_plan_id=
                             (SELECT id FROM {databaseName}.edu_plan where subject_id=
                             (SELECT id FROM {databaseName}.subjects Where title='{classTitle}'));";
            MySqlCommand command = new MySqlCommand(query, connection);
            return (int)command.ExecuteScalar();
        }

        private int SetAndGetCRoomFundId(SAcademicClass academicClass, string databaseName, MySqlConnection connection)
        {
            string queryGetClassroomId = $"SELECT id FROM {databaseName}.classrooms WHERE number='{academicClass.Audience}' LIMIT 1";
            MySqlCommand command = new MySqlCommand(queryGetClassroomId, connection);
            int AudId = (int)command.ExecuteScalar();
            int weekDay = (int)academicClass.WeekDay;

            string queryInsertCRoomFund = $@"INSERT INTO `test`.`classroom_funds` (`day_week`, `lesson_num`, `week_num`, `croom_id`)
                                             VALUES ('{weekDay}', '{academicClass.ClassNumber}',
                                             '1', '{AudId}');";
            command = new MySqlCommand(queryInsertCRoomFund, connection);
            command.ExecuteNonQuery();

            string queryGetCRoomFundId = $"SELECT LAST_INSERT_ID();";
            command = new MySqlCommand(queryGetCRoomFundId, connection);
            return int.Parse(command.ExecuteScalar().ToString());
        }

        private int GetEmployeeId(string name, string databaseName, MySqlConnection connection)
        {
            string surname = name.Split()[0];
            string queryGetGroupId = $"SELECT id FROM {databaseName}.employees WHERE surname='{surname}' LIMIT 1";
            MySqlCommand command = new MySqlCommand(queryGetGroupId, connection);
            return (int)command.ExecuteScalar();
        }
    }
}
