using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wyklad3.Models;
using System.Data.SqlClient;

namespace Wyklad3.Services
{
    public class SQLServerDBService : IDbService
    {
        private static string conString = "Data Source=db-mssql;Initial Catalog=s20015;Integrated Security=True";
        public void CreateStudent(Student student)
        {
            throw new NotImplementedException();
        }

        public void DeleteStudent(int id)
        {
            throw new NotImplementedException();
        }

        public Student GetStudent(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Student> GetStudents()
        {
            var students = new List<Student>();

            using (var con = new SqlConnection(conString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = @"select IndexNumber, FirstName, LastName, BirthDate, Student.IdEnrollment, Semester, StartDate, Studies.IdStudy, Name from Student
                                    join Enrollment on Student.IdEnrollment = Enrollment.IdEnrollment
                                    join Studies on Enrollment.IdStudy = Studies.IdStudy;";

                con.Open();
                var r = com.ExecuteReader();
                while (r.Read())
                {
                    var student = new Student();
                    student.IndexNumber = r.GetString(r.GetOrdinal("IndexNumber"));
                    student.FirstName = r.GetString(r.GetOrdinal("FirstName"));
                    student.LastName = r.GetString(r.GetOrdinal("LastName"));
                    student.BirthDate = r.GetDateTime(r.GetOrdinal("BirthDate"));

                    var enrollment = new Enrollment();
                    enrollment.IdEnrollment = r.GetInt32(r.GetOrdinal("IdEnrollment"));
                    enrollment.Semester = r.GetInt32(r.GetOrdinal("Semester"));
                    enrollment.StartDate = r.GetDateTime(r.GetOrdinal("StartDate"));

                    var study = new Study();
                    study.IdStudy = r.GetInt32(r.GetOrdinal("IdStudy"));
                    study.Name = r.GetString(r.GetOrdinal("Name"));

                    enrollment.Study = study;
                    student.Enrollment = enrollment;

                    students.Add(student);
                }
            }

            return students;
        }

        public void UpdateStudent(Student student)
        {
            throw new NotImplementedException();
        }
    }
}
