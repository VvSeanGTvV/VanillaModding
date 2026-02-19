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

namespace VanillaModding.Content.Items.Accessories.Book
{
    internal class BookSatanicBible : ModItem
    {
        public float damageBonus = 1.66f;
        public int forcedHealth = 2;
        public int defenseBonus = 5;
        public int manaBonus = 20;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Math.Floor((damageBonus - 1f) * 1000f) / 10f, defenseBonus, forcedHealth, manaBonus);
        public LocalizedText defaultTooltip;
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;
            Item.accessory = true;

            Item.value = Item.sellPrice(gold: 10, silver: 95);
            Item.rare = ItemRarityID.Yellow;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statDefense += (player.statLifeMax / 10) + defenseBonus;
            player.buffImmune[BuffID.Bleeding] = true;
            player.buffImmune[BuffID.Rabies] = true;
            player.GetDamage(DamageClass.Generic) *= (damageBonus + (player.statLifeMax / 10));

            player.statManaMax2 += (player.statLifeMax / 10) + manaBonus;
            player.statLifeMax2 -= (player.statLifeMax - forcedHealth);
            VanillaModdingPlayer VMP = player.GetModPlayer<VanillaModdingPlayer>();
            VMP.accSatanicBible = true;
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            if (
                equippedItem.type == ModContent.ItemType<BookofLeviathan>() ||
                equippedItem.type == ModContent.ItemType<BookofBelial>()
                ) return false;
            return base.CanAccessoryBeEquippedWith(equippedItem, incomingItem, player);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DemonScythe, 1)
                .AddIngredient(ItemID.CursedFlames, 1)
                .AddIngredient(ModContent.ItemType<BookofBelial>(), 1)
                .AddIngredient(ModContent.ItemType<BookofLeviathan>(), 1)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
