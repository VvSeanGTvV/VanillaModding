using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using VanillaModding.Common.Systems;
using VanillaModding.Content.Items.Materials;

namespace VanillaModding.Content.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    internal class DragonGreaves : ModItem
    {
        public static readonly int SpeedBoost = 12, MeleeSpeedPercent = 2;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SpeedBoost, MeleeSpeedPercent);
        public override void SetDefaults()
        {
            Item.width = 22; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(gold: 15); // How many coins the item is worth
            Item.rare = ItemRarityID.Pink; // The rarity of the item
            Item.defense = 14;
        }
        public override void SetStaticDefaults()
        {
            ArmorIDs.Legs.Sets.HidesBottomSkin[Item.legSlot] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += SpeedBoost / 100f;
            player.GetAttackSpeed(DamageClass.Melee) += MeleeSpeedPercent / 100;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
            => (head.type == ModContent.ItemType<DragonMask>() && body.type == ModContent.ItemType<DragonBreastplate>() && legs.type == ModContent.ItemType<DragonGreaves>());

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedGreaves, 1)
                .AddRecipeGroup(VMRecipeGroupID.AnyCobaltBar, 15)
                .AddRecipeGroup(VMRecipeGroupID.AnyMythrilBar, 15)
                .AddRecipeGroup(VMRecipeGroupID.AnyAdamantiteBar, 15)
                .AddIngredient(ItemID.SoulofMight, 15)
                .AddIngredient(ModContent.ItemType<SoulofBlight>(), 10)
                .AddTile(TileID.MythrilAnvil)
                .SortAfterFirstRecipesOf(ItemID.HallowedGreaves)
                .Register();
        }
    }
}
