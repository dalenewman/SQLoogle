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
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Compare
{
    internal class CompareColumnsConstraints:CompareBase<ColumnConstraint>
    {
        public static ColumnConstraint GenerateDiferences(Column CamposOrigen, Column CamposDestino)
        {
            if ((CamposOrigen.DefaultConstraint == null) && (CamposDestino.DefaultConstraint != null))
            {
                CamposOrigen.DefaultConstraint = CamposDestino.DefaultConstraint.Clone(CamposOrigen);
                CamposOrigen.DefaultConstraint.Status = Enums.ObjectStatusType.CreateStatus;
                CamposOrigen.DefaultConstraint.Parent.Status = Enums.ObjectStatusType.OriginalStatus;
                CamposOrigen.DefaultConstraint.Parent.Parent.Status = Enums.ObjectStatusType.AlterStatus;
            }
            else
            {
                if ((CamposOrigen.DefaultConstraint != null) && (CamposDestino.DefaultConstraint != null))
                {
                    if (!ColumnConstraint.Compare(CamposOrigen.DefaultConstraint, CamposDestino.DefaultConstraint))
                    {
                        CamposOrigen.DefaultConstraint = CamposDestino.DefaultConstraint.Clone(CamposOrigen);
                        //Indico que hay un ALTER TABLE, pero sobre la columna, no seteo ningun estado.
                        CamposOrigen.DefaultConstraint.Status = Enums.ObjectStatusType.AlterStatus;
                        CamposOrigen.DefaultConstraint.Parent.Status = Enums.ObjectStatusType.OriginalStatus;
                        CamposOrigen.DefaultConstraint.Parent.Parent.Status = Enums.ObjectStatusType.AlterStatus;
                    }
                }
                else
                    if ((CamposOrigen.DefaultConstraint != null) && (CamposDestino.DefaultConstraint == null))
                    {
                        CamposOrigen.DefaultConstraint.Status = Enums.ObjectStatusType.DropStatus;
                        CamposOrigen.DefaultConstraint.Parent.Status = Enums.ObjectStatusType.OriginalStatus;
                        CamposOrigen.DefaultConstraint.Parent.Parent.Status = Enums.ObjectStatusType.AlterStatus;
                    }
            }
            /*foreach (ColumnConstraint node in CamposDestino)
            {
                if (!CamposOrigen.Exists(node.FullName))
                {
                    node.Status = Enums.ObjectStatusType.CreateStatus;
                    CamposOrigen.Parent.Status = Enums.ObjectStatusType.OriginalStatus;
                    CamposOrigen.Parent.Parent.Status = Enums.ObjectStatusType.AlterStatus;
                    CamposOrigen.Add(node);
                }
                else
                {
                    if (!ColumnConstraint.Compare(CamposOrigen[node.FullName], node))
                    {
                        ColumnConstraint newNode = node.Clone(CamposOrigen.Parent);
                        //Indico que hay un ALTER TABLE, pero sobre la columna, no seteo ningun estado.
                        newNode.Status = Enums.ObjectStatusType.AlterStatus;
                        newNode.Parent.Status = Enums.ObjectStatusType.OriginalStatus;
                        newNode.Parent.Parent.Status = Enums.ObjectStatusType.AlterStatus;
                        CamposOrigen[node.FullName] = newNode;
                        
                    }
                }
            }

            MarkDrop(CamposOrigen, CamposDestino, node => 
            {
                node.Status = Enums.ObjectStatusType.DropStatus;
                CamposOrigen.Parent.Status = Enums.ObjectStatusType.OriginalStatus;
                CamposOrigen.Parent.Parent.Status = Enums.ObjectStatusType.AlterStatus;
            }
            );
            */
            return CamposOrigen.DefaultConstraint;
        }
    }
}
