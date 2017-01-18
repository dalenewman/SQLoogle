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

namespace Sqloogle.Libs.DBDiff.Schema
{
    public class SQLScript : IComparable<SQLScript>
    {
        private string sql;
        private int dependencies;
        private Enums.ScripActionType status;
        private int deep;

        public SQLScript(int deepvalue, string sqlScript, int dependenciesCount, Enums.ScripActionType action)
        {
            sql = sqlScript;
            dependencies = dependenciesCount;
            status = action;
            deep = deepvalue;
            //childs = new SQLScriptList();
        }

        public SQLScript(string sqlScript, int dependenciesCount, Enums.ScripActionType action)
        {
            sql = sqlScript;
            dependencies = dependenciesCount;
            status = action;
            //childs = new SQLScriptList();
        }

        /*public SQLScriptList Childs
        {
            get { return childs; }
            set { childs = value; }
        }*/

        public int Deep
        {
            get { return deep; }
            set { deep = value; }
        }

        public Enums.ScripActionType Status
        {
            get { return status; }
            set { status = value; }
        }

        public int Dependencies
        {
            get { return dependencies; }
            set { dependencies = value; }
        }

        public string SQL
        {
            get { return sql; }
            set { sql = value; }
        }

        public bool IsDropAction
        {
            get
            {
                return ((status == Enums.ScripActionType.DropView) || (status == Enums.ScripActionType.DropFunction)|| (status == Enums.ScripActionType.DropStoreProcedure));
            }
        }

        public bool IsAddAction
        {
            get
            {
                return ((status == Enums.ScripActionType.AddView) || (status == Enums.ScripActionType.AddFunction) || (status == Enums.ScripActionType.AddStoreProcedure));
            }
        }

        public int CompareTo(SQLScript other)
        {
            if (this.deep == other.deep)
            {
                if (this.Status == other.Status)
                {
                    if (this.Status == Enums.ScripActionType.DropTable || this.Status == Enums.ScripActionType.DropConstraint || this.Status == Enums.ScripActionType.DropTrigger)
                        return other.Dependencies.CompareTo(this.Dependencies);
                    else
                        return this.Dependencies.CompareTo(other.Dependencies);
                }
                else
                    return this.Status.CompareTo(other.Status);
            }
            else
                return this.Deep.CompareTo(other.Deep);
        }
    }
}
