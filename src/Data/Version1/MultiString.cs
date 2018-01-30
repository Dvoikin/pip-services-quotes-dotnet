using System.Collections.Generic;

namespace PipServices.Quotes.Data.Version1
{
    public class MultiString: Dictionary<string, string>
    {
        public const string English = "en";
        public const string Spanish = "sp";
        public const string French = "fr";
        public const string German = "de";
        public const string Russian = "ru";
        public const string Portuguese = "pt";
        public const string Italian = "it";
        public const string Japanese = "jp";
        public const string Chinese = "ch";

        public string Get(string language)
        {
            string value = null;
            if (TryGetValue(language, out value))
                return value;
            if (language != English && TryGetValue(English, out value))
                return value;
            return null;
        }

        public void Put(string language, string value)
        {
            this[language] = value;
        }
    }
}
