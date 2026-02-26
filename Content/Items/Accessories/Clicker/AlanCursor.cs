using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using VanillaModding.Common;
namespace VanillaModding.Content.Items.Accessories.Clicker
{
    internal class AlanCursor : ClickerItem
    {
        int incrementMaxMinions = 2;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(incrementMaxMinions);
        public override void SetDefaults()
        {
            hasCustomCursor = true;
            Item.width = 46;
            Item.height = 52;
            Item.accessory = true;
            Item.rare = ItemRarityID.Expert;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.maxMinions += incrementMaxMinions;
            UpdateCursorStats(player);
        }
    }
}
