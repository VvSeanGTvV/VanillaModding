using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using VanillaModding.Common.ResourceOverlay;
using VanillaModding.Content.Items.Weapon.Summoner.Clicker;

namespace VanillaModding
{
    [Autoload(true, Side = ModSide.Client)]
    internal class CursorSystem : ModSystem
    {
        bool hasCursor = false;
        private GameTime _lastUpdateUIGameTime;
        private static FieldInfo mouseTextCacheField;
        private static FieldInfo isValidField;
        private static bool reflectionFailed = false;

        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUIGameTime = gameTime;
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            CursorInterfaceResource.AddInterfaceLayers(layers);
            hasCursor = false;
            //Remove Mouse Cursor
            if (Main.cursorOverride == -1)
            {
                Player player = Main.LocalPlayer;
                if (CursorUI.CanDrawCursor(player.HeldItem))
                {
                    hasCursor = true;
                    for (int i = 0; i < layers.Count; i++)
                    {
                        if (layers[i].Name.Equals("Vanilla: Cursor"))
                        {
                            //This only removes the default cursor, see DetourSecondCursor for the second one
                            layers[i].Active = false;
                            //layers.RemoveAt(i); //Do not remove layers, mods with no defensive code cause this to break/delete all UI
                            break;
                        }
                    }
                }
            }
        }

        public override void OnModLoad()
        {
            On_Main.DrawInterface_36_Cursor += DetourSecondCursor;
            try
            {
                /*
					DrawGamepadInstructions();
					if (instance._mouseTextCache.isValid) {
						instance.MouseTextInner(instance._mouseTextCache);
						DrawInterface_36_Cursor(); //Detour only this one
						instance._mouseTextCache.isValid = false;
						instance._mouseTextCache.noOverride = false;
					}
				 */

                Type mainType = typeof(Main);
                mouseTextCacheField = mainType.GetField("_mouseTextCache", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                var nestedTypes = mainType.GetNestedTypes(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                var mouseTextCacheType = nestedTypes.FirstOrDefault(t => t.Name == "MouseTextCache");
                isValidField = mouseTextCacheType.GetField("isValid", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            }
            catch
            {
                reflectionFailed = true;
            }
            base.OnModLoad();
        }

        private static void DetourSecondCursor(On_Main.orig_DrawInterface_36_Cursor orig)
        {
            //This is used to detour the second cursor draw which happens on NPC mouseover in DrawInterface_41 after Main.instance._mouseTextCache.valid is true
            if (!reflectionFailed && CursorUI.detourSecondCursorDraw)
            {
                CursorUI.detourSecondCursorDraw = false;

                var mouseTextCache = mouseTextCacheField.GetValue(Main.instance);
                object isValid = isValidField.GetValue(mouseTextCache);
                if (isValid is bool valid && valid)
                {
                    return;
                }
            }

            orig();
        }
    }
}
