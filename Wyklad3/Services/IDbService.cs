using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wyklad3.Models;

namespace Wyklad3.Services
{
    public class StudentNotFoundException : Exception { }
    public class StudentIdAlseadyExistsException : Exception { }
    public interface IDbService
    {
        public void CreateStudent(Student student);
        public void UpdateStudent(Student student);
        public IEnumerable<Student> GetStudents();
        public Student GetStudent(int id);

        public void DeleteStudent(int id);
    }
}
