using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Projectiles.Hammer;

namespace VanillaModding.Content.Items.Weapon.Melee
{
    internal class Pwnageddon : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 46;
            Item.damage = 140;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useAnimation = Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 10f;
            Item.UseSound = SoundID.Item1;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.value = Item.sellPrice(0, 3, 0, 0);
            Item.rare = ItemRarityID.LightPurple;
            Item.shoot = ModContent.ProjectileType<PwnageddonProj>();
            Item.shootSpeed = 22f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Pwnhammer, 1)
                .AddIngredient(ItemID.HallowedBar, 10)
                .AddIngredient(ItemID.SoulofMight, 5)
                .AddIngredient(ItemID.SoulofFright, 5)
                .AddIngredient(ItemID.SoulofSight, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
