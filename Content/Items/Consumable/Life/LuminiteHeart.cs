using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using VanillaModding.Common;

namespace VanillaModding.Content.Items.Consumable.Life
{
    internal class LuminiteHeart : ModItem
    {
        public static readonly int MaxLuminiteHeart = 20;
        public static readonly int LifePerFruit = 5;

        public override void SetDefaults()
        {
            int width = 30; int height = 18;
            Item.Size = new Vector2(width, height);

            Item.useTime = Item.useAnimation = 17;
            //Item.holdStyle = ItemHoldStyleID.HoldFront;
            Item.useTurn = true;
            Item.maxStack = Item.CommonMaxStack;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
            Item.noMelee = true;

            Item.value = Item.sellPrice(gold: 10, silver: 80);
            Item.rare = ItemRarityID.LightRed;
            //Item.expert = true;

            Item.UseSound = SoundID.Item2;
            //Item.CloneDefaults(ItemID.LifeFruit);
        }
    }
}
