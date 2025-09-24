using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using VanillaModding.Content.Projectiles.KoboldDynamite;
using Terraria.DataStructures;
using Terraria.Localization;

namespace VanillaModding.Content.Items.Accessories
{
    public class KoboldPlayer : ModPlayer // tah thing to extend this
    {
        public Item theBomb;
        public bool onEquipped;
        bool exploded;

        public override void UpdateDead()
        {
            base.UpdateDead();
            if (!exploded && onEquipped) // Prevent from exploding too many times
            {
                theBomb.TurnToAir(true);
                exploded = true;
                Projectile.NewProjectile(Player.GetSource_FromAI(), Player.MountedCenter, Vector2.Zero, ModContent.ProjectileType<KoboldDynamite>(), 0, 0, Player.whoAmI);
            }
        }

        public override void ResetEffects()
        {
            onEquipped = false;
            if (!Player.deadForGood) if (exploded && !Player.dead)
                {
                    exploded = false;
                }
        }
    }

    [AutoloadEquip(EquipType.Back)]
    internal class KoboldDynamiteBackpack : ModItem
    {
        public static LocalizedText SelfDamage
        {
            get; private set;
        }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;

            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 4*3);
            //Item.vanity = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<KoboldPlayer>().onEquipped = true;
            player.GetModPlayer<KoboldPlayer>().theBomb = Item;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            SelfDamage = this.GetLocalization("SelfDamage").WithFormatArgs(250, 3);
            var line = new TooltipLine(Mod, "Damage", SelfDamage.Value);
            tooltips.Add(line);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Silk, 3);
            recipe.AddIngredient(ItemID.Dynamite, 3);
            recipe.AddIngredient(ItemID.Rope, 6);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}
