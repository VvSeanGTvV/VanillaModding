using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Items.Materials;

namespace VanillaModding.Content.Items.Weapon.Throwable.Lobotomy
{
    public class LobotomyThrowable : ModItem
    {
        public override void SetStaticDefaults()
        {
            //ItemID.Sets.ItemsThatCountAsBombsForDemolitionistToSpawn[Type] = true;
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shootSpeed = 12f;
            Item.shoot = ModContent.ProjectileType<Projectiles.Lobotomy.LobotomyNormal>();
            Item.damage = 150;
            Item.knockBack = 40;
            Item.crit = 1;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 5;
            //Item.UseSound = LobotomyNormal.SoundEpic();
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.value = Item.sellPrice(0, 5, 25, 0);
            Item.rare = ItemRarityID.LightPurple;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            //recipe.AddIngredient(ItemID.SoulofSight, 15);
            recipe.AddIngredient(ModContent.ItemType<LogodomyShard>(), 35);
            recipe.AddIngredient(ItemID.SoulofLight, 10);
            recipe.AddIngredient(ItemID.LightDisc, 1);
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.Register();
        }
    }
}