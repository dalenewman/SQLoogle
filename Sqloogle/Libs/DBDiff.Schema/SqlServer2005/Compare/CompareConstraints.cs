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
