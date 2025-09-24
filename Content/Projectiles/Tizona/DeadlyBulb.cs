using Microsoft.Xna.Framework;
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
using VanillaModding.External.AI;

namespace VanillaModding.Content.Projectiles.Tizona
{
    internal class DeadlyBulb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10; // The width of projectile hitbox
            Projectile.height = 10; // The height of projectile hitbox
            Projectile.aiStyle = -1; // The ai style of the projectile, please reference the source code of Terraria
            Projectile.friendly = true;
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Melee; // Is the projectile shoot by a ranged weapon?
            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 0; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.scale = 2f;
            Projectile.extraUpdates = 1; // Set to above 0 if you want the projectile to update multiple time in a frame

            //AIType = ProjectileID.Bullet; // Act exactly like default Bullet
        }

        //Vector2 speedSave = //Projectile.velocity = -Vector2.Lerp(-Projectile.velocity, (Projectile.Center - closestNPC.Center).SafeNormalize(Vector2.Zero) * projSpeed, 0.1f);
        float r1;
        float light;
        int alpha;
        public override void AI()
        {
            AdvAI.FrameAnimate(0, 3, 8, Projectile);
            Lighting.AddLight(Projectile.position, 1f * 0.87f, 1f * 0.87f, 0.824f * 0.87f);

            if (Projectile.timeLeft < 100f)
            {
                Projectile.alpha = alpha * (int)Math.Round(light * ((float)Projectile.timeLeft / 100f));
                Projectile.light = light * ((float)Projectile.timeLeft / 100f);
            }

            if (Projectile.ai[0] != 0)
            {
                Projectile.scale = 1.5f;
                Projectile.light = 0.5f;
                if (Projectile.ai[0] < 0) 
                {
                    NPC closestNPC = AdvAI.FindClosestNPC(512f, Projectile);
                    if (closestNPC == null)
                        return;
                    Projectile.velocity = -Vector2.Lerp(-Projectile.velocity, (Projectile.Center - closestNPC.Center).SafeNormalize(Vector2.Zero) * 20f, 0.0025f);
                    return;
                };
                NPC target = Main.npc[(int)Projectile.ai[1]];
                Projectile.velocity = -Vector2.Lerp(-Projectile.velocity, (Projectile.Center - target.Center).SafeNormalize(Vector2.Zero) * 20f, 0.005f);
            }
            else
            {
                NPC closestNPC = AdvAI.FindClosestNPC(512f, Projectile);
                if (closestNPC == null)
                    return;
                Projectile.velocity = -Vector2.Lerp(-Projectile.velocity, (Projectile.Center - closestNPC.Center).SafeNormalize(Vector2.Zero) * 20f, 0.0025f);
            }

            r1 = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.rotation = r1;
                explodeBulb(target);
            }
            else
            {
                ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.Excalibur,
                new ParticleOrchestraSettings { PositionInWorld = Main.rand.NextVector2FromRectangle(target.Hitbox) }, Projectile.owner);
                hit.HitDirection = (Main.player[Projectile.owner].Center.X < target.Center.X) ? 1 : (-1);
                target.AddBuff(BuffID.Bleeding, 720);
            }
        }

        private void explodeBulb(NPC target)
        {
            var position = Projectile.position;
            var speedX = Projectile.velocity.X;
            var speedY = Projectile.velocity.Y;
            float speedMul = 4f;
            float numberProjectiles = 3; // 3 shots
            float rotation = MathHelper.ToRadians(45);//Shoots them in a 45 degree radius. (This is technically 90 degrees because it's 45 degrees up from your cursor and 45 degrees down)
            position += Vector2.Normalize(new Vector2(speedX, speedY)) * 45f; //45 should equal whatever number you had on the previous line
            var enS = Projectile.GetSource_FromThis();
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f; // Vector for spread. Watch out for dividing by 0 if there is only 1 projectile.
                Projectile.NewProjectile(enS, position, perturbedSpeed * speedMul, ModContent.ProjectileType<DeadlyBulb>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1, target.whoAmI); //Creates a new projectile with our new vector for spread.
                
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            light = Projectile.light;
            alpha = Projectile.alpha;
        }
    }
}
