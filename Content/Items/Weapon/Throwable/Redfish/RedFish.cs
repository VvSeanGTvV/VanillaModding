using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaModding.Content.Items.Weapon.Throwable.Redfish
{
    internal class RedFish : ModItem
    {
        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shootSpeed = 12f;
            Item.shoot = ModContent.ProjectileType<Projectiles.FishProjectile.RedFish>();
            Item.damage = 5;
            Item.knockBack = int.MaxValue;
            Item.crit = 1;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.useAnimation = 15;
            Item.useTime = 30;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(0, 1, 25, 0);
            Item.rare = ItemRarityID.Red;
        }
    }
}
