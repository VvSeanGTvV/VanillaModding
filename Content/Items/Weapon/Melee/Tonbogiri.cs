using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Items.SoulofEssence;
using VanillaModding.Content.Projectiles.Tonbogiri;

namespace VanillaModding.Content.Items.Weapon.Melee
{
    public class Tonbogiri : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {

            // Common Properties
            Item.rare = ItemRarityID.Pink; // Assign this item a rarity level of Pink
            Item.value = Item.sellPrice(gold: 40); // The number and type of coins item can be sold for to an NPC

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot; // How you use the item (swinging, holding out, etc.)
            Item.useAnimation = 15; // The length of the item's use animation in ticks (60 ticks == 1 second.)
            Item.useTime = 17; // The length of the item's use time in ticks (60 ticks == 1 second.)
            Item.UseSound = SoundID.Item71; // The sound that this item plays when used.
            Item.autoReuse = true; // Allows the player to hold click to automatically use the item again. Most spears don't autoReuse, but it's possible when used in conjunction with CanUseItem()

            // Weapon Properties
            Item.damage = 50;
            Item.knockBack = 6.5f;
            Item.noUseGraphic = true; // When true, the item's sprite will not be visible while the item is in use. This is true because the spear projectile is what's shown so we do not want to show the spear sprite as well.
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true; // Allows the item's animation to do damage. This is important because the spear is actually a projectile instead of an item. This prevents the melee hitbox of this item.

            // Projectile Properties
            Item.shootSpeed = 5.6f; // The speed of the projectile measured in pixels per frame.
            Item.shoot = ModContent.ProjectileType<TonbogiriProjectile>(); // The projectile that is fired from this weapon
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.AdamantiteBar, 15);
            recipe.AddIngredient(ModContent.ItemType<SoulofBlight>(), 15);
            recipe.AddIngredient(ItemID.Gungnir, 1);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();

            Recipe recipe1 = CreateRecipe();
            recipe1.AddIngredient(ItemID.TitaniumBar, 15);
            recipe1.AddIngredient(ModContent.ItemType<SoulofBlight>(), 15);
            recipe1.AddIngredient(ItemID.Gungnir, 1);
            recipe1.AddTile(TileID.MythrilAnvil);
            recipe1.Register();
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Poisoned, 720);
            base.OnHitNPC(player, target, hit, damageDone);
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.Bleeding, 720);
            base.OnHitPvp(player, target, hurtInfo);
        }

        public override bool CanUseItem(Player player)
        {
            // Ensures no more than one spear can be thrown out, use this when using autoReuse
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override bool? UseItem(Player player)
        {
            // Because we're skipping sound playback on use animation start, we have to play it ourselves whenever the item is actually used.
            if (!Main.dedServ && Item.UseSound.HasValue)
            {
                SoundEngine.PlaySound(Item.UseSound.Value, player.Center);
            }

            return null;
        }
    }
}