using GoldSavings.App.Model;
using GoldSavings.App.Client;
namespace GoldSavings.App;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, Gold Saver!");

        GoldClient goldClient = new GoldClient();

        GoldPrice currentPrice = goldClient.GetCurrentGoldPrice().GetAwaiter().GetResult();
        Console.WriteLine($"The price for today is {currentPrice.Price}");

        List<GoldPrice> thisMonthPrices = goldClient.GetGoldPrices(new DateTime(2024, 03, 01), new DateTime(2024, 03, 11)).GetAwaiter().GetResult();
        foreach (var goldPrice in thisMonthPrices)
        {
            Console.WriteLine($"The price for {goldPrice.Date} is {goldPrice.Price}");
        }

        List<GoldPrice> lastYearPrices = goldClient.GetGoldPricesLastYear().GetAwaiter().GetResult();

        // get three highest prices
        var highestPrices = lastYearPrices.OrderByDescending(p => p.Price).Take(3);
        for (int i = 0; i < 3; i++)
        {
            Console.WriteLine($"The {i + 1} highest price is {highestPrices.ElementAt(i).Price}");
        }
        // get three highest prices with linq
        var highestPricesLinq = (from p in lastYearPrices
                                 orderby p.Price descending
                                 select p).Take(3);

        foreach (var goldPrice in highestPricesLinq)
        {
            Console.WriteLine($"The highest price is {goldPrice.Price}");
        }

        // get three lowest prices
        var lowestPrices = lastYearPrices.OrderBy(p => p.Price).Take(3);
        for (int i = 0; i < 3; i++)
        {
            Console.WriteLine($"The {i + 1} lowest price is {lowestPrices.ElementAt(i).Price}");
        }

        // get three lowest prices with linq
        var lowestPricesLinq = (from p in lastYearPrices
                                orderby p.Price
                                select p).Take(3);
        foreach (var goldPrice in lowestPricesLinq)
        {
            Console.WriteLine($"The lowest price is {goldPrice.Price}");
        }

        // Task 4
        // get all the values from 2020 january and check if comparing to 2020 01 01 the price is 1.05 time higher
        Console.WriteLine("Task 4 -----------------------");

        GoldPrice priceFirstJan2020 = goldClient.GetGoldPrices(new DateTime(2020, 01, 01), new DateTime(2020, 01, 02)).GetAwaiter().GetResult().First();

        List<GoldPrice> january2020Prices = goldClient.GetGoldPrices(new DateTime(2020, 01, 01), new DateTime(2020, 01, 31)).GetAwaiter().GetResult();


        foreach (GoldPrice price in january2020Prices)
        {
            if (price.Price >= priceFirstJan2020.Price * 1.05)
            {
                Console.WriteLine($"The price for {price.Date} is {price.Price} and is 1.05 times higher than the price for 2020, 01, 01");
            }
        }

        // Task 5
        
        Console.WriteLine("Task 5 -----------------------");
        List<GoldPrice> pricesTop = goldClient.GetPricesFromYears(2019, 2022).GetAwaiter().GetResult();

        // sort descending by price
        var sortedPricesTop = pricesTop.OrderByDescending(p => p.Price);

        // skip 10 and take 3 from sortedPricesTop
        var top3Prices = sortedPricesTop.Skip(10).Take(3);

        // using linq
        var top3PricesLinq = (from p in pricesTop
                              orderby p.Price descending
                              select p).Skip(10).Take(3);

        foreach (var goldPrice in top3PricesLinq)
        {
            Console.WriteLine($"The price for {goldPrice.Date} is {goldPrice.Price}");
        }

        foreach (var goldPrice in top3Prices)
        {
            Console.WriteLine($"The price for {goldPrice.Date} is {goldPrice.Price}");
        }


        // Task 6
        Console.WriteLine("Task 6 -----------------------");
        List<GoldPrice> prices2020 = goldClient.GetPricesFromWholeYear(2021).GetAwaiter().GetResult();

        // calculate average price from 2021 2022 2023
        double averagePrice2021 = goldClient.CalculateAveragePrice(2021).GetAwaiter().GetResult();
        double averagePrice2022 = goldClient.CalculateAveragePrice(2022).GetAwaiter().GetResult();
        double averagePrice2023 = goldClient.CalculateAveragePrice(2023).GetAwaiter().GetResult();

        Console.WriteLine($"The average price for 2021 is {averagePrice2021}");
        Console.WriteLine($"The average price for 2022 is {averagePrice2022}");
        Console.WriteLine($"The average price for 2023 is {averagePrice2023}");

        // Task 7 
        Console.WriteLine("Task 7 -----------------------");
        GoldPrice bestTimeToBuyAndSell = goldClient.GetBestTimeToBuyAndSell().GetAwaiter().GetResult();


        // save to file
        savetoXML(lastYearPrices);

        List<GoldPrice> newPrices = readFromXML();

        // foreach(var price in newPrices)
        // {
        //     Console.WriteLine($"The price for {price.Date} is {price.Price}");
        // }

    

    }
    static void savetoXML(List<GoldPrice> prices)
    {
        var xml = new System.Xml.Serialization.XmlSerializer(typeof(List<GoldPrice>));
        using (var stream = new System.IO.FileStream("prices.xml", System.IO.FileMode.Create))
        {
            xml.Serialize(stream, prices);
        }
    }

    static List<GoldPrice> readFromXML()
    {
        var xml = new System.Xml.Serialization.XmlSerializer(typeof(List<GoldPrice>));
        using (var stream = new System.IO.FileStream("prices.xml", System.IO.FileMode.Open))
        {
            return (List<GoldPrice>)xml.Deserialize(stream);
        }
    }
}
