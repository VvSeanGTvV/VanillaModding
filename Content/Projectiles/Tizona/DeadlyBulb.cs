using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Common.Utilities;

namespace VanillaModding.Content.Projectiles.Tizona
{
    internal class DeadlyBulb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;

            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18; // The width of projectile hitbox
            Projectile.height = 18; // The height of projectile hitbox
            Projectile.aiStyle = -1; // The ai style of the projectile, please reference the source code of Terraria
            Projectile.friendly = true;
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Melee; // Is the projectile shoot by a ranged weapon?
            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 0; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.scale = 1f;
            Projectile.extraUpdates = 1; // Set to above 0 if you want the projectile to update multiple time in a frame

            //AIType = ProjectileID.Bullet; // Act exactly like default Bullet
        }

        public override bool? CanDamage()
        => Projectile.ai[1] > 10;

        //Vector2 speedSave = //Projectile.velocity = -Vector2.Lerp(-Projectile.velocity, (Projectile.Center - closestNPC.Center).SafeNormalize(Vector2.Zero) * projSpeed, 0.1f);
        float r1;
        float light;
        int alpha;
        public override void AI()
        {
            AdvAI.FrameAnimate(0, 3, 8, Projectile);
            Lighting.AddLight(Projectile.position, Color.Yellow.ToVector3() * 0.45f);

            if (Projectile.timeLeft < 100f)
            {
                Projectile.alpha = alpha * (int)Math.Round(light * ((float)Projectile.timeLeft / 100f));
                //Projectile.light = light * ((float)Projectile.timeLeft / 100f);
            }

            NPC closestNPC = Main.npc[(int)Projectile.ai[0]].active ? Main.npc[(int)Projectile.ai[0]] : AdvAI.FindClosestNPC(512f, Projectile.Center, npc => npc.CanBeChasedBy());
            if (closestNPC == null)
                return;

            Projectile.scale = MathHelper.Clamp(Projectile.timeLeft / 300f, 0.25f, 1.75f);
            Projectile.ai[1]++;
            Projectile.ai[0] = closestNPC.whoAmI;
            Projectile.velocity = -Vector2.Lerp(-Projectile.velocity, (Projectile.Center - closestNPC.Center).SafeNormalize(Vector2.Zero) * 20f, 0.005f);

            r1 = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.Excalibur,
                new ParticleOrchestraSettings { PositionInWorld = Main.rand.NextVector2FromRectangle(target.Hitbox) }, Projectile.owner);
            hit.HitDirection = (Main.player[Projectile.owner].Center.X < target.Center.X) ? 1 : (-1);
            //if (Main.rand.NextBool(4)) target.AddBuff(BuffID.Bleeding, 720);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hit)
        {
            ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.Excalibur,
                new ParticleOrchestraSettings { PositionInWorld = Main.rand.NextVector2FromRectangle(target.Hitbox) }, Projectile.owner);
            hit.HitDirection = (Main.player[Projectile.owner].Center.X < target.Center.X) ? 1 : (-1);
            //if (Main.rand.NextBool(4)) target.AddBuff(BuffID.Bleeding, 720);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            // Calculate the height of a single frame
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            

            // Draw the custom frame
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Rectangle sourceRectangle = new Rectangle(0, (Math.Abs(Projectile.frame - k) % Main.projFrames[Projectile.type]) * frameHeight, texture.Width, frameHeight);
                Vector2 origin = sourceRectangle.Size() / 2f;

                SpriteEffects spriteEffects = Projectile.oldSpriteDirection[k] == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + origin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, sourceRectangle, color, Projectile.oldRot[k], origin, Projectile.scale, spriteEffects, 0);
            }

            // Return false to stop vanilla code from drawing the default sprite over yours
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
            => new Color(1f * 0.97f, 1f * 0.97f, 0.824f * 0.97f, 0.5f);

        public override void OnSpawn(IEntitySource source)
        {
            light = Projectile.light;
            alpha = Projectile.alpha;
        }
    }
}
