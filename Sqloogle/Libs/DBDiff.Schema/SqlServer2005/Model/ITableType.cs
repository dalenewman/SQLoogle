using Sqloogle.Libs.DBDiff.Schema.Model;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model
{
    public interface ITable<T> where T : ISchemaBase
    {
        Columns<T> Columns { get; }
        SchemaList<Constraint, T> Constraints { get; }
        SchemaList<Index, T> Indexes { get; }
        ISchemaBase Parent { get; set; }
        string Owner { get; set; }
    }
}
