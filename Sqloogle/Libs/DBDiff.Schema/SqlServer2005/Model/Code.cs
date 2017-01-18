﻿#region license
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
using System.Text.RegularExpressions;
using Sqloogle.Libs.DBDiff.Schema.Model;
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model.Interfaces;
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model.Util;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model
{
    public abstract class Code: SQLServerSchemaBase, ICode
    {
        private string text;
        private Boolean isSchemaBinding;
        private List<String> dependenciesIn;
        private List<String> dependenciesOut;
        protected string sql = null;
        protected string typeName = "";
        private int deepMax = 0;
        private int deepMin = 0;
        private Enums.ScripActionType addAction;
        private Enums.ScripActionType dropAction;

        public Code(ISchemaBase parent, Enums.ObjectType type, Enums.ScripActionType addAction, Enums.ScripActionType dropAction)
            : base(parent, type)
        {
            dependenciesIn = new List<String>();
            dependenciesOut = new List<String>();
            typeName = GetObjectTypeName(ObjectType);
            /*Por el momento, solo los Assemblys manejan deep de dependencias*/
            if (this.ObjectType == Enums.ObjectType.Assembly) 
            {
                deepMax = 501;
                deepMin = 500;
            }
            this.addAction = addAction;
            this.dropAction = dropAction;
        }

        public override SQLScript Create()
        {
            int iCount = DependenciesCount;
            if (iCount > 0) iCount = iCount * -1;
            if (!GetWasInsertInDiffList(addAction))
            {
                SetWasInsertInDiffList(addAction);
                return new SQLScript(this.ToSqlAdd(), iCount, addAction);
            }
            else
                return null;

        }

        public override SQLScript Drop()
        {
            int iCount = DependenciesCount;
            if (!GetWasInsertInDiffList(dropAction))
            {
                SetWasInsertInDiffList(dropAction);
                return new SQLScript(this.ToSqlDrop(), iCount, dropAction);
            }
            else
                return null;
        }

        private static string GetObjectTypeName(Enums.ObjectType type)
        {
            if (type == Enums.ObjectType.Rule) return "RULE";
            if (type == Enums.ObjectType.Trigger) return "TRIGGER";
            if (type == Enums.ObjectType.View) return "VIEW";
            if (type == Enums.ObjectType.Function) return "FUNCTION";            
            if (type == Enums.ObjectType.StoreProcedure) return "PROCEDURE";
            if (type == Enums.ObjectType.CLRStoreProcedure) return "PROCEDURE";
            if (type == Enums.ObjectType.CLRTrigger) return "TRIGGER";
            if (type == Enums.ObjectType.CLRFunction) return "FUNCTION";
            if (type == Enums.ObjectType.Assembly) return "ASSEMBLY";
            return "";
        }

        /// <summary>
        /// Coleccion de objetos dependientes de la funcion.
        /// </summary>
        public List<String> DependenciesOut
        {
            get { return dependenciesOut; }
            set { dependenciesOut = value; }
        }

        /// <summary>
        /// Coleccion de nombres de objetos de los cuales la funcion depende.
        /// </summary>
        public List<String> DependenciesIn
        {
            get { return dependenciesIn; }
            set { dependenciesIn = value; }
        }

        public Boolean IsSchemaBinding
        {
            get { return isSchemaBinding; }
            set { isSchemaBinding = value; }
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public override int DependenciesCount
        {
            get
            {
                int iCount = 0;
                if (this.DependenciesOut.Count > 0)
                {
                    Dictionary<string, bool> depencyTracker = new Dictionary<string, bool>();
                    iCount = DependenciesCountFilter(this.FullName, depencyTracker);
                }
                return iCount;
            }
        }

        private int DependenciesCountFilter(string FullName, Dictionary<string, bool> depencyTracker)
        {
            int count = 0;
            ICode item;
            try
            {
                item = (ICode)((Database)Parent).Find(FullName);
                if (item != null)
                {
                    for (int j = 0; j < item.DependenciesOut.Count; j++)
                    {
                        if (!depencyTracker.ContainsKey(FullName.ToUpper()))
                        {
                            depencyTracker.Add(FullName.ToUpper(), true);                            
                        }
                        count += 1 + DependenciesCountFilter(item.DependenciesOut[j], depencyTracker);
                    }
                }
                return count;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Indica si existen tablas de las cuales depende el objeto que deben ser reconstruidas.
        /// </summary>
        /// <returns></returns>
        public Boolean HasToRebuild
        {
            get
            {
                for (int j = 0; j < DependenciesIn.Count; j++)
                {
                    ISchemaBase item = ((Database)Parent).Find(DependenciesIn[j]);
                    if (item != null)
                    {
                        if ((item.Status == Enums.ObjectStatusType.RebuildStatus) || (item.Status == Enums.ObjectStatusType.RebuildDependenciesStatus))
                            return true;
                    }
                };
                return IsSchemaBinding;
            }
        }

        private SQLScriptList RebuildDependencys(List<string> depends, int deepMin, int deepMax)
        {
            int newDeepMax = (deepMax != 0) ? deepMax + 1 : 0;
            int newDeepMin = (deepMin != 0) ? deepMin - 1 : 0;
            SQLScriptList list = new SQLScriptList();
            for (int j = 0; j < depends.Count; j++)
            {
                ISchemaBase item = ((Database)Parent).Find(depends[j]);
                if (item != null)
                {
                    if ((item.Status != Enums.ObjectStatusType.CreateStatus) && (item.Status != Enums.ObjectStatusType.DropStatus))
                    {
                        if ((item.ObjectType != Enums.ObjectType.CLRStoreProcedure) && (item.ObjectType != Enums.ObjectType.Assembly) && (item.ObjectType != Enums.ObjectType.UserDataType) && (item.ObjectType != Enums.ObjectType.View) && (item.ObjectType != Enums.ObjectType.Function))
                        {
                            newDeepMin = 0;
                            newDeepMax = 0;
                        }
                        if (item.Status != Enums.ObjectStatusType.DropStatus)
                        {
                            if (!((item.Parent.HasState(Enums.ObjectStatusType.RebuildStatus)) && (item.ObjectType == Enums.ObjectType.Trigger)))
                                list.Add(item.Drop(), newDeepMin);
                        }
                        if ((this.Status != Enums.ObjectStatusType.DropStatus) && (item.Status != Enums.ObjectStatusType.CreateStatus)) 
                            list.Add(item.Create(), newDeepMax);
                        if (item.IsCodeType)
                            list.AddRange(RebuildDependencys(((ICode)item).DependenciesOut, newDeepMin, newDeepMax));
                    }
                }
            };
            return list;
        }

        /// <summary>
        /// Regenera el objeto, y todos sus objetos dependientes.
        /// </summary>
        /// <returns></returns>
        public SQLScriptList Rebuild()
        {
            SQLScriptList list = new SQLScriptList();
            list.AddRange(RebuildDependencys());
            if (this.Status != Enums.ObjectStatusType.CreateStatus) list.Add(Drop(), deepMin);
            if (this.Status != Enums.ObjectStatusType.DropStatus) list.Add(Create(), deepMax);
            return list;
        }

        /// <summary>
        /// Regenera los objetos dependientes.
        /// </summary>
        /// <returns></returns>
        public SQLScriptList RebuildDependencys()
        {
            return RebuildDependencys(this.DependenciesOut, deepMin, deepMax);
        }

        public override string ToSql()
        {
            if (String.IsNullOrEmpty(sql))
                sql = FormatCode.FormatCreate(typeName, Text, this);
            return sql;
        }

        public override string ToSqlAdd()
        {
            string sql = ToSql();
            sql += this.ExtendedProperties.ToSql();
            return sql;
        }

        public override string ToSqlDrop()
        {
            return String.Format("DROP {0} {1}\r\nGO\r\n", typeName, FullName);
        }

        public virtual bool Compare(ICode obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            string sql1 = this.ToSql();
            string sql2 = obj.ToSql();
            if (((Database)RootParent).Options.Comparison.IgnoreWhiteSpacesInCode)
            {
                Regex whitespace = new Regex(@"\s");
                sql1 = whitespace.Replace(this.ToSql(), "");
                sql2 = whitespace.Replace(obj.ToSql(), "");
            }
            if (((Database)RootParent).Options.Comparison.CaseSensityInCode == Options.SqlOptionComparison.CaseSensityOptions.CaseInsensity)
                return (sql1.Equals(sql2, StringComparison.InvariantCultureIgnoreCase));

            return (sql1.Equals(sql2, StringComparison.InvariantCulture));
        }
    }
}
