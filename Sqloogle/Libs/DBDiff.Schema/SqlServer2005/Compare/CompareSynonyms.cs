using Sqloogle.Libs.DBDiff.Schema.Model;
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Compare
{
    internal class CompareSynonyms:CompareBase<Synonym>
    {
        protected override void DoUpdate<Root>(SchemaList<Synonym, Root> CamposOrigen, Synonym node)
        {
            if (!Synonym.Compare(node, CamposOrigen[node.FullName]))
            {
                Synonym newNode = node;//.Clone(CamposOrigen.Parent);
                newNode.Status = Enums.ObjectStatusType.AlterStatus;
                CamposOrigen[node.FullName] = newNode;
            }
        }
    }
}
