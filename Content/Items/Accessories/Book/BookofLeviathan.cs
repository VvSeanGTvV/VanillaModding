using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using VanillaModding.Content.Items.Materials;

namespace VanillaModding.Content.Items.Accessories.Book
{
    internal class BookofLeviathan : ModItem
    {
        public float damageBonus = 1.66f;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Math.Floor((damageBonus - 1f) * 1000f) / 10f);
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 4, silver: 10);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.Bleeding] = true;
            player.buffImmune[BuffID.Rabies] = true;
            player.GetDamage(DamageClass.Melee) *= damageBonus;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GoldenKey, 1)
                .AddIngredient(ModContent.ItemType<BookofLeviathanLock>(), 1)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
