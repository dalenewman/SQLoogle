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
using System.ComponentModel;

namespace Sqloogle.Libs.DBDiff.Schema
{
    public static class Enums
    {
        /// <summary>
        /// OriginalStatus = El objeto no sufrio modificaciones.
        /// CreateStatus = El objeto se debe crear.
        /// DropStatus = El objeto se debe eliminar.
        /// AlterStatus = El objeto sufrio modificaciones.
        /// AlterRebuildStatus = El objeto sufrio modificaciones, pero se debe hacer un DROP y ADD.
        /// AlterPropertiesStatus = El objeto sufrio modificaciones en sus propiedades, pero no en su estructura.
        /// </summary>
        [Flags]
        public enum ObjectStatusType
        {
            OriginalStatus = 0,
            AlterStatus = 2,
            AlterBodyStatus = 4,
            RebuildStatus = 8,
            RebuildDependenciesStatus = 16,
            UpdateStatus = 32,
            CreateStatus = 64,
            DropStatus = 128,
            DisabledStatus = 256,
            ChangeOwner = 512,
            DropOlderStatus = 1024,
            BindStatus = 2048,
            PermisionSet = 4096
        }

        public enum ScripActionType
        {
            None = 0,
            UseDatabase = 1,
            AddFileGroup = 2,
            AddFile = 3,
            AlterFile = 4,
            AlterFileGroup = 5,
            UnbindRuleColumn = 6,
            UnbindRuleType = 7,
            DropRule = 8,
            AddRule = 9,
            
            DropFullTextIndex = 10,
            DropConstraintFK = 11,
            DropConstraint = 12,
            DropConstraintPK = 13,
            DropSynonyms = 14,
            DropStoreProcedure = 15,
            DropTrigger = 16,
            DropView = 17,
            DropFunction = 17,
            DropIndex = 18,            
            DropTable = 20,
            AlterColumnFormula = 21,
            AlterColumn = 22,
            AddRole = 23,
            AddUser = 24,
            AddSchema = 25,
            AddDefault = 26,                        
            AddAssembly = 27,
            AddAssemblyFile = 28,
            AddUserDataType = 29,
            AddTableType = 30,            
            AlterPartitionFunction = 31,
            AddPartitionFunction = 32,
            AddPartitionScheme = 33,         
            AddFullText = 34,
            AddXMLSchema = 35,
            AlterAssembly = 36,
            UpdateTable = 37,
            AlterTable = 38,
            AlterIndex = 39,
            AlterFullTextIndex = 40,
            AddTable = 41,
            RebuildTable = 42,
            AlterColumnRestore = 43,
            AlterColumnFormulaRestore = 44,
            AlterFunction = 45,
            AlterView = 46,
            AlterProcedure = 47,
            AddIndex = 48,                     
            AddFunction = 49,
            AddView = 49, /*AddFunction and AddView must have the same number!!!*/
            AddTrigger = 50,
            AddConstraint = 51,
            AddConstraintPK = 52,
            AddConstraintFK = 53,
            AlterConstraint = 54,
            AddFullTextIndex = 55,
            EnabledTrigger = 56,
            AddSynonyms = 57,
            AddStoreProcedure = 58,
            DropOptions = 59,
            AddOptions = 60,
            
            AlterTableChangeTracking = 61,

            DropFullText = 62,
            DropTableType = 63,
            DropUserDataType = 64,
            DropXMLSchema = 65,
            DropAssemblyUserDataType = 66,
            DropAssemblyFile = 67,
            DropAssembly = 68,
            DropDefault = 69,

            DropPartitionScheme = 70,
            DropPartitionFunction = 71,
            
            DropSchema = 72,
            DropUser = 73,
            DropRole = 74,
            DropFile = 75,
            DropFileGroup = 76,
            AddExtendedProperty = 77,
            DropExtendedProperty = 78
        }

        public enum ObjectType
        {
            None = 0,
            Table = 1,
            Column = 2,
            Trigger = 3,
            Constraint = 4,
            ConstraintColumn = 5,
            Index = 6,
            IndexColumn = 7,
            [Description("User Data Type")]
            UserDataType = 8,
            [Description("XML Schema")]
            XMLSchema = 9,
            View = 10,
            Function = 11,
            [Description("Store Procedure")]
            StoreProcedure = 12,
            TableOption = 13,
            Database = 14,
            Schema = 15,
            FileGroup = 16,
            File = 17,
            Default = 18,
            Rule = 19,
            Synonym = 20,
            Assembly = 21,
            User = 22,
            Role = 23,
            FullText = 24,
            AssemblyFile = 25,
            [Description("CLR Store Procedure")]
            CLRStoreProcedure = 26,
            [Description("CLR Trigger")]
            CLRTrigger = 27,
            [Description("CLR Function")]
            CLRFunction = 28,
            [Description("Extended Property")]
            ExtendedProperty = 30,
            Partition = 31,
            [Description("Partition Function")]
            PartitionFunction = 32,
            [Description("Partition Scheme")]
            PartitionScheme = 33,
            [Description("Table Type")]
            TableType = 34,
            FullTextIndex = 35
        }
    }
}
