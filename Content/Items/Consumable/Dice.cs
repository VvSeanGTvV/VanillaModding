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
using Terraria.Localization;
using Terraria.ModLoader;
using VanillaModding.Common;
using VanillaModding.Common.Systems;
using VanillaModding.Content.Buffs;
using VanillaModding.Content.Projectiles.DiceProjectile;
using VanillaModding.Content.Projectiles.MightyScythe;
using VanillaModding.Content.Projectiles.PhasicWarpEjector;
using VanillaModding.Content.Projectiles.Tizona;

namespace VanillaModding.Content.Items.Consumable
{
    internal class Dice : ModItem
    {
        public static LocalizedText BadLuckDeath
        {
            get; private set;
        }

        public static LocalizedText RanOutofHealth
        {
            get; private set;
        }

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
            Item.noUseGraphic = true;

            // by default with "test" to initalize it at hand
            BadLuckDeath = this.GetLocalization("BadLuckDeath").WithFormatArgs("test");
            RanOutofHealth = this.GetLocalization("RanOutofHealth").WithFormatArgs("test");
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BuffPotion;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        bool throwable = false;
        public override bool CanUseItem(Player player)
        {
            // This is to prevent multi Dice Buff
            VanillaModdingPlayer modPlayer = player.GetModPlayer<VanillaModdingPlayer>();
            
            if (player.altFunctionUse == 2)
            {
                throwable = true;
                Item.shoot = ModContent.ProjectileType<DiceThrowableProjectile>();
                Item.damage = 1;
                return true;
            }
            else
            {
                Item.shoot = ModContent.ProjectileType<DiceProjectile>();
                Item.damage = 0;
                return !modPlayer.hasAnyDiceEffect && !modPlayer.rolling;
            }
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Auto populate with the actual player name.
            BadLuckDeath = this.GetLocalization("BadLuckDeath").WithFormatArgs(player.name);
            RanOutofHealth = this.GetLocalization("RanOutofHealth").WithFormatArgs(player.name);

            Projectile.NewProjectile(source, player.MountedCenter, velocity, type, damage, knockback / 2, player.whoAmI, throwable ? 0 : 1);
            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.ShroomiteBar, 10);
            recipe.AddIngredient(ItemID.SoulofSight, 10);
            recipe.AddIngredient(ItemID.SoulofFright, 10);
            recipe.AddIngredient(ItemID.SoulofLight, 10);
            recipe.AddIngredient(ItemID.SoulofNight, 10);
            recipe.AddIngredient(ItemID.SoulofMight, 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
