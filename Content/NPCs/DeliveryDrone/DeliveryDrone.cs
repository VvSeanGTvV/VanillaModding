using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using ReLogic.Content;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using VanillaModding.Common.Systems;
using VanillaModding.Content.Projectiles.PackageProjectile;
using VanillaModding.Content.Projectiles.Tizona;
using static System.Net.Mime.MediaTypeNames;
using static Terraria.NPC;

namespace VanillaModding.Content.NPCs.DeliveryDrone
{
    public class DeliveryDronePortraitBackgroundProviderBestiaryInfoElement : IBestiaryInfoElement, IBestiaryBackgroundImagePathAndColorProvider
    {
        public Asset<Texture2D> GetBackgroundImage() => ModContent.Request<Texture2D>($"{nameof(VanillaModding)}/Assets/BestiaryBackground/DeliveryDrone_BestiaryBackground");
        public Color? GetBackgroundColor() => Color.Gray;
        public UIElement ProvideUIElement(BestiaryUICollectionInfo info) => null;
    }

    internal class DeliveryDrone : ModNPC
    {
        public static LocalizedText BestiaryText
        {
            get; private set;
        }

        // Speed Related NPC
        float npcSpeed = 40f; // The Speed at which the NPC moves towards the target
        float npcAccel = 0.05f; // The Acceleration at which the NPC moves towards the target 
        int npcExplodeDamage = 50; // The damage the NPC does when it collides with the player and/or other NPCs
        // Sway Related NPC
        float maxSwayRotation = 0.2f; // Maximum rotation in radians (~11.5 degrees)
        float swaySpeed = 0.1f;   // How quickly the rotation adjusts

        // Timer Related
        int idleTimer = 60 * 1; // 1 second idle time before dropping the package
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
        }
        public override void SetDefaults()
        {
            NPC.width = 68;
            NPC.height = 40;

            NPC.damage = 0;
            NPC.defense = 5;
            NPC.lifeMax = 80;

            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;

            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(0, 0, 0, 0);
            NPC.aiStyle = -1;
            NPC.scale = 1f;

            NPC.knockBackResist = 1f;
            BestiaryText = this.GetLocalization("Bestiary");
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // Sets the description of this NPC that is listed in the bestiary
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new DeliveryDronePortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement(BestiaryText.ToString())
            });
        }

        int timer = 0;
        int soundTimer = 0;
        bool packageDropped = false;

        public void dropPackage()
        {
            if (packageDropped) return;
            packageDropped = true;

            var source = NPC.GetSource_FromAI();
            SoundEngine.PlaySound(SoundID.Item11, NPC.position);
            Projectile.NewProjectile(source, NPC.Center - new Vector2(0f, -12f), NPC.velocity, ModContent.ProjectileType<PackageProjectile>(), 5, 0, -1, NPC.rotation);
        }


        SoundStyle Whirr = VanillaModdingSoundID.Whirr;
        public override void AI()
        {
            int deliveryTo = (int)NPC.ai[0];
            Player player = deliveryTo <= Main.maxPlayers ? Main.player[deliveryTo] : null;

            //Whirr.Pitch = 0.5f + (Math.Abs(NPC.velocity.X) / npcSpeed) * 0.5f; // Pitch from 0.5 to 1.0 based on speed
            Whirr.PlayOnlyIfFocused = true;
            if (!Main.dedServ && soundTimer <= 0)
            {
                SoundEngine.PlaySound(Whirr, NPC.position);
                soundTimer = Whirr.GetSoundEffect().Duration.Seconds * 60;
            }

            soundTimer--;
            if (packageDropped || player == null || !player.active || player.dead)
            {
                NPC.velocity.X *= 0.98f;
                if (NPC.velocity.X > -0.1f && NPC.velocity.X < 0.1f) NPC.velocity.X = 0f;
                NPC.velocity.Y -= 0.2f; // Fall down when done or player is invalid
                NPC.EncourageDespawn(250);
                return;
            }

            Vector2 abovePlayer = player.Top + new Vector2(NPC.direction, -250f) + new Vector2((150f * player.direction), NPC.direction);
            Vector2 toTarget = abovePlayer - NPC.Center;
            float distance = toTarget.Length();
            Vector2 desiredDirection = toTarget.SafeNormalize(Vector2.Zero);
            float speedFactor = MathHelper.Clamp(distance / 100f, 0f, 1f); // max at 100+ pixels away
            float targetSpeed = npcSpeed * speedFactor;
            Vector2 desiredVelocity = desiredDirection * targetSpeed;
            NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, npcAccel); // npcAccel should be small (like 0.05f)

            float targetRotation = NPC.velocity.X * 0.05f;
            targetRotation = MathHelper.Clamp(targetRotation, -maxSwayRotation, maxSwayRotation);
            NPC.rotation = MathHelper.Lerp(NPC.rotation, targetRotation, swaySpeed);

            bool atTarget = (NPC.Center - abovePlayer).LengthSquared() <= 100f; // 10^2 = 100
            if (atTarget && !packageDropped && player.active && !player.dead)
            {
                timer++;
                if (timer >= idleTimer)
                {
                    dropPackage();
                    // Drop the package
                    if (!Main.dedServ)
                    {
                        //SoundEngine.PlaySound(SoundID.Item, NPC.position);
                    }
                    //int packageItem = Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<Items.Misc.deliveryPackage>());
                    //Main.item[packageItem].velocity = new Vector2(0, 5f); // Drop straight down with a bit of speed
                    //NPC.active = false; // Despawn the NPC after dropping the package
                }
            }
            else
            {
                timer = 0;
            }

            if (
                Collision.SolidCollision(NPC.position, NPC.width, NPC.height)
                || Collision.WetCollision(NPC.position, NPC.width, NPC.height)
                )
            {
                OnKill();
                SoundEngine.PlaySound(NPC.DeathSound, NPC.position);
                NPC.life -= NPC.lifeMax * NPC.defense; // Instant death
            }

            foreach (var otherNpc in Main.ActiveNPCs)
            {
                if (otherNpc != NPC && otherNpc.active && otherNpc.getRect().Intersects(NPC.getRect()) && otherNpc.TypeName != NPC.TypeName) // Example using getRect() for bounding boxes
                {
                    otherNpc.SimpleStrikeNPC(npcExplodeDamage, 0);
                    OnKill();
                    SoundEngine.PlaySound(NPC.DeathSound, NPC.position);
                    NPC.life -= NPC.lifeMax * NPC.defense; // Instant death
                }
            }

            foreach (var otherPlayer in Main.ActivePlayers)
            {
                if (otherPlayer.active && otherPlayer.getRect().Intersects(NPC.getRect())) // Example using getRect() for bounding boxes
                {
                    otherPlayer.Hurt(PlayerDeathReason.ByNPC(NPC.type), npcExplodeDamage, 0);
                    OnKill();
                    SoundEngine.PlaySound(NPC.DeathSound, NPC.position);
                    NPC.life -= NPC.lifeMax * NPC.defense; // Instant death
                }
            }
            if (NPC.life < NPC.lifeMax/1.1f) dropPackage(); // EMERGENCY RETREAT! incase it got hurt.
        }

        public override void OnKill()
        {
            dropPackage();
            for (int i = 0; i < 10; i++)
            {
                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                dust.velocity *= 1.4f;
            }

            // Fire Dust spawn
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Torch, 0f, 0f, 100, default, 3f);
                dust.noGravity = true;
                dust.velocity *= 5f;
                dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Torch, 0f, 0f, 100, default, 2f);
                dust.velocity *= 3f;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            int startFrame = 0;
            int finalFrame = 3;

            int frameSpeed = 2;
            NPC.frameCounter += 0.5f;
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
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!packageDropped) 
            { 
                String package = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "_Package").Replace(@"\", "/");
                Texture2D PackageTexture = ModContent.Request<Texture2D>($"{package}").Value;
                Vector2 PackageOrigin = new(PackageTexture.Width / 2, PackageTexture.Height / 2);

                Vector2 pos = NPC.Center - Main.screenPosition;
                Main.spriteBatch.Draw(PackageTexture, pos - new Vector2(0, -10f).RotatedBy(NPC.rotation), null, drawColor, NPC.rotation, PackageOrigin, NPC.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
    }
}
