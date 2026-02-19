using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Items.Accessories.Book;

namespace VanillaModding.Common
{
    internal class ModifyGlobalItem : GlobalItem
    {
        // Add items to vanilla loot bags
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            switch (item.type)
            {
                /* Treasure Bags */
                #region Treasure Bags
                case ItemID.FishronBossBag:
                    {
                        itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<BookofLeviathanLock>(), 10));
                    }
                    break;
                    #endregion
            }
        }
    }
}
