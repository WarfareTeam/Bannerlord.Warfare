using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Base.Global;

namespace Bannerlord.Warfare
{
    public class Settings : AttributeGlobalSettings<Settings>
    {
        public Settings()
        {
            //Rmpty
        }

        private bool _logging = false;
        public override string Id => "Warfare";
        public override string DisplayName => $"Warfare";
        public override string FolderName => "Warfare";
        public override string FormatType => "json2";

        [SettingPropertyBool("Logging", Order = 0, RequireRestart = false, HintText = "Logs before and after magnitudes to a file, for testing and reporting purposes. Default: Disabled")]
        [SettingPropertyGroup("Other", GroupOrder = 100)]
        public bool Logging
        {
            get => _logging;
            set
            {
                if (_logging != value)
                {
                    _logging = value;
                    OnPropertyChanged();
                }
            }
        }

        public static Settings Current
        {
            get => Instance!;
        }
    }
}
