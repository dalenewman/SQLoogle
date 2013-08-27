﻿namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Generates.Util
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
