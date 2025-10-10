using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Common.Systems;
using VanillaModding.External.AI;

namespace VanillaModding.Content.Projectiles.Lobotomy
{
    public class LobotomyExtremeDemon_Enemy : ModProjectile
    {
        public override string Texture => $"{nameof(VanillaModding)}/Content/Projectiles/Lobotomy/LobotomyExtremeDemon";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 25; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // The recording mode
        }

        // Setting the default parameters of the projectile
        // You can check most of Fields and Properties here https://github.com/tModLoader/tModLoader/wiki/Projectile-Class-Documentation
        int timeLast = 600;
        public override void SetDefaults()
        {
            Projectile.width = 42; // The width of projectile hitbox
            Projectile.height = 38; // The height of projectile hitbox

            Projectile.aiStyle = 0; // The ai style of the projectile (0 means custom AI). For more please reference the source code of Terraria
            Projectile.DamageType = DamageClass.Ranged; // What type of damage does this projectile affect?
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = true; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.light = 1f; // How much light emit around the projectile
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.timeLeft = timeLast; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)


        }

        // Custom AI
        public override void AI()
        {
            float maxDetectRadius = 960f; // The maximum radius at which a projectile can detect a target
            float projSpeed = 40f; // The speed at which the projectile moves towards the target

            // Trying to find NPC closest to the projectile
            NPC closestNPC = null;
            Player closestPlayer = null;
            bool isEnemy = Projectile.hostile;
            if (isEnemy)
                closestPlayer = AdvAI.FindClosestPlayer(maxDetectRadius, Projectile);
            else
                closestNPC = AdvAI.FindClosestNPC(maxDetectRadius, Projectile);

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if ((closestNPC == null && !isEnemy) || (closestPlayer == null && isEnemy))
                return;

            Vector2 target = (isEnemy) ? closestPlayer.Center : closestNPC.Center;
            Projectile.velocity = -Vector2.Lerp(-Projectile.velocity, (Projectile.Center - target).SafeNormalize(Vector2.Zero) * projSpeed, 0.05f);


        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //base.OnHitNPC(target, hit, damageDone);
            //Projectile.damage += 10;
            explodeLobotomy();
        }

        public void explodeLobotomy()
        {
            var position = Projectile.position;
            var speedX = Projectile.velocity.X;
            var speedY = Projectile.velocity.Y;
            float speedMul = 2f;
            float numberProjectiles = 3; // 3 shots
            float rotation = MathHelper.ToRadians(45);//Shoots them in a 45 degree radius. (This is technically 90 degrees because it's 45 degrees up from your cursor and 45 degrees down)
            position += Vector2.Normalize(new Vector2(speedX, speedY)) * 45f; //45 should equal whatever number you had on the previous line
            var enS = Projectile.GetSource_FromThis();
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f; // Vector for spread. Watch out for dividing by 0 if there is only 1 projectile.
                Projectile.NewProjectile(enS, new Vector2(position.X, position.Y), new Vector2(perturbedSpeed.X, perturbedSpeed.Y) * speedMul, ModContent.ProjectileType<LobotomyNormal>(), Projectile.damage / 2, Projectile.damage / 2, Projectile.owner); //Creates a new projectile with our new vector for spread.
            }
        }

        public override void OnKill(int timeLeft)
        {
            //SoundEngine.PlaySound(VanillaModdingSoundID.FireInTheHole, Projectile.position);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            String pathGlow = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "_Glow").Replace(@"\", "/");
            String pathGlowTrail = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "_Glow_Trail").Replace(@"\", "/");

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glow = (Texture2D)ModContent.Request<Texture2D>($"{pathGlow}", AssetRequestMode.ImmediateLoad).Value;
            Texture2D glowTrail = (Texture2D)ModContent.Request<Texture2D>($"{pathGlowTrail}", AssetRequestMode.ImmediateLoad).Value;
            Vector2 origin = new(texture.Width / 2, texture.Height / 2);
            Vector2 originGlow = new(glow.Width / 2, glow.Height / 2);

            Vector2 position = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(glow, position, null, new Color(255, 0, 0, 0), 0f, originGlow, Projectile.scale - 0.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(glow, position, null, new Color(255, 50, 50, 0), 0f, originGlow, Projectile.scale - 0.75f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(glow, position, null, new Color(255, 250, 250, 0), 0f, originGlow, Projectile.scale - 0.8f, SpriteEffects.None, 0f);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                float c = k / (float)Projectile.oldPos.Length;
                float g = 1f - c;
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + origin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.spriteBatch.Draw(glowTrail, drawPos, null, new Color(255, 50, 50, 0) * g, Projectile.oldRot[k] + MathHelper.PiOver2, originGlow, Projectile.scale - 0.75f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(glowTrail, drawPos, null, new Color(255, 250, 250, 0) * g, Projectile.oldRot[k] + MathHelper.PiOver2, originGlow, Projectile.scale - 0.9f, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(texture, position, null, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f); // Main Body
            
            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(VanillaModdingSoundID.FireInTheHole, Projectile.position);
        }
    }
}