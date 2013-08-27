using System.Collections.Generic;

namespace Sqloogle.Search
{
    public interface IScriptSearcher {
        IEnumerable<IDictionary<string, string>> Search(string query);
        IDictionary<string, string> Find(string id); 
        void Close();
    }
}
