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
        /// The maximum distance that this cursor can reach (from <see cref="Player"/> to the Mouse Cursor).
        /// It can be stacked as well, but any item is held is considered as base, before getting modified by accessories.
        /// </summary>
        public float range;

        /// <summary>
        /// A collection of buffs (<see langword="int"/> BuffID, <see langword="int"/> BuffDuration) from an accessory or item, this gets added to the <see cref="VanillaModdingPlayer"/>'s stacked buff list 
        /// which by standard can stack more and more depending on how many accessory you equipped related to Clicker Class.
        /// </summary>
        public List<(int, int)> Buffs = new List<(int, int)>();

        /// <summary>
        /// <para/> Allows for multiple clicker accessories to be equipped at once. Allows <see cref="CanAccessoryBeEquippedWith(Item, Item, Player)"/> method to return true, except any existing/same of the accessory despite having it true will still be false to prevent stacking of the same accessory.
        /// <para/> by default it is set to <see langword="false"/>
        /// </summary>
        public bool multiAccessoryClicker = false;

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            if (incomingItem.accessory && incomingItem.ModItem is ClickerItem i && equippedItem.ModItem is ClickerItem o)
            {
                return i.multiAccessoryClicker && i.Type != o.Type;
            }
            return base.CanAccessoryBeEquippedWith(equippedItem, incomingItem, player);
        }

        public void UpdateCursorStats(Player player)
        {
            VanillaModdingPlayer cursorPlayer = player.GetModPlayer<VanillaModdingPlayer>();
            cursorPlayer.overrideCursor = true;
            if (ModContent.HasAsset($"{nameof(VanillaModding)}/Common/UI/CursorAsset/{Name}".Replace(@"\", "/"))) cursorPlayer.cursorItem = Type;
            cursorPlayer.cursorRange += range;
            cursorPlayer.cursorDamageTotal += Item.damage;

            cursorPlayer.stackedCursorBuff.AddRange(Buffs);
        }
    }
}
