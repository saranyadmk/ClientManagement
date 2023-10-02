using ClientManagement.Caching;
using ClientManagement.Database;
using ClientManagement.Filters;
using LazyCache;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;

namespace ClientManagement.Repository
{
    public class ClientRepository : IClientRepository
    {
        private readonly ClientDbContext _context;
        private readonly ICacheProvider _cacheProvider;

        public ClientRepository(ClientDbContext context, ICacheProvider cacheProvider)
        {
            _context = context;
            _cacheProvider = cacheProvider;
        }

        public async Task<List<Client>> GetAllClientsAsync()
        {
            if (!_cacheProvider.TryGetValue(CacheKeys.Client, out List<Client> clients))
            {
                var clientss = await _context.Clients.ToListAsync();

                var cacheEntryOption = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(90),
                    SlidingExpiration = TimeSpan.FromSeconds(60),
                    Size = 1024
                };
                _cacheProvider.Set(CacheKeys.Client, clientss, cacheEntryOption);
            }
            return clients;
        }

        public async Task<Client> GetClientByIdAsync(int id)
        {
            if (_cacheProvider.TryGetValue(CacheKeys.Client, out Client client))
            {
                return client;
            }

            client = await _context.Clients.FirstOrDefaultAsync(client => client.ClientId == id);

            if (client != null)
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(90),
                    SlidingExpiration = TimeSpan.FromSeconds(60),
                    Size = 1024
                };
                _cacheProvider.Set(CacheKeys.Client, client, cacheEntryOptions);
            }

            return client;
        }

        public async Task<Client> AddNewClientAsync(Client client)
        {
            _context.Clients.Add(client);
            client.CreatedDate = new DateTime(2023, 9, 21);
            await _context.SaveChangesAsync();
            return client;
        }

        public async Task<Client> UpdateClientAsync(int id, Client client)
        {
            _context.Clients.Update(client);
            client.LicenceKey = Guid.NewGuid();
            client.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return client;
        }

        public async Task UpdateClientUSingPatchAsync(int id, JsonPatchDocument jsonPatch)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                jsonPatch.ApplyTo(client);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteClientAsync(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<PaginationFilter> GetClients(string term, string sort, int page, int limit)
        {
            IQueryable<Client> clients;
            if (!string.IsNullOrWhiteSpace(term))
                clients = _context.Clients;
            else
            {
                term = term.Trim().ToLower();
                clients = _context.Clients.Where(patient => patient.ClientName.ToLower().Contains(term) || patient.Description.ToLower().Contains(term));
            }

            PaginationFilter pagedClientData = new PaginationFilter();

            if (!string.IsNullOrWhiteSpace(sort))
            {
                var sortedFields = sort.Split(',');

                StringBuilder orderQueryBuilder = new StringBuilder();
                PropertyInfo[] propertyInfo = typeof(Client).GetProperties();

                foreach (var field in sortedFields)
                {
                    string sortOrder = "ascending";
                    var sortField = field.Trim();

                    if (sortField.StartsWith("-"))
                    {
                        sortField = sortField.TrimStart('-');
                        sortOrder = "descending";
                    }

                    var property = propertyInfo.FirstOrDefault(name => name.Name.Equals(sortField, StringComparison.OrdinalIgnoreCase));
                    if (property == null)
                        continue;
                    orderQueryBuilder.Append($"{property.Name.ToString()} {sortOrder},");
                }

                string orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');
                if (!string.IsNullOrWhiteSpace(orderQuery))
                {
                    clients = clients.OrderBy(orderQuery);
                }
                else
                {
                    clients = clients.OrderBy(order => order.ClientId);
                }

                var totalCount = await _context.Clients.CountAsync();
                var totalPages = (int)Math.Ceiling(totalCount / (double)limit);
                var pagedClients = await clients.Skip((page - 1) * limit).Take(limit).ToListAsync();

                pagedClientData = new PaginationFilter
                {
                    Clients = pagedClients,
                    TotalPages = totalPages,
                    TotalCount = totalCount,
                };
            }
            return pagedClientData;
        }
    }
}
