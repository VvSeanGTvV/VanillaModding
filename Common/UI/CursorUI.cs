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
using VanillaModding.Content.Items;
using VanillaModding.Content.Items.Accessories.Clicker;

namespace VanillaModding.Common.UI
{
    internal class CursorUI : InterfaceResource
    {
        public CursorUI() : base("VMT: Cursor", InterfaceScaleType.UI) { }

        private float _clickerScale = 0f;
        private float _clickerAlpha = 0f;
        private static bool _lastMouseInterface = false;
        private static bool _lastMouseText = false;
        internal static bool detourSecondCursorDraw = false;

        public override void Update(GameTime gameTime)
        {
            _clickerScale = Main.cursorScale;
            // For some reason cursorAlpha is "flipped", revert it here via some maths (0.8 is the value it fluctuates around)
            float flipped = 2 * 0.8f - Main.cursorAlpha;
            _clickerAlpha = flipped * 0.3f + 0.7f;

            // To safely cache when the cursor is inside an interface (directly accessing it when adding the cursor will not work because the vanilla logic hasn't reached that stage yet)
            _lastMouseInterface = Main.LocalPlayer.mouseInterface;
            //_lastMouseText = Main.mouseText;
            _lastMouseText = Main.hoverItemName != null && Main.hoverItemName != "" && Main.mouseItem?.type == (int?)0; //Use immediate vanilla conditions
        }

        /// <summary>
		/// Helper method that determines when the cursor can be drawn/replaced
		/// </summary>
		public static bool CanDrawCursor(Player player)
        {
            VanillaModdingPlayer modPlayer = player.GetModPlayer<VanillaModdingPlayer>();
            return modPlayer.overrideCursor;
        }

        public static bool ValidCursorConditions(Player player, ModItem item)
        {
            
            return !player.dead && !player.ghost && !_lastMouseInterface && !_lastMouseText && item is ClickerItem clicker && player.position.DistanceSQ(Main.MouseWorld) <= clicker.range * clicker.range;
        }

        protected override bool DrawSelf()
        {
            Player player = Main.LocalPlayer;
            VanillaModdingPlayer modPlayer = player.GetModPlayer<VanillaModdingPlayer>();

            // Don't draw if the player is dead or a ghost
            if (player.dead || player.ghost)
            {
                return true;
            }

            Asset<Texture2D> borderAsset;
            Texture2D borderTexture;
            Texture2D texture;
            //Item item = player.HeldItem;
            int itemType = modPlayer.cursorItem;

            if (!CanDrawCursor(player))
            {
                return true;
            }

            // Actual cursor
            ModItem getCursor = ModContent.GetModItem(itemType);
            texture = (Texture2D)ModContent.Request<Texture2D>($"{nameof(VanillaModding)}/Common/UI/CursorAsset/{getCursor.Name}".Replace(@"\", "/"));//(Texture2D)ModContent.Request<Texture2D>($"{nameof(VanillaModding) + "/" + (getCursor.Texture + "_cursor").Replace(@"\", "/")}");
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
            Vector2 position = Main.MouseScreen - origin + origin / 2;
            Main.spriteBatch.Draw(texture, position, frame, color, 0f, Vector2.Zero, _clickerScale, SpriteEffects.FlipHorizontally, 0);

            detourSecondCursorDraw = true;

            return true;
        }

        public override int GetInsertIndex(List<GameInterfaceLayer> layers)
        {
            string [] layerNames = new string[]
            {
                "Vanilla: Cursor", // Cursor drawn ontop of everything except for Mouse_Text
                "Vanilla: Mouse Over", // Cursor drawn ontop of everything and top for Mouse_Text and etc.
            };
            return layers.FindIndex(layer => layer.Active && layer.Name.Equals("Vanilla: Interface Logic 4"));
            //return layers.FindIndex(layer => layer.Active && layer.Name.Equals("Vanilla: Mouse Text"));
        }
    }
}
