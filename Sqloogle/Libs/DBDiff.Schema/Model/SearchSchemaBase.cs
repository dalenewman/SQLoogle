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

namespace Sqloogle.Libs.DBDiff.Schema.Model
{
    public class SearchSchemaBase
    {
        private Dictionary<String, Enums.ObjectType> objectTypes;
        private Dictionary<String, String> objectParent;
        private Dictionary<Int32, String> objectId;

        public SearchSchemaBase()
        {
            objectTypes = new Dictionary<string, Enums.ObjectType>();
            objectParent = new Dictionary<string, string>();
            objectId = new Dictionary<Int32, string>();
        }

        public void Add(ISchemaBase item)
        {
            if (objectTypes.ContainsKey(item.FullName.ToUpper()))
                objectTypes.Remove(item.FullName.ToUpper());
            objectTypes.Add(item.FullName.ToUpper(), item.ObjectType);
            if ((item.ObjectType == Enums.ObjectType.Constraint) || (item.ObjectType == Enums.ObjectType.Index) || (item.ObjectType == Enums.ObjectType.Trigger) || (item.ObjectType == Enums.ObjectType.CLRTrigger))
            {
                if (objectParent.ContainsKey(item.FullName.ToUpper()))
                    objectParent.Remove(item.FullName.ToUpper());
                objectParent.Add(item.FullName.ToUpper(), item.Parent.FullName);

                if (objectId.ContainsKey(item.Id))
                    objectId.Remove(item.Id);
                objectId.Add(item.Id, item.FullName);
            }
        }

        public Enums.ObjectType GetType(string FullName)
        {
            return objectTypes[FullName.ToUpper()];
        }

        public string GetParentName(string FullName)
        {
            return objectParent[FullName.ToUpper()];
        }

        public string GetFullName(int Id)
        {
            return objectId[Id];
        }
    }
}
