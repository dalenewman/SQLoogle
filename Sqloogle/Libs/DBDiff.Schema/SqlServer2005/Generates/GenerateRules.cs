using System.Data.SqlClient;
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Generates
{
    public class GenerateRules
    {
        private Generate root;

        public GenerateRules(Generate root)
        {
            this.root = root;
        }

        private static string GetSQL()
        {
            string sql = "select obj.object_id, Name, SCHEMA_NAME(obj.schema_id) AS Owner, ISNULL(smobj.definition, ssmobj.definition) AS [Definition] from sys.objects obj  ";
            sql += "LEFT OUTER JOIN sys.sql_modules AS smobj ON smobj.object_id = obj.object_id ";
            sql += "LEFT OUTER JOIN sys.system_sql_modules AS ssmobj ON ssmobj.object_id = obj.object_id ";
            sql += "where obj.type='R'";
            return sql;
        }

        public void Fill(Database database, string connectionString)
        {
            if (database.Options.Ignore.FilterRules)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(GetSQL(), conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Rule item = new Rule(database);
                                item.Id = (int)reader["object_id"];
                                item.Name = reader["Name"].ToString();
                                item.Owner = reader["Owner"].ToString();
                                item.Text = reader["Definition"].ToString();
                                database.Rules.Add(item);
                            }
                        }
                    }
                }
            }
        }
    }
}
