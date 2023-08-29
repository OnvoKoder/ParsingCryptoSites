using ParserNansemAPI.Class.Patterns;
using System.Collections.Generic;

namespace ParserNansemAPI.Class.Interface
{
    public interface IData
    {
        void SaveInFile(ref List<ExcelTable> list);
    }
}
