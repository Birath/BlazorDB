using System.Threading.Tasks;

namespace BlazorDB
{
    public interface IBlazorDbFactory
    {
        Task<IIndexedDbManager> GetDbManager(string dbName);

        Task<IIndexedDbManager> GetDbManager(DbStore dbStore);
    }
}