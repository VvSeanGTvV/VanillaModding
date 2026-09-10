using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Items.Accessories;
using VanillaModding.Content.Items.Accessories.Book;
using VanillaModding.Content.Items.Weapon.Melee;
using VanillaModding.Content.Items.Weapon.Throwable;

namespace VanillaModding.Common
{
    internal class VanillaModdingGlobalItem : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine sellPrice = tooltips.FirstOrDefault(
                x => x.Mod == "Terraria" && x.Name == "Price"
            );

            if (sellPrice != null && item.value == int.MaxValue)
            {
                sellPrice.OverrideColor = Colors.CoinPlatinum;
                sellPrice.Text = VanillaModdingSystem.Priceless.Value;
            }
        }

        public override void SetDefaults(Item entity)
        {
            if (entity.type == ItemID.RottenEgg)
            {
                entity.ammo = ModContent.ItemType<Egg>();
            }
        }

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
            if ((item.type == ItemID.ManaFlower) || (item.type == ModContent.ItemType<HealingFlower>())) return !ItemExistInArmor(player, slot, ModContent.ItemType<RestorationFlower>());
            if (item.type == ModContent.ItemType<RestorationFlower>()) return !ItemExistInArmor(player, slot, ModContent.ItemType<HealingFlower>()) && !ItemExistInArmor(player, slot, ItemID.ManaFlower);

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
