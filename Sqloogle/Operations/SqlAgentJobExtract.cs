using System;
using System.Data;
using Sqloogle.Libs.Rhino.Etl.Core;
using Sqloogle.Operations.Support;

namespace Sqloogle.Operations {

    public class SqlAgentJobExtract : InputOperation {

        private const string SCHEMA = "sys";
        private const string SQL_TYPE = "Sql Agent Job";
        private const string PATH = @"/SqlAgentJobs/";
        private const string DATABASE = "msdb";

        public SqlAgentJobExtract(string connectionString) : base(connectionString) {}

        protected override Row CreateRowFromReader(IDataReader reader)
        {
            var row = Row.FromReader(reader);
            row["name"] = String.Format("{0} - Step {1:00} - {2}", row["jobname"], row["stepid"], row["stepname"]);
            row["schema"] = SCHEMA;
            row["type"] = SQL_TYPE;
            row["path"] = PATH;
            row["database"] = DATABASE;
            row["lastused"] = DateTime.MinValue;
            return row;
        }

        protected override void PrepareCommand(IDbCommand cmd)
        {
            cmd.CommandText = @"/* SQLoogle */

                USE msdb;

                SELECT
                    sj.name AS jobname,
                    sjs.step_id AS stepid,
                    sjs.step_name AS stepname,
                    sjs.command AS sqlscript,
                    sj.date_created AS created,
                    sj.date_modified AS modified
                FROM msdb.dbo.sysjobsteps sjs WITH (NOLOCK)
                INNER JOIN msdb.dbo.sysjobs sj WITH (NOLOCK) ON (sjs.job_id = sj.job_id)
                WHERE subsystem = 'TSQL'
                ORDER BY sj.name, sjs.step_id;
            ";
        }
    }
}
