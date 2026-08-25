using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Items.Accessories;
using VanillaModding.Content.Items.Accessories.Book;
using VanillaModding.Content.Items.Weapon.Melee;
using VanillaModding.Content.Rarities;

namespace VanillaModding.Common
{
    internal class VanillaModdingGlobalItem : GlobalItem
    {
        #region Tooltip Rarity
        public override bool PreDrawTooltipLine(
            Item item,
            DrawableTooltipLine line,
            ref int yOffset)
        {
            if (VanillaModdingSystem.RarityCustomExist(item.rare) && line.Name == "ItemName" && ModContent.GetInstance<VanillaModdingConfigClient>().CustomTooltip) return false;

            return true;
        }

        public override void PostDrawTooltipLine(
            Item item,
            DrawableTooltipLine line)
        {
            if (VanillaModdingSystem.RarityCustomExist(item.rare) && line.Name == "ItemName")
            {
                VanillaModdingSystem.RarityCustomByID(item.rare).Update();
                if (ModContent.GetInstance<VanillaModdingConfigClient>().CustomTooltip)
                {
                    VanillaModdingSystem.RarityCustomByID(item.rare).Draw(item, line);
                    if (ModContent.GetInstance<VanillaModdingConfigClient>().CustomSpecialEffectsTooltip) VanillaModdingSystem.RarityCustomByID(item.rare).SpecialDraw(line);
                }
            }
        }
        #endregion

        bool ItemExistInArmor(Player player, int slot, int item, bool ignoreSocialAccessory = true)
        {
            /*for (int i = 0; i < player.armor.Length; i++)
            {
                if (ignoreSocialAccessory && i > 12 && i < 19) continue;
                if (player.armor[i].type == item) return true;
            }
            return false;*/
            if ((ignoreSocialAccessory && slot > 12 && slot < 20)) return false;
            return player.armor.Take(13).Any(i => i.type == item);
        }
        // Equip Bool
        public override bool CanEquipAccessory(Item item, Player player, int slot, bool modded)
        {
            if ((item.type == ItemID.ManaFlower) || (item.type == ModContent.ItemType<HealingFlower>())) return !ItemExistInArmor(player, slot, ModContent.ItemType<RestorationFlower>()) && !ItemExistInArmor(player, slot, ModContent.ItemType<TropicalFlower>());
            if (item.type == ModContent.ItemType<RestorationFlower>()) return !ItemExistInArmor(player, slot, ModContent.ItemType<TropicalFlower>()) && !ItemExistInArmor(player, slot, ModContent.ItemType<HealingFlower>()) && !ItemExistInArmor(player, slot, ItemID.ManaFlower);
            if (item.type == ModContent.ItemType<TropicalFlower>()) return !ItemExistInArmor(player, slot, ModContent.ItemType<RestorationFlower>()) && !ItemExistInArmor(player, slot, ModContent.ItemType<HealingFlower>()) && !ItemExistInArmor(player, slot, ItemID.ManaFlower);

            return base.CanEquipAccessory(item, player, slot, modded);
        }

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

                /* Fishing Crate */
                #region Fishing Crates
                case ItemID.OceanCrate:
                case ItemID.OceanCrateHard:
                    {
                        itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<FishTrout>(), 30));
                    }
                    break;

                 #endregion
            }
        }
    }
}
