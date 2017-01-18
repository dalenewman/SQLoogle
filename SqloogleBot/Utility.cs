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
using System.Linq;
using Common.Logging;

namespace SqloogleBot {
    public static class Utility {

        public static LogLevel GetConsoleLogLevel() {
            var logLevel = LogLevel.Info;

            var target = NLog.LogManager.Configuration.FindTargetByName("console");
            if (target == null)
                return logLevel;

            var rule = NLog.LogManager.Configuration.LoggingRules.FirstOrDefault(r => r.Targets.Contains(target));
            var level = rule?.Levels.FirstOrDefault();
            if (level != null) {
                logLevel = ConvertLevel(level);
            }

            return logLevel;

        }
        private static LogLevel ConvertLevel(NLog.LogLevel level) {
            switch (level.Name) {
                case "Debug":
                    return LogLevel.Debug;
                case "Info":
                    return LogLevel.Info;
                case "Warn":
                    return LogLevel.Warn;
                case "Error":
                    return LogLevel.Error;
                case "Off":
                    return LogLevel.Off;
                default:
                    return LogLevel.Info;
            }
        }
    }
}
