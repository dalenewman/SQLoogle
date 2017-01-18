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
namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model
{
    internal class Dependence
    {
        private int objectId;
        private int subObjectId;
        private int ownerTableId;
        private string fullName;
        private int typeId;
        private Enums.ObjectType type;

        public string FullName
        {
            get { return fullName; }
            set { fullName = value; }
        }

        public int DataTypeId
        {
            get { return typeId; }
            set { typeId = value; }
        }

        public Enums.ObjectType Type
        {
            get { return type; }
            set { type = value; }
        }

        public int SubObjectId
        {
            get { return subObjectId; }
            set { subObjectId = value; }
        }

        /// <summary>
        /// ID de la tabla a la que hace referencia la constraint.
        /// </summary>
        public int ObjectId
        {
            get { return objectId; }
            set { objectId = value; }
        }

        /// <summary>
        /// ID de la tabla a la que pertenece la constraint.
        /// </summary>
        public int OwnerTableId
        {
            get { return ownerTableId; }
            set { ownerTableId = value; }
        }
    }
}
