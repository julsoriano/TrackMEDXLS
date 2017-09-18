using System.Collections.Generic;
//using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
//using TrackRE.DTOs;                   
using TrackMED.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace TrackMED.Services
{
    public interface IEntityService<T> where T: IEntity
    {
        Task<List<T>> GetEntitiesAsync(string id = null, CancellationToken cancelToken = default(CancellationToken));
        //Task<List<T>> GetEntitiesByIdAsync(string id = null, CancellationToken cancelToken = default(CancellationToken));

        Task<List<T>> GetEntitiesManyAsync(List<string> ids, CancellationToken cancelToken = default(CancellationToken));

        Task<List<T>> GetSelectedEntitiesAsync(string tableID, string id, CancellationToken cancelToken = default(CancellationToken));

        Task<T> GetEntityAsync(string id, CancellationToken cancelToken = default(CancellationToken));

        Task<T> GetEntityAsyncByDescription(string Description, CancellationToken cancelToken = default(CancellationToken));

        Task<T> GetEntityAsyncByFieldID(string fieldID, string id, string tableID, CancellationToken cancelToken = default(CancellationToken));

        Task<bool> DeleteEntityAsync(string id, CancellationToken cancelToken = default(CancellationToken));

        Task<HttpResponseMessage> EditEntityAsync(T Entity, CancellationToken cancelToken = default(CancellationToken));

        //public async Task<HttpResponseMessage> PostEntityAsync(Entity Entity,
        //Task<T> PostEntityAsync(IFormCollection Entity, CancellationToken cancelToken = default(CancellationToken));
        Task<T> PostEntityAsync(T Entity, CancellationToken cancelToken = default(CancellationToken));

        Task<T> PostEntitiesAsync(List<T> Entities, CancellationToken cancelToken = default(CancellationToken));
        //Task<List<T>> GetEntitiesAsyncByType(string proptype, CancellationToken cancelToken = default(CancellationToken));

        Task<T> VerifyEntityAsync(string id, CancellationToken cancelToken = default(CancellationToken));

        Task<bool> DropDatabaseAsync(CancellationToken cancelToken = default(CancellationToken));

    }
}