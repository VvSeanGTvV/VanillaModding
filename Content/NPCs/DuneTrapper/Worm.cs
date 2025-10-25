using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

namespace VanillaModding.Content.NPCs.DuneTrapper
{
    public enum WormSegmentType
    {
        /// <summary>
        /// The head segment for the worm.  Only one "head" is considered to be active for any given worm
        /// </summary>
        Head,
        /// <summary>
        /// The body segment.  Follows the segment in front of it
        /// </summary>
        Body,
        /// <summary>
        /// The tail segment.  Has the same AI as the body segments.  Only one "tail" is considered to be active for any given worm
        /// </summary>
        Tail
    }

    /// <summary>
    /// The base class for non-separating Worm enemies.
    /// </summary>
    public abstract class Worm : ModNPC
    {
        /*  ai[] usage:
		 *  
		 *  ai[0] = "follower" segment, the segment that's following this segment
		 *  ai[1] = "following" segment, the segment that this segment is following
		 *  
		 *  localAI[0] = used when syncing changes to collision detection
		 *  localAI[1] = checking if Init() was called
		 */

        /// <summary>
        /// Which type of segment this NPC is considered to be
        /// </summary>
        public abstract WormSegmentType SegmentType { get; }

        /// <summary>
        /// The maximum velocity for the NPC
        /// </summary>
        public float MoveSpeed { get; set; }

        /// <summary>
        /// The rate at which the NPC gains velocity
        /// </summary>
        public float Acceleration { get; set; }

        /// <summary>
        /// The NPC instance of the head segment for this worm.
        /// </summary>
        public NPC HeadSegment => Main.npc[NPC.realLife];

        /// <summary>
        /// The NPC instance of the segment that this segment is following (ai[1]).  For head segments, this property always returns <see langword="null"/>.
        /// </summary>
        public NPC FollowingNPC => SegmentType == WormSegmentType.Head ? null : Main.npc[(int)NPC.ai[1]];

        /// <summary>
        /// The NPC instance of the segment that is following this segment (ai[0]).  For tail segment, this property always returns <see langword="null"/>.
        /// </summary>
        public NPC FollowerNPC => SegmentType == WormSegmentType.Tail ? null : Main.npc[(int)NPC.ai[0]];

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return SegmentType == WormSegmentType.Head ? null : false;
        }

        private bool startDespawning;

        public sealed override bool PreAI()
        {
            if (NPC.localAI[1] == 0)
            {
                NPC.localAI[1] = 1f;
                Init();
            }

            if (SegmentType == WormSegmentType.Head)
            {
                HeadAI();

                if (!NPC.HasValidTarget)
                {
                    NPC.TargetClosest(true);

                    // If the NPC is a boss and it has no target, force it to fall to the underworld quickly
                    if (!NPC.HasValidTarget && NPC.boss)
                    {
                        NPC.velocity.Y += 8f;

                        MoveSpeed = 1000f;

                        if (!startDespawning)
                        {
                            startDespawning = true;

                            // Despawn after 90 ticks (1.5 seconds) if the NPC gets far enough away
                            NPC.timeLeft = 90;
                        }
                    }
                }
            }
            else
            {
                BodyTailAI();
            }

            return true;
        }

        // Not visible to public API, but is used to indicate what AI to run
        internal virtual void HeadAI() { }

        internal virtual void BodyTailAI() { }

        public abstract void Init();
    }

    /// <summary>
    /// The base class for head segment NPCs of Worm enemies
    /// </summary>
    public abstract class WormHead : Worm
    {
        public sealed override WormSegmentType SegmentType => WormSegmentType.Head;

        /// <summary>
        /// The NPCID or ModContent.NPCType for the body segment NPCs.<br/>
        /// This property is only used if <see cref="HasCustomBodySegments"/> returns <see langword="false"/>.
        /// </summary>
        public abstract int BodyType { get; }

        /// <summary>
        /// The NPCID or ModContent.NPCType for the tail segment NPC.<br/>
        /// This property is only used if <see cref="HasCustomBodySegments"/> returns <see langword="false"/>.
        /// </summary>
        public abstract int TailType { get; }

        /// <summary>
        /// The minimum amount of segments expected, including the head and tail segments
        /// </summary>
        public int MinSegmentLength { get; set; }

        /// <summary>
        /// The maximum amount of segments expected, including the head and tail segments
        /// </summary>
        public int MaxSegmentLength { get; set; }

        /// <summary>
        /// Whether the NPC ignores tile collision when attempting to "dig" through tiles, like how Wyverns work.
        /// </summary>
        public bool CanFly { get; set; }

        /// <summary>
        /// Instead of using HeadAI(), it uses primarily all on AI() or another function to work, this is ONLY intended for using a new pattern or such.
        /// If <see cref="UseCustomAI"/> are true, collisions will be returned to <see cref="GetCollision"/>. 
        /// Enabling this variable, also causes to not use HeadAI Movements function. Requiring you to make your own movements.
        /// </summary>
        public bool UseCustomAI { get; set; }

        /// <summary>
        /// The maximum distance in <b>pixels</b> within which the NPC will use tile collision, if <see cref="CanFly"/> returns <see langword="false"/>.<br/>
        /// Defaults to 1000 pixels, which is equivalent to 62.5 tiles.
        /// </summary>
        public virtual int MaxDistanceForUsingTileCollision => 1000;

        /// <summary>
        /// Whether the NPC uses 
        /// </summary>
        public virtual bool HasCustomBodySegments => false;

        /// <summary>
        /// If not <see langword="null"/>, this NPC will target the given world position instead of its player target
        /// </summary>
        public Vector2? ForcedTargetPosition { get; set; }

        /// <summary>
        /// Override this method to use custom body-spawning code.<br/>
        /// This method only runs if <see cref="HasCustomBodySegments"/> returns <see langword="true"/>.
        /// </summary>
        /// <param name="segmentCount">How many body segments are expected to be spawned</param>
        /// <returns>The whoAmI of the most-recently spawned NPC, which is the result of calling <see cref="NPC.NewNPC(Terraria.DataStructures.IEntitySource, int, int, int, int, float, float, float, float, int)"/></returns>
        public virtual int SpawnBodySegments(int segmentCount)
        {
            // Defaults to just returning this NPC's whoAmI, since the tail segment uses the return value as its "following" NPC index
            return NPC.whoAmI;
        }

        /// <summary>
        /// Spawns a body or tail segment of the worm.
        /// </summary>
        /// <param name="source">The spawn source</param>
        /// <param name="type">The ID of the segment NPC to spawn</param>
        /// <param name="latestNPC">The whoAmI of the most-recently spawned segment NPC in the worm, including the head</param>
        /// <returns></returns>
        protected int SpawnSegment(IEntitySource source, int type, int latestNPC)
        {
            // We spawn a new NPC, setting latestNPC to the newer NPC, whilst also using that same variable
            // to set the parent of this new NPC. The parent of the new NPC (may it be a tail or body part)
            // will determine the movement of this new NPC.
            // Under there, we also set the realLife value of the new NPC, because of what is explained above.
            int oldLatest = latestNPC;
            latestNPC = NPC.NewNPC(source, (int)NPC.Center.X, (int)NPC.Center.Y, type, NPC.whoAmI, 0, latestNPC);

            Main.npc[oldLatest].ai[0] = latestNPC;

            NPC latest = Main.npc[latestNPC];
            // NPC.realLife is the whoAmI of the NPC that the spawned NPC will share its health with
            latest.realLife = NPC.whoAmI;

            return latestNPC;
        }

        internal sealed override void HeadAI()
        {
            HeadAI_SpawnSegments();

            bool collision = HeadAI_CheckCollisionForDustSpawns();

            HeadAI_CheckTargetDistance(ref collision);
            if (!UseCustomAI) HeadAI_Movement(collision);
        }

        public bool GetCollision()
        {
            return HeadAI_CheckCollisionForDustSpawns();
        }

        private void HeadAI_SpawnSegments()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                // So, we start the AI off by checking if NPC.ai[0] (the following NPC's whoAmI) is 0.
                // This is practically ALWAYS the case with a freshly spawned NPC, so this means this is the first update.
                // Since this is the first update, we can safely assume we need to spawn the rest of the worm (bodies + tail).
                bool hasFollower = NPC.ai[0] > 0;
                if (!hasFollower)
                {
                    // So, here we assign the NPC.realLife value.
                    // The NPC.realLife value is mainly used to determine which NPC loses life when we hit this NPC.
                    // We don't want every single piece of the worm to have its own HP pool, so this is a neat way to fix that.
                    NPC.realLife = NPC.whoAmI;
                    // latestNPC is going to be used in SpawnSegment() and I'll explain it there.
                    int latestNPC = NPC.whoAmI;

                    // Here we determine the length of the worm.
                    int randomWormLength = Main.rand.Next(MinSegmentLength, MaxSegmentLength + 1);

                    int distance = randomWormLength - 2;

                    IEntitySource source = NPC.GetSource_FromAI();

                    if (HasCustomBodySegments)
                    {
                        // Call the method that'll handle spawning the body segments
                        latestNPC = SpawnBodySegments(distance);
                    }
                    else
                    {
                        // Spawn the body segments like usual
                        while (distance > 0)
                        {
                            latestNPC = SpawnSegment(source, BodyType, latestNPC);
                            distance--;
                        }
                    }

                    // Spawn the tail segment
                    SpawnSegment(source, TailType, latestNPC);

                    NPC.netUpdate = true;

                    // Ensure that all of the segments could spawn.  If they could not, despawn the worm entirely
                    int count = 0;
                    foreach (var n in Main.ActiveNPCs)
                    {
                        if ((n.type == Type || n.type == BodyType || n.type == TailType) && n.realLife == NPC.whoAmI)
                            count++;
                    }

                    if (count != randomWormLength)
                    {
                        // Unable to spawn all of the segments... kill the worm
                        foreach (var n in Main.ActiveNPCs)
                        {
                            if ((n.type == Type || n.type == BodyType || n.type == TailType) && n.realLife == NPC.whoAmI)
                            {
                                n.active = false;
                                n.netUpdate = true;
                            }
                        }
                    }

                    // Set the player target for good measure
                    NPC.TargetClosest(true);
                }
            }
        }

        private bool HeadAI_CheckCollisionForDustSpawns()
        {
            int minTilePosX = (int)(NPC.Left.X / 16) - 1;
            int maxTilePosX = (int)(NPC.Right.X / 16) + 2;
            int minTilePosY = (int)(NPC.Top.Y / 16) - 1;
            int maxTilePosY = (int)(NPC.Bottom.Y / 16) + 2;

            // Ensure that the tile range is within the world bounds
            if (minTilePosX < 0)
                minTilePosX = 0;
            if (maxTilePosX > Main.maxTilesX)
                maxTilePosX = Main.maxTilesX;
            if (minTilePosY < 0)
                minTilePosY = 0;
            if (maxTilePosY > Main.maxTilesY)
                maxTilePosY = Main.maxTilesY;

            bool collision = false;

            // This is the initial check for collision with tiles.
            for (int i = minTilePosX; i < maxTilePosX; ++i)
            {
                for (int j = minTilePosY; j < maxTilePosY; ++j)
                {
                    Tile tile = Main.tile[i, j];

                    // If the tile is solid or is considered a platform, then there's valid collision
                    if (tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType] && tile.TileFrameY == 0) || tile.LiquidAmount > 64)
                    {
                        Vector2 tileWorld = new Point16(i, j).ToWorldCoordinates(0, 0);

                        if (NPC.Right.X > tileWorld.X && NPC.Left.X < tileWorld.X + 16 && NPC.Bottom.Y > tileWorld.Y && NPC.Top.Y < tileWorld.Y + 16)
                        {
                            // Collision found
                            collision = true;

                            if (Main.rand.NextBool(100))
                                WorldGen.KillTile(i, j, fail: true, effectOnly: true, noItem: false);
                        }
                    }
                }
            }

            return collision;
        }

        private void HeadAI_CheckTargetDistance(ref bool collision)
        {
            // If there is no collision with tiles, we check if the distance between this NPC and its target is too large, so that we can still trigger "collision".
            if (!collision)
            {
                Rectangle hitbox = NPC.Hitbox;

                int maxDistance = MaxDistanceForUsingTileCollision;

                bool tooFar = true;

                foreach (var player in Main.ActivePlayers)
                {
                    Rectangle areaCheck;

                    if (ForcedTargetPosition is Vector2 target)
                        areaCheck = new Rectangle((int)target.X - maxDistance, (int)target.Y - maxDistance, maxDistance * 2, maxDistance * 2);
                    else if (!player.dead && !player.ghost)
                        areaCheck = new Rectangle((int)player.position.X - maxDistance, (int)player.position.Y - maxDistance, maxDistance * 2, maxDistance * 2);
                    else
                        continue;  // Not a valid player

                    if (hitbox.Intersects(areaCheck))
                    {
                        tooFar = false;
                        break;
                    }
                }

                if (tooFar)
                    collision = true;
            }
        }

        private void HeadAI_Movement(bool collision)
        {
            // MoveSpeed determines the max speed at which this NPC can move.
            // Higher value = faster speed.
            float speed = MoveSpeed;
            // acceleration is exactly what it sounds like. The speed at which this NPC accelerates.
            float acceleration = Acceleration;

            float targetXPos, targetYPos;

            Player playerTarget = Main.player[NPC.target];

            Vector2 forcedTarget = ForcedTargetPosition ?? playerTarget.Center;
            // Using a ValueTuple like this allows for easy assignment of multiple values
            (targetXPos, targetYPos) = (forcedTarget.X, forcedTarget.Y);

            // Copy the value, since it will be clobbered later
            Vector2 npcCenter = NPC.Center;

            float targetRoundedPosX = (float)((int)(targetXPos / 16f) * 16);
            float targetRoundedPosY = (float)((int)(targetYPos / 16f) * 16);
            npcCenter.X = (float)((int)(npcCenter.X / 16f) * 16);
            npcCenter.Y = (float)((int)(npcCenter.Y / 16f) * 16);
            float dirX = targetRoundedPosX - npcCenter.X;
            float dirY = targetRoundedPosY - npcCenter.Y;

            float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);

            // If we do not have any type of collision, we want the NPC to fall down and de-accelerate along the X axis.
            if (!collision && !CanFly)
                HeadAI_Movement_HandleFallingFromNoCollision(dirX, speed, acceleration);
            else
            {
                // Else we want to play some audio (soundDelay) and move towards our target.
                HeadAI_Movement_PlayDigSounds(length);

                HeadAI_Movement_HandleMovement(dirX, dirY, length, speed, acceleration);
            }

            HeadAI_Movement_SetRotation(collision);
        }

        private void HeadAI_Movement_HandleFallingFromNoCollision(float dirX, float speed, float acceleration)
        {
            // Keep searching for a new target
            NPC.TargetClosest(true);

            // Constant gravity of 0.11 pixels/tick
            NPC.velocity.Y += 0.11f;

            // Ensure that the NPC does not fall too quickly
            if (NPC.velocity.Y > speed)
                NPC.velocity.Y = speed;

            // The following behavior mimics vanilla worm movement
            if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.4f)
            {
                // Velocity is sufficiently fast, but not too fast
                if (NPC.velocity.X < 0.0f)
                    NPC.velocity.X -= acceleration * 1.1f;
                else
                    NPC.velocity.X += acceleration * 1.1f;
            }
            else if (NPC.velocity.Y == speed)
            {
                // NPC has reached terminal velocity
                if (NPC.velocity.X < dirX)
                    NPC.velocity.X += acceleration;
                else if (NPC.velocity.X > dirX)
                    NPC.velocity.X -= acceleration;
            }
            else if (NPC.velocity.Y > 4)
            {
                if (NPC.velocity.X < 0)
                    NPC.velocity.X += acceleration * 0.9f;
                else
                    NPC.velocity.X -= acceleration * 0.9f;
            }
        }

        private void HeadAI_Movement_PlayDigSounds(float length)
        {
            if (NPC.soundDelay == 0)
            {
                // Play sounds quicker the closer the NPC is to the target location
                float num1 = length / 40f;

                if (num1 < 10)
                    num1 = 10f;

                if (num1 > 20)
                    num1 = 20f;

                NPC.soundDelay = (int)num1;

                SoundEngine.PlaySound(SoundID.WormDig, NPC.position);
            }
        }

        private void HeadAI_Movement_HandleMovement(float dirX, float dirY, float length, float speed, float acceleration)
        {
            float absDirX = Math.Abs(dirX);
            float absDirY = Math.Abs(dirY);
            float newSpeed = speed / length;
            dirX *= newSpeed;
            dirY *= newSpeed;

            if ((NPC.velocity.X > 0 && dirX > 0) || (NPC.velocity.X < 0 && dirX < 0) || (NPC.velocity.Y > 0 && dirY > 0) || (NPC.velocity.Y < 0 && dirY < 0))
            {
                // The NPC is moving towards the target location
                if (NPC.velocity.X < dirX)
                    NPC.velocity.X += acceleration;
                else if (NPC.velocity.X > dirX)
                    NPC.velocity.X -= acceleration;

                if (NPC.velocity.Y < dirY)
                    NPC.velocity.Y += acceleration;
                else if (NPC.velocity.Y > dirY)
                    NPC.velocity.Y -= acceleration;

                // The intended Y-velocity is small AND the NPC is moving to the left and the target is to the right of the NPC or vice versa
                if (Math.Abs(dirY) < speed * 0.2 && ((NPC.velocity.X > 0 && dirX < 0) || (NPC.velocity.X < 0 && dirX > 0)))
                {
                    if (NPC.velocity.Y > 0)
                        NPC.velocity.Y += acceleration * 2f;
                    else
                        NPC.velocity.Y -= acceleration * 2f;
                }

                // The intended X-velocity is small AND the NPC is moving up/down and the target is below/above the NPC
                if (Math.Abs(dirX) < speed * 0.2 && ((NPC.velocity.Y > 0 && dirY < 0) || (NPC.velocity.Y < 0 && dirY > 0)))
                {
                    if (NPC.velocity.X > 0)
                        NPC.velocity.X = NPC.velocity.X + acceleration * 2f;
                    else
                        NPC.velocity.X = NPC.velocity.X - acceleration * 2f;
                }
            }
            else if (absDirX > absDirY)
            {
                // The X distance is larger than the Y distance.  Force movement along the X-axis to be stronger
                if (NPC.velocity.X < dirX)
                    NPC.velocity.X += acceleration * 1.1f;
                else if (NPC.velocity.X > dirX)
                    NPC.velocity.X -= acceleration * 1.1f;

                if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.5)
                {
                    if (NPC.velocity.Y > 0)
                        NPC.velocity.Y += acceleration;
                    else
                        NPC.velocity.Y -= acceleration;
                }
            }
            else
            {
                // The X distance is larger than the Y distance.  Force movement along the X-axis to be stronger
                if (NPC.velocity.Y < dirY)
                    NPC.velocity.Y += acceleration * 1.1f;
                else if (NPC.velocity.Y > dirY)
                    NPC.velocity.Y -= acceleration * 1.1f;

                if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.5)
                {
                    if (NPC.velocity.X > 0)
                        NPC.velocity.X += acceleration;
                    else
                        NPC.velocity.X -= acceleration;
                }
            }
        }

        private void HeadAI_Movement_SetRotation(bool collision)
        {
            // Set the correct rotation for this NPC.
            // Assumes the sprite for the NPC points upward.  You might have to modify this line to properly account for your NPC's orientation
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

            // Some netupdate stuff (multiplayer compatibility).
            if (collision)
            {
                if (NPC.localAI[0] != 1)
                    NPC.netUpdate = true;

                NPC.localAI[0] = 1f;
            }
            else
            {
                if (NPC.localAI[0] != 0)
                    NPC.netUpdate = true;

                NPC.localAI[0] = 0f;
            }

            // Force a netupdate if the NPC's velocity changed sign and it was not "just hit" by a player
            if (((NPC.velocity.X > 0 && NPC.oldVelocity.X < 0) || (NPC.velocity.X < 0 && NPC.oldVelocity.X > 0) || (NPC.velocity.Y > 0 && NPC.oldVelocity.Y < 0) || (NPC.velocity.Y < 0 && NPC.oldVelocity.Y > 0)) && !NPC.justHit)
                NPC.netUpdate = true;
        }


        /// <summary>
        /// Spawns Loot on the middle segment of the worm, nearest to the player. 
        /// Same of the hardcoded worm loot of the Destroyer, can be found in <see cref="NPC.checkDead()"/>.
        /// </summary>
        /// <param name="player">Center to Player</param>
        /// <param name="HeadType">The ID of the segment NPC of the Head</param>
        /// <param name="BodyType">The ID of the segment NPC of the Body</param>
        /// <param name="TailType">The ID of the segment NPC of the Tail</param>
        /// <returns></returns>
        public void NPCLoot_Center(int player, int HeadType, int BodyType, int TailType)
        {
            Vector2 vector = NPC.position;
            Vector2 center = Main.player[player].Center;
            float num8 = 100000000f;
            Vector2 vector2 = NPC.position;
            for (int n = 0; n < 200; n++)
            {
                if (Main.npc[n].active && (Main.npc[n].type == HeadType || Main.npc[n].type == BodyType || Main.npc[n].type == TailType))
                {
                    float num9 = Math.Abs(Main.npc[n].Center.X - center.X) + Math.Abs(Main.npc[n].Center.Y - center.Y);
                    if (num9 < num8)
                    {
                        num8 = num9;
                        vector2 = Main.npc[n].position;
                    }
                }
            }
            string typeName = NPC.TypeName;

            NPC.position = vector2;
            NPCLoot_DropItems(Main.player[player]);
            NPCLoot_DropMoney(Main.player[player]);
            NPCLoot_DropHeals(Main.player[player]);

            DoDeathEvents_DropBossPotionsAndHearts(ref typeName);
            DoDeathEvents_CelebrateBossDeath(typeName);
            NPC.position = vector;

            
        }

        //TML: Added 'typeName' parameter.
        private void DoDeathEvents_DropBossPotionsAndHearts(ref string typeName)
        {
            int stack = Main.rand.Next(5, 16);
            int num = 28;

            NPCLoader.BossLoot(NPC, ref typeName, ref num);

            Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, num, stack);
            int num2 = Main.rand.Next(5) + 5;
            for (int i = 0; i < num2; i++)
            {
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, 58);
            }
        }

        private void DoDeathEvents_CelebrateBossDeath(string typeName)
        {
            Color color = new Color(175, 75, 255);
            NetworkText custom = null;
            if (this.DeathMessage is LocalizedText customDeathMessage)
            {
                // Modder might have already used WithFormatArgs/BoundArgs to fill out custom boss fight name as well.
                custom = customDeathMessage.BoundArgs != null ? customDeathMessage.ToNetworkText() : customDeathMessage.ToNetworkText(NPC.GetFullNetName());
            }
            // Note: typeName comes from NPC.TypeName->NPCLoader.ModifyTypeName and NPCLoader.BossLoot, but is just a literal string and won't be localized on other clients properly if used, so it is not used in DeathMessage.
            bool skipMessage = NPCLoader.ModifyDeathMessage(NPC, ref custom, ref color);
            if (skipMessage)
            {
            }
            else if (Main.netMode == 0)
            {
                Main.NewText(Language.GetTextValue("Announcement.HasBeenDefeated_Single", typeName), color);
            }
            else if (Main.netMode == 2)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasBeenDefeated_Single", NPC.GetTypeNetName()), color);
            }
        }

        private void NPCLoot_DropHeals(Player closestPlayer)
        {
            NPCLoot_DropCommonLifeAndMana(closestPlayer);
        }

        private void NPCLoot_DropCommonLifeAndMana(Player closestPlayer)
        {
            if (!NPCID.Sets.NeverDropsResourcePickups[NPC.type] && closestPlayer.RollLuck(6) == 0 && NPC.lifeMax > 1 && NPC.damage > 0)
            {
                if (Main.rand.Next(2) == 0 && closestPlayer.statMana < closestPlayer.statManaMax2)
                    Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, 184);
                else if (Main.rand.Next(2) == 0 && closestPlayer.statLife < closestPlayer.statLifeMax2)
                    Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, 58);
            }

            if (!NPCID.Sets.NeverDropsResourcePickups[NPC.type] && closestPlayer.RollLuck(2) == 0 && NPC.lifeMax > 1 && NPC.damage > 0 && closestPlayer.statMana < closestPlayer.statManaMax2)
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, 184);
        }

        private void NPCLoot_DropItems(Player closestPlayer)
        {
            DropAttemptInfo dropAttemptInfo = default(DropAttemptInfo);
            dropAttemptInfo.player = closestPlayer;
            dropAttemptInfo.npc = this.NPC;
            dropAttemptInfo.IsExpertMode = Main.expertMode;
            dropAttemptInfo.IsMasterMode = Main.masterMode;
            dropAttemptInfo.IsInSimulation = false;
            dropAttemptInfo.rng = Main.rand;
            DropAttemptInfo info = dropAttemptInfo;
            Main.ItemDropSolver.TryDropping(info);
        }

        private void NPCLoot_DropMoney(Player closestPlayer)
        {
            float num = 0f;
            float luck = closestPlayer.luck;
            int num2 = 1;
            if (Main.rand.NextFloat() < Math.Abs(luck))
                num2 = 2;

            for (int i = 0; i < num2; i++)
            {
                float num3 = NPC.value;
                if (NPC.midas)
                    num3 *= 1f + (float)Main.rand.Next(10, 51) * 0.01f;

                num3 *= 1f + (float)Main.rand.Next(-20, 76) * 0.01f;
                if (Main.rand.Next(2) == 0)
                    num3 *= 1f + (float)Main.rand.Next(5, 11) * 0.01f;

                if (Main.rand.Next(4) == 0)
                    num3 *= 1f + (float)Main.rand.Next(10, 21) * 0.01f;

                if (Main.rand.Next(8) == 0)
                    num3 *= 1f + (float)Main.rand.Next(15, 31) * 0.01f;

                if (Main.rand.Next(16) == 0)
                    num3 *= 1f + (float)Main.rand.Next(20, 41) * 0.01f;

                if (Main.rand.Next(32) == 0)
                    num3 *= 1f + (float)Main.rand.Next(25, 51) * 0.01f;

                if (Main.rand.Next(64) == 0)
                    num3 *= 1f + (float)Main.rand.Next(50, 101) * 0.01f;

                if (Main.bloodMoon)
                    num3 *= 1f + (float)Main.rand.Next(101) * 0.01f;

                if (i == 0)
                {
                    num = num3;
                }
                else if (luck < 0f)
                {
                    if (num3 < num)
                        num = num3;
                }
                else if (num3 > num)
                {
                    num = num3;
                }
            }

            num += (float)NPC.extraValue;
            while ((int)num > 0)
            {
                if (num > 1000000f)
                {
                    int num4 = (int)(num / 1000000f);
                    if (num4 > 50 && Main.rand.Next(5) == 0)
                        num4 /= Main.rand.Next(3) + 1;

                    if (Main.rand.Next(5) == 0)
                        num4 /= Main.rand.Next(3) + 1;

                    int num5 = num4;
                    while (num5 > 999)
                    {
                        num5 -= 999;
                        Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, 74, 999);
                    }

                    num -= (float)(1000000 * num4);
                    Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, 74, num5);
                }
                else if (num > 10000f)
                {
                    int num6 = (int)(num / 10000f);
                    if (num6 > 50 && Main.rand.Next(5) == 0)
                        num6 /= Main.rand.Next(3) + 1;

                    if (Main.rand.Next(5) == 0)
                        num6 /= Main.rand.Next(3) + 1;

                    num -= (float)(10000 * num6);
                    Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, 73, num6);
                }
                else if (num > 100f)
                {
                    int num7 = (int)(num / 100f);
                    if (num7 > 50 && Main.rand.Next(5) == 0)
                        num7 /= Main.rand.Next(3) + 1;

                    if (Main.rand.Next(5) == 0)
                        num7 /= Main.rand.Next(3) + 1;

                    num -= (float)(100 * num7);
                    Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, 72, num7);
                }
                else
                {
                    int num8 = (int)num;
                    if (num8 > 50 && Main.rand.Next(5) == 0)
                        num8 /= Main.rand.Next(3) + 1;

                    if (Main.rand.Next(5) == 0)
                        num8 /= Main.rand.Next(4) + 1;

                    if (num8 < 1)
                        num8 = 1;

                    num -= (float)num8;
                    Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, 71, num8);
                }
            }
        }
    }

    public abstract class WormBody : Worm
    {
        public sealed override WormSegmentType SegmentType => WormSegmentType.Body;

        internal override void BodyTailAI()
        {
            CommonAI_BodyTail(this);
        }

        internal static void CommonAI_BodyTail(Worm worm)
        {
            if (!worm.NPC.HasValidTarget)
                worm.NPC.TargetClosest(true);

            if (Main.player[worm.NPC.target].dead && worm.NPC.timeLeft > 30000)
                worm.NPC.timeLeft = 10;

            NPC following = worm.NPC.ai[1] >= Main.maxNPCs ? null : worm.FollowingNPC;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                // Some of these conditions are possible if the body/tail segment was spawned individually
                // Kill the segment if the segment NPC it's following is no longer valid
                if (following is null || !following.active || following.friendly || following.townNPC || following.lifeMax <= 5)
                {
                    worm.NPC.life = 0;
                    worm.NPC.HitEffect(0, 10);
                    worm.NPC.active = false;
                }
            }

            if (following is not null)
            {
                // Follow behind the segment "in front" of this NPC
                // Use the current NPC.Center to calculate the direction towards the "parent NPC" of this NPC.
                float dirX = following.Center.X - worm.NPC.Center.X;
                float dirY = following.Center.Y - worm.NPC.Center.Y;
                // We then use Atan2 to get a correct rotation towards that parent NPC.
                // Assumes the sprite for the NPC points upward.  You might have to modify this line to properly account for your NPC's orientation
                worm.NPC.rotation = (float)Math.Atan2(dirY, dirX) + MathHelper.PiOver2;
                // We also get the length of the direction vector.
                float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
                // We calculate a new, correct distance.
                float dist = (length - worm.NPC.width) / length;
                float posX = dirX * dist;
                float posY = dirY * dist;

                // Reset the velocity of this NPC, because we don't want it to move on its own
                worm.NPC.velocity = Vector2.Zero;
                // And set this NPCs position accordingly to that of this NPCs parent NPC.
                worm.NPC.position.X += posX;
                worm.NPC.position.Y += posY;
            }
        }
    }

    // Since the body and tail segments share the same AI
    public abstract class WormTail : Worm
    {
        public sealed override WormSegmentType SegmentType => WormSegmentType.Tail;

        internal override void BodyTailAI()
        {
            WormBody.CommonAI_BodyTail(this);
        }
    }
}
