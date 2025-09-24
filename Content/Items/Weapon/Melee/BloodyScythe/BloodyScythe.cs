using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Projectiles.BloodyScythe;

namespace VanillaModding.Content.Items.Weapon.Melee.BloodyScythe
{
    internal class BloodyScythe : ModItem
    {
        public override void SetStaticDefaults()
        {
            //glowMask = GlowMaskAPI.Tools.instance.AddGlowMask(ModContent.Request<Texture2D>($"{nameof(VanillaModding)}/Items/MightyScythe_Glow", AssetRequestMode.ImmediateLoad).Value);
        }
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.VanillaModding.hjson file.

        public override void SetDefaults()
        {
            Item.damage = 59;

            Item.DamageType = DamageClass.Melee;
            Item.shootSpeed = 8f;
            Item.shoot = ModContent.ProjectileType<BloodyScytheProjectile>();

            Item.width = 66;
            Item.height = 58;

            Item.useTime = 25;
            Item.useAnimation = 25;

            Item.useStyle = ItemUseStyleID.Swing;

            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item71;
            Item.autoReuse = true;

            Item.knockBack = 6;
        }
    }
}
