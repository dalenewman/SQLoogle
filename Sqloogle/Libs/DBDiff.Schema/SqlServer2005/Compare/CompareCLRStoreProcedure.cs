﻿using Sqloogle.Libs.DBDiff.Schema.Model;
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Compare
{
    internal class CompareCLRStoreProcedure : CompareBase<CLRStoreProcedure>
    {
        protected override void DoUpdate<Root>(SchemaList<CLRStoreProcedure, Root> CamposOrigen, CLRStoreProcedure node)
        {
            if (!node.Compare(CamposOrigen[node.FullName]))
            {
                CLRStoreProcedure newNode = node;//.Clone(CamposOrigen.Parent);
                newNode.Status = Enums.ObjectStatusType.AlterStatus;
                CamposOrigen[node.FullName] = newNode;
            }
        }
    }
}
