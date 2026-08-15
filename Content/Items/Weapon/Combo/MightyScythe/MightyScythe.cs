using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Items.Materials.Bars;
using VanillaModding.Content.Items.Weapon.Melee.BloodyScythe;
using VanillaModding.Content.Projectiles.MightyScythe;
using VanillaModding.Content.Projectiles.MightyScythe.MightyProjectile;
using VanillaModding.Content.Rarities;

namespace VanillaModding.Content.Items.Weapon.Combo.MightyScythe
{
    public class MightyScythe : ModItem
    {
        public static short glowMask;
        public override void SetStaticDefaults()
        {
            VanillaModdingSystem.Sickle[Type] = new VanillaModdingSystem.SickleData(true, 2, 4, 4, 6);
        }


        // just a method to consolidate the default stats of the item, so we can easily switch between the two modes
        public void SetDefaultStats()
        {
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 5;
            Item.useAnimation = 15;
            Item.damage = 180;

            Item.axe = 26;
            Item.UseSound = SoundID.Item1;
            Item.pick = 210;

            hehealt = false;
            Item.shoot = ModContent.ProjectileType<MightyScytheProjectile>();
            Item.tileBoost = 2;
        }

        public override void SetDefaults()
        {
            Item.damage = 257;

            Item.DamageType = DamageClass.Melee;
            //Item.shootSpeed = 15f;
            //Item.shoot = ModContent.ProjectileType<MightyScythe_PROJ>();
            //Item.mana = 10;

            Item.width = 92;
            Item.height = 72;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.sellPrice(0, 6, 0, 0);

            Item.shootsEveryUse = true;

            Item.rare = ModContent.RarityType<BrightTurquoise>();
            Item.autoReuse = true;

            Item.noUseGraphic = true;
            SetDefaultStats();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Picksaw, 1)
                .AddIngredient(ItemID.DeathSickle, 1)
                .AddIngredient(ItemID.IceSickle, 1)
                .AddIngredient(ItemID.Sickle, 1)
                .AddIngredient(ModContent.ItemType<BloodyScythe>(), 1)
                .AddIngredient(ItemID.LunarBar, 25)
                .AddIngredient(ModContent.ItemType<ElectrifiedBar>(), 10)
                .AddIngredient(ItemID.SoulofMight, 15)
                .AddIngredient(ItemID.SoulofSight, 15)
                .AddIngredient(ItemID.SoulofFright, 15)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D texture = ModContent.Request<Texture2D>($"{nameof(VanillaModding)}/Content/Items/Weapon/Combo/MightyScythe/MightyScythe_Glow", AssetRequestMode.ImmediateLoad).Value;
            spriteBatch.Draw
            (
                texture,
                new Vector2
                (
                    Item.position.X - Main.screenPosition.X + Item.width * 0.5f,
                    Item.position.Y - Main.screenPosition.Y + Item.height - texture.Height * 0.5f + 2f
                ),
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.White,
                rotation,
                texture.Size() * 0.5f,
                scale,
                SpriteEffects.None,
                0f
            );
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        bool hehealt;
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2) // Projectile
            {
                Item.DamageType = DamageClass.Melee;
                Item.useTime = 60;
                Item.useAnimation = 36;
                Item.damage = 0;

                Item.pick = 0;
                Item.axe = 0;
                Item.UseSound = SoundID.Item9;

                hehealt = true;
                Item.shoot = ModContent.ProjectileType<MightyScytheProjectile>();
            }
            else
            {
                SetDefaultStats();
            }
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float adjustedItemScale = player.GetAdjustedItemScale(Item);
            Projectile.NewProjectile(source, player.MountedCenter, new Vector2(player.direction, 0f), type, damage, knockback, player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax * 2f, adjustedItemScale / 1.25f);
            if (hehealt) Projectile.NewProjectile(source, player.MountedCenter, new Vector2(player.direction, 0f), ModContent.ProjectileType<MightyProjectile>(), 320, 0f, player.whoAmI, Main.MouseWorld.X, Main.MouseWorld.Y);
            
            return false;
        }

        public override bool MeleePrefix()
        => true;
        public override bool MagicPrefix()
        => true;
        public override bool RangedPrefix()
        => false;
    }
}