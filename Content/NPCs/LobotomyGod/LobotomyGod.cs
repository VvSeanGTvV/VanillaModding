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

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 position = NPC.Center - Main.screenPosition;
            String pathMainGlow = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "_Main_Glow").Replace(@"\", "/");

            Texture2D MainGlow = (Texture2D)ModContent.Request<Texture2D>($"{pathMainGlow}", AssetRequestMode.ImmediateLoad).Value;
            Texture2D OriginTexture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = new(OriginTexture.Width / 2, OriginTexture.Height / 2);
            Vector2 originGlow = new(MainGlow.Width / 2, MainGlow.Height / 2);

            Vector2 eyeOffset0 = new(25, -10);
            Vector2 eyeOffset1 = new(-25, -10);
            Main.spriteBatch.Draw(MainGlow, position, null, new Color(200, 255, 200, 0), 0f, originGlow, NPC.scale + 0.25f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(MainGlow, position, null, new Color(50, 255, 50, 0), 0f, originGlow, NPC.scale + 0.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(MainGlow, position, null, new Color(0, 255, 0, 0), 0f, originGlow, NPC.scale + 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(OriginTexture, position, null, Color.White , NPC.rotation, origin, NPC.scale + 0.25f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(MainGlow, position + eyeOffset0, null, new Color(255, 0, 0, 0), 0f, originGlow, NPC.scale - 0.25f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(MainGlow, position + eyeOffset0, null, new Color(255, 50, 0, 0), 0f, originGlow, NPC.scale - 0.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(MainGlow, position + eyeOffset1, null, new Color(255, 0, 0, 0), 0f, originGlow, NPC.scale - 0.25f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(MainGlow, position + eyeOffset1, null, new Color(255, 50, 0, 0), 0f, originGlow, NPC.scale - 0.5f, SpriteEffects.None, 0f);
            return false;
        }
    }
}
