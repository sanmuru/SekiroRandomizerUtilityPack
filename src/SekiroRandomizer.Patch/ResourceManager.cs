using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SekiroRandomizer.Patch
{
    public static class ResourceManager
    {
        public const string Error_MissingDataDirectory = nameof(Error_MissingDataDirectory);
        public const string Log_OptionsAndSeeds = nameof(Log_OptionsAndSeeds);
        public const string Status_LoadingGameData = nameof(Status_LoadingGameData);
        public const string Error_ModsDirectoryNotFound = nameof(Error_ModsDirectoryNotFound);
        public const string Error_AlreadyRunningFromModsDirectory = nameof(Error_AlreadyRunningFromModsDirectory);
        public const string Log_EnemySearchInstruction = nameof(Log_EnemySearchInstruction);
        public const string Log_ItemSearchInstruction = nameof(Log_ItemSearchInstruction);
        public const string Status_RandomizingEnemies = nameof(Status_RandomizingEnemies);
        public const string Status_RandomizingItems = nameof(Status_RandomizingItems);
        public const string Status_EditingGameFiles = nameof(Status_EditingGameFiles);
        public const string Status_WritingGameFiles = nameof(Status_WritingGameFiles);
        public const string Status_Randomizing = nameof(Status_Randomizing);

        public const string Log_UsingModdedFile = nameof(Log_UsingModdedFile);
        public const string Error_MissingParamFiles = nameof(Error_MissingParamFiles);

        public const string Log_EnemyInLocation = nameof(Log_EnemyInLocation);
        public const string Log_EnemyFromLocation = nameof(Log_EnemyFromLocation);

        public const string Log_ReplacingEnemyTo = nameof(Log_ReplacingEnemyTo);

        public static CultureInfo CurrentCulture = CultureInfo.CurrentCulture;

        internal static readonly IDictionary<CultureInfo, IDictionary<string, string>> cultures = new Dictionary<CultureInfo, IDictionary<string, string>>();

        public static string GetResource(string key)
        {
            if (checkCurrentCulture())
            {
                if (cultures[CurrentCulture].TryGetValue(key, out var value))
                    return value;
                else
                    return null;
            }
            else
            {
                return null;
            }
        }

        private static bool checkCurrentCulture()
        {
            var currentCulture = CurrentCulture ?? CultureInfo.CurrentCulture;
            if (cultures.ContainsKey(currentCulture)) return true;
            else
            {
                var dict = LoadFile(currentCulture);
                if (!(dict is null)) { 
                    cultures.Add(currentCulture, dict);
                    return true;
                }
                else return false;
            }
        }

        private static IDictionary<string, string> LoadFile(CultureInfo culture)
        {
            string path = Path.Combine("patch", "dists", culture.Name + ".xml");
            if (!File.Exists(path)) return null;
            XDocument document = XDocument.Load(path);
            var dict = new Dictionary<string, string>();
            if (document.Root.Name != "Dictionary") return null;

            foreach (var element in document.Root.Elements("String"))
            {
                string key = element.Attribute("Key")?.Value;
                string value = element.Value;
                if (key is null) continue;
                else if (value is null)
                {
                    continue;
                }

                dict.Add(key, value);
            }

            return dict;
        }
    }
}
