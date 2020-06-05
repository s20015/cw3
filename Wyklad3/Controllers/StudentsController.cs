using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Wyklad3.Models;
using Wyklad3.Services;
using System.Collections.Generic;
using System.Data.SqlClient;
using Wyklad3.DTOs;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;

namespace Wyklad3.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        public IConfiguration Configuration { get; set; }
        private IDbService _dbService;

        public StudentsController(IDbService service)
        {
            _dbService = service;
        }

        //2. QueryString
        [HttpGet]
        public IActionResult GetStudents([FromQuery]string orderBy)
        {
            Type studentType = typeof(Student);

            if (orderBy != null && studentType.GetProperty(orderBy) == null)
            {
                return BadRequest("wrong orderBy parameter");
            }

            if (orderBy == "" || orderBy == null)
            {
                return Ok(_dbService.GetStudents());
            }

            // orderBy parameters should excactly match parametes in response ( IdStudent != idStudent )
            return Ok(_dbService.GetStudents().OrderBy(s => s.GetType().GetProperty(orderBy).GetValue(s)));
        }

        //[FromRoute], [FromBody], [FromQuery]
        //1. URL segment
        [HttpGet("{id}")]
        public IActionResult GetStudent([FromRoute]int id) //action method
        {
            try
            {
                return Ok(_dbService.GetStudent(id));
            }
            catch (StudentNotFoundException)
            {
                return NotFound("Student was not found");
            }

        }

        //3. Body - cialo zadan
        [HttpPost]
        public IActionResult CreateStudent([FromBody]Student student)
        {
            try
            {
                _dbService.CreateStudent(student);

            } catch (StudentIdAlseadyExistsException)
            {
                return BadRequest("Student with this id already exists");
            }

            return Ok(student); //JSON
        }


        [HttpPut("{id}")]
        public IActionResult UpdateStudent([FromRoute] int id, [FromBody]Student student)
        {
            if ( id != student.IdStudent)
            {
                return BadRequest("Changing id is forbidden");
            }

            try {
                _dbService.UpdateStudent(student);
            } catch (StudentNotFoundException)
            {
                return NotFound("Student was not found");
            }

            return Ok("Student updated");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent([FromRoute]int id)
        {
            try
            {
                _dbService.DeleteStudent(id);
                return Ok("Student deleted");
            }
            catch (StudentNotFoundException)
            {
                return NotFound("Student was not found");
            }
        }

        //[FromRoute], [FromBody], [FromQuery]
        //1. URL segment
        [HttpGet("{id}/enrollments")]
        public IActionResult GetStudentEnrollments([FromRoute]int id) //action method
        {
            var enrollments = new List<Enrollment>();
            // Should have dedicated DAL/repository (just like students), should have error handling
            string conString = "Data Source=db-mssql;Initial Catalog=s20015;Integrated Security=True";
            using (var con = new SqlConnection(conString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = @"select Enrollment.IdEnrollment, Semester, StartDate, Studies.IdStudy, Name from Enrollment
                                    join Student on Student.IdEnrollment = Enrollment.IdEnrollment
                                    join Studies on Enrollment.IdStudy = Studies.IdStudy
                                    where Student.IndexNumber = @id;";
                com.Parameters.AddWithValue("id", id);

                con.Open();
                var r = com.ExecuteReader();
                while (r.Read())
                {
                    var enrollment = new Enrollment();
                    enrollment.IdEnrollment = r.GetInt32(r.GetOrdinal("IdEnrollment"));
                    enrollment.Semester = r.GetInt32(r.GetOrdinal("Semester"));
                    enrollment.StartDate = r.GetDateTime(r.GetOrdinal("StartDate"));

                    var study = new Study();
                    study.IdStudy = r.GetInt32(r.GetOrdinal("IdStudy"));
                    study.Name = r.GetString(r.GetOrdinal("Name"));

                    enrollment.Study = study;

                    enrollments.Add(enrollment);
                }
            }
            return Ok(enrollments);
        }
    }
}