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
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model.Interfaces;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model
{
    public class Trigger : Code
    {        
        private Boolean isDisabled;
        private Boolean insteadOf;
        private Boolean notForReplication;
        private Boolean isDDLTrigger;

        public Trigger(ISchemaBase parent)
            : base(parent, Enums.ObjectType.Trigger, Enums.ScripActionType.AddTrigger, Enums.ScripActionType.DropTrigger)
        {
            this.Parent = parent;
        }

        /// <summary>
        /// Clona el objeto en una nueva instancia.
        /// </summary>
        public override ISchemaBase Clone(ISchemaBase parent)
        {
            Trigger trigger = new Trigger(parent);
            trigger.Text = this.Text;
            trigger.Status = this.Status;
            trigger.Name = this.Name;
            trigger.IsDisabled = this.IsDisabled;
            trigger.InsteadOf = this.InsteadOf;
            trigger.NotForReplication = this.NotForReplication;
            trigger.Owner = this.Owner;
            trigger.Id = this.Id;
            trigger.IsDDLTrigger = this.IsDDLTrigger;
            trigger.Guid = this.Guid;
            return trigger;
        }

        public Boolean IsDDLTrigger
        {
            get { return isDDLTrigger; }
            set { isDDLTrigger = value; }
        }

        public Boolean InsteadOf
        {
            get { return insteadOf; }
            set { insteadOf = value; }
        }

        public Boolean IsDisabled
        {
            get { return isDisabled; }
            set { isDisabled = value; }
        }

        public Boolean NotForReplication
        {
            get { return notForReplication; }
            set { notForReplication = value; }
        }

        public override Boolean IsCodeType
        {
            get { return true; }
        }

        public override string ToSqlDrop()
        {
            if (!IsDDLTrigger)
                return "DROP TRIGGER " + FullName + "\r\nGO\r\n";
            else
                return "DROP TRIGGER " + FullName + " ON DATABASE\r\nGO\r\n";
        }

        public string ToSQLEnabledDisabled()
        {
            if (!IsDDLTrigger)
            {
                if (IsDisabled)
                    return "DISABLE TRIGGER [" + Name + "] ON " + Parent.FullName + "\r\nGO\r\n";
                else
                    return "ENABLE TRIGGER [" + Name + "] ON " + Parent.FullName + "\r\nGO\r\n";
            }
            else
            {
                if (IsDisabled)
                    return "DISABLE TRIGGER [" + Name + "]\r\nGO\r\n";
                else
                    return "ENABLE TRIGGER [" + Name + "]\r\nGO\r\n";
            }
        }

        public override SQLScriptList ToSqlDiff()
        {
            SQLScriptList list = new SQLScriptList();
            if (this.Status == Enums.ObjectStatusType.DropStatus)
                list.Add(Drop());
            if (this.Status == Enums.ObjectStatusType.CreateStatus)
                list.Add(Create());
            if (this.HasState(Enums.ObjectStatusType.AlterStatus))
                list.AddRange(Rebuild());
            if (this.HasState(Enums.ObjectStatusType.DisabledStatus))
                list.Add(this.ToSQLEnabledDisabled(), 0, Enums.ScripActionType.EnabledTrigger);
            return list;
        }

        public override bool Compare(ICode obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            if (!this.ToSql().Equals(obj.ToSql())) return false;
            if (this.InsteadOf != ((Trigger)obj).InsteadOf) return false;
            if (this.IsDisabled != ((Trigger)obj).IsDisabled) return false;
            if (this.NotForReplication != ((Trigger)obj).NotForReplication) return false;
            return true;
        }
    }
}
