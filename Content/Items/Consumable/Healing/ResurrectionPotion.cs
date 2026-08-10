using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaModding.Content.Items.Consumable.Healing
{
    internal class ResurrectionPotion : ModItem
    {
        public override void SetDefaults()
        {
            int width = 26; int height = 38;
            Item.Size = new Vector2(width, height);

            Item.useTime = Item.useAnimation = 17;
            Item.useTurn = true;
            Item.maxStack = Item.CommonMaxStack;
            Item.noMelee = true;

            Item.value = Item.sellPrice(gold: 30);
            Item.rare = ItemRarityID.Green;

            Item.UseSound = SoundID.Item3;
        }
    }
}
