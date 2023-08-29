using ParserNansemAPI.Class.Patterns;
using System;

namespace ParserNansemAPI.Class.Interface
{
    public interface IParsing
    {
        void Parsing(string address);
        event EventHandler<MessageResult> ProccessCompleted;
    }
}
