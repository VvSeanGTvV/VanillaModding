using Microsoft.Xna.Framework;
using Mono.Cecil;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using VanillaModding.Common.Systems;
using VanillaModding.Content.Items.Consumable.BossBags;
using VanillaModding.Content.Items.Materials;
using VanillaModding.Content.Projectiles.DuneTrapper;

namespace VanillaModding.Content.NPCs.DuneTrapper
{
    // These three class showcase usage of the WormHead, WormBody and WormTail classes from Worm.cs
    [AutoloadBossHead]
    internal class DuneTrapperHead : WormHead
    {
        public override int BodyType => ModContent.NPCType<DuneTrapperBody>();

        public override int TailType => ModContent.NPCType<DuneTrapperTail>();

        bool spawnInit;

        public static LocalizedText BestiaryText
        {
            get; private set;
        }

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Dune Trapper");
            /*NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "VanillaModding/Content/NPCs/Ocram/OcramBestiary",
                PortraitScale = 0.6f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                PortraitPositionYOverride = -((190f / 2f - 190f / 2f) * 0.6f),
            };*/
            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()
            { // Influences how the NPC looks in the Bestiary
                //CustomTexturePath = "VanillaTestimony/NPCs/ExampleWorm_Bestiary", // Custom Texture needed for this specific if it has mutliple segments.
                CustomTexturePath = "VanillaModding/Content/NPCs/DuneTrapper/DuneTrapperBestiary",
                Position = new Vector2(58f, 15f), // Initial POS
                PortraitPositionXOverride = 62f, // Offset POS X
                PortraitPositionYOverride = 18f, // Offset POS Y
                PortraitScale = 0.75f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                Scale = 0.5f,
                Rotation = 0f // Rotation

            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
            BestiaryText = this.GetLocalization("Bestiary");

            // Add this in for bosses that have a summon item, requires corresponding code in the item (See MinionBossSummonItem.cs)
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            // Automatically group with other bosses
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            // Specify the debuffs it is immune to. Most NPCs are immune to Confused.
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SetDefaults()
        {
            // Head is 10 defense, body 20, tail 30.
            //NPC.CloneDefaults(NPCID.DuneSplicerHead);
            NPC.damage = 300;
            NPC.defense = 40;
            NPC.lifeMax = 40000;

            NPC.aiStyle = -1;
            NPC.width = 84;
            NPC.height = 100;
            NPC.boss = true;
            NPC.HitSound = SoundID.NPCHit1;

            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.scale = 1.5f;
            NPC.behindTiles = true;

            NPC.SpawnWithHigherTime(30);
            NPC.npcSlots = 10f;
            NPC.value = Item.sellPrice(0, 2, 10, 10);
            //NPC.BossBar = ModContent.GetInstance<ExampleWormHeadBossBar>();

            // The following code assigns a music track to the boss in a simple way.
            if (!Main.dedServ)
            {
                Music = VanillaModdingMusicID.GettingSandy;
            }
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
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.Sandstorm,
				//BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

				// Sets the description of this NPC that is listed in the bestiary.
                new FlavorTextBestiaryInfoElement(BestiaryText.ToString())

                //new FlavorTextBestiaryInfoElement("A worm that likes eating people.")
            });
        }

        public override void Init()
        {
            // Set the segment variance
            // If you want the segment length to be constant, set these two properties to the same value
            MinSegmentLength = 48;
            MaxSegmentLength = MinSegmentLength;
            UseCustomAI = true;
            CommonWormInit(this);
        }

        // This method is invoked from ExampleWormHead, ExampleWormBody and ExampleWormTail
        internal static void CommonWormInit(Worm worm)
        {
            // These two properties handle the movement of the worm
            worm.MoveSpeed = 16.5f;
            worm.Acceleration = 0.4f;

        }

        private int attackCounter;
        private int attackProj;
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
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();
            
            Player player = Main.player[NPC.target];
            float speed = 14f;
            float inertia = 40f;
            float turnSpeed = MathHelper.ToRadians(25f); // radians per frame — smaller = slower turning
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
            float distanceFactor = MathHelper.Clamp(distance / 400f, 1f, 40f);
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


            if (attackCounter < 1) attackCounter = 150;
            if (attackProj > 0) attackProj--;

            annoyDune();

            //16 = 1 tile
            if (attackProj <= 0 && Vector2.Distance(NPC.Center, player.Center) < 480 && Collision.CanHit(NPC.Center, 1, 1, player.Center, 1, 1))
            {
                //Vector2 direction = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
                //direction = direction.RotatedByRandom(MathHelper.ToRadians(10));
                int count = 5;
                int prj = 0;

                while (prj < count)
                {
                    Vector2 offset = new Vector2(Main.rand.Next(-640, 640), Main.rand.Next(-640, 640));
                    int projectile = Projectile.NewProjectile(NPC.GetSource_FromThis(), player.Center - offset, new Vector2(0, 0), ProjectileID.SandnadoHostileMark, 20, 5, -1);
                    Main.projectile[projectile].timeLeft = 500;
                    prj++;
                }

                attackProj = 350;
                NPC.netUpdate = true;
            }
        }

        /*private Player CloseTargetPlayer()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active) NPC.TargetClosest();

            Player closestNPC = Main.player[NPC.target];
            if (closestNPC == null)
                return null;

            bool InDesert = (closestNPC.ZoneDesert || closestNPC.ZoneUndergroundDesert);
            if (closestNPC.dead || !InDesert || !Sandstorm.Happening)
            {
                // If the targeted player is dead or out of range of the Desert, flee
               ForcedTargetPosition = new Vector2(closestNPC.Center.X, closestNPC.Center.Y + 2000f);
                // This method makes it so when the boss is in "despawn range" (outside of the screen), it despawns in 10 ticks
                NPC.EncourageDespawn(10);
                return null;
            }
            ForcedTargetPosition = null;
            return closestNPC;
        }*/
        public void annoyDune()
        {
            int count = 2;
            var entitySource = NPC.GetSource_FromAI();
            attackCounter--;


            if (NPC.CountNPCS(ModContent.NPCType<DuneSplicerCloneHead>()) < count && attackCounter < 1)
            {
                Vector2 offset = new Vector2(Main.rand.Next(-640, 640), Main.rand.Next(-640, 640));
                if (Main.netMode != NetmodeID.MultiplayerClient) NPC.NewNPC(entitySource, (int)(NPC.Center.X + offset.X), (int)(NPC.Center.Y + offset.Y), ModContent.NPCType<DuneSplicerCloneHead>(), NPC.whoAmI);
            }
        }

        public override void OnKill()
        {
            

            //NPC.SetEventFlagCleared(ref DownedBossSystem.downedDuneTrapper, -1);
        }

        public override bool PreKill()
        {

            NPCLoot_Center(NPC.target, this.Type, BodyType, TailType);
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedDuneTrapper, -1);
            this.OnKill();
            return false;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Do NOT misuse the ModifyNPCLoot and OnKill hooks: the former is only used for registering drops, the latter for everything else

            // Add the treasure bag using ItemDropRule.BossBag (automatically checks for expert mode)
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<SandTrapperBag>()));

            // Trophies are spawned with 1/10 chance
            //npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Placeable.Furniture.MinionBossTrophy>(), 10));

            // ItemDropRule.MasterModeCommonDrop for the relic
            //npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.Furniture.MinionBossRelic>()));

            // ItemDropRule.MasterModeDropOnAllPlayers for the pet
            //npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<MinionBossPetItem>(), 4));

            // All our drops here are based on "not expert", meaning we use .OnSuccess() to add them into the rule, which then gets added
            //LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());

            // Notice we use notExpertRule.OnSuccess instead of npcLoot.Add so it only applies in normal mode
            // Boss masks are spawned with 1/7 chance
            //notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<MinionBossMask>(), 7));

            // This part is not required for a boss and is just showcasing some advanced stuff you can do with drop rules to control how items spawn
            // We make 12-15 ExampleItems spawn randomly in all directions, like the lunar pillar fragments. Hereby we need the DropOneByOne rule,
            // which requires these parameters to be defined
            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            int itemType = ModContent.ItemType<SoulofBlight>();
            int itemType2 = ItemID.AdamantiteOre;
            var parameters = new DropOneByOne.Parameters()
            {
                ChanceNumerator = 1,
                ChanceDenominator = 1,
                MinimumStackPerChunkBase = 1,
                MaximumStackPerChunkBase = 1,
                MinimumItemDropsCount = 5,
                MaximumItemDropsCount = 16,
            };

            notExpertRule.OnSuccess(new DropOneByOne(itemType, parameters));
            //notExpertRule.OnSuccess(new DropOneByOne(itemType2, parameters2));

            // Finally add the leading rule
            npcLoot.Add(notExpertRule);
        }
    }

    internal class DuneTrapperBody : WormBody
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Dune Trapper");
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
            NPCID.Sets.RespawnEnemyID[NPC.type] = ModContent.NPCType<DuneTrapperHead>();
        }

        public override void SetDefaults()
        {
            //NPC.CloneDefaults(NPCID.DuneSplicerBody);
            NPC.damage = 30;
            NPC.defense = 60;
            NPC.lifeMax = 40000;

            NPC.aiStyle = -1;
            NPC.width = 46;
            NPC.height = 54;
            NPC.scale = 1.5f;
            NPC.HitSound = SoundID.NPCHit1;

            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.behindTiles = true;
            NPC.boss = true;
        }

        public override void AI()
        {
            NPC.timeLeft = 1000;
        }

        public override void Init()
        {
            DuneTrapperHead.CommonWormInit(this);
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
            if (Main.rand.Next(4) != 0) return;
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

    }

    internal class DuneTrapperTail : WormTail
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Dune Trapper");
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
            NPCID.Sets.RespawnEnemyID[NPC.type] = ModContent.NPCType<DuneTrapperHead>();
        }

        public override void SetDefaults()
        {
            //NPC.CloneDefaults(NPCID.DuneSplicerTail);
            NPC.damage = 0;
            NPC.defense = 50;
            NPC.lifeMax = 40000;

            NPC.aiStyle = -1;
            NPC.width = 58;
            NPC.height = 82;
            NPC.scale = 1.5f;
            NPC.HitSound = SoundID.NPCHit1;

            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.behindTiles = true;
            NPC.boss = true;
        }

        public override void AI()
        {
            NPC.timeLeft = 1000;
        }

        public override void Init()
        {
            DuneTrapperHead.CommonWormInit(this);
        }
    }
}
