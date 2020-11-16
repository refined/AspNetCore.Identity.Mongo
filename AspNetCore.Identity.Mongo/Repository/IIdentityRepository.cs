using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Mongo.Repository
{
    public interface IIdentityRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetAsync(string id, CancellationToken cancellationToken);

        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> searchExpression, CancellationToken cancellationToken);

        Task SaveAsync(TEntity user, CancellationToken cancellationToken);

        Task DeleteAsync(string userId, CancellationToken cancellationToken);

        Task<TEntity> UpdateAsync<TField>(Expression<Func<TEntity, bool>> searchExpression, Expression<Func<TEntity, TField>> fieldExpression, 
            TField fieldValue, CancellationToken cancellationToken = default);
    }
}