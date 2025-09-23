using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using System.Threading.Tasks;
using System;
using Terraria.Localization;

namespace VanillaModding.NPCs.DuneTrapper
{
    // These three class showcase usage of the WormHead, WormBody and WormTail classes from Worm.cs
    internal class DuneSplicerCloneHead : WormHead
    {
        public override int BodyType => ModContent.NPCType<DuneSplicerCloneBody>();

        public override int TailType => ModContent.NPCType<DuneSplicerCloneTail>();


        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Dune Trapper");
            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()
            { // Influences how the NPC looks in the Bestiary
                //CustomTexturePath = "VanillaTestimony/NPCs/ExampleWorm_Bestiary", // Custom Texture needed for this specific if it has mutliple segments.
                Position = new Vector2(40f, 24f), // Initial POS
                PortraitPositionXOverride = 0f, // Offset POS X
                PortraitPositionYOverride = 12f // Offset POS Y
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }

        public override void SetDefaults()
        {
            // Head is 10 defense, body 20, tail 30.
            NPC.CloneDefaults(NPCID.DuneSplicerHead);
            NPC.damage = 58;
            NPC.defense = 18;
            NPC.lifeMax = 550;

            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
        }

        //public override void ScaleExpertStats(int numPlayers, float bossLifeScale){
        //	NPC.lifeMax = (int)(NPC.lifeMax * 0.225f * bossLifeScale);
        //	NPC.damage = (int)(NPC.damage * 0.6f);
        //}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Desert,
				//BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("A worm that likes eating people.")
            });
        }

        public override void Init()
        {
            // Set the segment variance
            // If you want the segment length to be constant, set these two properties to the same value
            MinSegmentLength = 4;
            MaxSegmentLength = 6;

            CommonWormInit(this);
        }

        // This method is invoked from ExampleWormHead, ExampleWormBody and ExampleWormTail
        internal static void CommonWormInit(WormProjectile worm)
        {
            // These two properties handle the movement of the worm
            worm.MoveSpeed = 25.5f;
            worm.Acceleration = 0.2f;

        }

        private int attackCounter;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(attackCounter);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            attackCounter = reader.ReadInt32();
        }

        public override void AI()
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<DuneTrapperHead>())) //no boss no bitches
            {
                NPC.active = false;
                NPC.life = 0;
                return;
            }

        }
    }

    internal class DuneSplicerCloneBody : WormBody
    {

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Dune Trapper");
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);


        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DuneSplicerBody);
            NPC.damage = 54;
            NPC.defense = 28;
            NPC.lifeMax = 550;

            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
        }

        public override void Init()
        {
            DuneSplicerCloneHead.CommonWormInit(this);
        }
    }

    internal class DuneSplicerCloneTail : WormTail
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Dune Trapper");
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DuneSplicerTail);
            NPC.damage = 50;
            NPC.defense = 34;
            NPC.lifeMax = 550;

            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
        }

        public override void Init()
        {
            DuneSplicerCloneHead.CommonWormInit(this);
        }

    }
}
