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

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model
{
    public class IndexColumn : SQLServerSchemaBase, IComparable<IndexColumn>
    {
        private Boolean order;
        private Boolean isIncluded;
        private int keyOrder;
        private int dataTypeId;

        public IndexColumn(ISchemaBase parentObject)
            : base(parentObject, Enums.ObjectType.IndexColumn)
        {
        }

        public new IndexColumn Clone(ISchemaBase parent)
        {
            IndexColumn column = new IndexColumn(parent);
            column.Id = this.Id;
            column.IsIncluded = this.IsIncluded;
            column.Name = this.Name;
            column.Order = this.Order;
            column.Status = this.Status;
            column.KeyOrder = this.KeyOrder;
            column.DataTypeId = this.DataTypeId;
            return column;
        }

        public int DataTypeId
        {
            get { return dataTypeId; }
            set { dataTypeId = value; }
        }

        public int KeyOrder
        {
            get { return keyOrder; }
            set { keyOrder = value; }
        }

        public Boolean IsIncluded
        {
            get { return isIncluded; }
            set { isIncluded = value; }
        }

        public Boolean Order
        {
            get { return order; }
            set { order = value; }
        }

        public static Boolean Compare(IndexColumn origen, IndexColumn destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if (origen.IsIncluded != destino.IsIncluded) return false;
            if (origen.Order != destino.Order) return false;
            if (origen.KeyOrder != destino.KeyOrder) return false;
            return true;
        }
        
        public override string ToSqlDrop()
        {
            return "";
        }

        public override string ToSqlAdd()
        {
            return "";
        }

        public override string ToSql()
        {
            return "";
        }

        public int CompareTo(IndexColumn other)
        {
            /*if (other.Name.Equals(this.Name))
            {*/
                if (other.IsIncluded == this.IsIncluded)
                    return this.KeyOrder.CompareTo(other.KeyOrder);
                else
                    return other.IsIncluded.CompareTo(this.IsIncluded);
            /*}
            else
                return this.Name.CompareTo(other.Name);*/
        }
    }
}
