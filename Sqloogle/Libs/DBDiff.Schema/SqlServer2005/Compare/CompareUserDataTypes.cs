using System;
using Sqloogle.Libs.DBDiff.Schema.Model;
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Compare
{
    internal class CompareUserDataTypes:CompareBase<UserDataType>
    {
        protected override void DoNew<Root>(SchemaList<UserDataType, Root> CamposOrigen, UserDataType node)
        {
            UserDataType newNode = (UserDataType)node.Clone(CamposOrigen.Parent);
            newNode.Status = Enums.ObjectStatusType.CreateStatus;
            Boolean HasAssembly = CamposOrigen.Exists(item => item.AssemblyFullName.Equals(node.AssemblyFullName) && item.IsAssembly);
            if (HasAssembly)
                newNode.Status += (int)Enums.ObjectStatusType.DropOlderStatus;
            CamposOrigen.Add(newNode);            
        }

        protected override void DoUpdate<Root>(SchemaList<UserDataType, Root> CamposOrigen, UserDataType node)
        {
            if (!node.Compare(CamposOrigen[node.FullName]))
            {
                UserDataType newNode = (UserDataType)node.Clone(CamposOrigen.Parent);
                newNode.Dependencys.AddRange(CamposOrigen[node.FullName].Dependencys);

                if (!UserDataType.CompareDefault(node, CamposOrigen[node.FullName]))
                {
                    if (!String.IsNullOrEmpty(node.Default.Name))
                        newNode.Default.Status = Enums.ObjectStatusType.CreateStatus;
                    else
                        newNode.Default.Status = Enums.ObjectStatusType.DropStatus;
                    newNode.Status = Enums.ObjectStatusType.AlterStatus;
                }
                else
                {
                    if (!UserDataType.CompareRule(node, CamposOrigen[node.FullName]))
                    {
                        if (!String.IsNullOrEmpty(node.Rule.Name))
                            newNode.Rule.Status = Enums.ObjectStatusType.CreateStatus;
                        else
                            newNode.Rule.Status = Enums.ObjectStatusType.DropStatus;
                        newNode.Status = Enums.ObjectStatusType.AlterStatus;
                    }
                    else
                        newNode.Status = Enums.ObjectStatusType.RebuildStatus;
                }
                CamposOrigen[node.FullName] = newNode;
            }            
        }
    }
}
