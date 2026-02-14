using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace VanillaModding.Content.Items
{
    public abstract class ClickerItem : ModItem
    {
        /// <summary>
        /// Max distance from the player that the clicker can hit.
        /// </summary>
        public float range;
        /// <summary>
        /// Represents a collection of ID Buffs
        /// int, int is represented as BuffID, Duration
        /// </summary>
        /// <remarks>This list is used for any clicker class and be used to affect NPC or PVP Players on attack.</remarks>
        public List<(int, int)> Buffs = new List<(int, int)>();
    }
}
