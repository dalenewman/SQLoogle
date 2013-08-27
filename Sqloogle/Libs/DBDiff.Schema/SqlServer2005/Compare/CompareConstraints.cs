using Sqloogle.Libs.DBDiff.Schema.Model;
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Compare
{
    internal class CompareConstraints:CompareBase<Constraint>
    {
        protected override void DoUpdate<Root>(SchemaList<Constraint, Root> CamposOrigen, Constraint node) 
        {
            Constraint origen = CamposOrigen[node.FullName];
            if (!Constraint.Compare(origen, node))
            {
                Constraint newNode = (Constraint)node.Clone(CamposOrigen.Parent);
                if (node.IsDisabled == origen.IsDisabled)
                {
                    newNode.Status = Enums.ObjectStatusType.AlterStatus;
                }
                else
                    newNode.Status = Enums.ObjectStatusType.AlterStatus + (int)Enums.ObjectStatusType.DisabledStatus;
                CamposOrigen[node.FullName] = newNode;
            }
            else
            {
                if (node.IsDisabled != origen.IsDisabled)
                {
                    Constraint newNode = (Constraint)node.Clone(CamposOrigen.Parent);
                    newNode.Status = Enums.ObjectStatusType.DisabledStatus;
                    CamposOrigen[node.FullName] = newNode;
                }
            }
        }

        protected override void DoNew<Root>(SchemaList<Constraint, Root> CamposOrigen, Constraint node) 
        {
            Constraint newNode = (Constraint)node.Clone(CamposOrigen.Parent);
            newNode.Status = Enums.ObjectStatusType.CreateStatus;
            CamposOrigen.Add(newNode);
        }
    }
}
