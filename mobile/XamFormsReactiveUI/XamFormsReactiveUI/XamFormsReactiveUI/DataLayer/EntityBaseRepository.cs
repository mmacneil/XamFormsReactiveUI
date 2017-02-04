 
using System.Threading.Tasks;
using XamFormsReactiveUI.DataLayer.Abstract;

namespace XamFormsReactiveUI.DataLayer
{
    public class EntityBaseRepository<T> : IEntityBaseRepository<T>
       where T : class, IEntityBase, new()
    {
        protected readonly IAppDatabase Database;

        #region Properties
        public EntityBaseRepository(IAppDatabase database)
        {
            Database = database;
        }
        #endregion

        public async Task<int> SaveItemAsync(T item)
        {
            return item.Id != 0 ? await Database.SqlLiteAsyncConnection.UpdateAsync(item) : await Database.SqlLiteAsyncConnection.InsertAsync(item);
        }

        public async Task<T> GetItemAsync(int id)
        {
            return await Database.SqlLiteAsyncConnection.Table<T>().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

    }
}
