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
    public class ConstraintColumns:SchemaList<ConstraintColumn, Constraint>
    {
        public ConstraintColumns(Constraint parent)
            : base(parent)
        {
        }

        /// <summary>
        /// Clona el objeto ColumnConstraints en una nueva instancia.
        /// </summary>
        public ConstraintColumns Clone()
        {
            ConstraintColumns columns = new ConstraintColumns(this.Parent);
            for (int index = 0; index < this.Count; index++)
            {
                columns.Add(this[index].Clone());
            }
            return columns;
        }

        /// <summary>
        /// Compara dos campos y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean Compare(ConstraintColumns origen, ConstraintColumns destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if (origen.Count != destino.Count) return false;
            for (int j = 0; j < origen.Count; j++)
            {
                ConstraintColumn item = destino[origen[j].FullName];
                if (item == null)
                    return false;
                else
                    if (!ConstraintColumn.Compare(origen[j], item)) return false;
            }
            for (int j = 0; j < destino.Count; j++)
            {
                ConstraintColumn item = origen[destino[j].FullName];
                if (item == null)
                    return false;
                else
                    if (!ConstraintColumn.Compare(destino[j], item)) return false;
            }
            return true;
        }
    }
}
