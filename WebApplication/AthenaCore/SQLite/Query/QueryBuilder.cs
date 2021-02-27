namespace WebApplication.AthenaCore.SQLite.Query
{
    public class QueryBuilder<T> where T: class, new()
    {

        // public static SelectQuery<T> Select(params string[] columns)
        // {
        //     if (columns == null || columns.Length == 0) return null;
        //     
        //     return new SelectQuery<T>(columns);
        // }
        //
        // public static SelectQuery<T> SelectAll()
        // {
        //     return Select("*");
        // }
    }
}