using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using VanillaModding.Common;

namespace VanillaModding.Content.Items
{
    public abstract class ClickerItem : ModItem
    {
        /// <summary>
        /// Max distance from the player that the clicker can hit.
        /// </summary>
        public float range;

        /// <summary>
        /// <para>
        /// Represents a collection of ID Buffs
        /// int, int is represented as BuffID, Duration
        /// </para>
        /// </summary>
        /// <remarks>This list is used for any clicker class and be used to affect NPC or PVP Players on attack.</remarks>
        public List<(int, int)> Buffs = new List<(int, int)>();

        /// <summary>
        /// <para>
        /// Allows for multiple clicker accessories to be equipped at once. This is done by setting this variable to true, which will allow the CanAccessoryBeEquippedWith method to return true even if both the currently equipped item and the incoming item are ClickerItems.
        /// By default false.
        /// </para>
        /// </summary>
        public bool multiAccessoryClicker = false;

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            if (incomingItem.accessory && incomingItem.ModItem is ClickerItem i && equippedItem.ModItem is ClickerItem o)
            {
                return i.multiAccessoryClicker;
            }
            return base.CanAccessoryBeEquippedWith(equippedItem, incomingItem, player);
        }

        public void UpdateCursorStats(Player player)
        {
            VanillaModdingPlayer cursorPlayer = player.GetModPlayer<VanillaModdingPlayer>();
            cursorPlayer.overrideCursor = true;
            if (ModContent.HasAsset($"{nameof(VanillaModding)}/Common/UI/CursorAsset/{Item.Name}".Replace(@"\", "/"))) cursorPlayer.cursorItem = Type;
            cursorPlayer.cursorRange += range;
            cursorPlayer.cursorDamageTotal += Item.damage;

            cursorPlayer.stackedCursorBuff.AddRange(Buffs);
        }
    }
}
