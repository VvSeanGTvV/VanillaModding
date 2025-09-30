using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace VanillaModding.Content.Projectiles.PrismLaser
{
    internal class PrismLaser : ModProjectile
    {
        // The width of the beam in pixels for the purposes of entity hitbox collision.
        // This gets scaled with the beam's scale value, so as the beam visually grows its hitbox gets wider as well.
        private const float BeamHitboxCollisionWidth = 22f;

        // This property encloses the internal AI variable Projectile.localAI[1].
        // Normally, localAI is not synced over the network. This beam manually syncs this variable using SendExtraAI and ReceiveExtraAI.
        private float BeamLength
        {
            get => Projectile.localAI[1];
            set => Projectile.localAI[1] = value;
        }

        // This property encloses the internal AI variable Projectile.ai[1].
        private float HostPrismIndex
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private const float OuterBeamOpacityMultiplier = 0.75f;
        private const float InnerBeamOpacityMultiplier = 0.1f;

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            // The beam itself still stops on tiles, but its invisible "source" Projectile ignores them.
            // This prevents the beams from vanishing if the player shoves the Prism into a wall.
            Projectile.tileCollide = false;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        // Send beam length over the network to prevent hitbox-affecting and thus cascading desyncs in multiplayer.
        public override void SendExtraAI(BinaryWriter writer) => writer.Write(BeamLength);
        public override void ReceiveExtraAI(BinaryReader reader) => BeamLength = reader.ReadSingle();

        public override void AI()
        {
            // If something has gone wrong with either the beam or the host Prism, destroy the beam.
            Projectile hostPrism = Main.projectile[(int)HostPrismIndex];
            if (Projectile.type != ModContent.ProjectileType<PrismLaser>() || !hostPrism.active || hostPrism.type != ModContent.ProjectileType<PrismLaser>())
            {
                Projectile.Kill();
                return;
            }

            // Grab some variables from the host Prism.
            Vector2 hostPrismDir = Vector2.Normalize(hostPrism.velocity);

            Projectile.rotation = Projectile.velocity.ToRotation();

            // This Vector2 stores the beam's hitbox statistics. X = beam length. Y = beam width.
            Vector2 beamDims = new Vector2(Projectile.velocity.Length() * BeamLength, Projectile.width * Projectile.scale);

            // Make the beam cast light along its length. The brightness of the light scales with the charge.
            // v3_1 is an unnamed decompiled variable which is the color of the light cast by DelegateMethods.CastLight
            Color beamColor = GetOuterBeamColor();
            DelegateMethods.v3_1 = beamColor.ToVector3() * 0.7f * 1;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * BeamLength, beamDims.Y, new Utils.TileActionAttempt(DelegateMethods.CastLight));
        }

        // Determines whether the specified target hitbox is intersecting with the beam.
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // If the target is touching the beam's hitbox (which is a small rectangle vaguely overlapping the host Prism), that's good enough.
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }

            // Otherwise, perform an AABB line collision check to check the whole beam.
            float _ = float.NaN;
            Vector2 beamEndPos = Projectile.Center + Projectile.velocity * BeamLength;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, beamEndPos, BeamHitboxCollisionWidth * Projectile.scale, ref _);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // If the beam doesn't have a defined direction, don't draw anything.
            if (Projectile.velocity == Vector2.Zero)
            {
                return false;
            }

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 centerFloored = Projectile.Center.Floor() + Projectile.velocity * Projectile.scale * 10.5f;
            Vector2 drawScale = new Vector2(Projectile.scale);

            // Reduce the beam length proportional to its square area to reduce block penetration.
            float visualBeamLength = BeamLength - 14.5f * Projectile.scale * Projectile.scale;

            DelegateMethods.f_1 = 1f; // f_1 is an unnamed decompiled variable whose function is unknown. Leave it at 1.
            Vector2 startPosition = centerFloored - Main.screenPosition;
            Vector2 endPosition = startPosition + Projectile.velocity * visualBeamLength;

            // Draw the outer beam.
            DrawBeam(Main.spriteBatch, texture, startPosition, endPosition, drawScale, GetOuterBeamColor() * OuterBeamOpacityMultiplier * Projectile.Opacity);

            // Draw the inner beam, which is half size.
            drawScale *= 0.5f;
            DrawBeam(Main.spriteBatch, texture, startPosition, endPosition, drawScale, GetInnerBeamColor() * InnerBeamOpacityMultiplier * Projectile.Opacity);

            // Returning false prevents Terraria from trying to draw the Projectile itself.
            return false;
        }

        // Inner beams are always pure white so that they act as a "blindingly bright" center to each laser.
        private Color GetInnerBeamColor() => Color.Red;
        private Color GetOuterBeamColor() => Color.DarkRed;

        private void DrawBeam(SpriteBatch spriteBatch, Texture2D texture, Vector2 startPosition, Vector2 endPosition, Vector2 drawScale, Color beamColor)
        {
            Utils.LaserLineFraming lineFraming = new Utils.LaserLineFraming(DelegateMethods.RainbowLaserDraw);

            // c_1 is an unnamed decompiled variable which is the render color of the beam drawn by DelegateMethods.RainbowLaserDraw.
            DelegateMethods.c_1 = beamColor;
            Utils.DrawLaser(spriteBatch, texture, startPosition, endPosition, drawScale, lineFraming);
        }
    }
}
