using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wyklad3.Models
{
    public class Student
    {
        //prop+tabx2
        public int IdStudent { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string IndexNumber { get; set; }
        public Enrollment Enrollment { get; set; }
    }

    public class Enrollment
    {
        public int IdEnrollment { get; set; }
        public int Semester { get; set; }
        public DateTime StartDate { get; set; }
        public Study Study { get; set; }
    }

    public class Study
    {
        public int IdStudy { get; set; }
        public string Name { get; set; }
    }
}
