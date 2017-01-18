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
    internal class CompareDDLTriggers : CompareBase<Trigger>
    {
        protected override void DoUpdate<Root>(SchemaList<Trigger, Root> CamposOrigen, Trigger node)
        {
            if (!node.Compare(CamposOrigen[node.FullName]))
            {
                Trigger newNode = (Trigger)node.Clone(CamposOrigen.Parent);
                if (!newNode.Text.Equals(CamposOrigen[node.FullName].Text))
                    newNode.Status = Enums.ObjectStatusType.AlterStatus;
                if (node.IsDisabled != CamposOrigen[node.FullName].IsDisabled)
                    newNode.Status = newNode.Status + (int)Enums.ObjectStatusType.DisabledStatus;
                CamposOrigen[node.FullName] = newNode;
            }            
        }

        protected override void DoNew<Root>(SchemaList<Trigger, Root> CamposOrigen, Trigger node)
        {
            Trigger newNode = (Trigger)node.Clone(CamposOrigen.Parent);
            newNode.Status = Enums.ObjectStatusType.CreateStatus;
            CamposOrigen.Add(newNode);
        }
    }
}
