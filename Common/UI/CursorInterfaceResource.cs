using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.UI;

namespace VanillaModding.Common.UI
{
    [Autoload(true, Side = ModSide.Client)]
    internal class CursorInterfaceResource : ModSystem
    {
        private static List<InterfaceResource> Resources;

        public override void OnModLoad()
        {
            Resources = new List<InterfaceResource>
            {
                new CursorUI(),
            };
        }

        public override void OnModUnload()
        {
            Resources = null;
        }

        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            //Only called if not in the ingame menu/fancyUI
            InterfaceResource.ResetClickerGaugeOffset();
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (Resources != null)
            {
                foreach (var resource in Resources)
                {
                    resource.Update(gameTime);
                }
            }
        }

        //No override, has to be called manually before the cursor layer is removed (which may be used for determining GetInsertIndex)
        public static void AddInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            if (Resources != null)
            {
                foreach (var resource in Resources)
                {
                    int layer = resource.GetInsertIndex(layers);
                    if (layer != -1)
                    {
                        layers.Insert(layer, resource);
                    }
                }
            }
        }
    }
}
