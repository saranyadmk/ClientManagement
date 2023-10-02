using ClientManagement.Database;

namespace ClientManagement.Filters
{
    public class PaginationFilter
    {
        public IEnumerable<Client> Clients { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
