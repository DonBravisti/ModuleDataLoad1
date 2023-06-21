using ScheduleEditorClassLibrary;
using SpannedDataGridView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleEditorClassLibrary
{

    #region 
    //public class RowItem
    //{
    //    public string Subject;
    //    public string Audience;

    //    public RowItem(string subject, string audience)
    //    {
    //        if (!int.TryParse(audience, out _)) throw new Exception("Аудитория должна быть числом");
    //        Subject = subject;
    //        Audience = audience;
    //    }
    //}
    #endregion
    public class ScheduleRow
    {
        RowTypes rowType;
        //Dictionary<string, ScheduleAcademicClass> items;
        SAcademicClass group1week1;
        SAcademicClass group1week2;
        SAcademicClass group2week1;
        SAcademicClass group2week2;
        void setRowType(SAcademicClass academicClass)
        {
            if (academicClass == null)
            {
                RowType = RowTypes.Simple;
                return;
            }
            if (academicClass.Type == ClassTypes.Lecture && academicClass.Hours > 36) RowType = RowTypes.Simple;
            if (academicClass.Type == ClassTypes.Lecture && academicClass.Hours <= 36) RowType = RowTypes.TwoWeeks;
            if (academicClass.Type == ClassTypes.Practice && academicClass.Hours > 36) RowType = RowTypes.TwoGroups;
            if (academicClass.Type == ClassTypes.Practice && academicClass.Hours <= 36) RowType = RowTypes.TwoGroupsAndTwoWeeks;

        }
        public SAcademicClass Group1week1
        {
            get { return group1week1; }
            set
            {
                setRowType(value);
                //tut
                group1week1 = value;
            }
        }
        public SAcademicClass Group1week2
        {
            get { return group1week2; }
            set { setRowType(value); group1week2 = value; }
        }
        public SAcademicClass Group2week1
        {
            get { return group2week1; }
            set
            {
                setRowType(value);
                group2week1 = value;
            }
        }
        public SAcademicClass Group2week2
        {
            get { return group2week2; }
            set { setRowType(value); group2week2 = value; }
        }
        public DayOfWeek WeekDay { get; set; }
        int classNumber;
        /// <summary>
        /// номер пары
        /// </summary>
        public int ClassNumber
        {
            get
            {
                return classNumber;
            }
            set
            {
                if (value < 1 || value > 4)
                    throw new Exception("номер пары должен быть числом от 1 до 4");
                classNumber = value;
            }
        }
        //List<List<ScheduleAcademicClass>> items;
        public int CountOfWeeks { get; private set; }
        public int CountOfGroups { get; private set; }

        public RowTypes RowType
        {
            get { return rowType; }
            set
            {
                CountOfWeeks = (value == RowTypes.Simple || value == RowTypes.TwoGroups ? 1 : 2);
                CountOfGroups = (value == RowTypes.Simple || value == RowTypes.TwoWeeks ? 1 : 2);
                rowType = value;
            }
        }

        //public Dictionary<string, ScheduleAcademicClass> Items
        //{
        //    get
        //    {
        //        return items;
        //    }
        //}

        public ScheduleRow(DayOfWeek dayOfWeek, int classNumber)
        {
            WeekDay = dayOfWeek;
            ClassNumber = classNumber;
        }
        public ScheduleRow(RowTypes rowType, int classNumber, DayOfWeek dayOfWeek, SAcademicClass group1week1 = null, SAcademicClass group1week2 = null, SAcademicClass group2week1 = null, SAcademicClass group2week2 = null)
        {
            RowType = rowType;
            ClassNumber = classNumber;
            WeekDay = dayOfWeek;
            Group1week1 = group1week1;
            Group1week2 = group1week2;
            Group2week1 = group2week1;
            Group2week2 = group2week2;

            //items["group1week1"] = group1week1;
            //items["group1week2"] = group1week2;
            //items["group2week1"] = group2week1;
            //items["group2week2"] = group2week2;
        }
    }
}

