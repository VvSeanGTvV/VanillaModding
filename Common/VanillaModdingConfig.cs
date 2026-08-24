using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace VanillaModding.Common
{
    internal class VanillaModdingConfigClient : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Items")] // Headers are like titles in a config. You only need to declare a header on the item it should appear over, not every item in the category. 
                          // [Label("$Some.Key")] // A label is the text displayed next to the option. This should usually be a short description of what it does. By default all ModConfig fields and properties have an automatic label translation key, but modders can specify a specific translation key.
                          // [Tooltip("$Some.Key")] // A tooltip is a description showed when you hover your mouse over the option. It can be used as a more in-depth explanation of the option. Like with Label, a specific key can be provided.
        [DefaultValue(true)] 
        //[ReloadRequired]
        public bool CustomTooltip;

        [DefaultValue(true)]
        //[ReloadRequired]
        public bool CustomSpecialEffectsTooltip;
    }

    /*internal class VanillaModdingConfigServer : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

    }*/
}
