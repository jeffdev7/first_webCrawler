using System;
using HtmlAgilityPack;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Text.Json;
using System.IO;

namespace first_web_crawler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            StartCrawlerAsync();
            Console.ReadLine();
        }

        private static async Task StartCrawlerAsync()
        {
            var url = "http://www.automobile.tn/neuf/bmw.3";
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var cars = new List<Car>();

            var divs = htmlDocument.DocumentNode.Descendants("div").
                Where(node=> node.GetAttributeValue("class","").
                Equals("versions-item")).ToList();

            foreach (var div in divs)
            {
                var car = new Car
                {
                    Model = div.Descendants("h2").FirstOrDefault().InnerText,
                    Price = div.Descendants("div").FirstOrDefault().InnerText,
                    Link = div.Descendants("a").FirstOrDefault().ChildAttributes("href").FirstOrDefault().Value,
                    ImageUrl = div.Descendants("img").FirstOrDefault().ChildAttributes("src").FirstOrDefault().Value
                };
                cars.Add(car);

                string fileName = "cars.json";
                string json = JsonSerializer.Serialize(car);
                File.WriteAllText(fileName, json);
                Console.WriteLine(File.ReadAllText(fileName));
            }

            MySqlConnection myStringConnection = new MySqlConnection(@"Server=localhost;DataBase=WebCrawler;User=root;Pwd=123456");
            myStringConnection.Open();

            try
            {
                int count = cars.Count;

                foreach (var item in cars)
                {
                    for (int i = 0; i < count; i++)
                    {
                        MySqlCommand cmd = new MySqlCommand("insert into first_web_crawler(model, price, link, imageUrl) values(?,?,?,?);", myStringConnection);
                        cmd.Parameters.Add("?model", MySqlDbType.VarChar).Value = cars[i].Model;
                        cmd.Parameters.Add("?price", MySqlDbType.VarChar).Value = cars[i].Price;
                        cmd.Parameters.Add("?link", MySqlDbType.VarChar).Value = cars[i].Link;
                        cmd.Parameters.Add("?imageUrl", MySqlDbType.VarChar).Value = cars[i].ImageUrl;
                        cmd.ExecuteNonQuery();

                    }

                    count = 0;
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }

            myStringConnection.Close();
            Console.WriteLine("Successful!");
            Console.WriteLine("Press enter to exit the program...");
            ConsoleKeyInfo key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Enter)
            {
                System.Environment.Exit(0);
            }

        }
    }
}
