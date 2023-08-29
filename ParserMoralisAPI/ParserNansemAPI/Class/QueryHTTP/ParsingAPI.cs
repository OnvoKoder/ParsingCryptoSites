using Newtonsoft.Json;
using ParserNansemAPI.Class.Interface;
using ParserNansemAPI.Class.Patterns;
using ParserNansemAPI.Class.settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace ParserNansemAPI.Class.QueryHtttp
{
    class ParsingAPI : IParsing
    {
        private static QueryFirst query = null;
        private readonly static Settings settings;
        private readonly static ConfigurationManager manager = new ConfigurationManager();
        private static int counter = 0;
        public event EventHandler<MessageResult> ProccessCompleted;
        static ParsingAPI() => settings = manager.GetCurrentSettings();
        /// <summary>
        /// Parsing Moralis API by http query
        /// </summary>
        /// <param name="address">address for api</param>
        public void Parsing(string address)
        {
            GetOwnerByAddress(address);
            if (query == null)//if query not result
                return;
            counter = query.result.Length;
            Thread[] thread = new Thread[query.result.Length];
            for (int i = 0; i < query.result.Length; i++)
            {
                thread[i] = new Thread(ENSLookupByAddress);
                thread[i].IsBackground = true;
                thread[i].Name = i.ToString();
                thread[i].Start(i);
            }
        }
        /// <summary>
        /// Get owner by address. Can be return null
        /// </summary>
        /// <param name="address">address for api</param>
        private void GetOwnerByAddress(string address)
        {
            OnProcessCompleted(new MessageResult("Запрос на получение списка владельцев", false));
            string link = $"https://deep-index.moralis.io/api/v2/nft/{address}/owners?chain=eth&format=decimal&media_items=false";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(link);
            request.Headers.Add("X-API-Key", settings.Key);
            request.Accept = "application/json";
            if (settings.Proxy.Enable == true)//check on proxy
                request.Proxy = new WebProxy($"{settings.Proxy.Address}:{settings.Proxy.Port}/", true, null);
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    string result = "";
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        result = reader.ReadToEnd();
                    query = JsonConvert.DeserializeObject<QueryFirst>(result);
                    query.result = query.result.ToList().Distinct().ToArray();
                    OnProcessCompleted(new MessageResult("OK", false));
                }             
            }
            catch (WebException ex)//error is only badly status (400, 401, 404, 429)
            {
                OnProcessCompleted(new MessageResult(ex.Message, true));
            }
        }
        /// <summary>
        /// Get ens by owner's address
        /// </summary>
        /// <param name="index"> index for list of owner</param>
        private void ENSLookupByAddress(object index)
        {
            string link = "";
            string line = "";
            int param = Convert.ToInt32(index);
            OnProcessCompleted(new MessageResult($"Query to get owner's ENS {Environment.NewLine}{query.result[param].owner_of}", false));
            link = $"https://deep-index.moralis.io/api/v2/resolve/{query.result[param].owner_of}/reverse";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(link);
            if (settings.Proxy.Enable == true)
                request.Proxy = new WebProxy($"{settings.Proxy.Address}:{settings.Proxy.Port}/", true, null);
            else
                request.Proxy = null;
            request.Accept = "application/json";
            request.Headers.Add("X-API-Key", settings.Key);
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        line = reader.ReadToEnd();
                    ENS ens = JsonConvert.DeserializeObject<ENS>(line);
                    ExcelTable table = new ExcelTable { OwnerOf = query.result[param].owner_of, ENS = ens.name };
                    GetBalanceByWallet(ref table);
                    GetTransactionByWallet(ref table);
                    manager.SaveData(table);
                    if (manager.GetCount() == counter)//sync complete work
                        OnProcessCompleted(new MessageResult("Complete", false));
                    else
                        OnProcessCompleted(new MessageResult("ОК", false));
                    Thread.CurrentThread.Abort();
                }
            }
            catch (WebException ex)
            {
                OnProcessCompleted(new MessageResult(ex.Message, true));
                counter--;
            }
        }
        /// <summary>
        /// Get ens by owner's balance
        /// </summary>
        /// <param name="index"> index for list of owner</param>
        private void GetBalanceByWallet(ref ExcelTable excel)
        {
            string link = "";
            string result =  "";
            try
            {
                OnProcessCompleted(new MessageResult($"Query to get owner's balance {Environment.NewLine}{excel.OwnerOf}", false));
                link = $"https://deep-index.moralis.io/api/v2/{excel.OwnerOf}/balance?chain=eth";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(link);
                if (settings.Proxy.Enable == true)
                    request.Proxy = new WebProxy($"{settings.Proxy.Address}:{settings.Proxy.Port}/", true, null);
                else
                    request.Proxy = null;
                request.Accept = "application/json";
                request.Headers.Add("X-API-Key", settings.Key);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        result = reader.ReadToEnd();
                    Balance balance = JsonConvert.DeserializeObject<Balance>(result);
                    excel.Balance = balance.balance;
                    OnProcessCompleted(new MessageResult("OK", false));
                }
                
            }
            catch (WebException ex)
            {
                OnProcessCompleted(new MessageResult(ex.Message, true));
            }
        }
        /// <summary>
        /// Get ens by owner's transactions
        /// </summary>
        /// <param name="index"> index for list of owner</param>
        private void GetTransactionByWallet(ref ExcelTable excel)
        {
            string link = "";
            string result = "";
            List<string> list = new List<string>();
            try
            {
                OnProcessCompleted(new MessageResult($"Query to get owner's transaction {Environment.NewLine}{excel.OwnerOf}", false));
                link = $"https://deep-index.moralis.io/api/v2/{excel.OwnerOf}?chain=eth";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(link);
                if (settings.Proxy.Enable == true)
                    request.Proxy = new WebProxy($"{settings.Proxy.Address}:{settings.Proxy.Port}/", true, null);
                else
                    request.Proxy = null;
                request.Accept = "application/json";
                request.Headers.Add("X-API-Key", settings.Key);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        result = reader.ReadToEnd();
                    QueryFour transaction = JsonConvert.DeserializeObject<QueryFour>(result);
                    foreach(Transaction transact in transaction.result)
                    {
                        if (transact.from_address == excel.OwnerOf)
                            list.Add(transact.to_address);
                        else
                            list.Add(transact.to_address);
                    }
                    excel.AddressTransactions = list.Distinct().ToArray();
                    OnProcessCompleted(new MessageResult("OK", false));
                }
            }
            catch (WebException ex)
            {
                OnProcessCompleted(new MessageResult(ex.Message, true));
            }
        }
        /// <summary>
        /// Event on result of work
        /// </summary>
        /// <param name="result">Status on frontend </param>
        public virtual void OnProcessCompleted(MessageResult result) => ProccessCompleted?.Invoke(this, result);
    }
}
