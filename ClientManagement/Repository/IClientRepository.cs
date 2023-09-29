using ClientManagement.Database;
using ClientManagement.Filters;
using Microsoft.AspNetCore.JsonPatch;

namespace ClientManagement.Repository
{
    public interface IClientRepository
    {
        Task<List<Client>> GetAllClientsAsync();
        Task<Client> GetClientByIdAsync(int id);
        Task<Client> AddNewClientAsync(Client client);
        Task<Client> UpdateClientAsync(int id, Client client);
        Task UpdateClientUSingPatchAsync(int id, JsonPatchDocument jsonPatch);
        Task DeleteClientAsync(int id);
        Task<PaginationFilter> GetClients(string term, string sort, int page, int limit);
    }
}