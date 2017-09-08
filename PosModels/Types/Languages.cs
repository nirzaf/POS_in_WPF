namespace PosModels.Types
{
    public enum Languages
    {
#if DEBUG
        Debug = 0,
#endif
        English = 1,
        Spanish = 2,
        French = 3,
        Italian = 4,
        German = 5,
        Portuguese = 6,
        Dutch = 7,
    }

    public static class LanguagesExtenstions
    {
        public static string GetAbbreviatedCode(this Languages language)
        {
            switch (language)
            {
                case Languages.English:
                    return "en";
                case Languages.French:
                    return "fr";
                case Languages.Spanish:
                    return "es";
                case Languages.Italian:
                    return "it";
                case Languages.German:
                    return "de";
                case Languages.Dutch:
                    return "nl";
                case Languages.Portuguese:
                    return "pt";
#if DEBUG
                case Languages.Debug:
                    return "en";
#endif
            }
            return null;
        }
    }
}
