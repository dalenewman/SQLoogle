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

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model
{
    public class ConstraintColumn : SQLServerSchemaBase, IComparable<ConstraintColumn>
    {
        private string columnRelationalName;
        private int columnRelationalId;
        private int columnRelationalDataTypeId;
        private Boolean order;
        private Boolean isIncluded;
        private int keyOrder;
        private int dataTypeId;

        public ConstraintColumn(Constraint parentObject)
            : base(parentObject, Enums.ObjectType.ConstraintColumn)
        {
        }

        public ConstraintColumn Clone()
        {
            ConstraintColumn ccol = new ConstraintColumn((Constraint)this.Parent);
            ccol.ColumnRelationalName = this.ColumnRelationalName;
            ccol.ColumnRelationalId = this.ColumnRelationalId;
            ccol.Name = this.Name;
            ccol.IsIncluded = this.IsIncluded;
            ccol.Order = this.Order;
            ccol.KeyOrder = this.KeyOrder;
            ccol.Id = this.Id;
            ccol.DataTypeId = this.DataTypeId;
            ccol.ColumnRelationalDataTypeId = this.ColumnRelationalDataTypeId;
            return ccol;
        }

        public int DataTypeId
        {
            get { return dataTypeId; }
            set { dataTypeId = value; }
        }

        public int ColumnRelationalDataTypeId
        {
            get { return columnRelationalDataTypeId; }
            set { columnRelationalDataTypeId = value; }
        }

        public int ColumnRelationalId
        {
            get { return columnRelationalId; }
            set { columnRelationalId = value; }
        }

        /// <summary>
        /// Gets or sets the column key order in the index.
        /// </summary>
        /// <value>The key order.</value>
        public int KeyOrder
        {
            get { return keyOrder; }
            set { keyOrder = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this column is included in the index leaf page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this column is included; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsIncluded
        {
            get { return isIncluded; }
            set { isIncluded = value; }
        }

        /// <summary>
        /// Orden de la columna (Ascendente o Descendente). Se usa solo en Primary Keys.
        /// </summary>
        public Boolean Order
        {
            get { return order; }
            set { order = value; }
        }

        public string ColumnRelationalName
        {
            get { return columnRelationalName; }
            set { columnRelationalName = value; }
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

        public static Boolean Compare(ConstraintColumn origen, ConstraintColumn destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if ((origen.ColumnRelationalName == null) && (destino.ColumnRelationalName != null)) return false;
            if (origen.ColumnRelationalName != null)
            {
                if (!origen.ColumnRelationalName.Equals(destino.ColumnRelationalName, StringComparison.CurrentCultureIgnoreCase)) return false;
            }
            if (origen.IsIncluded != destino.IsIncluded) return false;
            if (origen.Order != destino.Order) return false;
            if (origen.KeyOrder != destino.KeyOrder) return false;
            return true;
        }

        public int CompareTo(ConstraintColumn other)
        {
            return this.KeyOrder.CompareTo(other.KeyOrder);
        }
    }
}
