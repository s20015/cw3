using System.Collections.Generic;
using Wyklad3.DTOs;
using Wyklad3.Models;

namespace Wyklad3.Services
{
    public class MockDbService : IDbService
    {
        private static IEnumerable<Student> _students = new List<Student>
        {
            new Student{IdStudent=1, FirstName="Jan", LastName="Kowalski", IndexNumber="s1234"},
            new Student{IdStudent=2, FirstName="Anna", LastName="Malewski", IndexNumber="s2342"},
            new Student{IdStudent=3, FirstName="Krzysztof", LastName="Andrzejewicz", IndexNumber="s5432"}
        };

        public bool AreCredentialsValid(LoginRequest request)
        {
            throw new System.NotImplementedException();
        }

        public bool AreCredentialsValid(LoginRequest request, byte[] salt)
        {
            throw new System.NotImplementedException();
        }

        public void CreateStudent(Student student)
        {
            var existingStudent = ((List<Student>)_students).Find(s => s.IdStudent == student.IdStudent);
            if (existingStudent != null)
            {
                throw new StudentIdAlseadyExistsException();
            }

            ((List<Student>)_students).Add(student);
        }

        public Student GetStudentByRefreshToken(string refreshToken)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Student> GetStudents()
        {
            return _students;
        }

        public void SetNewRefreshToken(string login, string refreshToken)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateStudent(Student student)
        {
            var idx = ((List<Student>)_students).FindIndex(s => s.IdStudent == student.IdStudent);
            if (idx == -1)
            {
                throw new StudentNotFoundException();
            }

            ((List<Student>)_students)[idx] = student;
        }

        void IDbService.DeleteStudent(int id)
        {
            var num = ((List<Student>)_students).RemoveAll(s => s.IdStudent == id);

            if (num == 0) {
                throw new StudentNotFoundException();
            }
        }

        Student IDbService.GetStudent(int id)
        {
            var student = ((List<Student>)_students).Find(s => s.IdStudent == id);
            if (student == null)
            {
                throw new StudentNotFoundException();
            }

            return student;
        }
    }
}
