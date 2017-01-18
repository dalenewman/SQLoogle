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
    internal class CompareAssemblies : CompareBase<Assembly>
    {
        protected override void DoUpdate<Root>(SchemaList<Assembly, Root> CamposOrigen, Assembly node)
        {
            if (!node.Compare(CamposOrigen[node.FullName]))
            {
                Assembly newNode = (Assembly)node.Clone(CamposOrigen.Parent);
                newNode.Status = Enums.ObjectStatusType.AlterStatus;

                if (node.Text.Equals(CamposOrigen[node.FullName].Text))
                {
                    if (!node.PermissionSet.Equals(CamposOrigen[node.FullName].PermissionSet))
                        newNode.Status += (int)Enums.ObjectStatusType.PermisionSet;
                    if (!node.Owner.Equals(CamposOrigen[node.FullName].Owner))
                        newNode.Status += (int)Enums.ObjectStatusType.ChangeOwner;
                }
                else
                    newNode.Status = Enums.ObjectStatusType.RebuildStatus;

                CamposOrigen[node.FullName].Files.ForEach(item =>
                {
                    if (!newNode.Files.Exists(item.FullName))
                        newNode.Files.Add(new AssemblyFile(newNode, item, Enums.ObjectStatusType.DropStatus));
                    else
                        item.Status = Enums.ObjectStatusType.AlterStatus;
                });
                newNode.Files.ForEach(item =>
                {
                    if (!CamposOrigen[node.FullName].Files.Exists(item.FullName))
                    {
                        item.Status = Enums.ObjectStatusType.CreateStatus;
                    }
                });
                CompareExtendedProperties(CamposOrigen[node.FullName], newNode);
                CamposOrigen[node.FullName] = newNode;
            }
        }

        protected override void DoNew<Root>(SchemaList<Assembly, Root> CamposOrigen, Assembly node)
        {
            bool pass = true;    
            Assembly newNode = (Assembly)node.Clone(CamposOrigen.Parent);               
            if ((((Database)newNode.RootParent).Info.Version == DatabaseInfo.VersionTypeEnum.SQLServer2005) 
                && (((Database)node.RootParent).Info.Version == DatabaseInfo.VersionTypeEnum.SQLServer2008))
                pass = node.FullName.Equals("Microsoft.SqlServer.Types");
            if (pass)
            {            
                newNode.Status = Enums.ObjectStatusType.CreateStatus;
                CamposOrigen.Add(newNode);
            }            
        }
    }
}
