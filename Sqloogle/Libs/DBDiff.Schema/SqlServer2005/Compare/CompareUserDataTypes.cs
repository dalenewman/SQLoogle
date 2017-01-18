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
