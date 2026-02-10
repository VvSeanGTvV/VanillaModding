using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VanillaModding.Content.Prefixes
{
    internal class Hot : Spicy
    {
        public override float Power => base.Power + 2.15f;
        public override int tier => 1;
    }
}
