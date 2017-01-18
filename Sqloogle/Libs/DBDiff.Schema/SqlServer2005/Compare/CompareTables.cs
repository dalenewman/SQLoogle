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
    internal class CompareTables:CompareBase<Table>
    {
        protected override void DoUpdate<Root>(SchemaList<Table, Root> CamposOrigen, Table node)
        {
            if (node.Status != Enums.ObjectStatusType.DropStatus)
            {
                Table tablaOriginal = CamposOrigen[node.FullName];
                tablaOriginal.OriginalTable = (Table)CamposOrigen[node.FullName].Clone((Database)tablaOriginal.Parent);
                (new CompareColumns()).GenerateDiferences<Table>(tablaOriginal.Columns, node.Columns);
                (new CompareConstraints()).GenerateDiferences<Table>(tablaOriginal.Constraints, node.Constraints);
                (new CompareIndexes()).GenerateDiferences<Table>(tablaOriginal.Indexes, node.Indexes);
                (new CompareTablesOptions()).GenerateDiferences<Table>(tablaOriginal.Options, node.Options);
                (new CompareTriggers()).GenerateDiferences<Table>(tablaOriginal.Triggers, node.Triggers);
                (new CompareCLRTriggers()).GenerateDiferences<Table>(tablaOriginal.CLRTriggers, node.CLRTriggers);
                (new CompareFullTextIndex()).GenerateDiferences<Table>(tablaOriginal.FullTextIndex, node.FullTextIndex);
                if (!Table.CompareFileGroup(tablaOriginal, node))
                {
                    tablaOriginal.FileGroup = node.FileGroup;
                    /*Esto solo aplica a las tablas heap, el resto hace el campo en el filegroup del indice clustered*/
                    if (!tablaOriginal.HasClusteredIndex)
                        tablaOriginal.Status = Enums.ObjectStatusType.RebuildStatus;
                }
                if (!Table.CompareFileGroupText(tablaOriginal, node))
                {
                    tablaOriginal.FileGroupText = node.FileGroupText;
                    tablaOriginal.Status = Enums.ObjectStatusType.RebuildStatus;
                }
                if (node.HasChangeTracking != tablaOriginal.HasChangeTracking)
                {
                    tablaOriginal.HasChangeTracking = node.HasChangeTracking;
                    tablaOriginal.HasChangeTrackingTrackColumn = node.HasChangeTrackingTrackColumn;
                    tablaOriginal.Status += (int)Enums.ObjectStatusType.DisabledStatus;
                }
            }
        }

        /// <summary>
        /// Compara las colecciones de tablas de dos bases diferentes y marca el estado de los objetos
        /// dependiendo si existen o si deben borrarse.
        /// </summary>
        /// <param name="tablasOrigen"></param>
        /// Tablas originales, donde se guardaran los estados de las tablas.
        /// <param name="tablasDestino">
        /// Tablas comparativas, que se usa para comparar con la base original.
        /// </param>
        /*public static void GenerateDiferences(SchemaList<Table, Database> tablasOrigen, SchemaList<Table, Database> tablasDestino)
        {
            MarkDrop(tablasOrigen, tablasDestino);

            foreach (Table node in tablasDestino)
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
                        Table tablaOriginal = tablasOrigen[node.FullName];
                        tablaOriginal.OriginalTable = (Table)tablasOrigen[node.FullName].Clone((Database)tablaOriginal.Parent);
                        CompareColumns.GenerateDiferences<Table>(tablaOriginal.Columns, node.Columns);
                        CompareConstraints.GenerateDiferences<Table>(tablaOriginal.Constraints, node.Constraints);
                        CompareIndexes.GenerateDiferences(tablaOriginal.Indexes, node.Indexes);
                        CompareTablesOptions.GenerateDiferences(tablaOriginal.Options, node.Options);
                        (new CompareTriggers()).GenerateDiferences<Table>(tablaOriginal.Triggers, node.Triggers);
                        (new CompareCLRTriggers()).GenerateDiferences<Table>(tablaOriginal.CLRTriggers, node.CLRTriggers);
                        if (!Table.CompareFileGroup(tablaOriginal, node))
                        {
                            tablaOriginal.FileGroup = node.FileGroup;
                            //Esto solo aplica a las tablas heap, el resto hace el campo en el filegroup del indice clustered
                            if (!tablaOriginal.HasClusteredIndex)
                                tablaOriginal.Status = Enums.ObjectStatusType.RebuildStatus;
                        }
                        if (!Table.CompareFileGroupText(tablaOriginal, node))
                        {
                            tablaOriginal.FileGroupText = node.FileGroupText;
                            tablaOriginal.Status = Enums.ObjectStatusType.RebuildStatus;
                        }
                    }
                }
            }                       
        }*/
    }
}
