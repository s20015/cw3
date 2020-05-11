using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wyklad3.Models;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using Wyklad3.Services;
using Microsoft.AspNetCore.Http;

namespace Wyklad3.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private IStudentBbService _service;

        public EnrollmentsController(IStudentBbService service)
        {
            _service = service;
        }

        [HttpPost]
        public IActionResult CreateEnrollment([FromBody]EnrollmentRequest request)
        {
            // example request (notice differant date format)
            //{
            //  "IndexNumber": "s12345",
	        //  "FirstName": "Andrzej",
	        //  "LastName": "Malewski",
	        //  "BirthDate": "1995-01-01",
	        //  "Studies": "IT"
            //}

            try
            {
                _service.EnrollStudent(request);
            } catch (ValidationError exc)
            {
                return BadRequest(exc.Message);
            } catch (InternalError exc)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exc.Message);
            }

            var resp = new EnrollmentResponse(); // should be returned from service
            resp.LastName = request.LastName;
            resp.Semester = 1;
            resp.StartDate = DateTime.Today;
            return Ok(resp);
        }

        [HttpPost("promotions")]
        public IActionResult Promote([FromBody] Promotion request)
        {
            try
            {
                _service.PromoteStudents(request.Semester, request.Studies);
            } catch (NotFoundError exc)
            {
                return NotFound(exc.Message);
            } catch (InternalError exc)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exc.Message);
            }

            request.Semester += 1; // should be returned as service (no business logic in "view" layer ideally 
            return StatusCode(StatusCodes.Status201Created, request);
        }
    }
}