using System.Collections.Generic;

namespace Animator_Enum_Codegen.Runtime
{
    public class StatesData
    {
        public readonly List<string> Names;
        public readonly List<int>    Hashes;
        public readonly List<float>  Durations;
        
        public StatesData(List<string> names, List<int> hashes, List<float> durations)
        {
            Names     = names;
            Hashes    = hashes;
            Durations = durations;
        }
    }
}