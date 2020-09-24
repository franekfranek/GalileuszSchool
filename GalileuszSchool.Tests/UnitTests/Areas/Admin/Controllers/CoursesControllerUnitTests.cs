using GalileuszSchool.Areas.Admin.Controllers;
using GalileuszSchool.Infrastructure;
using GalileuszSchool.Models.ModelsForAdminArea;
using GalileuszSchool.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class CoursesControllerUnitTests
    {
        private readonly CoursesController _controller;
        private readonly Mock<IRepository<Course>> _repoMock = new Mock<IRepository<Course>>();
        
        public CoursesControllerUnitTests()
        {
            //var mockSet = new Mock<DbSet<Course>>();
            //_contextMock.Setup(m => m.Courses).Returns(mockSet.Object);
            
            _controller = new CoursesController(_repoMock.Object);
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

        // CREATE COURSE ====================
        [Fact]
        public async Task Create_ShouldReturnJsonAnonymousObject_WhenInvalidObjectPassed()
        {
            // Arrange
            var course = new Course()
            {
                Level = "Advanced",
                Price = 10
            };
            _controller.ModelState.AddModelError("Name", "Required");

            // Act
            dynamic response = await _controller.Create(course);
            var valueFromAnnonymous = response.Value.GetType().GetProperty("text").GetValue(response.Value, null);

            // Assert
            Assert.IsType<JsonResult>(response);
            Assert.NotNull(response);
            Assert.Equal("Invalid Course model!", valueFromAnnonymous);

        }
        [Fact]
        public async Task Create_ShouldReturnOk_WhenValidObjectPassed()
        {
            // Arrange
            var course = GetCourse();
            // Act
            var response = await _controller.Create(course);

            // Assert
            Assert.NotNull(response);
            Assert.IsType<OkResult>(response);
        }
        [Fact]
        public async Task Create_ShouldReturnAnonymousObject_WhenTeacherAlreadyExists()
        {
            // Arrange
            var course = GetCourse();
            _repoMock.Setup(x => x.GetBySlug(course.Slug)).ReturnsAsync(course);
            //Act          
            dynamic result = await _controller.Create(course);
            var valueFromAnnonymous = result.Value.GetType().GetProperty("text").GetValue(result.Value, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Course already exists!", valueFromAnnonymous);

        }

        // CREATE COURSE END====================

        // EDIT COURSE ====================
        [Fact]
        public async Task Edit_ShouldReturnAnonymousObject_WhenInvalidObjectPassed()
        {
            // Arrange
            var course = new Course()
            {
                Level = "Advanced",
                Price = 30
            };
            _controller.ModelState.AddModelError("Name", "Required");

            // Act
            var response = await _controller.Edit(course);
            dynamic responseFromTask = response;
            var valueFromAnnonymous = responseFromTask.Value.GetType().GetProperty("text").GetValue(responseFromTask.Value, null);

            // Assert
            Assert.IsType<JsonResult>(response);
            Assert.NotNull(response);
            Assert.Equal("Invalid Course model!", valueFromAnnonymous);
        }
        [Fact]
        public async Task Edit_ShouldReturnOk_WhenValidObjectPassed()
        {
            // Arrange
            var course = GetCourse();
            // Act
            var response = await _controller.Edit(course);

            // Assert
            Assert.NotNull(response);
            Assert.IsType<OkResult>(response);
        }
        [Fact]
        public async Task Edit_ShouldReturnAnonymousObject_WhenTeacherAlreadyExists()
        {
            // Arrange
            var course = GetCourse();
            _repoMock.Setup(x => x.GetModelByWhereAndFirstConditions(x => x.Id != course.Id, x => x.Slug == course.Slug)).ReturnsAsync(course);
            //Act          
            dynamic result = await _controller.Edit(course);
            var valueFromAnnonymous = result.Value.GetType().GetProperty("text").GetValue(result.Value, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Course with that name already exists!", valueFromAnnonymous);
        }

        // EDIT COURSE END====================

        // DELETE COURSE ====================
        [Fact]
        public async Task Delete_ShouldReturnAnonymousObject_WhenNonExistingIdPassed()
        {
            // Arrange
            var courseId = 0;

            // Act
            _repoMock.Setup(x => x.GetById(courseId)).ReturnsAsync((Course)null);
            dynamic result = await _controller.Delete(courseId);
            var valueFromAnnonymous = result.Value.GetType().GetProperty("text").GetValue(result.Value, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Course does not exists!", valueFromAnnonymous);
        }
        [Fact]
        public async Task Delete_ShouldReturnOk_WhenExistingIdPassed()
        {
            // Arrange
            var course = GetCourse();
            // Act
            _repoMock.Setup(x => x.GetById(course.Id)).ReturnsAsync(course);
            var response = await _controller.Delete(course.Id);

            // Assert
            Assert.NotNull(response);
            Assert.IsType<OkResult>(response);
        }
        [Fact]
        public async Task Delete_ShouldRemoveOneCourse_WhenExistingIdPassed()
        {
            //Arrange
            var id = 1;
            _repoMock.Setup(repo => repo.GetById(id)).ReturnsAsync(new Course() { });
            _repoMock.Setup(repo => repo.Delete(It.IsAny<int>())).Returns(Task.CompletedTask);

            //Act
            await _controller.Delete(id);
            //Assert
            _repoMock.Verify(repo => repo.Delete(It.IsAny<int>()), Times.Once);

        }
        // DELETE COURSE END====================

        // FindCourse =================================
        [Fact]
        //[Fact]
        public async Task FindCourse_ShouldReturnAnonymousObject_WhenUknownIdPassed()
        {
            // Arrange
            var courseId = 0;

            // Act
            _repoMock.Setup(x => x.GetById(courseId)).ReturnsAsync((Course)null);
            dynamic result = await _controller.FindCourse(courseId);
            var valueFromAnnonymous = result.Value.GetType().GetProperty("text").GetValue(result.Value, null);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("Server error!", valueFromAnnonymous);
        }

        [Fact]
        public async Task FindCourse_ShouldReturnJson_WhenCalled()
        {
            //act
            var result = await _controller.FindCourse(1);

            //assert
            Assert.IsType<JsonResult>(result);
        }
        [Fact]
        public async Task FindCourse_ShouldReturnTeacher_WhenExists()
        {
            //Arrange
            int courseId = 1;
            var course = GetCourse();
            _repoMock.Setup(x => x.GetById(courseId)).ReturnsAsync(course);
            //Act          
            dynamic result = await _controller.FindCourse(courseId);

            //Assert
            Assert.Equal(course.Id, result.Value.Id);
            Assert.Equal(course.Name, result.Value.Name);
            Assert.Equal(course.Level, result.Value.Level);

            Assert.NotNull(result);
        }
        // FindCourse END =================================

        // GetCourses ================================
        [Fact]
        public async Task GetCourses_ShouldReturnAnonymousObject_WhenQueryFromDbIsNullOrListLenIs0()
        {
            //Arrange
            List<Course> teachers = new List<Course>();
            var mock = teachers.AsQueryable().BuildMock();
            _repoMock.Setup(x => x.GetAll()).Returns(mock.Object);
            //Act

            var result = await _controller.GetCourses();
            var valueFromAnnonymous = result.Value.GetType().GetProperty("text").GetValue(result.Value, null);

            ////Assert

            Assert.NotNull(result);
            Assert.Equal("No courses found!", valueFromAnnonymous);


        }
        [Fact]
        public async Task GetCourses_ShouldReturnJson_WhenCalled()
        {
            //Arrange
            var teachers = GetCoursesList();
            var mock = teachers.AsQueryable().BuildMock();
            _repoMock.Setup(x => x.GetAll()).Returns(mock.Object);

            //Act
            var result = await _controller.GetCourses();

            //Assert
            Assert.IsType<JsonResult>(result);

        }
        [Fact]
        public async Task GetCourses_ShouldReturnAllTeachers_WhenExist()
        {
            //Arrange
            var teachers = GetCoursesList();
            var mock = teachers.AsQueryable().BuildMock();
            _repoMock.Setup(x => x.GetAll()).Returns(mock.Object);

            //Act
            var result = await _controller.GetCourses();

            ////Assert
            Assert.NotNull(result);
            var teachersJson = Assert.IsType<List<Course>>(result.Value);
            Assert.Equal(3, teachersJson.Count);

        }
        // GetCourses END ================================

        private Course GetCourse()
        {
            return new Course()
            {   
                Id = 1,
                Name = "English",
                Level = "Advanced",
                Price = 50,
                Slug = "english"
            };
        }
        private List<Course> GetCoursesList()
        {
            return new List<Course> {
                new Course()
                {
                    Id = 1,
                    Name = "English",
                    Level = "Advanced"
                },
                new Course()
                {
                    Id = 2,
                    Name = "Spanish",
                    Level = "Intermediate"
                },
                new Course()
                {
                    Id = 3,
                    Name = "French",
                    Level = "Beginner"
                },
                
            };
        }
    }
}
