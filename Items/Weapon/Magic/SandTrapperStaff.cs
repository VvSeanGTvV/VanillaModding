using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Projectiles.BloodyScythe;
using VanillaModding.Projectiles.DuneTrapper;
using VanillaModding.Projectiles.Pets.SandTrapperPet;
using static Humanizer.In;

namespace VanillaModding.Items.Weapon.Magic
{
    public class ExampleSimpleMinionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true; // This buff won't save when you exit the world
            Main.buffNoTimeDisplay[Type] = true; // The time remaining won't display on this buff
        }

        public override void Update(Player player, ref int buffIndex)
        {
            // If the minions exist reset the buff time, otherwise remove the buff from the player
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SandTrapperPetHead>()] > 0)
            {
                player.buffTime[buffIndex] = 18000;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }

    internal class SandTrapperStaff : ModItem
    {

        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true; // This lets the player target anywhere on the whole screen while using a controller
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;

            ItemID.Sets.StaffMinionSlotsRequired[Type] = 1f; // The default value is 1, but other values are supported. See the docs for more guidance. 
        }

        public override void SetDefaults()
        {
            Item.damage = 59;

            //Item.DefaultToStaff(ModContent.ProjectileType<LeftoverSpike>(), 16, 25, 12);
            Item.shootSpeed = 8f;
            Item.shoot = ModContent.ProjectileType<LeftoverSpikeFriendly>();
            //Item.mana = 10;

            Item.width = 44;
            Item.height = 44;

            Item.useTime = 25;
            Item.useAnimation = 25;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = //Item.sellPrice(0, 6, 0, 0);

            //Item.shootsEveryUse = true;

            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;

            //Item.axe = 35;
            Item.knockBack = 6;

            // These below are needed for a minion weapon
            Item.noMelee = true; // this item doesn't do any melee damage
            Item.DamageType = DamageClass.Summon; // Makes the damage register as summon. If your item does not have any damage type, it becomes true damage (which means that damage scalars will not affect it). Be sure to have a damage type
            Item.buffType = ModContent.BuffType<ExampleSimpleMinionBuff>();
            // No buffTime because otherwise the item tooltip would say something like "1 minute duration"
            Item.shoot = ModContent.ProjectileType<SandTrapperPetHead>(); // This item creates the minion projectile
        }
        
        List<Projectile> reformatProjectile = new List<Projectile>();
        Projectile[] formatTable;

        //Projecto
        int latestProjectile;
        int lastProjectile;
        //int relocate;
        Projectile mainhead;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies

            if (mainhead != null)
            {
                mainhead.active = false;
                mainhead.timeLeft = 0;
            }
            player.AddBuff(Item.buffType, 2);
            latestProjectile = Projectile.NewProjectile(source, position, velocity, type, Item.damage, knockback, Main.myPlayer);
            mainhead = Main.projectile[(int)latestProjectile];

            if (formatTable != null && formatTable.Length > 0 )
            {
                for( int i = 0; i < formatTable.Length; i++)
                {
                    formatTable[i].active = false;
                    formatTable[i].timeLeft = 0;
                }
            }

            for (int i = 0; i < 10; i++)
            {
                lastProjectile = latestProjectile;
                latestProjectile = Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<SandTrapperPetBody>(), Item.damage, knockback, Main.myPlayer, lastProjectile);

                Main.projectile[(int)latestProjectile].ai[0] = lastProjectile;
                reformatProjectile.Add(Main.projectile[(int)latestProjectile]);
            }

            latestProjectile = Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<SandTrapperPetTail>(), Item.damage, knockback, Main.myPlayer, lastProjectile);
            
            Main.projectile[(int)latestProjectile].ai[0] = lastProjectile;
            reformatProjectile.Add(Main.projectile[(int)latestProjectile]);

            formatTable = reformatProjectile.ToArray();


            // Minions have to be spawned manually, then have originalDamage assigned to the damage of the summon item

            //projectile.originalDamage = Item.damage;

            // Since we spawned the projectile manually already, we do not need the game to spawn it for ourselves anymore, so return false
            return false;
        }
    }
}
