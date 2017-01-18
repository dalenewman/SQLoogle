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
using System;
using System.Collections.Generic;
using System.Linq;
using Sqloogle.Libs.DBDiff.Schema.Model;
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Generates;
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model;
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model.Interfaces;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Compare
{
    internal abstract class CompareBase<T> where T:ISchemaBase
    {
        protected virtual void DoUpdate<Root>(SchemaList<T, Root> CamposOrigen, T node) where Root:ISchemaBase
        {

        }

        protected virtual void DoNew<Root>(SchemaList<T, Root> CamposOrigen, T node) where Root : ISchemaBase
        {
            T newNode = node;//.Clone(CamposOrigen.Parent);
            newNode.Parent = CamposOrigen.Parent;
            newNode.Status = Enums.ObjectStatusType.CreateStatus;
            CamposOrigen.Add(newNode);
        }

        protected void DoDelete(T node)
        {
            node.Status = Enums.ObjectStatusType.DropStatus;
        }

        public void GenerateDiferences<Root>(SchemaList<T, Root> CamposOrigen, SchemaList<T, Root> CamposDestino) where Root : ISchemaBase
        {
            bool has = true;
            int DestinoIndex = 0;
            int OrigenIndex = 0;
            int DestinoCount = CamposDestino.Count;
            int OrigenCount = CamposOrigen.Count;
            T node;

            while (has)
            {
                has = false;
                if (DestinoCount > DestinoIndex)
                {
                    node = CamposDestino[DestinoIndex];
                    Generate.RaiseOnCompareProgress("Comparing Destination {0}: [{1}]", node.ObjectType, node.Name);
                    if (!CamposOrigen.Exists(node.FullName))
                    {
                        Generate.RaiseOnCompareProgress("Adding {0}: [{1}]", node.ObjectType, node.Name);
                        DoNew<Root>(CamposOrigen, node);
                    }
                    else
                    {
                        Generate.RaiseOnCompareProgress("Updating {0}: [{1}]", node.ObjectType, node.Name);
                        DoUpdate<Root>(CamposOrigen, node);
                    }

                    DestinoIndex++;
                    has = true;
                }

                if (OrigenCount > OrigenIndex)
                {
                    node = CamposOrigen[OrigenIndex];
                    Generate.RaiseOnCompareProgress("Comparing Source {0}: [{1}]", node.ObjectType, node.Name);
                    if (!CamposDestino.Exists(node.FullName))
                    {
                        Generate.RaiseOnCompareProgress("Deleting {0}: [{1}]", node.ObjectType, node.Name);
                        DoDelete(node);
                    }

                    OrigenIndex++;
                    has = true;
                }
            }
        }

        protected static void CompareExtendedProperties(ISQLServerSchemaBase origen, ISQLServerSchemaBase destino)
        {
            List<ExtendedProperty> dropList = (from node in origen.ExtendedProperties
                                               where !destino.ExtendedProperties.Exists(item => item.Name.Equals(node.Name, StringComparison.CurrentCultureIgnoreCase))
                                               select node).ToList<ExtendedProperty>();
            List<ExtendedProperty> addList = (from node in destino.ExtendedProperties
                                              where !origen.ExtendedProperties.Exists(item => item.Name.Equals(node.Name, StringComparison.CurrentCultureIgnoreCase))
                                               select node).ToList<ExtendedProperty>();
            dropList.ForEach(item => { item.Status = Enums.ObjectStatusType.DropStatus;} );
            addList.ForEach(item => { item.Status = Enums.ObjectStatusType.CreateStatus; });
            origen.ExtendedProperties.AddRange(addList);
        }
    }
}
