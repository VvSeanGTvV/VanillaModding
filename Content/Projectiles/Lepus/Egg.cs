using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaModding.Content.Projectiles.Lepus
{
    internal class Egg : ModProjectile
    {
        public override string Texture => ($"{nameof(VanillaModding)}/{TextureAssets.Item[ModContent.ItemType<Content.Items.Weapon.Throwable.Egg>()].Name}").Replace(@"\", "/");
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 1200;
        }

        public override void AI()
        {
            Projectile.rotation += MathHelper.ToRadians(10.5f + Math.Abs(Projectile.velocity.X / 5)) * Projectile.direction;
            Projectile.velocity.Y = Projectile.velocity.Y + 0.25f; // 0.1f for arrow gravity, 0.4f for knife gravity
            if (Projectile.velocity.Y > 32f) Projectile.velocity.Y = 32f;
        }

        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            int bird = Main.rand.NextBool(5) ? Main.rand.NextBool(5) ? Main.rand.NextBool(5) ? NPCID.GoldBird : NPCID.BirdBlue : NPCID.BirdRed : NPCID.Bird;
            if (Main.rand.NextBool(200)) NPC.NewNPC(Projectile.GetSource_Death(), (int)Projectile.Center.X, (int)Projectile.Center.Y, bird);
            //if (Main.rand.NextBool(200) && WorldGen) NPC.NewNPC(Projectile.GetSource_Death(), (int)Projectile.Center.X, (int)Projectile.Center.Y, NPCID.ExplosiveBunny);

            /*if (Main.netMode != NetmodeID.Server)
            {
                int easterEggGoreType = ModContent.Find<ModGore>("CLA/EasterEggGore").Type;
                for (int i = 0; i < 1; i++)
                {
                    Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, new Vector2(Main.rand.Next(-2, 2), -1), easterEggGoreType);
                    Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, new Vector2(Main.rand.Next(-2, 2), -1), easterEggGoreType);
                }
            }*/
            //SoundEngine.PlaySound(SoundID. { Volume = 0.4f, Pitch = 0.2f, MaxInstances = 0, PitchVariance = 0.1f }, Projectile.position);
        }
    }
}
