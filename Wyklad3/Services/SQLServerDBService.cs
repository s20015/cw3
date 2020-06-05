using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wyklad3.Models;
using System.Data.SqlClient;
using Wyklad3.DTOs;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

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

        // salt should be stored next to password hash in db, simplified solution here
        public bool AreCredentialsValid(LoginRequest request, byte[] salt)
        {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: request.Password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            using (var con = new SqlConnection(conString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();

                try
                {
                    com.CommandText = "select IndexNumber from Student where IndexNumber = @index and Password = @password";
                    com.Parameters.AddWithValue("index", request.Login);
                    com.Parameters.AddWithValue("password", hashed);

                    var r = com.ExecuteReader();
                    if (!r.Read())
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                catch (SqlException exc)
                {
                    throw new InternalError(exc.Message);
                }
            }
        }

        public void SetNewRefreshToken(string login, string refreshToken)
        {
            using (var con = new SqlConnection(conString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();


                try
                {
                    com.CommandText = "update Student set RefreshToken = @token where IndexNumber = @index";
                    com.Parameters.AddWithValue("token", refreshToken);
                    com.Parameters.AddWithValue("index", login);

                    com.ExecuteNonQuery();
                }
                catch (SqlException exc)
                {
                    throw new InternalError(exc.Message);
                }
            }
        }

        public Student GetStudentByRefreshToken(string refreshToken)
        {
            using (var con = new SqlConnection(conString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();

                try
                {
                    com.CommandText = @"select IndexNumber, FirstName, LastName, BirthDate from Student where RefreshToken = @token";
                    com.Parameters.AddWithValue("token", refreshToken);

                    var r = com.ExecuteReader();
                    if (!r.Read())
                    {
                        throw new StudentNotFoundException();
                    }
                    else
                    {
                        var student = new Student();
                        student.IndexNumber = r.GetString(r.GetOrdinal("IndexNumber"));
                        student.FirstName = r.GetString(r.GetOrdinal("FirstName"));
                        student.LastName = r.GetString(r.GetOrdinal("LastName"));
                        student.BirthDate = r.GetDateTime(r.GetOrdinal("BirthDate"));

                        return student;
                    }
                }
                catch (SqlException exc)
                {
                    throw new InternalError(exc.Message);
                }
            }
        }
    }
}
