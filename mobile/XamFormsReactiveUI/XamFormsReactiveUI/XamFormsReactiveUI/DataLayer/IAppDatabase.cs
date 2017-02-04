using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SQLite;
using XamFormsReactiveUI.Models.Entities;

namespace XamFormsReactiveUI.DataLayer
{
    public interface IAppDatabase
    {
        SQLiteAsyncConnection SqlLiteAsyncConnection { get; }
        Task<List<T>> GetItemsAsync<T>(Expression<Func<T, bool>> predicate = null) where T : EntityBase, new();
        
    }
}
