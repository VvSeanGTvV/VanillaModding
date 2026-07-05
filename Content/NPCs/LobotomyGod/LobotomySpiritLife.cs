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
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Common.Systems;
using static VanillaModding.VanillaModding;

namespace VanillaModding.Content.NPCs.LobotomyGod
{
    internal class LobotomySpiritLife : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 32;

            NPC.damage = 0;
            NPC.defense = 5;
            NPC.lifeMax = 100;

            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;

            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(0, 20, 0, 10);
            NPC.aiStyle = -1;

            NPC.knockBackResist = 0;
        }

        public override void OnKill()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            if (NPC.AnyNPCs(ModContent.NPCType<LobotomyGod>()))
                return;

            SoundEngine.PlaySound(SoundID.Shatter, new Vector2(NPC.Center.X, NPC.Center.Y));
            SoundEngine.PlaySound(VanillaModdingSoundID.FireInTheHole, new Vector2(NPC.Center.X, NPC.Center.Y));

            NPC.NewNPC(
                NPC.GetSource_Death(),
                (int)NPC.Center.X,
                (int)NPC.Center.Y + 350,
                ModContent.NPCType<LobotomyGod>());
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 position = NPC.Center - Main.screenPosition;
            String pathMainGlow = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "_Glow").Replace(@"\", "/");

            Texture2D MainGlow = (Texture2D)ModContent.Request<Texture2D>($"{pathMainGlow}", AssetRequestMode.ImmediateLoad).Value;
            Texture2D OriginTexture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = new(OriginTexture.Width / 2, OriginTexture.Height / 2);
            Vector2 originGlow = new(MainGlow.Width / 2, MainGlow.Height / 2);



            // GLOW
            Main.spriteBatch.Draw(MainGlow, position, null, new Color(0, 255, 0, 0), 0f, originGlow, NPC.scale - 0.75f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(OriginTexture, position, null, Color.White, NPC.rotation, origin, NPC.scale - 0.15f, SpriteEffects.None, 0f); // Main Body

            return false;
        }
    }
}
