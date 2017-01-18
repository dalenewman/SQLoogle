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
    internal class CompareFullText : CompareBase<FullText>
    {
        protected override void DoUpdate<Root>(SchemaList<FullText, Root> CamposOrigen, FullText node)
        {
            if (!node.Compare(CamposOrigen[node.FullName]))
            {
                FullText newNode = node;//.Clone(CamposOrigen.Parent);                
                if (node.IsDefault != CamposOrigen[node.FullName].IsDefault)                
                    newNode.Status += (int)Enums.ObjectStatusType.DisabledStatus;
                if (!node.Owner.Equals(CamposOrigen[node.FullName].Owner))
                    newNode.Status += (int)Enums.ObjectStatusType.ChangeOwner;
                if (node.IsAccentSensity != CamposOrigen[node.FullName].IsAccentSensity)
                    newNode.Status += (int)Enums.ObjectStatusType.AlterStatus;
                CamposOrigen[node.FullName] = newNode;
            }
        }
    }
}
