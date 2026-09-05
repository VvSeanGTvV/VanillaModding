using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using VanillaModding.Content.Items.Materials;
using VanillaModding.Content.Rarities;

namespace VanillaModding.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Shoes)]
    internal class LunarBoots : ModItem
    {
        public float speedBoost = 0.125f;
        public int lavaMax = 420;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(speedBoost*100, lavaMax / 60);

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 38;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<BrightTurquoise>();
            Item.value = Item.buyPrice(0, 25, 0, 0);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.waterWalk = true;
            player.fireWalk = true;
            player.lavaMax += lavaMax;
            player.lavaRose = true;

            player.accRunSpeed = 10.925f;
            player.rocketBoots = (player.vanityRocketBoots = 4);
            player.moveSpeed += speedBoost;
            player.iceSkate = true;

            player.autoJump = true;
            player.jumpSpeedBoost += 1.6f;
            player.noFallDmg = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.TerrasparkBoots)
                .AddIngredient(ItemID.FrogLeg)
                .AddIngredient(ItemID.LuckyHorseshoe)
                .AddIngredient<SoulofUnity>(15)
                .AddIngredient(ItemID.LunarBar, 12)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
