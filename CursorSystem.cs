using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using VanillaModding.Content.Items.Weapon.Summoner.Clicker;

namespace VanillaModding
{
    [Autoload(true, Side = ModSide.Client)]
    internal class CursorSystem : ModSystem
    {
        bool hasCursor = false;
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            hasCursor = false;
            //Remove Mouse Cursor
            if (Main.cursorOverride == -1)
            {
                Player player = Main.LocalPlayer;
                if (player.HeldItem.ModItem is testcursor a)
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

        // Cursor Variables
        private float _clickerScale = 0f, _clickerAlpha = 0f;

        public override void UpdateUI(GameTime gameTime)
        {
            _clickerScale = Main.cursorScale;

            float flipped = 2 * 0.8f - Main.cursorAlpha;
            _clickerAlpha = flipped * 0.3f + 0.7f;
            base.UpdateUI(gameTime);
        }

        public void DrawCursor(int item)
        {
            // Actual cursor
            ModItem getCursor = ModContent.GetModItem(item);
            Texture2D texture = TextureAssets.Item[item].Value;
            /*String pathBorder = nameof(VanillaModding) + "/" + (getCursor.Texture + "_border").Replace(@"\", "/");
            Texture2D borderTexture = (Texture2D)ModContent.Request<Texture2D>($"{pathBorder}", AssetRequestMode.ImmediateLoad).Value;
            Rectangle borderFrame = borderTexture.Frame(1, 1);
            Vector2 borderOrigin = borderFrame.Size() / 2;*/

            Rectangle frame = texture.Frame(1, 1);
            Vector2 origin = frame.Size() / 2;

            Color color = Color.White;
            color.A = (byte)(_clickerAlpha * 255);

            Vector2 borderPosition = Main.MouseScreen;
            // Actual cursor is not drawn in the top left of the border but a bit offset, have to add/substract origins here
            Vector2 position = Main.MouseScreen - origin + origin/2;
            Main.spriteBatch.Draw(texture, position, frame, color, 0f, Vector2.Zero, _clickerScale, SpriteEffects.FlipHorizontally, 0f);
        }

        public override void OnModLoad()
        {
            On_Main.DrawInterface_2_SmartCursorTargets += (orig) => {
                if (hasCursor)
                {
                    DrawCursor(ModContent.ItemType<testcursor>());
                    return;
                }
                // default smart cursor target draw code
                orig();
            };
            On_Main.DrawInterface_36_Cursor += (orig) => {
                if (hasCursor)
                {
                    DrawCursor(ModContent.ItemType<testcursor>());
                    return;
                }

                // default cursor draw code
                orig();
            };
            base.OnModLoad();
        }
    }
}
