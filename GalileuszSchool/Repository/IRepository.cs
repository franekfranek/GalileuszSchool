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
        Task<TModel> GetModelByCondition(Expression<Func<TModel, bool>> expression,
                                                Expression<Func<TModel, bool>> secondExpression);
        Task<bool> IsInDB(int id);
    }

}
