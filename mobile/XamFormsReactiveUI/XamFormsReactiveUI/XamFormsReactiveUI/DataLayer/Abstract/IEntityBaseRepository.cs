
using System.Threading.Tasks;

namespace XamFormsReactiveUI.DataLayer.Abstract
{
    public interface IEntityBaseRepository<T> where T : class, IEntityBase, new()
    {
        Task<int> SaveItemAsync(T item);
        Task<T> GetItemAsync(int id);
    }
}
