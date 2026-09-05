using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Items.Materials;
using VanillaModding.Content.Rarities;

namespace VanillaModding.Content.Items.Weapon.Melee
{
    public class BasicSword : ModItem
    {
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.VanillaModding.hjson file.

        public override void SetDefaults()
        {
            Item.damage = 1;
            Item.DamageType = DamageClass.Melee;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 0;
            Item.value = int.MaxValue;
            Item.rare = ModContent.RarityType<HoloRainbow>();
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.DefenseEffectiveness *= 0;
            modifiers.FinalDamage.Base = !target.immortal && !target.dontTakeDamage ? target.lifeMax : 0;
            for (int i = 0; i < 25; i++) Dust.NewDustDirect(target.position, target.width, target.height, DustID.Dirt, Main.rand.NextFloat(-2f, 2f), -8f, 100, default, Main.rand.NextFloat(0.5f, 2f));
        }

        public override void ModifyHitPvp(Player player, Player target, ref Player.HurtModifiers modifiers)
        {
            for (int i = 0; i < 25; i++) Dust.NewDustDirect(target.position, target.width, target.height, DustID.Dirt, Main.rand.NextFloat(-2f, 2f), -8f, 100, default, Main.rand.NextFloat(0.5f, 2f));

            modifiers.FinalDamage.Base = !target.immune && !target.dead ? target.statLife : 0;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DirtiestBlock, 2)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}