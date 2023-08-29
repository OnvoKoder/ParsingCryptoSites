namespace ParserNansemAPI.Class.Patterns
{
    class Settings
    {
        public string Key { get; }
        public Proxy Proxy { get; }
        public string Format { get; }
        public string Site { get; }
        public Settings(string key, Proxy proxy, string format, string site)
        {
            Key = key;
            Proxy = proxy;
            Format = format;
            Site = site;

        }
    }
}
