using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace VanillaModding.Common.Systems
{
    internal class DownedBossSystem : ModSystem
    {
        /// <summary>
        /// Denotes whether or not the Dune Trapper has been defeated at least once in the current world.
        /// </summary>
        public static bool downedDuneTrapper = false;
        // public static bool downedOtherBoss = false;

        public override void ClearWorld()
        {
            downedDuneTrapper = false;
            // downedOtherBoss = false;
        }

        // We save our data sets using TagCompounds.
        // NOTE: The tag instance provided here is always empty by default.
        public override void SaveWorldData(TagCompound tag)
        {
            if (downedDuneTrapper)
            {
                tag["downedDuneTrapper"] = true;
            }

            // if (downedOtherBoss) {
            //	tag["downedOtherBoss"] = true;
            // }
        }

        public override void LoadWorldData(TagCompound tag)
        {
            downedDuneTrapper = tag.ContainsKey("downedDuneTrapper");
            // downedOtherBoss = tag.ContainsKey("downedOtherBoss");
        }

        public override void NetSend(BinaryWriter writer)
        {
            // Order of parameters is important and has to match that of NetReceive
            writer.WriteFlags(downedDuneTrapper/*, downedOtherBoss*/);
            // WriteFlags supports up to 8 entries, if you have more than 8 flags to sync, call WriteFlags again.

            // If you need to send a large number of flags, such as a flag per item type or something similar, BitArray can be used to efficiently send them. See Utils.SendBitArray documentation.
        }

        public override void NetReceive(BinaryReader reader)
        {
            // Order of parameters is important and has to match that of NetSend
            reader.ReadFlags(out downedDuneTrapper/*, out downedOtherBoss*/);
            // ReadFlags supports up to 8 entries, if you have more than 8 flags to sync, call ReadFlags again.
        }
    }
}
