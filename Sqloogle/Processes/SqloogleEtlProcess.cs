/*
   Copyright 2013 Dale Newman

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/


using Sqloogle.Libs.Rhino.Etl.Core;
using Sqloogle.Libs.Rhino.Etl.Core.Operations;

namespace Sqloogle.Processes {

    /// <summary>
    /// An EtlProcess with some extra logging.
    /// </summary>
    public abstract class SqloogleEtlProcess : EtlProcess {

        protected override void OnFinishedProcessing(IOperation op) {
            Info("{0} Output {1} rows.", op.Name, op.Statistics.OutputtedRows);
            base.OnFinishedProcessing(op);
        }

        protected override void PostProcessing() {
            Info("Process Complete.");
            base.PostProcessing();
        }

        protected override void OnRowProcessed(IOperation op, Row dictionary) {
            if (op.Statistics.OutputtedRows % 1000 == 0)
                Info("Processed {0} rows in {1}", op.Statistics.OutputtedRows, op.Name);
            else {
                if (op.Statistics.OutputtedRows % 100 == 0)
                    Debug("Processed {0} rows in {1}", op.Statistics.OutputtedRows, op.Name);
                else
                    Trace("Processed {0} rows in {1}", op.Statistics.OutputtedRows, op.Name);
            }
        }

        public void ReportErrors() {
            foreach (var exception in GetAllErrors()) {
                Error(exception, "*** ERROR IN {0} PROCESS ***", Name.ToUpper());
                Info(exception.Message);
                if(exception.StackTrace != null)
                    Info(exception.StackTrace);
            }
        }
    }
}
