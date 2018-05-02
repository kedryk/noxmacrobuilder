using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroBuilder {
    [Serializable]
    public class NoxApp {
        public string Name { get; set; }

        public List<NoxButton> Buttons { get; set; }

        public List<NoxScreen> Screens { get; set; }

        public List<NoxMacro> Macros { get; set; }
    }
}
