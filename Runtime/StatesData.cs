using System.Collections.Generic;

namespace Animator_Enum_Codegen.Runtime
{
    public class StatesData
    {
        public readonly List<string> Names;
        public readonly List<int>    Hashes;
        
        public StatesData(List<string> names, List<int> hashes)
        {
            Names  = names;
            Hashes = hashes;
        }
    }
}