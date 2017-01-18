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
    internal class CompareFileGroups:CompareBase<FileGroup>
    {
        protected override void DoNew<Root>(SchemaList<FileGroup, Root> CamposOrigen, FileGroup node)
        {
            FileGroup newNode = (FileGroup)node.Clone(CamposOrigen.Parent);
            newNode.Status = Enums.ObjectStatusType.CreateStatus;
            /*If the Logical File Name exists in another filegroup,
             * we must change the new Logical File Name.
             */
            CamposOrigen.ForEach(file =>
            {
                if (file.Status != Enums.ObjectStatusType.DropStatus)
                {
                    file.Files.ForEach(group =>
                    {
                        newNode.Files.ForEach(ngroup =>
                        {
                            if (group.CompareFullNameTo(group.FullName, ngroup.FullName) == 0)
                            {
                                newNode.Files[ngroup.FullName].Name = group.Name + "_2";
                            }
                        });
                    });
                }
            });
            CamposOrigen.Add(newNode);
        }

        protected override void DoUpdate<Root>(SchemaList<FileGroup, Root> CamposOrigen, FileGroup node)
        {
            if (!FileGroup.Compare(node, CamposOrigen[node.FullName]))
            {
                FileGroup newNode = (FileGroup)node.Clone(CamposOrigen.Parent);
                newNode.Status = Enums.ObjectStatusType.AlterStatus;
                CamposOrigen[node.FullName] = newNode;
            }
        }
    }
}