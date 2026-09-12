using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using VanillaModding.Common;
using VanillaModding.Common.Systems;
using VanillaModding.Content.Items.Materials;

namespace VanillaModding.Content.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    internal class DragonMask : ModItem
    {
        public static readonly int MeleeDamagePercent = 16, MeleeSpeedPercent = MeleeDamagePercent, MeleeCritChance = 16;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MeleeDamagePercent, MeleeCritChance);
        public override void SetDefaults()
        {
            Item.width = 30; // Width of the item
            Item.height = 26; // Height of the item
            Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
            Item.rare = ItemRarityID.Pink; // The rarity of the item
            Item.defense = 26;
        }
        public override void SetStaticDefaults()
        {
            //ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false;
            Item.ResearchUnlockCount = 1;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Melee) += MeleeDamagePercent / 100;
            player.GetAttackSpeed(DamageClass.Melee) += MeleeSpeedPercent / 100;
            player.GetCritChance(DamageClass.Melee) += MeleeCritChance;

            if (player.GetModPlayer<VanillaModdingPlayer>().isDragonArmorSet)
            {
                player.dashType = DashID.ShieldOfCthulhu;
            }
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
            => (head.type == ModContent.ItemType<DragonMask>() && body.type == ModContent.ItemType<DragonBreastplate>() && legs.type == ModContent.ItemType<DragonGreaves>());

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedMask, 1)
                .AddRecipeGroup(VMRecipeGroupID.AnyCobaltBar, 10)
                .AddRecipeGroup(VMRecipeGroupID.AnyMythrilBar, 10)
                .AddRecipeGroup(VMRecipeGroupID.AnyAdamantiteBar, 10)
                .AddIngredient(ItemID.SoulofMight, 15)
                .AddIngredient(ModContent.ItemType<SoulofBlight>(), 15)
                .AddTile(TileID.MythrilAnvil)
                .SortAfterFirstRecipesOf(ItemID.HallowedGreaves)
                .Register();
        }
    }
}
