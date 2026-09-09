using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaModding.Content.Projectiles.Lepus
{
    internal class EasterEgg : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.RottenEgg);
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.Size = new Vector2(22, 24);
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            int evilBunny = WorldGen.crimson ? NPCID.CrimsonBunny : NPCID.CorruptBunny;
            if (Main.rand.NextBool(200)) NPC.NewNPC(Projectile.GetSource_Death(), (int)Projectile.Center.X, (int)Projectile.Center.Y, NPCID.Bunny);
            if (Main.rand.NextBool(300)) NPC.NewNPC(Projectile.GetSource_Death(), (int)Projectile.Center.X, (int)Projectile.Center.Y, evilBunny);
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
