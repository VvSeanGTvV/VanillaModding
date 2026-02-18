using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Items.Materials;

namespace VanillaModding.Content.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Body)]
    internal class TorosChest : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30; // Width of the item
            Item.height = 22; // Height of the item
            Item.value = Item.sellPrice(gold: 5); // How many coins the item is worth
            Item.rare = ItemRarityID.White; // The rarity of the item
            //Item.defense = 5; // The amount of defense the item will give when equipped
            Item.vanity = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Silk, 20)
                .AddIngredient(ModContent.ItemType<WhiteThread>(), 5)
                .AddTile(TileID.Loom)
                .Register();
        }

        public override void SetStaticDefaults()
        {
            // HidesHands defaults to true which we don't want.
            ArmorIDs.Body.Sets.HidesHands[Item.bodySlot] = true;
            ArmorIDs.Body.Sets.HidesArms[Item.bodySlot] = true;
            //ArmorIDs.Body.Sets.HidesBottomSkin[Item.bodySlot] = true;
            ArmorIDs.Body.Sets.HidesTopSkin[Item.bodySlot] = true;
        }
    }
}
