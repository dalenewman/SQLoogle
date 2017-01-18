﻿#region license
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
namespace Sqloogle.Libs.DBDiff.Schema.Errors
{
    public class MessageLog
    {
        public enum LogType        
        {
            Information = 0,
            Warning = 1,
            Error = 2
        }
        private string description;
        private string fullDescription;
        private LogType type;

        public MessageLog(string description, string fullDescription, LogType type)
        {
            this.description = description;
            this.fullDescription = fullDescription;
            this.type = type;
        }

        public LogType Type
        {
            get { return type; }
        }

        public string FullDescription
        {
            get { return fullDescription; }
        }

        public string Description
        {
            get { return description; }
        }
        
    }
}
