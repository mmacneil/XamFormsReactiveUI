 
using SQLite;
using XamFormsReactiveUI.DataLayer.Abstract;

namespace XamFormsReactiveUI.Models.Entities
{
    public abstract class EntityBase : IEntityBase
    {
        /// <summary>
        /// Gets or sets the Database ID.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
    }
}
