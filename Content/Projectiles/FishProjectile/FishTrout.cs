using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Common.GlobalNPCs;
using VanillaModding.Common.Systems;
using VanillaModding.Content.Dusts.SMASH;
using VanillaModding.External.AI;

namespace VanillaModding.Content.Projectiles.FishProjectile
{
    internal class FishTrout : ModProjectile
    {
        private Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // The recording mode
        }

        public override void SetDefaults()
        {
            Projectile.width = 64; // Hitbox width of projectile
            Projectile.height = 64; // Hitbox height of projectile
            Projectile.friendly = true; // Projectile hits enemies
            Projectile.timeLeft = 10000; // Time it takes for projectile to expire
            Projectile.penetrate = -1; // Projectile pierces infinitely
            Projectile.tileCollide = false; // Projectile does not collide with tiles
            Projectile.usesLocalNPCImmunity = true; // Uses local immunity frames
            Projectile.localNPCHitCooldown = -1; // We set this to -1 to make sure the projectile doesn't hit twice
            Projectile.ownerHitCheck = true; // Make sure the owner of the projectile has line of sight to the target (aka can't hit things through tile).
            Projectile.DamageType = DamageClass.Melee; // Projectile is a melee projectile
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.spriteDirection = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            // Projectile.spriteDirection for this projectile is derived from the mouse position of the owner in OnSpawn, as such it needs to be synced. spriteDirection is not one of the fields automatically synced over the network. All Projectile.ai slots are used already, so we will sync it manually. 
            writer.Write((sbyte)Projectile.spriteDirection);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.spriteDirection = reader.ReadSByte();
        }

        public override void AI()
        {
            // Extend use animation until projectile is killed
            Projectile.localAI[1] += 1f / (Projectile.ai[1] / 30f);
            Projectile.localAI[0]++;

            Owner.itemAnimation = 2;
            //Owner.itemTime = 2;

            // Kill the projectile if the player dies or gets crowd controlled
            if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed || Projectile.localAI[0] >= Projectile.ai[1] / 3f)
            {
                Projectile.Kill();
                return;
            }

            float rotation = (!(Projectile.ai[0] >= 0f)) ? 0.15f + Projectile.spriteDirection * Projectile.localAI[1] : MathHelper.Pi + 0.15f + Projectile.spriteDirection * Projectile.localAI[1];
            Projectile.rotation = rotation; // Set projectile rotation

            // Set composite arm allows you to set the rotation of the arm and stretch of the front and back arms independently
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f)); // set arm position (90 degree offset since arm starts lowered)
            Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2); // get position of hand

            armPosition.Y += Owner.gfxOffY;
            Projectile.Center = armPosition + new Vector2(28, 0).RotatedBy(Projectile.rotation); // Set projectile to arm position 
            Projectile.scale = 1 * 1.2f * Owner.GetAdjustedItemScale(Owner.HeldItem); // Slightly scale up the projectile and also take into account melee size modifiers

            Owner.heldProj = Projectile.whoAmI; // set held projectile to this projectile
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects effects = (Projectile.spriteDirection > 0) ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = new(texture.Width / 2, texture.Height / 2);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                float c = k / (float)Projectile.oldPos.Length;
                float g = 0.85f - c;
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + origin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                if (g > 0.15f) Main.spriteBatch.Draw(texture, drawPos, null, color * g, Projectile.oldRot[k], origin, Projectile.scale, effects, 0f);
            }

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, default, lightColor * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, effects, 0);
            // Don't draw the projectile, the item texture is drawn in the player's hand instead
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!target.boss)
            {
                target.velocity.Y -= 10f;
                target.velocity.X += Projectile.spriteDirection * 30f;
            }
            Dust.NewDustDirect(Owner.position - new Vector2(142 / 2, Owner.height), 1, 1, ModContent.DustType<SMASH>());
            SoundEngine.PlaySound(VanillaModdingSoundID.FishHit, Projectile.position);
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.velocity.Y -= 10f;
            target.velocity.X += Projectile.spriteDirection * 30f;
            SoundEngine.PlaySound(VanillaModdingSoundID.FishHit, Projectile.position);
            Dust.NewDustDirect(Owner.position - new Vector2(142 / 2, Owner.height), 1, 1, ModContent.DustType<SMASH>());
            base.OnHitPlayer(target, info);
        }
    }
}
