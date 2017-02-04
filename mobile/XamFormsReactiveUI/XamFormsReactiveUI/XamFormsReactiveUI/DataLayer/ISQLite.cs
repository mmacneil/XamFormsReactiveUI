 

using SQLite;

namespace XamFormsReactiveUI.DataLayer
{
    public interface ISQLite
    {
        SQLiteAsyncConnection GetConnection();
    }
}
