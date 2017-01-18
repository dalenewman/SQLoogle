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
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Sqloogle.Processes;

namespace Sqloogle
{
    public class SqloogleBotConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("searchIndexPath", DefaultValue = "c:\\sqloogle\\searchIndex\\", IsRequired = true)]
        public string SearchIndexPath
        {
            get
            {
                return this["searchIndexPath"] as string;
            }
        }

        [ConfigurationProperty("filePath", DefaultValue = "", IsRequired = false)]
        public string FilePath
        {
            get
            {
                return this["filePath"] as string;
            }
        }

        [ConfigurationProperty("servers")]
        public ServerElementCollection Servers
        {
            get
            {
                return this["servers"] as ServerElementCollection;
            }
        }

        [ConfigurationProperty("skips")]
        public SkipElementCollection Skips
        {
            get
            {
                return this["skips"] as SkipElementCollection;
            }
        }

        public IEnumerable<ServerCrawlProcess> ServerCrawlProcesses()
        {
            return
                (from ServerConfigurationElement server in Servers
                 select new ServerCrawlProcess(server.ConnectionString, server.Name));
        } 

        public IEnumerable<ServerMiaCrawlProcess> ServerMiaCrawlProcesses()
        {
            return
                (from ServerConfigurationElement server in Servers
                 select new ServerMiaCrawlProcess(server.ConnectionString, server.Name));            
        } 
    
    }

    public class ServerConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return this["name"] as string;
            }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("connectionString", IsRequired = true)]
        public string ConnectionString
        {
            get
            {
                return this["connectionString"] as string;
            }
            set { this["connectionString"] = value; }
        }

    }

    public class ServerElementCollection : ConfigurationElementCollection
    {
        public ServerConfigurationElement this[int index]
        {
            get
            {
                return base.BaseGet(index) as ServerConfigurationElement;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ServerConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ServerConfigurationElement)element).Name;
        }
    }

    public class SkipConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return ((string)this["name"]).ToLower(); }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("operator", IsRequired = false, DefaultValue = "Equals")]
        public string Operator
        {
            get { return ((string) this["operator"]).ToLower(); }
            set { this["operator"] = value; }
        }

    }


    public class SkipElementCollection : ConfigurationElementCollection
    {
        public SkipConfigurationElement this[int index]
        {
            get
            {
                return base.BaseGet(index) as SkipConfigurationElement;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new SkipConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SkipConfigurationElement)element).Name;
        }

        public IEnumerable<SkipConfigurationElement> ToEnumerable()
        {
            var skips = new List<SkipConfigurationElement>();
            for (int i = 0; i < this.Count; i++)
                skips.Add(this.BaseGet(i) as SkipConfigurationElement);
            return skips;
        }
 
        public IEnumerable<string> ToNames(string op)
        {
            return this.Count == 0 ? new List<string>() : this.ToEnumerable().Where(s=>s.Operator == op).Select(s => s.Name);
        }

        public string ToInExpression()
        {
            if (this.Count == 0)
                return "''";

            var databases = from string name in this.ToNames("equals") select "'" + name + "'";
            return string.Join(",", databases.ToArray());
        }

        public IEnumerable<string> ToLikeExpressions()
        {
            var databases = new List<string>();
            databases.AddRange(from string name in this.ToNames("startswith") select "'" + name + "%'");
            databases.AddRange(from string name in this.ToNames("endswith") select "'%" + name + "'");
            databases.AddRange(from string name in this.ToNames("contains") select "'%" + name + "%'");
            return databases;
        }

        public bool Match(string database)
        {
            database = database.ToLower();
            if (this.ToNames("equals").Any(database.Equals)) {
                return true;
            }

            if (this.ToNames("startswith").Any(database.StartsWith)) {
                return true;
            }

            return this.ToNames("endswith").Any(database.EndsWith) || this.ToNames("contains").Any(database.Contains);
        }
    }

}
