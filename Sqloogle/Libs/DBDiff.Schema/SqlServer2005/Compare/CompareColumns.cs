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
using Sqloogle.Libs.DBDiff.Schema.Model;
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Compare
{
    internal class CompareColumns
    {
        public void GenerateDiferences<T>(Columns<T> CamposOrigen, Columns<T> CamposDestino) where T:ISchemaBase
        {
            int restPosition = 0;
            int sumPosition = 0;

            foreach (Column node in CamposOrigen)
            {
                if (!CamposDestino.Exists(node.FullName))
                {
                    node.Status = Enums.ObjectStatusType.DropStatus;
                    restPosition++;
                }
                else
                    CamposOrigen[node.FullName].Position -= restPosition;
            }
            foreach (Column node in CamposDestino)
            {                
                if (!CamposOrigen.Exists(node.FullName))
                {
                    Column newNode = node.Clone(CamposOrigen.Parent);
                    if ((newNode.Position == 1) || ((newNode.DefaultConstraint == null) && (!newNode.IsNullable) && (!newNode.IsComputed) && (!newNode.IsIdentity) && (!newNode.IsIdentityForReplication)))
                    {
                        newNode.Status = Enums.ObjectStatusType.CreateStatus;
                        newNode.Parent.Status = Enums.ObjectStatusType.RebuildStatus;
                    }
                    else
                        newNode.Status = Enums.ObjectStatusType.CreateStatus;
                    sumPosition++;             
                    CamposOrigen.Add(newNode);
                }
                else
                {
                    Column campoOrigen = CamposOrigen[node.FullName];
                    /*ColumnConstraint oldDefault = null;
                    if (campoOrigen.DefaultConstraint != null)
                        oldDefault = campoOrigen.DefaultConstraint.Clone(campoOrigen);*/
                    Boolean IsColumnEqual = Column.Compare(campoOrigen, node);
                    if ((!IsColumnEqual) || (campoOrigen.Position != node.Position))
                    {
                        if (Column.CompareIdentity(campoOrigen, node))
                        {

                            if (node.HasToRebuildOnlyConstraint)
                            {
                                node.Status = Enums.ObjectStatusType.AlterStatus;
                                if ((campoOrigen.IsNullable) && (!node.IsNullable))
                                    node.Status += (int)Enums.ObjectStatusType.UpdateStatus;
                            }
                            else
                            {
                                if (node.HasToRebuild(campoOrigen.Position + sumPosition, campoOrigen.Type, campoOrigen.IsFileStream))
                                    node.Status = Enums.ObjectStatusType.RebuildStatus;
                                else
                                {
                                    if (!IsColumnEqual)
                                    {
                                        node.Status = Enums.ObjectStatusType.AlterStatus;
                                        if ((campoOrigen.IsNullable) && (!node.IsNullable))
                                            node.Status += (int)Enums.ObjectStatusType.UpdateStatus;                                        
                                    }
                                }
                            }
                            if (node.Status != Enums.ObjectStatusType.RebuildStatus)
                            {
                                if (!Column.CompareRule(campoOrigen, node))
                                {
                                    node.Status += (int)Enums.ObjectStatusType.BindStatus;
                                }
                            }
                        }
                        else
                        {
                            node.Status = Enums.ObjectStatusType.RebuildStatus;
                        }                        
                        CamposOrigen[node.FullName] = node.Clone(CamposOrigen.Parent);
                    }
                    CamposOrigen[node.FullName].DefaultConstraint = CompareColumnsConstraints.GenerateDiferences(campoOrigen, node);
                }
            }
        }
    }
}

