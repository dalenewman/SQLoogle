using Sqloogle.Libs.DBDiff.Schema.Model;
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Compare
{
    internal class CompareRules:CompareBase<Rule>
    {
        protected override void DoUpdate<Root>(SchemaList<Rule, Root> CamposOrigen, Rule node)
        {
            if (!node.Compare(CamposOrigen[node.FullName]))
            {
                Rule newNode = node.Clone(CamposOrigen.Parent);
                newNode.Status = Enums.ObjectStatusType.AlterStatus;
                CamposOrigen[node.FullName] = newNode;
            }
        }

        protected override void DoNew<Root>(SchemaList<Rule, Root> CamposOrigen, Rule node)
        {
            Rule newNode = node.Clone(CamposOrigen.Parent);
            newNode.Status = Enums.ObjectStatusType.CreateStatus;
            CamposOrigen.Add(newNode);
        }
    }
}