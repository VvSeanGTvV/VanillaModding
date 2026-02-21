using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using VanillaModding.Common;

namespace VanillaModding
{
    partial class VanillaModding
    {
        internal enum MessageType : byte
        {
            VMTStatIncreasePlayerSync,
        }

        // Override this method to handle network packets sent for this mod.
        // TODO: Introduce OOP packets into tML, to avoid this god-class level hardcode.
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            MessageType msgType = (MessageType)reader.ReadByte();

            switch (msgType)
            {
                // This message syncs ExampleStatIncreasePlayer.exampleLifeFruits and ExampleStatIncreasePlayer.exampleManaCrystals
                case MessageType.VMTStatIncreasePlayerSync:
                    byte playerNumber = reader.ReadByte();
                    VanillaModdingPlayer examplePlayer = Main.player[playerNumber].GetModPlayer<VanillaModdingPlayer>();
                    examplePlayer.ReceivePlayerSync(reader);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        // Forward the changes to the other clients
                        examplePlayer.SyncPlayer(-1, whoAmI, false);
                    }
                    break;
                default:
                    Logger.WarnFormat("[VanillaModding]: Unknown Message type: {0}", msgType);
                    break;
            }
        }
    }
}
