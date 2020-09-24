using GalileuszSchool.Infrastructure;
using GalileuszSchool.Models.ModelsForAdminArea;
using GalileuszSchool.Models.ModelsForNormalUsers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GalileuszSchool.Repository
{
    public class Repository<TModel> : IRepository<TModel>
        where TModel : class, IEntity
    {
        private readonly GalileuszSchoolContext _context;

        public Repository(GalileuszSchoolContext context)
        {
            _context = context;
        }

        public async Task Create(TModel model)
        {
            await _context.Set<TModel>().AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var entity = await GetById(id);

            _context.Set<TModel>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<TModel> GetModelByWhereAndFirstConditions(Expression<Func<TModel, bool>> expression,
                                                        Expression<Func<TModel, bool>> secondExpression)
        {
            return await _context.Set<TModel>().Where(expression).AsNoTracking()
                .FirstOrDefaultAsync(secondExpression);
        }

        public IQueryable<TModel> GetAll()
        {
            return _context.Set<TModel>().AsNoTracking();
        }
        

        public async Task<TModel> GetById(int id)
        {
            return await _context.Set<TModel>().AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<TModel> GetBySlug(string slug)
        {
            return await _context.Set<TModel>().AsNoTracking()
                .FirstOrDefaultAsync(x => x.Slug == slug);
        }

        public async Task Update(TModel model)
        {
            _context.Set<TModel>().Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsInDB(int id)
        {
            return await _context.Set<TModel>().AnyAsync(e => e.Id == id);
        }
        public async Task<TModel> GetModelByFirstCondition(Expression<Func<TModel, bool>> expression)
        {
            return await _context.Set<TModel>().FirstOrDefaultAsync(expression);
        }

        //TODO: generic needed for these, bad practice
        public IOrderedQueryable<Teacher> GetAllTeachers()
        {
            return _context.Teachers.OrderBy(x => x.Id);
        }

        public IOrderedQueryable<Student> GetAllStudents()
        {
            return _context.Students.OrderBy(x => x.Id);
        }

        
    }
}
