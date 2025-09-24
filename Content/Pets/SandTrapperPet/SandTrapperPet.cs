using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.External.AI;
using VanillaModding.Content.Items.Weapon.Magic;
using VanillaModding.Content.Projectiles.DuneTrapper;
using static Humanizer.In;

namespace VanillaModding.Content.Pets.SandTrapperPet
{
    internal class SandTrapperPetHead : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

            Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
        }

        public override void SetDefaults()
        {
            Projectile.width = (int)Math.Round(40 * 1f);
            Projectile.height = (int)Math.Round(46 * 1.5f) - 2;

            Projectile.tileCollide = false; // Makes the minion go through tiles freely

            // These below are needed for a minion weapon
            Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.minion = true; // Declares this as a minion (has many effects)
            Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
            Projectile.minionSlots = 1f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (!CheckActive(owner) && Projectile.ai[0] == 0)
            {
                return;
            }
            float maxVelocity = 10f;
            NPC closestNPC = AdvAI.FindClosestNPC(512f, Projectile);

            if (closestNPC != null) BombRushBehavior(closestNPC.Center, maxVelocity * 4f); else GeneralBehavior(owner, maxVelocity);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        bool updateNewPos = true;
        float idleY;
        private void GeneralBehavior(Player owner, float MAX_VELOCITY)
        {
            
            Vector2 idlePosition = owner.Center;
            if (updateNewPos)
            {
                idleY = Main.rand.NextFloat(-5f, 5f);
                updateNewPos = false;
            }

            //idlePosition.Y -= 48f; // Go up 48 coordinates (three tiles from the center of the player)
            var distanceToIdlePosition = Projectile.Distance(idlePosition);

            if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 2000f)
            {
                // Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile,
                // and then set netUpdate to true
                Projectile.position = idlePosition;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }

            if (distanceToIdlePosition < 160f)
            {
                Projectile.velocity.Y += idleY/10;
                return;
            }
            else
            {
                updateNewPos = true;
            }
            Projectile.velocity += Projectile.DirectionTo(idlePosition) * MAX_VELOCITY / 60f;  // Adjust the 4f constant to adjust the homing strength

            if (Projectile.velocity.LengthSquared() > MAX_VELOCITY * MAX_VELOCITY)
                Projectile.velocity = Vector2.Normalize(Projectile.velocity) * MAX_VELOCITY;
        }

        private void BombRushBehavior(Vector2 target, float MAX_VELOCITY)
        {
            Projectile.velocity += Projectile.DirectionTo(target) * MAX_VELOCITY / 60f;  // Adjust the 4f constant to adjust the homing strength

            if (Projectile.velocity.LengthSquared() > MAX_VELOCITY * MAX_VELOCITY)
                Projectile.velocity = Vector2.Normalize(Projectile.velocity) * MAX_VELOCITY;
        }

        // This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<ExampleSimpleMinionBuff>());

                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<ExampleSimpleMinionBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            return true;
        }

        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return false;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override bool MinionContactDamage()
        {
            return true;
        }
    }

    internal class SandTrapperPetBody : ModProjectile
    {
        public override void SetStaticDefaults()
        {

            Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 18;

            Projectile.tileCollide = false; // Makes the minion go through tiles freely

            // These below are needed for a minion weapon
            Projectile.timeLeft = int.MaxValue / 2;
            Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.minion = true; // Declares this as a minion (has many effects)
            Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
            Projectile.minionSlots = 0f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
        }

        public override void AI()
        {
            NPC closestNPC = AdvAI.FindClosestNPC(256f, Projectile);
            if (Projectile.ai[0] == 0 || !Main.projectile[(int)Projectile.ai[0]].active || Main.projectile[(int)Projectile.ai[0]] == null)
            {
                Projectile.active = false;
                Projectile.timeLeft = 0;
                return;
            }
            Projectile.ai[2]++;
            if (Projectile.ai[2] >= 50f)
            {
                if(closestNPC != null) spikenooo(closestNPC);
                Projectile.ai[2] = 0;
            }

            Player owner = Main.player[Projectile.owner];
            //if (Projectile.ai[1] + owner.slotsMinions > owner.maxMinions && Projectile.ai[2] == 0) //Responsible to replace teh tail
            //{
            //    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SandTrapperPetTail>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0]);
            //    Projectile.ai[2]++;
            //
            //    Projectile.active = false;
            //    Projectile.timeLeft = 0;
            //    return;
            //}
            //else
            //{
            //    Projectile.minionSlots = 1f;
            //}

            if (!CheckActive(owner) && Projectile.ai[0] == 0)
            {
                return;
            }

            //Projectile.timeLeft = Main.projectile[(int)Projectile.ai[0]].timeLeft;
        }

        void spikenooo(NPC target)
        {
            Vector2 velco = new Vector2(Main.rand.Next(-25, 25), -10);
            var source = Projectile.GetSource_FromAI();

            int damage = (int)(Projectile.damage * 0.25f);
            float knockback = 4f;
            float speed = 9f;
            Vector2 velocity = Vector2.Normalize(target.Center - Projectile.Center) * speed;

            Projectile.NewProjectile(source, Projectile.Center, new Vector2(velocity.X, velco.Y + velocity.Y), ModContent.ProjectileType<LeftoverSpikeFriendly>(), 10, 6, Projectile.owner);
        }

        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<ExampleSimpleMinionBuff>());

                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<ExampleSimpleMinionBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            return true;
        }

        public override bool PreAI()
        {
            Vector2 npcCenter = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
            // Then using that center, we calculate the direction towards the 'parent NPC' of this NPC.
            float dirX = Main.projectile[(int)Projectile.ai[0]].position.X + (float)(Main.projectile[(int)Projectile.ai[0]].width / 2) - npcCenter.X;
            float dirY = Main.projectile[(int)Projectile.ai[0]].position.Y + (float)(Main.projectile[(int)Projectile.ai[0]].height / 2) - npcCenter.Y;
            // We then use Atan2 to get a correct rotation towards that parent NPC.
            Projectile.rotation = (float)Math.Atan2(dirY, dirX) + 1.57f;
            // We also get the length of the direction vector.
            float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
            // We calculate a new, correct distance.
            float dist = (length - (float)Projectile.height) / length;
            float posX = dirX * dist;
            float posY = dirY * dist;

            // Reset the velocity of this NPC, because we don't want it to move on its own
            Projectile.velocity = Vector2.Zero;
            // And set this NPCs position accordingly to that of this NPCs parent NPC.
            Projectile.position.X = Projectile.position.X + posX;
            Projectile.position.Y = Projectile.position.Y + posY;

            return true;
        }

        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return false;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override bool MinionContactDamage()
        {
            return true;
        }
    }

    internal class SandTrapperPetTail : ModProjectile
    {
        public override void SetStaticDefaults()
        {

            Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 22; 
            Projectile.tileCollide = false; // Makes the minion go through tiles freely

            // These below are needed for a minion weapon
            Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.minion = true; // Declares this as a minion (has many effects)
            Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
            Projectile.minionSlots = 0f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0 || !Main.projectile[(int)Projectile.ai[0]].active || Main.projectile[(int)Projectile.ai[0]] == null)
            {
                Projectile.active = false;
                Projectile.timeLeft = 0;
                return;
            }

            Player owner = Main.player[Projectile.owner];

            if (!CheckActive(owner) && Projectile.ai[0] == 0)
            {
                return;
            }

            //Projectile.timeLeft = Main.projectile[(int)Projectile.ai[0]].timeLeft;
        }

        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<ExampleSimpleMinionBuff>());

                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<ExampleSimpleMinionBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            return true;
        }

        public override bool PreAI()
        {
            Vector2 npcCenter = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
            // Then using that center, we calculate the direction towards the 'parent NPC' of this NPC.
            float dirX = Main.projectile[(int)Projectile.ai[0]].position.X + (float)(Main.projectile[(int)Projectile.ai[0]].width / 2) - npcCenter.X;
            float dirY = Main.projectile[(int)Projectile.ai[0]].position.Y + (float)(Main.projectile[(int)Projectile.ai[0]].height / 2) - npcCenter.Y;
            // We then use Atan2 to get a correct rotation towards that parent NPC.
            Projectile.rotation = (float)Math.Atan2(dirY, dirX) + 1.57f;
            // We also get the length of the direction vector.
            float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
            // We calculate a new, correct distance.
            float dist = (length - (float)Projectile.height) / length;
            float posX = dirX * dist;
            float posY = dirY * dist;

            // Reset the velocity of this NPC, because we don't want it to move on its own
            Projectile.velocity = Vector2.Zero;
            // And set this NPCs position accordingly to that of this NPCs parent NPC.
            Projectile.position.X = Projectile.position.X + posX;
            Projectile.position.Y = Projectile.position.Y + posY;

            return true;
        }
    }
}
