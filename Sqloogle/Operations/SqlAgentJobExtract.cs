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
using System.Data;
using Rhino.Etl.Core;
using Sqloogle.Operations.Support;

namespace Sqloogle.Operations {

    public class SqlAgentJobExtract : InputOperation {

        private const string SCHEMA = "sys";
        private const string SQL_TYPE = "Sql Agent Job";
        private const string PATH = @"/SqlAgentJobs/";
        private const string DATABASE = "msdb";

        public SqlAgentJobExtract(string connectionString) : base(connectionString) {
            UseTransaction = false;
        }

        protected override Row CreateRowFromReader(IDataReader reader) {
            var row = Row.FromReader(reader);
            row["name"] = String.Format("{0} - Step {1:00} - {2}", row["jobname"], row["stepid"], row["stepname"]);
            row["schema"] = SCHEMA;
            row["type"] = SQL_TYPE;
            row["path"] = PATH;
            row["database"] = DATABASE;
            row["lastused"] = DateTime.MinValue;
            return row;
        }

        protected override void PrepareCommand(IDbCommand cmd) {
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
