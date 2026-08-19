using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;

namespace VanillaModding.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Waist)]
    internal class HealingFlower : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 44;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 1, 0, 0);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.statLife <= player.statLifeMax2 * 0.25f && player.potionDelay <= 0 && !player.HasBuff(BuffID.PotionSickness)) player.QuickHeal();
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.FlowerofFire, 1)
                .AddIngredient(ItemID.HealingPotion, 1)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
