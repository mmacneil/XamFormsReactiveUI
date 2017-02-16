

using System.Collections.Generic;
using System.Threading.Tasks;
using XamFormsReactiveUI.Models.Entities;

namespace XamFormsReactiveUI.DataLayer.Abstract
{
    public interface IWordRepository : IEntityBaseRepository<Word>
    {
        Task<IList<Word>> GetWords(int floor, int ceiling);
    }
}
