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
    [AutoloadEquip(EquipType.Body)]
    internal class DragonBreastplate : ModItem
    {
        public static readonly int MeleeDamagePercent = 5, MeleeCritChance = 10;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MeleeDamagePercent, MeleeCritChance);
        public override void SetDefaults()
        {
            Item.width = 30; // Width of the item
            Item.height = 26; // Height of the item
            Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
            Item.rare = ItemRarityID.Pink; // The rarity of the item
            Item.defense = 20;
        }
        public override void SetStaticDefaults()
        {
            // HidesHands defaults to true which we don't want.
            ArmorIDs.Body.Sets.HidesHands[Item.bodySlot] = true;
            ArmorIDs.Body.Sets.HidesArms[Item.bodySlot] = true;
            //ArmorIDs.Body.Sets.HidesBottomSkin[Item.bodySlot] = true;
            ArmorIDs.Body.Sets.HidesTopSkin[Item.bodySlot] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Melee) += MeleeDamagePercent / 100;
            player.GetCritChance(DamageClass.Melee) += MeleeCritChance;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
            => (head.type == ModContent.ItemType<DragonMask>() && body.type == ModContent.ItemType<DragonBreastplate>() && legs.type == ModContent.ItemType<DragonGreaves>());

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedPlateMail, 1)
                .AddRecipeGroup(VMRecipeGroupID.AnyCobaltBar, 20)
                .AddRecipeGroup(VMRecipeGroupID.AnyMythrilBar, 20)
                .AddRecipeGroup(VMRecipeGroupID.AnyAdamantiteBar, 20)
                .AddIngredient(ItemID.SoulofMight, 15)
                .AddIngredient(ModContent.ItemType<SoulofBlight>(), 20)
                .AddTile(TileID.MythrilAnvil)
                .SortAfterFirstRecipesOf(ItemID.HallowedPlateMail)
                .Register();
        }
    }
}
