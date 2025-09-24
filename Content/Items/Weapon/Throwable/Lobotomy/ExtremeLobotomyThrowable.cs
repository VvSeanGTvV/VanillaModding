using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaModding.Content.Items.Weapon.Throwable.Lobotomy
{
    public class ExtremeLobotomyThrowable : ModItem
    {
        public override void SetStaticDefaults()
        {
            //ItemID.Sets.ItemsThatCountAsBombsForDemolitionistToSpawn[Type] = true;
            //Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shootSpeed = 24f;
            Item.shoot = ModContent.ProjectileType<Projectiles.Lobotomy.LobotomyExtremeDemon>();
            Item.damage = 185;
            Item.knockBack = 40;
            Item.crit = 1;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 1;
            //Item.UseSound = LobotomyNormal.SoundEpic();
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(0, 30, 50, 0);
            Item.rare = ItemRarityID.Master;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.SoulofFright, 20);
            recipe.AddIngredient(ItemID.AdamantiteBar, 35);
            recipe.AddIngredient(ItemID.AntlionMandible, 6);
            recipe.AddIngredient(ItemID.SoulofNight, 10);
            recipe.AddIngredient(ModContent.ItemType<LobotomyThrowable>(), 1);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
    }
}