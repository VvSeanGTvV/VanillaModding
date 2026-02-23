using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace VanillaModding.Content.Items.Accessories.Clicker
{
    internal class Mouse : ModItem
    {
        // This is the main hook that allows for our info display to actually work with this accessory. 
        public override void UpdateInfoAccessory(Player player)
        {
            player.GetModPlayer<ExampleInfoDisplayPlayer>().showMinionCount = true;
        }
    }
}
