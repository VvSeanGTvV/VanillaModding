using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Buffs;
using VanillaModding.Content.Projectiles.DiceProjectile;
using VanillaModding.Content.Projectiles.PhasicWarpEjector;
using VanillaModding.Content.Projectiles.Tizona;

namespace VanillaModding.Content.Items.Consumable
{
    internal class Dice : ModItem
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(16, 6));

            Item.ResearchUnlockCount = 5;
            //ItemID.Sets.SortingPriorityBossSpawns[Type] = 12; // This helps sort inventory know that this is a boss summoning Item.

            //ItemID.Sets.ItemNoGravity[Item.type] = true; // Makes the item have no gravity
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.maxStack = 1;
            Item.value = 100;
            Item.rare = ItemRarityID.Blue;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            //Item.consumable = true;

            // Gun Properties
            Item.shoot = ModContent.ProjectileType<DiceProjectile>(); // For some reason, all the guns in the vanilla source have this.
            Item.shootSpeed = 16f; // The speed of the projectile (measured in pixels per frame.)
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BuffPotion;
        }

        public override bool CanUseItem(Player player)
        {
            // If you decide to use the below UseItem code, you have to include !NPC.AnyNPCs(id), as this is also the check the server does when receiving MessageID.SpawnBoss.
            // If you want more constraints for the summon item, combine them as boolean expressions:
            //    return !Main.dayTime && !NPC.AnyNPCs(ModContent.NPCType<MinionBossBody>()); would mean "not daytime and no MinionBossBody currently alive"
            return !player.HasBuff(ModContent.BuffType<DiceBuff>());
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, player.MountedCenter, velocity, ModContent.ProjectileType<DiceProjectile>(), damage, knockback / 2, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            /*Recipe recipe = CreateRecipe();
            //recipe.AddIngredient(ItemID.MechanicalSkull, 1);
            recipe.AddIngredient(ItemID.AdamantiteBar, 30);
            recipe.AddIngredient(ItemID.SoulofSight, 20);
            recipe.AddIngredient(ItemID.SoulofFright, 20);
            recipe.AddIngredient(ModContent.ItemType<SoulofBlight>(), 20);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();*/
        }

        
    }
}
