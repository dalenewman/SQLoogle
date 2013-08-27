using Sqloogle.Libs.DBDiff.Schema.Model;
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model.Interfaces;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model
{
    public abstract class SQLServerSchemaBase:SchemaBase, ISQLServerSchemaBase
    {
        private SchemaList<ExtendedProperty,ISchemaBase> extendedProperties;

        protected SQLServerSchemaBase(ISchemaBase parent, Enums.ObjectType objectType):base("[", "]", objectType)
        {
            this.Parent = parent;
            extendedProperties = new SchemaList<ExtendedProperty, ISchemaBase>(parent);
        }

        public SchemaList<ExtendedProperty, ISchemaBase> ExtendedProperties
        {
            get { return extendedProperties; }
        }
    }
}
