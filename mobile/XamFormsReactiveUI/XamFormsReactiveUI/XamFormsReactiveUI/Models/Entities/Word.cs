 

using SQLite;

namespace XamFormsReactiveUI.Models.Entities
{
    [Table("Words")]
    public class Word : EntityBase
    {
        public string Name { get; set; }
        public string Definition { get; set; }
    }
}
