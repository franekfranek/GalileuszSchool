using GalileuszSchool.Areas.Admin.Controllers;
using GalileuszSchool.Models.ModelsForAdminArea;
using GalileuszSchool.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GalileuszSchool.Tests.UnitTests.Areas.Admin.Controllers
{
    public class StudentsControllerUnitTests
    {
        private readonly StudentsController _controller;
        private readonly Mock<IRepository<Student>> _repoMock = new Mock<IRepository<Student>>();
        private readonly Mock<IWebHostEnvironment> _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();

        public StudentsControllerUnitTests()
        {
            //var mockSet = new Mock<DbSet<Course>>();
            //_contextMock.Setup(m => m.Courses).Returns(mockSet.Object);

            _controller = new StudentsController(_webHostEnvironmentMock.Object, _repoMock.Object);
        }
        [Fact]
        public async Task Index_ShouldReturnView()
        {
            //Arrange

            //Act
            var result = await _controller.Index();
            //Assert
            Assert.IsType<ViewResult>(result);


        }

        // CREATE STUDENT ====================
        [Fact]
        public async Task Create_ShouldReturnJsonAnonymousObject_WhenInvalidObjectPassed()
        {
            // Arrange
            var course = new Student()
            {
                LastName = "Advanced",
                Email = "MrAdvanced@gmail.com"
            };
            _controller.ModelState.AddModelError("FirstName", "Required");

            // Act
            dynamic response = await _controller.Create(course);
            var valueFromAnnonymous = response.Value.GetType().GetProperty("text").GetValue(response.Value, null);

            // Assert
            Assert.IsType<JsonResult>(response);
            Assert.NotNull(response);
            Assert.Equal("Invalid Student model!", valueFromAnnonymous);

        }
        [Fact]
        public async Task Create_ShouldReturnOk_WhenValidObjectPassed()
        {
            // Arrange
            var course = GetStudent();
            // Act
            var response = await _controller.Create(course);

            // Assert
            Assert.NotNull(response);
            Assert.IsType<OkResult>(response);
        }
        [Fact]
        public async Task Create_ShouldReturnAnonymousObject_WhenStudentAlreadyExists()
        {
            // Arrange
            var student = GetStudent();
            _repoMock.Setup(x => x.GetBySlug(student.Slug)).ReturnsAsync(student);
            //Act          
            dynamic result = await _controller.Create(student);
            var valueFromAnnonymous = result.Value.GetType().GetProperty("text").GetValue(result.Value, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Student already exists!", valueFromAnnonymous);

        }

        // CREATE STUDENT END====================

        // EDIT STUDENT ====================
        [Fact]
        public async Task Edit_ShouldReturnAnonymousObject_WhenInvalidObjectPassed()
        {
            // Arrange
            var course = new Student()
            {
                LastName = "Advanced",
                Email = "MrAdvanced@gmail.com"
            };
            _controller.ModelState.AddModelError("FirstName", "Required");

            // Act
            var response = await _controller.Edit(course);
            dynamic responseFromTask = response;
            var valueFromAnnonymous = responseFromTask.Value.GetType().GetProperty("text").GetValue(responseFromTask.Value, null);

            // Assert
            Assert.IsType<JsonResult>(response);
            Assert.NotNull(response);
            Assert.Equal("Invalid Student model!", valueFromAnnonymous);
        }
        [Fact]
        public async Task Edit_ShouldReturnOk_WhenValidObjectPassed()
        {
            // Arrange
            var course = GetStudent();
            // Act
            var response = await _controller.Edit(course);

            // Assert
            Assert.NotNull(response);
            Assert.IsType<OkResult>(response);
        }
        [Fact]
        public async Task Edit_ShouldReturnAnonymousObject_WhenStudentAlreadyExists()
        {
            // Arrange
            var course = GetStudent();
            _repoMock.Setup(x => x.GetModelByWhereAndFirstConditions(x => x.Id != course.Id, x => x.Slug == course.Slug)).ReturnsAsync(course);
            //Act          
            dynamic result = await _controller.Edit(course);
            var valueFromAnnonymous = result.Value.GetType().GetProperty("text").GetValue(result.Value, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Student with that data already exists!", valueFromAnnonymous);
        }

        // EDIT STUDENT END====================

        // DELETE STUDENT ====================
        [Fact]
        public async Task Delete_ShouldReturnAnonymousObject_WhenNonExistingIdPassed()
        {
            // Arrange
            var studentId = 0;

            // Act
            _repoMock.Setup(x => x.GetById(studentId)).ReturnsAsync((Student)null);
            dynamic result = await _controller.Delete(studentId);
            var valueFromAnnonymous = result.Value.GetType().GetProperty("text").GetValue(result.Value, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Student does not exists!", valueFromAnnonymous);
        }
        [Fact]
        public async Task Delete_ShouldReturnOk_WhenExistingIdPassedAndStudentHadNoProfilePic()
        {
            // Arrange
            var student = GetStudent();
            student.Image = "noimage.jpg";
            // Act
            _repoMock.Setup(x => x.GetById(student.Id)).ReturnsAsync(student);
            var response = await _controller.Delete(student.Id);

            // Assert
            Assert.NotNull(response);
            Assert.IsType<OkResult>(response);
        }
        [Fact]
        public async Task Delete_ShouldRemoveOneCourse_WhenExistingIdPassedAndStudentHadNoProfilePic()
        {
            //Arrange
            var id = 1;
            _repoMock.Setup(repo => repo.GetById(id)).ReturnsAsync(new Student() { Image = "noimage.jpg" });
            _repoMock.Setup(repo => repo.Delete(It.IsAny<int>())).Returns(Task.CompletedTask);

            //Act
            await _controller.Delete(id);
            //Assert
            _repoMock.Verify(repo => repo.Delete(It.IsAny<int>()), Times.Once);

        }
        // DELETE STUDENT END====================

        // FindStudent =================================
        [Fact]
        //[Fact]
        public async Task FindStudent_ShouldReturnAnonymousObject_WhenUknownIdPassed()
        {
            // Arrange
            var studentId = 0;

            // Act
            _repoMock.Setup(x => x.GetById(studentId)).ReturnsAsync((Student)null);
            dynamic result = await _controller.FindStudent(studentId);
            var valueFromAnnonymous = result.Value.GetType().GetProperty("text").GetValue(result.Value, null);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("Server error!", valueFromAnnonymous);
        }

        [Fact]
        public async Task FindStudent_ShouldReturnJson_WhenCalled()
        {
            //act
            var result = await _controller.FindStudent(1);

            //assert
            Assert.IsType<JsonResult>(result);
        }
        [Fact]
        public async Task FindStudent_ShouldReturnStudent_WhenExists()
        {
            //Arrange
            int studentId = 1;
            var student = GetStudent();
            _repoMock.Setup(x => x.GetById(studentId)).ReturnsAsync(student);
            //Act          
            dynamic result = await _controller.FindStudent(studentId);

            //Assert
            Assert.Equal(student.Id, result.Value.Id);
            Assert.Equal(student.FirstName, result.Value.FirstName);
            Assert.Equal(student.LastName, result.Value.LastName);
            Assert.Equal(student.PhoneNumber, result.Value.PhoneNumber);

            Assert.NotNull(result);
        }
        // FindStudent END =================================

        // GetStudents ================================
        [Fact]
        public async Task GetStudents_ShouldReturnAnonymousObject_WhenQueryFromDbIsNullOrListLenIs0()
        {
            //Arrange
            List<Student> teachers = new List<Student>();
            var mock = teachers.AsQueryable().BuildMock();
            _repoMock.Setup(x => x.GetAll()).Returns(mock.Object);
            //Act

            var result = await _controller.GetStudents();
            var valueFromAnnonymous = result.Value.GetType().GetProperty("text").GetValue(result.Value, null);

            ////Assert

            Assert.NotNull(result);
            Assert.Equal("No students found!", valueFromAnnonymous);


        }
        [Fact]
        public async Task GetStudents_ShouldReturnJson_WhenCalled()
        {
            //Arrange
            var teachers = GetStudentsList();
            var mock = teachers.AsQueryable().BuildMock();
            _repoMock.Setup(x => x.GetAll()).Returns(mock.Object);

            //Act
            var result = await _controller.GetStudents();

            //Assert
            Assert.IsType<JsonResult>(result);

        }
        [Fact]
        public async Task GetStudents_ShouldReturnAllTeachers_WhenExist()
        {
            //Arrange
            var students = GetStudentsList();
            var mock = students.AsQueryable().BuildMock();
            _repoMock.Setup(x => x.GetAll()).Returns(mock.Object);

            //Act
            var result = await _controller.GetStudents();

            ////Assert
            Assert.NotNull(result);
            var teachersJson = Assert.IsType<List<Student>>(result.Value);
            Assert.Equal(3, teachersJson.Count);

        }
        // GetStudents END ================================


        private Student GetStudent()
        {
            return new Student()
            {
                Id = 1,
                FirstName = "Michael",
                LastName = "Scott",
                Slug = "michaelscott",
                PhoneNumber = "000-000-000"
            };
        }
        private List<Student> GetStudentsList()
        {
            return new List<Student> {
                new Student()
                {
                    Id = 1,
                    FirstName = "Franciszek",
                    LastName = "Zawadzki"
                },
                new Student()
                {
                    Id = 2,
                    FirstName = "John",
                    LastName = "Rambo"
                },
                new Student()
                {
                    Id = 3,
                    FirstName = "Son",
                    LastName = "Goku"
                },

            };
        }
    }
}
