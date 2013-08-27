using Sqloogle.Libs.DBDiff.Schema.Model;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model.Interfaces
{
    public interface ISQLServerSchemaBase
    {
        SchemaList<ExtendedProperty, ISchemaBase> ExtendedProperties { get; }
    }
}
