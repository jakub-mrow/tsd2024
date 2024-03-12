using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GoldSavings.App.Model;

namespace GoldSavings.App.Client;

public class GoldClient
{
    private HttpClient _client;
    public GoldClient()
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri("http://api.nbp.pl/api/");
        _client.DefaultRequestHeaders.Accept.Clear();

    }
    public async Task<GoldPrice> GetCurrentGoldPrice()
    {
        HttpResponseMessage responseMsg = _client.GetAsync("cenyzlota/").GetAwaiter().GetResult();
        if (responseMsg.IsSuccessStatusCode)
        {
            string content = await responseMsg.Content.ReadAsStringAsync();
            List<GoldPrice> prices = JsonConvert.DeserializeObject<List<GoldPrice>>(content);
            if (prices != null && prices.Count == 1)
            {
                return prices[0];
            }
        }
        return null;
    }

    public async Task<List<GoldPrice>> GetGoldPrices(DateTime startDate, DateTime endDate)
    {
        string dateFormat = "yyyy-MM-dd";
        string requestUri = $"cenyzlota/{startDate.ToString(dateFormat)}/{endDate.ToString(dateFormat)}";
        HttpResponseMessage responseMsg = _client.GetAsync(requestUri).GetAwaiter().GetResult();
        if (responseMsg.IsSuccessStatusCode)
        {
            string content = await responseMsg.Content.ReadAsStringAsync();
            List<GoldPrice> prices = JsonConvert.DeserializeObject<List<GoldPrice>>(content);
            return prices;
        }
        else
        {
            return null;
        }

    }

    public async Task<List<GoldPrice>> GetGoldPricesLastYear()
    {
        DateTime endDate = DateTime.Now.Date;
        DateTime startDate = endDate.AddYears(-1);

        return await GetGoldPrices(startDate, endDate);
    }

    public async Task<List<GoldPrice>> GetPricesFromWholeYear(int year)
    {
        DateTime startDate = new DateTime(year, 01, 01);
        DateTime endDate = new DateTime(year, 12, 31);

        // create intervals that differ by 82 days
        List<GoldPrice> prices = new List<GoldPrice>();
        while (startDate < endDate)
        {
            DateTime nextDate = startDate.AddDays(82);
            if (nextDate > endDate)
            {
                nextDate = endDate;
            }
            List<GoldPrice> intervalPrices = await GetGoldPrices(startDate, nextDate);
            prices.AddRange(intervalPrices);
            startDate = nextDate.AddDays(1);
        }

        return prices;
    }

    public async Task<List<GoldPrice>> GetPricesFromYears(int startYear, int endYear)
    {
        List<GoldPrice> prices = new List<GoldPrice>();
        for (int year = startYear; year <= endYear; year++)
        {
            List<GoldPrice> yearPrices = await GetPricesFromWholeYear(year);
            prices.AddRange(yearPrices);
        }
        return prices;
    }

    public async Task<double> CalculateAveragePrice(int year)
    {
        List<GoldPrice> prices = await GetPricesFromWholeYear(year);
        double sum = 0;
        foreach (var price in prices)
        {
            sum += price.Price;
        }
        return sum / prices.Count;
    }

    // When it would be best to buy gold and sell it between 2019 and 2023? What would be the return on investment?
    public async Task<GoldPrice> GetBestTimeToBuyAndSell()
    {
        List<GoldPrice> prices = new List<GoldPrice>();
        for (int year = 2019; year <= 2023; year++)
        {
            List<GoldPrice> yearPrices = await GetPricesFromWholeYear(year);
            prices.AddRange(yearPrices);
        }

        GoldPrice buyPrice = prices[0];
        GoldPrice sellPrice = prices[0];
        double maxProfit = 0;
        for (int i = 0; i < prices.Count; i++)
        {
            if (prices[i].Price < buyPrice.Price)
            {
                buyPrice = prices[i];
            }
            if (prices[i].Price > sellPrice.Price)
            {
                sellPrice = prices[i];
            }
            if (sellPrice.Price - buyPrice.Price > maxProfit)
            {
                maxProfit = sellPrice.Price - buyPrice.Price;
            }
        }
        Console.WriteLine($"The best time to buy is {buyPrice.Date} and the best time to sell is {sellPrice.Date}");
        Console.WriteLine($"The return on investment would be {maxProfit}: buy price {buyPrice.Price} and sell price {sellPrice.Price}");
        return buyPrice;
    }



}