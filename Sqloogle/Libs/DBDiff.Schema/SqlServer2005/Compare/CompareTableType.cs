#region license
// Sqloogle
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using Sqloogle.Libs.DBDiff.Schema.Model;
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Compare
{
    internal class CompareTableType : CompareBase<TableType>
    {
        protected override void DoUpdate<Root>(SchemaList<TableType, Root> CamposOrigen, TableType node)
        {
            if (node.Status != Enums.ObjectStatusType.DropStatus)
            {
                TableType tablaOriginal = CamposOrigen[node.FullName];
                (new CompareColumns()).GenerateDiferences<TableType>(tablaOriginal.Columns, node.Columns);
                (new CompareConstraints()).GenerateDiferences<TableType>(tablaOriginal.Constraints, node.Constraints);
                (new CompareIndexes()).GenerateDiferences<TableType>(tablaOriginal.Indexes, node.Indexes);
            }
        }

        /*public static void GenerateDiferences(SchemaList<TableType, Database> tablasOrigen, SchemaList<TableType, Database> tablasDestino)
        {
            MarkDrop(tablasOrigen, tablasDestino);

            foreach (TableType node in tablasDestino)
            {
                if (!tablasOrigen.Exists(node.FullName))
                {
                    node.Status = Enums.ObjectStatusType.CreateStatus;
                    node.Parent = tablasOrigen.Parent;
                    tablasOrigen.Add(node);
                }
                else
                {
                    if (node.Status != Enums.ObjectStatusType.DropStatus)
                    {
                        TableType tablaOriginal = tablasOrigen[node.FullName];
                        CompareColumns.GenerateDiferences<TableType>(tablaOriginal.Columns, node.Columns);
                        CompareConstraints.GenerateDiferences<TableType>(tablaOriginal.Constraints, node.Constraints);
                        CompareIndexes.GenerateDiferences(tablaOriginal.Indexes, node.Indexes);
                    }
                }
            }
        }*/
    }
}
