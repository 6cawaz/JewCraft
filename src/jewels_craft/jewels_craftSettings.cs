using PoeHUD.Plugins;
using PoeHUD.Hud.Settings;
using SharpDX;

namespace jewels_craft
{
    public class jewels_craftSettings : SettingsBase
    {
        public jewels_craftSettings()
        {
            vaal = false;
            regal = false;
            alt_augm = false;
            Color_Vaal = new ColorBGRA(255, 0, 0, 255);
            Color_Regal = new ColorBGRA(0, 255, 0, 255);
            Color_Augm = new ColorBGRA(0, 0, 255, 255);
            
            CraftSpeed = new RangeNode<float>(750, 200, 2000);

        }

        [Menu("CraftSpeed", 110)]
        public RangeNode<float> CraftSpeed { get; set; }

        #region Autos

        [Menu("Auto_Vaal:")]
        public ToggleNode vaal { get; set; }

        [Menu("Auto_Regal")]
        public ToggleNode regal { get; set; }

        [Menu("Auto_Alt_Augm")]
        public ToggleNode alt_augm { get; set; }

        #endregion

        #region Colors

        [Menu("Color_Vaal")]
        public ColorNode Color_Vaal { get; set; }

        [Menu("Color_Regal")]
        public ColorNode Color_Regal { get; set; }

        [Menu("Color_Augm")]
        public ColorNode Color_Augm { get; set; }

        #endregion

    }
}