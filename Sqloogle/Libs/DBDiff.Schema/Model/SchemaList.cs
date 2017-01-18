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
using System.Text;
using System.Linq;

namespace Sqloogle.Libs.DBDiff.Schema.Model
{
    public class SchemaList<T, P> : List<T>, DBDiff.Schema.Model.ISchemaList<T,P>
        where T : ISchemaBase
        where P : ISchemaBase
    {
        private P parent;
        private Dictionary<string, int> nameMap = new Dictionary<string, int>();
        private SearchSchemaBase allObjects = null;
        private StringComparison comparion;
        private bool IsCaseSensity = false;

        public SchemaList(P parent, SearchSchemaBase allObjects)
        {
            this.parent = parent;
            this.allObjects = allObjects;
            this.comparion = StringComparison.CurrentCultureIgnoreCase;
        }

        public SchemaList<T, P> Clone(P parentObject)
        {
            SchemaList<T, P> options = new SchemaList<T, P>(parentObject, allObjects);
            this.ForEach(item =>
            {
                object cloned = item.Clone(parentObject);

                //Not everything implements the clone methd, so make sure we got some actual cloned data before adding it back to the list
                if (cloned != null)
                    options.Add((T)cloned);
            });

            return options;
        }

        protected StringComparison Comparion
        {
            get { return comparion; }
        }

        public SchemaList(P parent)
        {
            this.parent = parent;
        }

        public new void Add(T item)
        {
            base.Add(item);
            if (allObjects != null)
                allObjects.Add(item);

            string name = item.FullName;
            IsCaseSensity = item.RootParent.IsCaseSensity;
            if (!IsCaseSensity)
                name = name.ToUpper();

            if (!nameMap.ContainsKey(name))
                nameMap.Add(name, base.Count - 1);
        }
        /// <summary>
        /// Devuelve el objecto Padre perteneciente a la coleccion.
        /// </summary>
        public P Parent
        {
            get { return parent; }
        }

        /// <summary>
        /// Devuelve el objeto correspondiente a un ID especifico.
        /// </summary>
        /// <param name="id">ID del objecto a buscar</param>
        /// <returns>Si no encontro nada, devuelve null, de lo contrario, el objeto</returns>
        public T Find(int id)
        {
            return Find(Item => Item.Id == id);
        }

        /// <summary>
        /// Indica si el nombre del objecto existe en la coleccion de objectos del mismo tipo.
        /// </summary>
        /// <param name="table">
        /// Nombre del objecto a buscar.
        /// </param>
        /// <returns></returns>
        public Boolean Exists(string name)
        {
            if (IsCaseSensity)
                return nameMap.ContainsKey(name);
            else
                return nameMap.ContainsKey(name.ToUpper());
        }

        public virtual T this[string name]
        {
            get
            {
                try
                {
                    if (IsCaseSensity)
                        return this[nameMap[name]];
                    else
                        return this[nameMap[name.ToUpper()]];
                }
                catch
                {
                    return default(T);
                }
            }
            set
            {
                if (IsCaseSensity)
                    base[nameMap[name]] = value;
                else
                    base[nameMap[name.ToUpper()]] = value;
            }
        }

        public virtual SQLScriptList ToSqlDiff()
        {
			return ToSqlDiff(new List<ISchemaBase>());	
		}
		public virtual SQLScriptList ToSqlDiff(List<ISchemaBase> schemas)
        {
            SQLScriptList listDiff = new SQLScriptList();
            this.Where(item => (schemas.Count == 0 || schemas.FirstOrDefault(sch => sch.Id == item.Id) != default(ISchemaBase))).ToList().ForEach(item => { item.ResetWasInsertInDiffList(); listDiff.AddRange(item.ToSqlDiff().WarnMissingScript(item)); });
            return listDiff;
        }


        public virtual string ToSql()
        {
            StringBuilder sql = new StringBuilder();
            this.ForEach(item =>
                {
                    if (item.Status != Enums.ObjectStatusType.DropStatus)
                        sql.Append(item.ToSql() + "\r\n");
                });
            return sql.ToString();
        }
    }
}
