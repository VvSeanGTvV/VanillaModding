using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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
using VanillaModding.Common.Systems;
using VanillaModding.Content.Projectiles.Lobotomy;
using VanillaModding.External.AI;

namespace VanillaModding.Content.NPCs.LobotomyGod
{
    internal class LobotomyExtremeDemonNPC : ModNPC
    {
        public override void SetStaticDefaults()
        {
            //NPCID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
            NPCID.Sets.TrailCacheLength[NPC.type] = 25; // The length of old position to be recorded
            NPCID.Sets.TrailingMode[NPC.type] = 3; // The recording mode
        }

        public override void SetDefaults()
        {
            NPC.width = 42; // The width of projectile hitbox
            NPC.height = 38; // The height of projectile hitbox

            NPC.aiStyle = -1;
            NPC.damage = 120;
            NPC.defense = 0;
            NPC.lifeMax = 500;

            NPC.HitSound = SoundID.NPCHit1;

            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            //NPC.DamageType = DamageClass.Ranged; // What type of damage does this projectile affect?
        }

        // Custom AI

        int alpha = 0;
        public override void AI()
        {
            float maxDetectRadius = 960f; // The maximum radius at which a projectile can detect a target
            float projSpeed = 40f; // The speed at which the projectile moves towards the target

            // Trying to find NPC closest to the projectile
            Player closestPlayer = null;
            closestPlayer = AdvAI.FindClosestPlayerNPC(maxDetectRadius, NPC);

            NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;
            if (closestPlayer == null)
                return;

            Vector2 target = closestPlayer.Center;
            NPC.velocity = -Vector2.Lerp(-NPC.velocity, (NPC.Center - target).SafeNormalize(Vector2.Zero) * projSpeed, 0.0125f);
            if (NPC.localAI[0] < (int)(60 * 1f))
            {
                // Linearly increase alpha over the last 300 ticks
                int fadeDuration = (int)(60 * 1f);
                int timeFading = (int)(fadeDuration - NPC.localAI[0]);

                // Alpha goes from 0 to 255 over 300 ticks
                alpha = (int)(255f * timeFading / fadeDuration);

                // Clamp to avoid going above 255
                if (alpha > 255)
                    alpha = 255;
            }
            else
            {
                // Fully visible before fading starts
                alpha = 0;
            }

            NPC.localAI[0]--;
            if (NPC.localAI[0] <= 0)
            {
                explodeLobotomy();
                NPC.active = false;
                NPC.life = 0;
                return;
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            NPC.localAI[0] = 60 * 6; // Used to track if the spawn sound has been played
            SoundEngine.PlaySound(VanillaModdingSoundID.ExtremeDemonFire, NPC.position);
            SoundEngine.PlaySound(VanillaModdingSoundID.ExtremeDemonFire2, NPC.position);
        }

        public override void OnKill()
        {
            explodeLobotomy();
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            explodeLobotomy();
        }

        public void explodeLobotomy()
        {
            var position = NPC.position;
            var speedX = NPC.velocity.X;
            var speedY = NPC.velocity.Y;
            float speedMul = 2f;
            float numberProjectiles = 3; // 3 shots
            float rotation = MathHelper.ToRadians(45);//Shoots them in a 45 degree radius. (This is technically 90 degrees because it's 45 degrees up from your cursor and 45 degrees down)
            position += Vector2.Normalize(new Vector2(speedX, speedY)) * 45f; //45 should equal whatever number you had on the previous line
            var enS = NPC.GetSource_FromThis();
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f; // Vector for spread. Watch out for dividing by 0 if there is only 1 projectile.
                if (Main.netMode != NetmodeID.MultiplayerClient) Projectile.NewProjectile(enS, new Vector2(position.X, position.Y), new Vector2(perturbedSpeed.X, perturbedSpeed.Y) * speedMul, ModContent.ProjectileType<LobotomyNormal_Enemy>(), NPC.damage / 2, NPC.damage / 2, -1); //Creates a new projectile with our new vector for spread.
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            String pathGlow = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "_Glow").Replace(@"\", "/");
            String pathGlowTrail = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "_Glow_Trail").Replace(@"\", "/");

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glow = (Texture2D)ModContent.Request<Texture2D>($"{pathGlow}", AssetRequestMode.ImmediateLoad).Value;
            Texture2D glowTrail = (Texture2D)ModContent.Request<Texture2D>($"{pathGlowTrail}", AssetRequestMode.ImmediateLoad).Value;
            Vector2 origin = new(texture.Width / 2, texture.Height / 2);
            Vector2 originGlow = new(glow.Width / 2, glow.Height / 2);

            float opacity = 1f - (alpha / 255f);
            Vector2 position = NPC.Center - Main.screenPosition;
            Main.spriteBatch.Draw(glow, position, null, new Color(255, 0, 0, 0) * opacity, 0f, originGlow, NPC.scale - 0.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(glow, position, null, new Color(255, 50, 50, 0) * opacity, 0f, originGlow, NPC.scale - 0.75f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(glow, position, null, new Color(255, 250, 250, 0) * opacity, 0f, originGlow, NPC.scale - 0.8f, SpriteEffects.None, 0f);

            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                float c = k / (float)NPC.oldPos.Length;
                float g = 1f - c;
                Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + origin + new Vector2(0f, NPC.gfxOffY);
                Color color = NPC.GetAlpha(lightColor) * ((NPC.oldPos.Length - k) / (float)NPC.oldPos.Length);
                Main.spriteBatch.Draw(glowTrail, drawPos, null, new Color(255, 50, 50, 0) * g * opacity, NPC.oldRot[k] + MathHelper.PiOver2, originGlow, NPC.scale - 0.75f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(glowTrail, drawPos, null, new Color(255, 250, 250, 0) * g * opacity, NPC.oldRot[k] + MathHelper.PiOver2, originGlow, NPC.scale - 0.9f, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(texture, position, null, Color.White * opacity, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f); // Main Body

            return false;
        }
    }
}
