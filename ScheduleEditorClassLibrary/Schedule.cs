using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
    }
}
