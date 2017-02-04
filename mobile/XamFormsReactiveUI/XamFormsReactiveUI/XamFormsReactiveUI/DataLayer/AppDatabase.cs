
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SQLite;
using XamFormsReactiveUI.Models.Entities;

namespace XamFormsReactiveUI.DataLayer
{
    public class AppDatabase : IAppDatabase
    {
        public SQLiteAsyncConnection SqlLiteAsyncConnection { get; }

        //private static object _locker = new object();

        public AppDatabase(ISQLite sqlLite)
        {
            SqlLiteAsyncConnection = sqlLite.GetConnection();
        }

        public async Task<bool> Exists()
        {
            return await SqlLiteAsyncConnection.ExecuteScalarAsync<int>("SELECT count(*) FROM sqlite_master WHERE type = 'table' AND name = 'Credentials'") > 0;
        }

      
        public Task<List<T>> GetItemsAsync<T>(Expression<Func<T, bool>> predicate = null) where T : EntityBase, new()
        {
            return predicate == null ? SqlLiteAsyncConnection.Table<T>().ToListAsync() : SqlLiteAsyncConnection.Table<T>().Where(predicate).ToListAsync();
        }
    }
}
