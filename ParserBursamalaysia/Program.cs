using HtmlAgilityPack;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
namespace ParserBursamalaysia
{
    class Program 
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        [STAThread]
        static void Main(string[] args)
        {
            logger.Info("НАЧАЛО РАБОТЫ ПАРСЕРА");
            string url = "https://www.bursamalaysia.com/market_information/equities_prices?page=";
            int page = 1;
            HtmlWeb htmlWeb = new HtmlWeb();
            List<string> data = new List<string>();
            bool exit = true;
            int j;
            string line, name, LastDone ;           
            while (exit)
            {
                exit = false;
                HtmlDocument webDoc = htmlWeb.Load(url + page);
                HtmlNodeCollection nodes = webDoc.DocumentNode.SelectNodes("//tr/td[@class=\"nowrap\"] | //tbody[@class=\" font-xsmall\"]/tr/td ");             
                if (nodes != null)
                {
                    j = 0;
                    line = "";
                    name = "";
                    LastDone = "";
                    foreach (var item in nodes)
                    {
                        switch (j)
                        {
                            case 0:
                                j += 1;
                                break;
                            case 1:
                                name = item.InnerText.Trim().Replace(",", ".") + ", ";
                                j += 1;
                                break;
                            case 2:
                                line += item.InnerText.Trim().Replace(",", ".") + ", " + name;
                                name = "";
                                j += 1;
                                break;
                            case 3:
                                LastDone = item.InnerText.Trim().Replace(",", ".") + ", ";
                                j += 1;
                                break;
                            case 4:
                                line += item.InnerText.Trim().Replace(",", ".") + ", " + LastDone ;
                                j += 1;
                                LastDone = "";
                                break;
                            case 15:
                                data.Add(line);
                                line = "";
                                j = 0;
                                break;
                            case 14:
                                line += item.InnerText.Trim().Replace(",", ".");
                                j += 1;
                                break;
                            default:
                                line += item.InnerText.Trim().Replace(",", ".") + ", ";
                                j += 1;
                                break;
                        }
                    }
                    Console.WriteLine("Загрузка " + page + " страницы завершилось");
                    exit = true;
                    page += 1;                   
                }               
            }
            logger.Info("Начало загрузки данных в файл");
            try
            {
                using (StreamWriter stream = new StreamWriter(args[0] + "\\" + DateTime.Today.ToString("dd MM yyyy").Replace(" ", "") + ".csv"))
                {
                    stream.WriteLine("sep=,");
                    stream.WriteLine("Code, Name, Last done, REM, LACP, CHG, Percent change, VOL, Buy VOL, Buy, Sell, Sell VOL, High, Low");
                    foreach (var item in data)
                    {
                        stream.WriteLine(item);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex);
                logger.Info(ex);
            }           
            logger.Info("ОКОНЧАНИЕ РАБОТЫ ПАРСЕРА");
        }
    }
}
