namespace ParserNansemAPI.Class.Patterns
{
    class Proxy
    {
        public bool Enable { get;  }
        public string Address { get;  }
        public string Port { get;  }
        public Proxy(bool enable, string address, string port)
        {
            Enable = enable;
            Address = address;
            Port = port;
        }
    }
}
