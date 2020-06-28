using System;
using System.Threading.Tasks;
using TangentWatson.Entities;

namespace TangentWatson.Datastore.Interfaces
{
    public interface ICommentDataStoreService
    {
        Task<RatedComment> CreateAndStoreMessage(string message);
        Task<RatedComment> GetMessage(Guid id, bool forUpdate = false);
        Task UpdateWithResults(RatedComment comment, WatsonResponse watsonResponse);
        Task UpdateAsFailure(RatedComment comment);
    }
}