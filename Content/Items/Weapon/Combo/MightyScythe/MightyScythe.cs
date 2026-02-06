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
using VanillaModding.Content.Items.Weapon.Melee.BloodyScythe;
using VanillaModding.Content.Projectiles.MightyScythe;
using VanillaModding.Content.Projectiles.MightyScythe.MightyProjectile;

namespace VanillaModding.Content.Items.Weapon.Combo.MightyScythe
{
    public class MightyScythe : ModItem
    {
        public static short glowMask;
        public override void SetStaticDefaults()
        {
            //glowMask = GlowMaskAPI.Tools.instance.AddGlowMask(ModContent.Request<Texture2D>($"{nameof(VanillaModding)}/Items/MightyScythe_Glow", AssetRequestMode.ImmediateLoad).Value);
        }
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.VanillaModding.hjson file.

        bool onMagicMode;
        public override void SetDefaults()
        {
            Item.damage = 257;

            Item.DamageType = DamageClass.Melee;
            //Item.shootSpeed = 15f;
            //Item.shoot = ModContent.ProjectileType<MightyScythe_PROJ>();
            //Item.mana = 10;

            Item.width = 92;
            Item.height = 72;

            Item.useTime = 36;
            Item.useAnimation = 36;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.sellPrice(0, 6, 0, 0);

            Item.shootsEveryUse = true;

            Item.rare = ItemRarityID.Expert;
            Item.UseSound = SoundID.Item9;
            Item.autoReuse = true;

            Item.axe = 35;
            Item.knockBack = 20;

            Item.noUseGraphic = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.DeathSickle, 1);
            recipe.AddIngredient(ItemID.IceSickle, 1);
            recipe.AddIngredient(ModContent.ItemType<BloodyScythe>(), 1);
            //recipe.AddIngredient(ItemID.ChlorophyteBar, 15);
            recipe.AddIngredient(ItemID.LunarBar, 35);
            //recipe.AddIngredient(ItemID.MeteoriteBar, 15);
            recipe.AddIngredient(ItemID.HallowedBar, 35);
            //recipe.AddIngredient(ItemID.AdamantiteBar, 15);
            recipe.AddIngredient(ItemID.SoulofMight, 15);
            recipe.AddIngredient(ItemID.SoulofSight, 15);
            recipe.AddIngredient(ItemID.SoulofFright, 15);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
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
            if (player.altFunctionUse == 2)
            {
                Item.DamageType = DamageClass.Magic;
                Item.useTime = 60;
                Item.useAnimation = 36;
                Item.damage = 257;

                Item.axe = 0;
                Item.UseSound = SoundID.Item9;

                hehealt = true;
                Item.shoot = ModContent.ProjectileType<MightyScytheProjectile>();
            }
            else
            {
                Item.DamageType = DamageClass.Melee;
                Item.useTime = 2;
                Item.useAnimation = 15;
                Item.damage = 277;

                Item.axe = 35;
                Item.UseSound = SoundID.Item1;
                Item.pick = 210;

                hehealt = false;
                Item.shoot = ModContent.ProjectileType<MightyScytheProjectile>();

            }
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float adjustedItemScale = player.GetAdjustedItemScale(Item);
            Projectile.NewProjectile(source, player.MountedCenter, new Vector2(player.direction, 0f), type, damage, knockback, player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax * 2f, adjustedItemScale / 1.25f);
            if (hehealt) Projectile.NewProjectile(source, player.MountedCenter, new Vector2(player.direction, 0f), ModContent.ProjectileType<MightyProjectile>(), damage, knockback, player.whoAmI, Main.MouseWorld.X, Main.MouseWorld.Y);
            
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