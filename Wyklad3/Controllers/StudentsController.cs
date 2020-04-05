using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Wyklad3.Models;
using Wyklad3.Services;

namespace Wyklad3.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
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
    }
}