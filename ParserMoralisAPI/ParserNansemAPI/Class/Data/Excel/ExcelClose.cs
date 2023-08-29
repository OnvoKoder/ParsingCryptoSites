using System.Diagnostics;

namespace ParserNansemAPI.Class.Data.Excel
{
    class ExcelClose
    {
        public void CloseProcess()
        {
            Process[] List;
            List = Process.GetProcessesByName("EXCEL");
            foreach (Process proc in List)
                proc.Kill();
        }
    }
}
