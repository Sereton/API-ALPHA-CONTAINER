using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Repository
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected RepositoryContext RepositoryContext;

        public RepositoryBase(RepositoryContext repositoryContext)
        {
            RepositoryContext = repositoryContext;
        }

        public IQueryable<T> FindAll(bool trackChanges) =>
            !trackChanges ?
            RepositoryContext.Set<T>()
                .AsNoTracking() :
            RepositoryContext.Set<T>();

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression,
            bool trackChanges) =>
            !trackChanges ?
            RepositoryContext.Set<T>()

            .Where(expression)
             .AsNoTracking() :
            RepositoryContext.Set<T>()
                .Where(expression);

        public void Create(T entitity) => RepositoryContext.Set<T>().Add(entitity);
        public void Update(T entitity) => RepositoryContext.Set<T>().Update(entitity);

        public void Delete(T entitity) => RepositoryContext.Set<T>().Remove(entitity);

    }

}
