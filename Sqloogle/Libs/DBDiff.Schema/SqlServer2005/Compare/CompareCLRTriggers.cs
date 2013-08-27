using Sqloogle.Libs.DBDiff.Schema.Model;
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Compare
{
    internal class CompareCLRTriggers : CompareBase<CLRTrigger>
    {
        protected override void DoUpdate<Root>(SchemaList<CLRTrigger, Root> CamposOrigen, CLRTrigger node)
        {
            if (!node.Compare(CamposOrigen[node.FullName]))
            {
                CLRTrigger newNode = node;
                newNode.Status = Enums.ObjectStatusType.AlterStatus;
                CompareExtendedProperties(newNode, CamposOrigen[node.FullName]);
                CamposOrigen[node.FullName] = newNode;
            }
        }
    }
}
