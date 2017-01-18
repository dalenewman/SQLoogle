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
namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Generates.Util
{
    internal static class ConvertType
    {
        public static Enums.ObjectType GetObjectType(string type)
        {
            if (type.Trim().Equals("V")) return Enums.ObjectType.View;
            if (type.Trim().Equals("U")) return Enums.ObjectType.Table;
            if (type.Trim().Equals("FN")) return Enums.ObjectType.Function;
            if (type.Trim().Equals("TF")) return Enums.ObjectType.Function;
            if (type.Trim().Equals("IF")) return Enums.ObjectType.Function;
            if (type.Trim().Equals("P")) return Enums.ObjectType.StoreProcedure;
            if (type.Trim().Equals("TR")) return Enums.ObjectType.Trigger;
            return Enums.ObjectType.None;
        }
    }
}
