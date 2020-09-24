using GalileuszSchool.Areas.Admin.Controllers;
using GalileuszSchool.Models.ModelsForAdminArea;
using GalileuszSchool.Repository;
using Microsoft.AspNetCore.Mvc;
using MockQueryable.Moq;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace GalileuszSchool.Tests.UnitTests.Areas.Admin.Controllers
{
    public class TeachersControllerUnitTests
    {
        private readonly TeachersController _controller;
        private readonly Mock<IRepository<Teacher>> _repoMock = new Mock<IRepository<Teacher>>();
        // Arrange

        // Act

        // Assert
        public TeachersControllerUnitTests()
        {
            _controller = new TeachersController(_repoMock.Object);
        }
        [Fact]
        public async Task Index_ShouldReturnView()
        {
            //Arrange
            
            //_repoMock.Setup(x => x.GetAll()).Returns(list.AsQueryable());
            //Act
            var result = await _controller.Index();
            //Assert
            Assert.IsType<ViewResult>(result);
            

        }

        // CREATE TEACHER ====================
        [Fact]
        public async Task Create_ShouldReturnJsonAnonymousObject_WhenInvalidObjectPassed()
        {
            // Arrange
            var teacher = new Teacher()
            {
                LastName = "Brzeczyszczykiewicz",
                Email = "elo@gmail.com"
            };
            _controller.ModelState.AddModelError("FirstName", "Required");

            // Act
            dynamic response = await _controller.Create(teacher);

            var valueFromAnnonymous = response.Value.GetType().GetProperty("text").GetValue(response.Value, null);

            // Assert
            Assert.IsType<JsonResult>(response);
            //Assert.Equal("{ text = Invalid Techer model! }", s.Value.ToString());
            Assert.NotNull(response);
            Assert.Equal("Invalid Techer model!", valueFromAnnonymous);

        }
        [Fact]
        public async Task Create_ShouldReturnOk_WhenValidObjectPassed()
        {
            // Arrange
            var teacher = GetTeacher();
            // Act
            var response = await _controller.Create(teacher);

            // Assert
            Assert.NotNull(response);
            Assert.IsType<OkResult>(response);
        }
        [Fact]
        public async Task Create_ShouldReturnAnonymousObject_WhenTeacherAlreadyExists()
        {
            // Arrange
            var teacher = GetTeacher();
            _repoMock.Setup(x => x.GetBySlug(teacher.Slug)).ReturnsAsync(teacher);
            //Act          
            dynamic result = await _controller.Create(teacher);
            var valueFromAnnonymous = result.Value.GetType().GetProperty("text").GetValue(result.Value, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Teacher already exists!", valueFromAnnonymous);

        }

        // CREATE TEACHER END====================

        // EDIT TEACHER ====================
        [Fact]
        public async Task Edit_ShouldReturnAnonymousObject_WhenInvalidObjectPassed()
        {
            // Arrange
            var teacher = new Teacher()
            {
                LastName = "Brzeczyszczykiewicz",
                Email = "elo@gmail.com"
            };
            _controller.ModelState.AddModelError("FirstName", "Required");

            // Act
            dynamic response = await _controller.Edit(teacher);

            var valueFromAnnonymous = response.Value.GetType().GetProperty("text").GetValue(response.Value, null);

            // Assert
            Assert.IsType<JsonResult>(response);
            Assert.NotNull(response);
            Assert.Equal("Invalid Techer model!", valueFromAnnonymous);
        }
        [Fact]
        public async Task Edit_ShouldReturnOk_WhenValidObjectPassed()
        {
            // Arrange
            var teacher = GetTeacher();
            // Act
            var response = await _controller.Edit(teacher);

            // Assert
            Assert.NotNull(response);
            Assert.IsType<OkResult>(response);
        }
        [Fact]
        public async Task Edit_ShouldReturnAnonymousObject_WhenTeacherAlreadyExists()
        {
            // Arrange
            var teacher = GetTeacher();
            _repoMock.Setup(x => x.GetModelByWhereAndFirstConditions(x => x.Id != teacher.Id, x => x.Slug == teacher.Slug)).ReturnsAsync(teacher);
            //Act          
            dynamic result = await _controller.Edit(teacher);
            var valueFromAnnonymous = result.Value.GetType().GetProperty("text").GetValue(result.Value, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Teacher with that name already exists!", valueFromAnnonymous);
        }

        // EDIT TEACHER END====================

        // DELETE TEACHER ====================
        [Fact]
        public async Task Delete_ShouldReturnAnonymousObject_WhenNonExistingIdPassed()
        {
            // Arrange
            var teacherId = 0;

            // Act
            _repoMock.Setup(x => x.GetById(teacherId)).ReturnsAsync((Teacher)null);
            dynamic result = await _controller.Delete(teacherId);
            var valueFromAnnonymous = result.Value.GetType().GetProperty("text").GetValue(result.Value, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Teacher does not exists!", valueFromAnnonymous);
        }
        [Fact]
        public async Task Delete_ShouldReturnOk_WhenExistingIdPassed()
        {
            // Arrange
            var teacher = GetTeacher();
            // Act
            _repoMock.Setup(x => x.GetById(teacher.Id)).ReturnsAsync(teacher);
            var response = await _controller.Delete(teacher.Id);

            // Assert
            Assert.NotNull(response);
            Assert.IsType<OkResult>(response);
        }
        [Fact]
        public async Task Delete_ShouldRemoveOneTeacher_WhenExistingIdPassed()
        {
            //Arrange
            var id = 1;
            _repoMock.Setup(repo => repo.GetById(id)).ReturnsAsync(new Teacher() { });
            _repoMock.Setup(repo => repo.Delete(It.IsAny<int>())).Returns(Task.CompletedTask);

            //Act
            await _controller.Delete(id);
            //Assert
            _repoMock.Verify(repo => repo.Delete(It.IsAny<int>()), Times.Once);

        }
        // DELETE TEACHER END====================

        // FindTeacher =================================
        [Fact]
        //[Fact]
        public async Task FindTeacher_ShouldReturnAnonymousObject_WhenUknownIdPassed()
        {
            // Arrange
            var teacherId = 0;

            // Act
            _repoMock.Setup(x => x.GetById(teacherId)).ReturnsAsync((Teacher)null);
            var result = await _controller.FindTeacher(teacherId);
            var valueFromAnnonymous = result.Value.GetType().GetProperty("text").GetValue(result.Value, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Server error!", valueFromAnnonymous);
        }

        [Fact]
        public async Task FindTeacher_ShouldReturnJson_WhenCalled()
        {
            //act 
            var result = await _controller.FindTeacher(1);

            //assert
            Assert.IsType<JsonResult>(result);
        }
        [Fact]
        public async Task FindTeacher_ShouldReturnTeacher_WhenExists()
        {
            //Arrange
            int teacherId = 1;
            var teacherFromDb = GetTeacher();
            _repoMock.Setup(x => x.GetById(teacherId)).ReturnsAsync(teacherFromDb);
            //Act          
            dynamic result = await _controller.FindTeacher(teacherId);

            //Assert
            Assert.Equal(teacherFromDb.Id, result.Value.Id);
            Assert.Equal(teacherFromDb.FirstName, result.Value.FirstName);
            Assert.Equal(teacherFromDb.LastName, result.Value.LastName);

            Assert.NotNull(result);
        }
        // FindTeacher END =================================

        // GetTeachers ================================
        //TODO: how to mock list<Teacher> as null
        [Fact]
        public async Task GetTeachers_ShouldReturnAnonymousObject_WhenQueryFromDbIsNullOrListLenIs0()
        {
            //Arrange
            List<Teacher> teachers = new List<Teacher>();
            var mock = teachers.AsQueryable().BuildMock();
            _repoMock.Setup(x => x.GetAll()).Returns(mock.Object);
            ////Act

            var result = await _controller.GetTeachers();
            var valueFromAnnonymous = result.Value.GetType().GetProperty("text").GetValue(result.Value, null);

            ////Assert

            Assert.NotNull(result);
            Assert.Equal("No teachers found!", valueFromAnnonymous);


        }
        [Fact]
        public async Task GetTeachers_ShouldReturnJson_WhenCalled()
        {
            //Arrange
            var teachers = GetTeachersList();
            var mock = teachers.AsQueryable().BuildMock();
            _repoMock.Setup(x => x.GetAll()).Returns(mock.Object);

            //Act
            var result = await _controller.GetTeachers();

            //Assert
            Assert.IsType<JsonResult>(result);

        }
        [Fact]
        public async Task GetTeachers_ShouldReturnAllTeachers_WhenExist()
        {
            //Arrange
            var teachers = GetTeachersList();
            //var query = _repoMock.Setup(x => x.GetAll()).Returns(teachers.AsQueryable());
            var mock = teachers.AsQueryable().BuildMock();
            _repoMock.Setup(x => x.GetAll()).Returns(mock.Object);

            ////Act
            var result = await _controller.GetTeachers();
            //var serializedTeachers = JsonConvert.SerializeObject(result.Value);
            //List<Teacher> newTeachers = JsonConvert.DeserializeObject<List<Teacher>>(serializedTeachers);

            ////Assert
            Assert.NotNull(result);
            var teachersJson = Assert.IsType<List<Teacher>>(result.Value);
            Assert.Equal(3,teachersJson.Count);

        }
        // GetTeachers END ================================

        private Teacher GetTeacher()
        {
            return new Teacher
            {
                Id = 1,
                FirstName = "Franciszek",
                LastName = "Zawadzki",
                Slug = "franciszekzawadzki",
                Email = "czesc@gmail.com"
            };
        }

        private List<Teacher> GetTeachersList()
        {
            return new List<Teacher> {
                new Teacher
                {
                    Id = 1,
                    FirstName = "Franciszek",
                    LastName = "Zawadzki"
                },
                new Teacher
                {
                    Id = 2,
                    FirstName = "John",
                    LastName = "Romero"
                },
                new Teacher
                {
                    Id = 3,
                    FirstName = "Lenny",
                    LastName = "Kravitz"
                }
            };
        }
        // Support Function to evaluate JsonResult
        private T GetVal<T>(JsonResult jsonResult, string propertyName)
        {
            var property = jsonResult.Value.GetType().GetProperties()
                    .Where(p => string.Compare(p.Name, propertyName) == 0)
                    .FirstOrDefault();
            if (null == property)
                throw new ArgumentException("propertyName not found", "propertyName");
            return (T)property.GetValue(jsonResult.Value, null);
        }
    }
}
