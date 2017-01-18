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
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model
{
    public class FileGroupFiles : List<FileGroupFile>
    {
        private Hashtable hash = new Hashtable();
        private FileGroup parent;

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="parent">
        /// Objeto Database padre.
        /// </param>
        public FileGroupFiles(FileGroup parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Clona el objeto FileGroups en una nueva instancia.
        /// </summary>
        public FileGroupFiles Clone(FileGroup parentObject)
        {
            FileGroupFiles columns = new FileGroupFiles(parentObject);
            for (int index = 0; index < this.Count; index++)
            {
                columns.Add((FileGroupFile)this[index].Clone(parentObject));
            }
            return columns;
        }

        /// <summary>
        /// Indica si el nombre del FileGroup existe en la coleccion de tablas del objeto.
        /// </summary>
        /// <param name="table">
        /// Nombre de la tabla a buscar.
        /// </param>
        /// <returns></returns>
        public Boolean Find(string table)
        {
            return hash.ContainsKey(table);
        }

        /// <summary>
        /// Agrega un objeto columna a la coleccion de columnas.
        /// </summary>
        public new void Add(FileGroupFile file)
        {
            if (file != null)
            {
                hash.Add(file.FullName, file);
                base.Add(file);
            }
            else
                throw new ArgumentNullException("file");
        }

        public FileGroupFile this[string name]
        {
            get { return (FileGroupFile)hash[name]; }
            set
            {
                hash[name] = value;
                for (int index = 0; index < base.Count; index++)
                {
                    if (((FileGroupFile)base[index]).Name.Equals(name))
                    {
                        base[index] = value;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Devuelve la tabla perteneciente a la coleccion de campos.
        /// </summary>
        public FileGroup Parent
        {
            get { return parent; }
        }

        public string ToSQL()
        {
            StringBuilder sql = new StringBuilder();
            for (int index = 0; index < this.Count; index++)
            {
                sql.Append(this[index].ToSql());
            }
            return sql.ToString();
        }

        public string ToSQLDrop()
        {
            StringBuilder sql = new StringBuilder();
            for (int index = 0; index < this.Count; index++)
            {
                sql.Append(this[index].ToSqlDrop());
            }
            return sql.ToString();
        }
    }
}
