using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using VanillaModding.Common;

namespace VanillaModding.Content.InfoDisplays
{
    internal class ClickInfoDisplay : InfoDisplay
    {
        public static LocalizedText CPS { get; private set; }
        public static LocalizedText noCPS { get; private set; }
        public override void SetStaticDefaults()
        {
            CPS = this.GetLocalization("CPS");
            noCPS = this.GetLocalization("noCPS");
        }

        public override string HoverTexture => Texture + "_Hover";

        public override bool Active()
        {
            return Main.LocalPlayer.GetModPlayer<VanillaModdingPlayer>().showClicksTotal;
        }

        public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor)
        {
            bool noInfo = Main.LocalPlayer.GetModPlayer<VanillaModdingPlayer>().clicksPerSecond <= 0;
            if (noInfo) displayColor = InactiveInfoTextColor;

            return noInfo ? (string)noCPS : (string)CPS.WithFormatArgs(Main.LocalPlayer.GetModPlayer<VanillaModdingPlayer>().clicksPerSecond);
        }
    }
}
