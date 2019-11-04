using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

class Solution
{

    /*
    * Complete the function below.
    */
    public const string API_URL = "https://jsonmock.hackerrank.com/api/stocks";
    public const string DATE_FORMAT = "d-MMMM-yyyy ";
    public const string SPACE = " ";
    public const string URL_FILTER_PAGE = "/?page=";
    public const string URL_FILTER_YEAR = "/search?date=";


    static void openAndClosePrices(string firstDate, string lastDate, string weekDay)
    {
        if (DateTime.TryParse(firstDate, out DateTime firstDateTime) && DateTime.TryParse(lastDate, out DateTime lastDateTime))
        {
            ConverToDayOfWeek(weekDay);

            var filteredStock = GetAllStock(firstDateTime, lastDateTime)
                                        .Where(x => x.Date >= firstDateTime && x.Date <= lastDateTime && x.Date.DayOfWeek == ConverToDayOfWeek(weekDay));

            foreach (var item in filteredStock)
            {
                Console.WriteLine(item.Date.ToString(DATE_FORMAT + item.Open + SPACE + item.Close));
            }
        }
    }

    static List<StockInformation> GetAllStock(DateTime firstDateTime, DateTime lastDateTime)
    {
        var stockPerPage = new ResponseStock();
        var stock = new List<StockInformation>();
        var pageNumber = 1;
        var urlAux = API_URL;

        if (firstDateTime.Year == lastDateTime.Year)
        {
            urlAux += URL_FILTER_YEAR + firstDateTime.ToString("yyyy") + "&page=";
        }
        else
        {
            urlAux += URL_FILTER_PAGE;
        }
        do
        {
            var request = (HttpWebRequest)WebRequest.Create(urlAux + pageNumber);
            var response = request.GetResponse();

            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);

                stockPerPage = JsonConvert.DeserializeObject<ResponseStock>(reader.ReadToEnd());

                var dataLen = stockPerPage.Data.Count();

                if (stockPerPage.Data.Count() > 0
                    && (firstDateTime.Date <= stockPerPage.Data[dataLen - 1].Date.Date && stockPerPage.Data[0].Date.Date <= lastDateTime.Date ))
                    stock.AddRange(stockPerPage.Data);

                pageNumber++;

                if (stockPerPage.Data[dataLen - 1].Date > lastDateTime)
                {
                    break;
                }
            }
        }
        while (pageNumber <= stockPerPage.Total_pages);

        return stock;
    }

    public class StockInformation
    {
        public DateTime Date { get; set; }
        public string Open { get; set; }
        public string High { get; set; }
        public string Low { get; set; }
        public string Close { get; set; }
    }

    public class ResponseStock
    {
        public int Total_pages { get; set; }
        public List<StockInformation> Data { get; set; }
    }

    public static DayOfWeek ConverToDayOfWeek(string weekDay)
    {
        switch (weekDay)
        {
            case ("Sunday"):
                return DayOfWeek.Sunday;
            case ("Monday"):
                return DayOfWeek.Monday;
            case ("Tuesday"):
                return DayOfWeek.Tuesday;
            case ("Wednesday"):
                return DayOfWeek.Wednesday;
            case ("Thursday"):
                return DayOfWeek.Thursday;
            case ("Friday"):
                return DayOfWeek.Friday;
            case ("Saturday"):
                return DayOfWeek.Saturday;
            default:
                throw new Exception("Invalid Day");
        }
    }

    static void Main(String[] args)
    {
        string _firstDate;
        _firstDate = Console.ReadLine();

        string _lastDate;
        _lastDate = Console.ReadLine();

        string _weekDay;
        _weekDay = Console.ReadLine();

        openAndClosePrices(_firstDate, _lastDate, _weekDay);

    }
}