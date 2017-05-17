using System.Collections.Generic;
using System.Linq;
using System.IO;
using PoeHUD.Framework;
using PoeHUD.Plugins;
using PoeHUD.Poe;
using PoeHUD.Poe.RemoteMemoryObjects;
using PoeHUD.Poe.Elements;
using PoeHUD.Models;
using PoeHUD.Poe.Components;
using PoeHUD.Hud.AdvancedTooltip;
using PoeHUD.Models.Enums;
using PoeHUD.Poe.FilesInMemory;
using Newtonsoft.Json;
using SharpDX;

namespace jewels_craft
{
    public class jewels_craft : BaseSettingsPlugin<jewels_craftSettings>
    {

        private List<RectangleF> _goodItemsPos;
        private Element _curInventRoot;
        private HoverItemIcon _currentHoverItem;

        public override void Initialise()
        {
            var probsFileName = PluginDirectory + @"/affixesProbs.json";
            if (!File.Exists(fileName))
            {
                //Error Log
            }

            string json = File.ReadAllText(fileName);
            _invArr = JsonConvert.DeserializeObject<int[,]>(json);
        }

        public override void Render()
        {
            if (!GameController.Game.IngameState.IngameUi.InventoryPanel.IsVisible)
                return;

            _currentHoverItem = GameController.Game.IngameState.UIHover.AsObject<HoverItemIcon>();

            if (_currentHoverItem.ToolTipType == ToolTipType.InventoryItem && _currentHoverItem.Item != null)
            {
                _curInventRoot = _currentHoverItem.Parent;
            }

            ScanInventory();
        }

        private void ScanInventory()
        {
            if (_curInventRoot == null)
                return;

            var goodToVaal = new List<RectangleF>();
            var goodToRegal = new List<RectangleF>();
            var goodToAlt = new List<RectangleF>();
            var goodToAugm = new List<RectangleF>();

            foreach (var child in _curInventRoot.Children)
            {
                var item = child.AsObject<NormalInventoryItem>().Item;

                BaseItemType bit = GameController.Files.BaseItemTypes.Translate(item.Path);

                var modsComponent = item?.GetComponent<Mods>();
                if (modsComponent.Identified == true && !string.IsNullOrEmpty(item.Path))
                {
                    List<ItemMod> itemMods = modsComponent.ItemMods;
                    List<ModValue> JewelMods =
                    itemMods.Select(
                        it => new ModValue(it, GameController.Files, modsComponent.ItemLevel, GameController.Files.BaseItemTypes.Translate(item.Path))).ToList();
                    var amountPerfect = 0;
                    var affixesAmount = 0;

                    foreach (var mod in JewelMods)
                    {
                        var isPerfect = true;
                        for (int i = 0; i < 4; i++)
                        {

                            IntRange range = mod.Record.StatRange[i];
                            if (range.Min == 0 && range.Max == 0) { continue; }
                            StatsDat.StatRecord stat = mod.Record.StatNames[i];
                            int value = mod.StatValue[i];
                            if (value <= -1000 || stat == null) { continue; }
                            bool noSpread = range.Max != range.Min;

                            if ((mod.CouldHaveTiers() && mod.Tier > 1) || range.Max != value)
                            {
                                isPerfect = false;

                            }
                        }
                        affixesAmount += 1;
                        if (isPerfect)
                            amountPerfect += 1;

                    }

                    var drawRect = child.GetClientRect();

                    if (modsComponent?.ItemRarity == ItemRarity.Rare && amountPerfect == 3)
                    {
                        goodToVaal.Add(drawRect);
                    }
                    if (modsComponent?.ItemRarity == ItemRarity.Magic && amountPerfect == 2)
                    {
                        goodToRegal.Add(drawRect);
                    }
                    else if (modsComponent?.ItemRarity == ItemRarity.Magic && amountPerfect == 1 && affixesAmount == 1)
                    {
                        goodToAugm.Add(drawRect);
                    }

                }
            }

            foreach (var position in goodToVaal)
            {
                RectangleF border = new RectangleF { X = position.X + 6, Y = position.Y + 6, Width = position.Width - 6, Height = position.Height - 6 };
                Graphics.DrawFrame(border, 3, Settings.Color_Vaal);
            }

            foreach (var position in goodToRegal)
            {
                RectangleF border = new RectangleF { X = position.X + 6, Y = position.Y + 6, Width = position.Width - 6, Height = position.Height - 6 };
                Graphics.DrawFrame(border, 3, Settings.Color_Regal);
            }

            foreach (var position in goodToAugm)
            {
                
                RectangleF border = new RectangleF { X = position.X + 6, Y = position.Y + 6, Width = position.Width - 6, Height = position.Height - 6 };
                Graphics.DrawFrame(border, 3, Settings.Color_Augm);
            }

        }

        private static int Average(IReadOnlyList<int> x) => (x[0] + x[1]) / 2;

        private static List<ModValue> SumAffix(List<ModValue> mods)
        {
            foreach (var mod in mods)
            foreach (var mod2 in mods.Where(x => x != mod && mod.Record.Group == x.Record.Group))
            {
                mod2.StatValue[0] += mod.StatValue[0];
                mod2.StatValue[1] += mod.StatValue[1];
                mods.Remove(mod);
                return mods;
            }
            return mods;
        }

        private static int FixTierEs(string key) => 9 - int.Parse(key.Last().ToString());
    }

    public static class ModsExtension
    {
        public static float GetStatValue(this List<ItemMod> mods, string name)
        {
            var m = mods.FirstOrDefault(mod => mod.Name == name);
            return m?.Value1 ?? 0;
        }

        public static float GetAverageStatValue(this List<ItemMod> mods, string name)
        {
            var m = mods.FirstOrDefault(mod => mod.Name == name);
            return (m?.Value1 + m?.Value2) / 2 ?? 0;
        }
    }
}