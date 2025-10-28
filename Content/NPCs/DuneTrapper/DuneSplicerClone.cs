using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using VanillaModding.Content.Projectiles.DuneTrapper;

namespace VanillaModding.Content.NPCs.DuneTrapper
{
    // These three class showcase usage of the WormHead, WormBody and WormTail classes from Worm.cs
    internal class DuneSplicerCloneHead : WormHead
    {
        public override int BodyType => ModContent.NPCType<DuneSplicerCloneBody>();

        public override int TailType => ModContent.NPCType<DuneSplicerCloneTail>();

        public static LocalizedText BestiaryText
        {
            get; private set;
        }

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
            BestiaryText = this.GetLocalization("Bestiary");
        }

        public override void SetDefaults()
        {
            // Head is 10 defense, body 20, tail 30.
            NPC.width = 42;
            NPC.height = 12;
            NPC.damage = 28;
            NPC.defense = 14;
            NPC.lifeMax = 1050;

            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.scale = 1.15f;
            NPC.behindTiles = true;
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
				new FlavorTextBestiaryInfoElement(BestiaryText.ToString())
            });
        }

        public override void Init()
        {
            // Set the segment variance
            // If you want the segment length to be constant, set these two properties to the same value
            MinSegmentLength = 6;
            MaxSegmentLength = MinSegmentLength;
            UseCustomAI = true;
            CommonWormInit(this);
        }

        // This method is invoked from ExampleWormHead, ExampleWormBody and ExampleWormTail
        internal static void CommonWormInit(Worm worm)
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

            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];
            float speed = 22f;
            float inertia = 40f;
            float turnSpeed = MathHelper.ToRadians(35f); // radians per frame — smaller = slower turning
            bool InDesert = (player.ZoneDesert || player.ZoneUndergroundDesert);

            Vector2 targetWNoSpeed = ((NPC.Center + new Vector2(0, 100)) - NPC.Center).SafeNormalize(Vector2.Zero);
            Vector2 targetWSpeed = targetWNoSpeed * speed;
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
            if (player == null || !InDesert || !Sandstorm.Happening)
            {
                NPC.velocity = (NPC.velocity * (inertia - 1f) + targetWSpeed) / inertia;

                NPC.EncourageDespawn(10);
                NPC.netUpdate = true;
                return;
            }
            targetWNoSpeed = (player.Center - NPC.Center).SafeNormalize(Vector2.Zero);
            float distance = (NPC.Center - player.Center).Length();
            // This smoothly scales between 0.8x (close) and 2.5x (far)
            float distanceFactor = MathHelper.Clamp(distance / 400f, 1f, 60f);
            turnSpeed = turnSpeed * distanceFactor;

            targetWSpeed = targetWNoSpeed * speed;
            // Smoothly rotate toward target direction
            Vector2 targetDir = targetWSpeed.SafeNormalize(Vector2.UnitY);
            float currentRot = NPC.velocity.SafeNormalize(Vector2.UnitY).ToRotation();
            float targetRot = targetDir.ToRotation();
            float newRot = currentRot.AngleTowards(targetRot, turnSpeed);

            // Apply the new direction and speed
            Vector2 moveTo = newRot.ToRotationVector2() * speed;

            // Apply inertia-based smoothing
            NPC.velocity = (NPC.velocity * (inertia - 1f) + moveTo) / inertia;

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
            NPC.width = 44;
            NPC.height = 8;
            NPC.damage = 24;
            NPC.defense = 14;
            NPC.lifeMax = 1050;

            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.scale = 1.15f;
            NPC.behindTiles = true;
        }

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            spikenooo();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit)
        {
            spikenooo();
        }
        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            spikenooo(projectile);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            spikenooo();
        }

        void spikenooo(Projectile proj = null)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            if (Main.rand.Next(2) != 0) return;
            if (proj != null && proj.velocity.LengthSquared() > 0f)
            {

                Vector2 perturbedSpeed = new Vector2(-proj.velocity.X, -proj.velocity.Y).RotatedByRandom(MathHelper.ToRadians(12));
                var source = NPC.GetSource_FromAI();

                int damage = (int)(NPC.damage * 0.25f);
                float knockback = 4f;
                float speed = 9f;

                Projectile.NewProjectile(source, NPC.Center, perturbedSpeed / 2f, ModContent.ProjectileType<LeftoverSpike>(), 10, 6, -1);
            }
            else
            {
                Vector2 velco = new Vector2(Main.rand.Next(-25, 25), -10);
                var source = NPC.GetSource_FromAI();

                int damage = (int)(NPC.damage * 0.25f);
                float knockback = 4f;
                float speed = 9f;
                Vector2 velocity = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * speed;

                Projectile.NewProjectile(source, NPC.Center, new Vector2(velocity.X, velco.Y + velocity.Y), ModContent.ProjectileType<LeftoverSpike>(), 10, 6, -1);
            }
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
            NPC.width = 50/2;
            NPC.height = 44/6;
            NPC.damage = 20;
            NPC.defense = 16;
            NPC.lifeMax = 1050;

            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.scale = 1.15f;
            NPC.behindTiles = true;
        }

        public override void Init()
        {
            DuneSplicerCloneHead.CommonWormInit(this);
        }

    }
}
