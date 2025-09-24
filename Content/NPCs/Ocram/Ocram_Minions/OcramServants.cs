using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VanillaModding.Content.NPCs.Ocram.Ocram_Minions
{
    internal class OcramServants : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.width = 62;
            NPC.height = 62;

            NPC.damage = 20;
            NPC.defense = 5;
            NPC.lifeMax = 600;

            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;

            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(0,0,0,0);
            NPC.aiStyle = -1;
        }

        readonly float rotSpeed = 0.25f;
        float rotdef;
        bool hehe;
        public override void AI()
        {
            NPC.ai[1]++;
            if (!Main.npc[(int)NPC.ai[0]].active || Main.npc[(int)NPC.ai[0]] == null || (int)NPC.ai[0] == 0)
            {
                // If the parent is no where just disappear plz, need performance.
                NPC.active = false;
                NPC.life = 0;
                return;
            }

            float projSpeed = 40f; // The speed at which the projectile moves towards the target
            rotdef += rotSpeed * NPC.velocity.Length() / 10f;

            NPC.spriteDirection = NPC.direction;

            // Trying to find NPC closest to the projectile

            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active) NPC.TargetClosest();
            Player closestNPC = Main.player[NPC.target];
            NPC.rotation = rotdef * NPC.direction + MathHelper.PiOver2;
            if (closestNPC == null)
                return;

            if (NPC.ai[1] >= 50) hehe = true;
            if (!hehe) { NPC.velocity = -Vector2.Lerp(-NPC.velocity, (NPC.Center - closestNPC.Center).SafeNormalize(Vector2.Zero) * projSpeed, 0.005f); NPC.ai[2] = 0; }
            else
                if (NPC.ai[2] == 0) { NPC.velocity = Vector2.Zero; NPC.ai[2]++; } else if (NPC.ai[2] < 3) { NPC.velocity += NPC.DirectionTo(closestNPC.Center) * 640f / 60f; NPC.ai[2]++; }
            if(NPC.ai[1] >= 100 && hehe) { hehe = false; NPC.ai[1] = 0; NPC.ai[2] = 0; }


        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
            SpriteEffects effects = (NPC.spriteDirection == -1) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 origin = new(texture.Width / 2, texture.Height / Main.npcFrameCount[NPC.type] / 2);
            
            //Main.spriteBatch.Draw(texture, new Vector2(NPC.position.X - Main.screenPosition.X + NPC.width / 2 - texture.Width * NPC.scale / 2f + origin.X * NPC.scale, NPC.position.Y - Main.screenPosition.Y + NPC.height - texture.Height * NPC.scale / Main.npcFrameCount[NPC.type] + 4f + origin.Y * NPC.scale), new Rectangle?(NPC.frame), Color.White, NPC.rotation, origin, NPC.scale, effects, 0f);
            if (hehe)
            {
                for (int i = 1; i < NPC.oldPos.Length; i++)
                {
                    Color color = Color.Red;
                    color = NPC.GetAlpha(color);
                    color *= (NPC.oldPos.Length - i) / 15f;
                    Main.spriteBatch.Draw(texture, new Vector2(NPC.position.X - Main.screenPosition.X + NPC.width / 2 - texture.Width * NPC.scale / 2f + origin.X * NPC.scale, NPC.position.Y - Main.screenPosition.Y + NPC.height - texture.Height * NPC.scale / Main.npcFrameCount[NPC.type] + 4f + origin.Y * NPC.scale) - NPC.velocity * i * 0.5f, new Rectangle?(NPC.frame), color, NPC.rotation, origin, NPC.scale, effects, 0f);
                }
            }
            return true;
        }

        public override void FindFrame(int frameHeight)
        {
            // This NPC animates with a simple "go from start frame to final frame, and loop back to start frame" rule
            // In this case: First stage: 0-1-2-0-1-2, Second stage: 3-4-5-3-4-5, 5 being "total frame count - 1"
            int startFrame = 0;
            int finalFrame = 3;

            int frameSpeed = 5;
            NPC.frameCounter += 1.75f;
            NPC.frameCounter += NPC.velocity.Length() / 10f; // Make the counter go faster with more movement speed
            if (NPC.frameCounter > frameSpeed)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;

                if (NPC.frame.Y > finalFrame * frameHeight)
                {
                    NPC.frame.Y = startFrame * frameHeight;
                }
            }
        }

    }
}
