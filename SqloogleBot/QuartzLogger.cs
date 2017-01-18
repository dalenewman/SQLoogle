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
using Common.Logging.Simple;
using NLog;
using LogLevel = Common.Logging.LogLevel;

namespace SqloogleBot {
    public class QuartzLogger : AbstractSimpleLogger {
        private readonly Logger _logger;

        public QuartzLogger(LogLevel level, bool showLevel, bool showDateTime, bool showLogName, string dateTimeFormat = "o")
            : base("SQLoogleBot", level, showLevel, showDateTime, showLogName, dateTimeFormat) {
            _logger = LogManager.GetLogger("SQLoogleBot");
        }

        protected override void WriteInternal(LogLevel level, object message, Exception exception) {
            switch (level) {
                case LogLevel.All:
                    _logger.Debug(message.ToString);
                    break;
                case LogLevel.Debug:
                    _logger.Debug(message.ToString);
                    break;
                case LogLevel.Error:
                    if (exception == null) {
                        _logger.Error(message.ToString());
                    } else {
                        _logger.Error(exception, message.ToString());
                    }
                    break;
                case LogLevel.Fatal:
                    if (exception == null) {
                        _logger.Error(message.ToString());
                    } else {
                        _logger.Error(exception, message.ToString());
                    }
                    break;
                case LogLevel.Off:
                    break;
                case LogLevel.Trace:
                    _logger.Debug(message.ToString);
                    break;
                case LogLevel.Warn:
                    _logger.Warn(message.ToString());
                    break;
                case LogLevel.Info:
                    _logger.Info(message.ToString());
                    break;
                default:
                    _logger.Info(message.ToString());
                    break;
            }
        }
    }
}
