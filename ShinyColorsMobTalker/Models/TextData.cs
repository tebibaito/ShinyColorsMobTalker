using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShinyColorsMobTalker.Models
{
    internal class TextData
    {
        public String speaker;

        public String text;

        public TextData(String speaker, String text)
        {
            this.speaker = speaker;
            this.text = text;
        }

    }
}
