using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;

namespace VanillaModding.Content.Items.Materials
{
    internal class SoulofBlight : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Registers a vertical animation with 4 frames and each one will last 5 ticks (1/12 second)
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 8));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true; // Makes the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation

            ItemID.Sets.ItemIconPulse[Item.type] = true; // The item pulses while in the player's inventory
            ItemID.Sets.ItemNoGravity[Item.type] = true; // Makes the item have no gravity

            Item.ResearchUnlockCount = 25; // Configure the amount of this item that's needed to research it in Journey mode.
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.maxStack = Item.CommonMaxStack;

            Item.value = Item.sellPrice(gold: 1, silver: 50);
            Item.rare = ItemRarityID.Pink;
        }

        /*public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, new Vector3(1f, 1f, 0.824f) * 0.55f * Main.essScale); // Makes this item glow when thrown out of inventory.
        }*/
        public override void PostUpdate()
            => Lighting.AddLight(Item.Center, Color.Yellow.ToVector3() * 0.45f * Main.essScale);
        public override Color? GetAlpha(Color lightColor)
            => new Color(1f * 0.97f, 1f * 0.97f, 0.824f * 0.97f, 0.5f);
    }
}
