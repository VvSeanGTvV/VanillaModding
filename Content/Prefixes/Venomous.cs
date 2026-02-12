using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace VanillaModding.Content.Prefixes
{
    internal class Venomous : Spiky
    {
        public override float Power => base.Power + 0.15f;
    }
}
