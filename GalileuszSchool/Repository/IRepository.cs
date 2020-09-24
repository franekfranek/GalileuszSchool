using GalileuszSchool.Infrastructure;
using GalileuszSchool.Models.ModelsForAdminArea;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GalileuszSchool.Repository
{
    public interface IRepository<TModel> where TModel : class
    {
        IQueryable<TModel> GetAll();
        Task<TModel> GetById(int id);
        Task Create(TModel model);
        Task Update(TModel model);
        Task Delete(int id);
        Task<TModel> GetBySlug(string slug);
        Task<TModel> GetModelByWhereAndFirstConditions(Expression<Func<TModel, bool>> expression,
                                                Expression<Func<TModel, bool>> secondExpression);
        Task<TModel> GetModelByFirstCondition(Expression<Func<TModel, bool>> expression);
        Task<bool> IsInDB(int id);

        //TODO: refactor those 2 bad boyz
        IOrderedQueryable<Teacher> GetAllTeachers();
        IOrderedQueryable<Student> GetAllStudents();
    }

}
