using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaModding.Items.Weapon.Melee
{
    public class BasicSword : ModItem
    {
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.VanillaModding.hjson file.

        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = 1;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.White;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe0 = CreateRecipe();
            recipe0.AddIngredient(ItemID.DirtBlock, 15);
            recipe0.AddIngredient(ItemID.SilverBar, 4);
            recipe0.AddTile(TileID.WorkBenches);
            recipe0.Register();
        }
    }
}