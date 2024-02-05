using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories.Interfaces
{
    public interface IWalkRepository
    {
        Task<Walk> CreateAsync(Walk walk);
    }
}
