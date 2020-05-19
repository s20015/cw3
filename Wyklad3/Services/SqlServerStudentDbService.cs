using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wyklad3.Models;
using System.Data.SqlClient;


namespace Wyklad3.Services
{
    public class ValidationError : Exception
    {
        public ValidationError(string message) : base(message) { }
    }

    public class NotFoundError : Exception
    {
        public NotFoundError(string message) : base(message) { }
    }

    public class InternalError: Exception {
        public InternalError(string message) : base (message) { }
    }

    public class SqlServerStudentDbService : IStudentBbService
    {
        private static string conString = "Data Source=db-mssql;Initial Catalog=s20015;Integrated Security=True";

        public void EnrollStudent(EnrollmentRequest request)
        {
            using (var con = new SqlConnection(conString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();

                var trans = con.BeginTransaction();
                com.Transaction = trans;

                try
                {
                    // check if studies exists
                    com.CommandText = "select IdStudy from studies where name= @name";
                    com.Parameters.AddWithValue("name", request.Studies);

                    var r = com.ExecuteReader();
                    if (!r.Read())
                    {
                        trans.Rollback();
                        throw new ValidationError("Study not found");
                    }

                    int idStudy = r.GetInt32(r.GetOrdinal("IdStudy"));
                    r.Close();

                    com.CommandText = "select IdEnrollment from Enrollment where IdStudy = @id and Semester = @semester order by StartDate desc";
                    com.Parameters.AddWithValue("id", idStudy);
                    com.Parameters.AddWithValue("semester", 1);

                    r = com.ExecuteReader();
                    if (!r.Read())
                    {
                        r.Close();
                        // create enrollemnt if not exists
                        com.CommandText = "INSERT INTO Enrollment(IdEnrollment, Semester, IdStudy, StartDate) VALUES(@idEnrollemnt, @semester, @id, @startDate)";

                        var rand = new Random();
                        com.Parameters.AddWithValue("idEnrollemnt", rand.Next(101)); // should be a sequence in DB
                        com.Parameters.AddWithValue("startDate", DateTime.Today);
                        com.ExecuteNonQuery();
                    }

                    if (!r.IsClosed)
                    {
                        r.Close();
                    }

                    // get EnrollmentId
                    com.CommandText = "select IdEnrollment from Enrollment  where IdStudy = @id and Semester = @semester order by StartDate desc";
                    r = com.ExecuteReader();
                    r.Read();
                    var IdEnrollment = r.GetInt32(r.GetOrdinal("IdEnrollment"));
                    r.Close();

                    // create student
                    com.CommandText = "INSERT INTO Student(IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) VALUES(@Index, @FirstName, @LastName, @BirthDate, @Enrollment)";
                    com.Parameters.AddWithValue("Index", request.IndexNumber);
                    com.Parameters.AddWithValue("FirstName", request.FirstName);
                    com.Parameters.AddWithValue("LastName", request.LastName);
                    com.Parameters.AddWithValue("BirthDate", request.BirthDate);
                    com.Parameters.AddWithValue("Enrollment", IdEnrollment);
                    try
                    {
                        com.ExecuteNonQuery();
                    }
                    catch (SqlException exc)
                    { // this shoul be checked more carefully, in this form only an assumption
                        throw new ValidationError("Index number not unique");
                    }
                }
                catch (SqlException exc)
                {
                    trans.Rollback();
                    throw new InternalError(exc.Message);
                }

                trans.Commit();

            }
        }

        public bool IsValidStudent(string index)
        {
            using (var con = new SqlConnection(conString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();

                try
                {
                    com.CommandText = "select IndexNumber from Student where IndexNumber = @index";
                    com.Parameters.AddWithValue("index", index);

                    var r = com.ExecuteReader();
                    if (!r.Read())
                    {
                        return false;
                    } else
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

        public void PromoteStudents(int semester, string studies)
        {
            using (var con = new SqlConnection(conString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();


                try
                {
                    // check if enrollemnt exists
                    com.CommandText = "select IdEnrollment from Enrollment join Studies on Studies.IdStudy = Enrollment.IdStudy  where Studies.Name = @studies and Enrollment.Semester = @semester";
                    com.Parameters.AddWithValue("studies", studies);
                    com.Parameters.AddWithValue("semester", semester);

                    var r = com.ExecuteReader();
                    if (!r.Read())
                    {
                        throw new NotFoundError("Enrollment does not exist");
                    }
                    r.Close();

                    using (var procCom = new SqlCommand("PromoteStudents"))
                    {
                        procCom.Connection = con;
                        procCom.CommandType = System.Data.CommandType.StoredProcedure;

                        procCom.Parameters.Add(new SqlParameter("@Studies", studies));
                        procCom.Parameters.Add(new SqlParameter("@Semester", semester));
                        procCom.ExecuteNonQuery();
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
