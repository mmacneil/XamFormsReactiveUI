

using System.Collections.Generic;
using System.Threading.Tasks;
using XamFormsReactiveUI.DataLayer.Abstract;
using XamFormsReactiveUI.Helpers;
using XamFormsReactiveUI.Models.Entities;

namespace XamFormsReactiveUI.DataLayer
{
    public class WordRepository : EntityBaseRepository<Word>, IWordRepository
    {
        private static readonly AsyncLock Locker = new AsyncLock();

        public WordRepository(IAppDatabase database)
            : base(database)
        {
        }

        public async Task<IList<Word>> GetWords()
        {
            using (await Locker.LockAsync())
            {
                return await Database.SqlLiteAsyncConnection.Table<Word>().Where(x => x.Id > 0).ToListAsync();
            }
        }
    }
}

 