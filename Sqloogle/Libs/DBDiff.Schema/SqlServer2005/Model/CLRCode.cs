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
    public abstract class CLRCode:Code
    {
        private Boolean isAssembly;
        private int assemblyId;
        private string assemblyClass;
        private string assemblyName;
        private string assemblyExecuteAs;
        private string assemblyMethod;

        public CLRCode(ISchemaBase parent, Enums.ObjectType type, Enums.ScripActionType addAction, Enums.ScripActionType dropAction)
            : base(parent, type, addAction, dropAction)
        {
        }

        public string AssemblyMethod
        {
            get { return assemblyMethod; }
            set { assemblyMethod = value; }
        }

        public string AssemblyExecuteAs
        {
            get { return assemblyExecuteAs; }
            set { assemblyExecuteAs = value; }
        }

        public string AssemblyName
        {
            get { return assemblyName; }
            set { assemblyName = value; }
        }

        public Boolean IsAssembly
        {
            get { return isAssembly; }
            set { isAssembly = value; }
        }

        public string AssemblyClass
        {
            get { return assemblyClass; }
            set { assemblyClass = value; }
        }

        public int AssemblyId
        {
            get { return assemblyId; }
            set { assemblyId = value; }
        }

        public override Boolean IsCodeType
        {
            get { return true; }
        }
    }
}
