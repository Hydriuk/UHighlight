using Hydriuk.UnturnedModules.Adapters;
using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace UHighlight.RocketMod
{
    public class ConfigurationAdapter : Configuration, IConfigurationAdapter<Configuration>, IRocketPluginConfiguration
    {
        [XmlIgnore]
        public Configuration Configuration { get => this; }

        public void LoadDefaults()
        {
            IconURL = "";
        }
    }
}
