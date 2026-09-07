using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using VanillaModding.Content.Projectiles.DirtiestSword;
using VanillaModding.Content.Rarities;

namespace VanillaModding.Content.Items.Weapon.Melee
{
    public class DirtiestSword : ModItem
    {
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.VanillaModding.hjson file.
        public override void SetDefaults()
        {
            Item.damage = 1;
            Item.DamageType = DamageClass.Melee;
            Item.width = 62;
            Item.height = 62;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 0;
            Item.value = int.MaxValue;
            Item.rare = ModContent.RarityType<HoloRainbow>();
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        private void DirtExplodeEffect(Player player, Entity target)
        {
            for (int i = 0; i < 25; i++) Dust.NewDustDirect(target.Center, target.width, target.height, DustID.Dirt, Main.rand.NextFloat(-2f, 2f), -8f, 100, default, Main.rand.NextFloat(0.5f, 2f));
            for (int i = 0; i < 8; i++) Dust.NewDustDirect(target.Center, target.width, target.height, DustID.Smoke, Main.rand.NextFloat(-2f, 2f), -8f, 100, default, Main.rand.NextFloat(0.25f, 1f));
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, target.Center);
            //Projectile.NewProjectile(player.GetSource_ItemUse(Item), target.Center, Vector2.Zero, ModContent.ProjectileType<DirtiestExplode>(), 0, 0f, player.whoAmI);
        }

        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            
            modifiers.DefenseEffectiveness *= 0;
            modifiers.SetInstantKill();
            DirtExplodeEffect(player, target);
        }

        public override void ModifyHitPvp(Player player, Player target, ref Player.HurtModifiers modifiers)
        {
            target.statLife = 1;
            modifiers.FinalDamage.Base = !target.immune && !target.dead ? target.statLife : 0;
            DirtExplodeEffect(player, target);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DirtiestBlock, 2)
                .AddIngredient(ItemID.DirtBlock, 9999)
                .AddIngredient(ItemID.GrassSeeds, 4999)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}