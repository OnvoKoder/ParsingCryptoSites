using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ParserNansemAPI.Class.Data;
using ParserNansemAPI.Class.Data.Excel;
using ParserNansemAPI.Class.Interface;
using ParserNansemAPI.Class.Patterns;
using ParserNansemAPI.Class.settings;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ParserNansemAPI.Class.Selenium
{
    class ParsingSite : IParsing
    {
        private static List<ExcelTable> list = new List<ExcelTable>();
        private ConfigurationManager manager = new ConfigurationManager();
        private static string link = "";
        public event EventHandler<MessageResult> ProccessCompleted;
        private static int counter = 0;
        private static Mutex mutex = new Mutex(false, "sync");
        private ExcelData excel = new ExcelData();
        private JsonData json = new JsonData();
        /// <summary>
        /// Parsing site by Selenium
        /// </summary>
        /// <param name="address"> link on site</param>
        public void Parsing(string address)
        {
            OnProcessCompleted(new MessageResult($"Start parsing site", false));
            list = manager.GetData();
            link = address;
            Thread[] thread = new Thread[list.Count];
            for (int i = 0; i < thread.Length; i++)
            {
                thread[i] = new Thread(GetLinkTweets);
                thread[i].Name = i.ToString();
                thread[i].Start(i);
            }
        }
        /// <summary>
        /// Parsing nitter on tweet about crypt by by Selenium
        /// </summary>
        /// <param name="index">index of list</param>
        private void GetLinkTweets(object index)
        {
            mutex.WaitOne(); //If you use multi threading instance so much that will be exceptions
            int param = Convert.ToInt32(index);
            OnProcessCompleted(new MessageResult($"Start parsing site: tweets{Environment.NewLine} {list[param].OwnerOf}", false));
            List<string> listTweets = new List<string>();
            IWebDriver driver = new ChromeDriver();
            driver.Url = $"{link}/search?f=tweets&q={list[param].OwnerOf}";
            int count = driver.FindElements(By.XPath("/html/body/div/div/div[2]/div")).Count;
            if (count > 0) //if has tweets on nitter
            {
                for (int i = 1; i < count; i++)
                {
                    IWebElement tweets = driver.FindElement(By.CssSelector($"body > div > div > div.timeline > div:nth-child({i}) > a"));
                    listTweets.Add(tweets.GetAttribute("href"));
                }
                OnProcessCompleted(new MessageResult($"Has {count} tweets by address {count}", false));
            }
            list[param].Link = listTweets.ToArray();
            counter++;
            if(counter == list.Count)//sync complete work
            {
                OnProcessCompleted(new MessageResult($"Complete", false));
                Settings settings = manager.GetCurrentSettings();
                if (settings.Format == "Excel")
                    excel.SaveInFile(ref list);
                else
                    json.SaveInFile(ref list);
                OnProcessCompleted(new MessageResult($"File created", false));
            }
            else
                OnProcessCompleted(new MessageResult($"Running", false));
            driver.Dispose();
            mutex.ReleaseMutex();
            Thread.CurrentThread.Abort();
        }
        /// <summary>
        /// Event on result of work
        /// </summary>
        /// <param name="result">Status on frontend </param>
        public virtual void OnProcessCompleted(MessageResult result) => ProccessCompleted?.Invoke(this, result);
    }
}
