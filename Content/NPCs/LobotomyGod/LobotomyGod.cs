using log4net.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Common.Systems;
using VanillaModding.Content.NPCs.Ocram.Ocram_Minions;

namespace VanillaModding.Content.NPCs.LobotomyGod
{
    internal class LobotomyGod : ModNPC
    {

        // Sway Related NPC
        float maxSwayRotation = 0.3f; // Maximum rotation in radians (~11.5 degrees)
        float swaySpeed = 0.1f;   // How quickly the rotation adjusts
        public override void SetDefaults()
        {
            NPC.width = 100;
            NPC.height = 100;

            NPC.damage = 35;
            NPC.defense = 20;
            NPC.lifeMax = 35000;

            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;

            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(0, 0, 0, 0);
            NPC.aiStyle = -1;

            NPC.knockBackResist = 0;

            NPC.boss = true;

            if (!Main.dedServ)
            {
                Music = VanillaModdingMusicID.LobotomyGod;
            }
        }

        public float rotationSpeed = 0.02f;
        public float rotating = 0f;
        public override void AI()
        {
            rotating += rotationSpeed;
            // This should almost always be the first code in AI() as it is responsible for finding the proper player target
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }

            Player player = Main.player[NPC.target];

            if (player.dead)
            {
                // If the targeted player is dead, flee
                NPC.velocity.Y -= 0.04f;
                // This method makes it so when the boss is in "despawn range" (outside of the screen), it despawns in 10 ticks
                NPC.EncourageDespawn(10);
                return;
            }

            float offsetX = 200f;
            float offsetY = -150f;
            Vector2 abovePlayer = player.Top + new Vector2(NPC.direction * offsetX, -(NPC.height + offsetY));

            Vector2 toAbovePlayer = abovePlayer - NPC.Center;
            Vector2 toAbovePlayerNormalized = toAbovePlayer.SafeNormalize(Vector2.UnitY);

            float speed = 24f;
            float inertia = 40f;

            // If the boss is somehow below the player, move faster to catch up
            if (NPC.Top.Y > player.Bottom.Y)
            {
                speed = 48f;
            }

            Vector2 moveTo = toAbovePlayerNormalized * speed;
            NPC.velocity = (NPC.velocity * (inertia - 1) + moveTo) / inertia;

            float targetRotation = NPC.velocity.X * 0.05f;
            targetRotation = MathHelper.Clamp(targetRotation, -maxSwayRotation, maxSwayRotation);
            NPC.rotation = MathHelper.Lerp(NPC.rotation, targetRotation, swaySpeed);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 position = NPC.Center - Main.screenPosition;
            String pathMainGlow = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "_Main_Glow").Replace(@"\", "/");
            String pathHands = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "_Hands").Replace(@"\", "/");

            Texture2D Hands = (Texture2D)ModContent.Request<Texture2D>($"{pathHands}", AssetRequestMode.ImmediateLoad).Value;
            Texture2D MainGlow = (Texture2D)ModContent.Request<Texture2D>($"{pathMainGlow}", AssetRequestMode.ImmediateLoad).Value;
            Texture2D OriginTexture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = new(OriginTexture.Width / 2, OriginTexture.Height / 2);
            Vector2 originGlow = new(MainGlow.Width / 2, MainGlow.Height / 2);
            Vector2 originHands = new(Hands.Width / 2, Hands.Height / 2);

            Vector2 eyeOffset0 = new(25, -10);
            Vector2 eyeOffset1 = new(-25, -10);
            Main.spriteBatch.Draw(MainGlow, position, null, new Color(200, 255, 200, 0), 0f, originGlow, NPC.scale + 0.25f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(MainGlow, position, null, new Color(50, 255, 50, 0), 0f, originGlow, NPC.scale + 0.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(MainGlow, position, null, new Color(0, 255, 0, 0), 0f, originGlow, NPC.scale + 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(OriginTexture, position, null, Color.White , NPC.rotation, origin, NPC.scale + 0.25f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(MainGlow, position + eyeOffset0.RotatedBy(NPC.rotation), null, new Color(255, 0, 0, 0), 0f, originGlow, NPC.scale - 0.25f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(MainGlow, position + eyeOffset0.RotatedBy(NPC.rotation), null, new Color(255, 50, 50, 0), 0f, originGlow, NPC.scale - 0.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(MainGlow, position + eyeOffset0.RotatedBy(NPC.rotation), null, new Color(255, 225, 200, 0), 0f, originGlow, NPC.scale - 0.75f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(MainGlow, position + eyeOffset1.RotatedBy(NPC.rotation), null, new Color(255, 0, 0, 0), 0f, originGlow, NPC.scale - 0.25f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(MainGlow, position + eyeOffset1.RotatedBy(NPC.rotation), null, new Color(255, 50, 50, 0), 0f, originGlow, NPC.scale - 0.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(MainGlow, position + eyeOffset1.RotatedBy(NPC.rotation), null, new Color(255, 225, 200, 0), 0f, originGlow, NPC.scale - 0.75f, SpriteEffects.None, 0f);
            for (int angle = 0; angle < 360; angle += 45)
            {
                // Convert degrees to radians for use in Vector2 rotation
                float radians = MathHelper.ToRadians(angle) + rotating;

                Vector2 HandsOffset = new(-225, 0);
                Main.spriteBatch.Draw(Hands, position + HandsOffset.RotatedBy(radians), null, Color.Green, radians - MathHelper.PiOver2, originHands, NPC.scale, SpriteEffects.None, 0f);
            }

            for (int angle = 0; angle < 360; angle += 24)
            {
                // Convert degrees to radians for use in Vector2 rotation
                float radians = MathHelper.ToRadians(angle) - rotating;

                Vector2 HandsOffset = new(-125, 0);
                Main.spriteBatch.Draw(Hands, position + HandsOffset.RotatedBy(radians), null, Color.Green, radians - MathHelper.PiOver2, originHands, NPC.scale - 0.5f, SpriteEffects.None, 0f);
            }

            if (rotating >= MathHelper.TwoPi) rotating = 0f;
            return false;
        }
    }
}
