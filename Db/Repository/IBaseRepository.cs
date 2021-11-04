using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ServiceBusDriver.Db.Entities;

namespace ServiceBusDriver.Db.Repository
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
#pragma warning disable 693
        Task<T> Get<T>(string id, CancellationToken cancellationToken) where T : BaseEntity;
        Task<List<T>> GetAll<T>(string query, CancellationToken cancellationToken) where T : BaseEntity;
        Task<List<T>> GetAll<T>(CancellationToken cancellationToken) where T : BaseEntity;
        Task<T> Add<T>(T record, CancellationToken cancellationToken) where T : BaseEntity;
        Task<bool> Update<T>(T record, CancellationToken cancellationToken) where T : BaseEntity;
        Task<bool> Delete<T>(T record, CancellationToken cancellationToken) where T : BaseEntity;

#pragma warning restore 693
    }
}