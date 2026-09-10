using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace VanillaModding.Common.Systems
{
    internal class VanillaModdingRecipeGroupID
    {
        public static int AnyCopperBar { get; private set; }
        public static int AnyIronBar { get; private set; }
        public static int AnySilverBar { get; private set; }
        public static int AnyPlatinumBar { get; private set; }
        public static int AnyCobaltBar { get; private set; }
        public static int AnyMythrilBar { get; private set; }
        public static int AnyAdamantiteBar { get; private set; }

        public static void RegisterRecipeGroups()
        {
            AnyCopperBar = RecipeGroup.RegisterGroup(nameof(ItemID.TinBar),
                new RecipeGroup(
                    () => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.TinBar)}",
                    ItemID.TinBar, ItemID.CopperBar
                )
            );
            AnyIronBar = RecipeGroup.RegisterGroup(nameof(ItemID.IronBar),
                new RecipeGroup(
                    () => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.IronBar)}",
                    ItemID.IronBar, ItemID.LeadBar
                )
            );
            AnySilverBar = RecipeGroup.RegisterGroup(nameof(ItemID.SilverBar),
                new RecipeGroup(
                    () => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.SilverBar)}",
                    ItemID.SilverBar, ItemID.TungstenBar
                )
            );
            AnyPlatinumBar = RecipeGroup.RegisterGroup(nameof(ItemID.PlatinumBar),
                new RecipeGroup(
                    () => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.PlatinumBar)}",
                    ItemID.PlatinumBar, ItemID.GoldBar
                )
            );
            AnyCobaltBar = RecipeGroup.RegisterGroup(nameof(ItemID.CobaltBar),
                new RecipeGroup(
                    () => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.CobaltBar)}",
                    ItemID.CobaltBar, ItemID.PalladiumBar
                )
            );
            AnyMythrilBar = RecipeGroup.RegisterGroup(nameof(ItemID.MythrilBar),
                new RecipeGroup(
                    () => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.MythrilBar)}",
                    ItemID.MythrilBar, ItemID.OrichalcumBar
                )
            );
            AnyAdamantiteBar = RecipeGroup.RegisterGroup(nameof(ItemID.AdamantiteBar),
                new RecipeGroup(
                    () => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.AdamantiteBar)}",
                    ItemID.AdamantiteBar, ItemID.TitaniumBar
                )
            );
        }
    }
}
