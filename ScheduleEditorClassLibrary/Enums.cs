using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleEditorClassLibrary
{
    public enum ClassTypes
    {
        Lecture,
        Practice
    }

    public enum SubGroups
    {
        First,
        Second
    }
    public enum RowTypes
    {
        Simple,
        TwoGroups,
        TwoWeeks,
        TwoGroupsAndTwoWeeks
    }
    public enum Results
    {
        Available,
        TeacherIsBusy,
        TypeMismatch,
        AudienceIsOccupied
    }
}
