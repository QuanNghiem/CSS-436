using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Program_2
{
    class Program
    {
        #region Attributes
        // Openweather
        private static readonly string key = "Key";
        private static string currLocation;
        private static string openWeatherSearch;

        // Geonames
        private static readonly string username = "usernamebro";
        private static string currCountry;
        private static string geoNamesSearch;

        // Result
        private static HttpResponseMessage response = new HttpResponseMessage();
        private static string result;
        private static bool isSuccess = true;
        private static Forecast.Rootobject weatherDetails;
        private static Facts.Rootobject factDetails;

        // Retry logic
        private static readonly int retry = 3;
        //private static int delayTime = 1000;

        // Output format
        #endregion

        static void Main(string[] args)
        {
            string city;
            if (args.Length == 1)
            {
                city = args[0];

            }
            else
            {
                city = args[0];
                for (int i = 1; i < args.Length; i++)
                {
                    city = city + " " + args[i];
                }
            }
            // Get information for weather
            printWeatherData(city);
            Console.WriteLine(" ");
            printGeoData();
        }

        #region Print out
        private static void printWeatherData(string city)
        {
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine(">> GETTING FORECAST FROM " + city.ToUpper() + ".");
            initWeather(city);
            Console.WriteLine(" - Connecting to the Weather API.");
            getData(openWeatherSearch, retry).Wait();
            if (isSuccess)
            {
                weatherDetails = JsonConvert.DeserializeObject<Forecast.Rootobject>(result);
                currCountry = weatherDetails.sys.country;
                Console.WriteLine("     Weather forecast today for " + weatherDetails.name + ", " + weatherDetails.sys.country + " is " + weatherDetails.weather[0].description + ".");
                Console.WriteLine("     Current temperature " + weatherDetails.name + ", " + weatherDetails.sys.country + " is " + weatherDetails.main.temp + " °C.");
                Console.WriteLine("     Max temperature of " + weatherDetails.name + ", " + weatherDetails.sys.country + " is " + weatherDetails.main.temp_max + " °C.");
                Console.WriteLine("     Minimun temperature of " + weatherDetails.name + ", " + weatherDetails.sys.country + " is " + weatherDetails.main.temp_min + " °C.");
                Console.WriteLine("     Currently, temperature of " + weatherDetails.name + ", " + weatherDetails.sys.country + " feels like " + weatherDetails.main.feels_like + "°C.");
                Console.WriteLine("     Currently, humidity of " + weatherDetails.name + ", " + weatherDetails.sys.country + " is " + weatherDetails.main.humidity + "%.");
                Console.WriteLine("     Wind speed of " + weatherDetails.name + ", " + weatherDetails.sys.country + " is " + weatherDetails.wind.speed + "m/s.");
            }
            else
            {
                currCountry = null;
                Console.WriteLine(" - Failed to get information about " + city.ToUpper() + ".");
            }
        }
        private static void printGeoData()
        {
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine(">> GETTING A FACT FROM " + currLocation.ToUpper() + ".");
            initFact();
            Console.WriteLine(" - Connecting to the Fact API.");
            getData(geoNamesSearch, retry).Wait();
            if (isSuccess)
            {
                factDetails = JsonConvert.DeserializeObject<Facts.Rootobject>(result);
                if (factDetails.geonames.Length != 0)
                {
                    Console.WriteLine("     " + factDetails.geonames[0].name + " is in " + factDetails.geonames[0].countryName + ".");
                    Console.WriteLine("     Population of " + factDetails.geonames[0].name + ", " + factDetails.geonames[0].countryCode + " is " + factDetails.geonames[0].population + " people.");
                }
                else
                {
                    Console.WriteLine(" - This city doesn't exist. You can try to build one lol.");
                }
            }
            else
            {
                Console.WriteLine(" - Failed to connect to the API.");
            }
        }
        #endregion

        #region Create the links
        private static void initWeather(string location)
        {
            // Get rid of all the space by replacing it with %20
            currLocation = location;
            // Combine the string into a link
            openWeatherSearch = "http://api.openweathermap.org/data/2.5/weather?q=" + location.Replace(" ", "%20") + "&units=metric&mode=json&APPID=" + key;
            return;
        }
        private static void initFact()
        {
            if (currCountry != null)
            {
                // Combine the string into a link
                geoNamesSearch = "http://api.geonames.org/searchJSON?name_equals=" + currLocation + "&country=" + currCountry + "&username=" + username;
            }
            else
            {
                geoNamesSearch = "http://api.geonames.org/searchJSON?name_equals=" + currLocation + "&username=" + username;
            }
            return;
        }
        #endregion

        #region Retry logic
        private static async Task getData(string search, int retryCount)
        {
            int currentRetry = 0;
            int delayTime = 1000;
            for (; ; )
            {
                if (!connectWebsite(search))
                {
                    currentRetry++;
                    if (currentRetry > retryCount)
                    {
                        return;
                    }
                    int delay = delayTime;
                    delayTime += (delay ^ 2) - 2;
                    Console.WriteLine(" - Retrying in " + delayTime);
                    await Task.Delay(delayTime);
                }
                else
                {
                    result = response.Content.ReadAsStringAsync().Result;
                    return;
                }
            }

        }
        private static bool connectWebsite(string search)
        {
            HttpClient client = new HttpClient();
            response = client.GetAsync(search).Result;
            isSuccess = response.IsSuccessStatusCode;
            return isSuccess;
        }
        #endregion
    }
}

