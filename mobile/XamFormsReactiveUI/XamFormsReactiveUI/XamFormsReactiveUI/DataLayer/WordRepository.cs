

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

        public async Task<IList<Word>> GetWords(int floor, int ceiling)
        {
            using (await Locker.LockAsync())
            {
                return await Database.SqlLiteAsyncConnection.QueryAsync<Word>(@"

                        SELECT * FROM words WHERE Id IN 

                        (SELECT Id FROM words where Id > ? AND Id < ? ORDER BY RANDOM() LIMIT 4)",floor,ceiling);
            }
        }
    }
}

 