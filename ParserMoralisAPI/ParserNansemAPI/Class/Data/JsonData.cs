using Newtonsoft.Json;
using ParserNansemAPI.Class.Interface;
using ParserNansemAPI.Class.Patterns;
using System;
using System.Collections.Generic;
using System.IO;

namespace ParserNansemAPI.Class.Data
{
    internal class JsonData : IData
    {
        public void SaveInFile(ref List<ExcelTable> list)=> File.WriteAllText($"data.json", JsonConvert.SerializeObject(list));
    }
}
