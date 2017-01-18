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

namespace Sqloogle.Libs.DBDiff.Schema.Events
{
    public class ProgressEventArgs:EventArgs 
    {
        private int progress;
        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public ProgressEventArgs(string message, int progress)
        {
            this.progress = progress;
            this.message = message;
        }

        public int Progress
        {
            get { return progress; }
            set { progress = value; }
        }
    }
}
