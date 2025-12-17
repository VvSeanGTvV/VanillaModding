using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace VanillaModding.Content.Items.Consumable.Healing
{
    internal class Cantaloupe : ModItem
    {
        public override void SetDefaults()
        {
            int width = 32; int height = 18;
            Item.Size = new Vector2(width, height);

            Item.useTime = Item.useAnimation = 17;
            Item.holdStyle = ItemHoldStyleID.HoldFront;
            Item.useTurn = true;

            Item.healLife = 75;
            Item.potion = true;
            Item.potionDelay = 2100;
            Item.maxStack = Item.CommonMaxStack;

            Item.useStyle = ItemUseStyleID.EatFood;
            Item.consumable = true;
            Item.noMelee = true;

            Item.value = Item.sellPrice(gold: 1, silver: 80);
            Item.rare = ItemRarityID.Orange;
            //Item.expert = true;

            Item.UseSound = SoundID.Item2;
        }
    }
}
