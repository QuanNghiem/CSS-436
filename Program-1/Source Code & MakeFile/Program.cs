using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;

    class Program
    {
        #region Variables
        // Static variables
        private static HttpClient client = new HttpClient();
        private static List<string> visitedURL = new List<string>();
        private static List<string> arrayOfURL = new List<string>();
        private static string currURL;
        private static string currHTML;
        #endregion

        static void Main(string[] args)
        {
            string url = args[0];
            int numOfHops = int.Parse(args[1]);
            runProgram(url, numOfHops);
        }

        #region Print to Console
        private static void runProgram(string url, int numOfHops)
        {
            currURL = url;
            if (!loadPage(currURL))
            {
                Console.WriteLine("The starting URL provides status code 40x or 50x.");
                return;
            }

            for (int i = 0; i < numOfHops; i++)
            {
                Console.WriteLine("Hop: " + (i+1));
                if (i == numOfHops - 1)
                {
                    loadPage(currURL);
                    Console.WriteLine(currURL);
                    printPage();
                    return;
                }

                loadPage(currURL);
                Console.WriteLine(currURL);

                if (getURLs(currHTML))
                {
                    if (notVisited(arrayOfURL))
                    {
                        continue;
                    }
                    else
                    {
                        printPage();
                        return;
                    }
                }
                else
                {
                    printPage();
                    return;
                }
            }
        }

        private static void printPage()
        {
            Console.WriteLine(currHTML.ToString());
            return;
        }
        #endregion

        #region Load and Verify URL
        private static bool loadPage(string url)
        {
            HttpResponseMessage response = client.GetAsync(url).Result;
            if ((int)response.StatusCode >= 400)
            {
                return false;
            }
            currHTML = response.Content.ReadAsStringAsync().Result;
            return true;
        }

        private static bool notVisited(List<string> input)
        {
            foreach (string s in input)
            {
                string temp;
                if (s.EndsWith("/"))
                {
                    temp = s;
                }
                else
                {
                    temp = s + "/";
                }
                if (!visitedURL.Contains(temp))
                {
                    if (loadPage(temp))
                    {
                        visitedURL.Add(temp);
                        currURL = temp;
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

        #region Get URL from HTML
        private static bool getURLs(string result)
        {
            Regex regex = new Regex("<a (.*\\s)?href=\"?'?(.*?)\"?'?(\\s.*)?>");
            MatchCollection matches = regex.Matches(result);
            arrayOfURL.Clear();
            foreach (Match m in matches)
            {
                if (isValidURL(m.Groups[2].Value))
                {
                    arrayOfURL.Add(m.Groups[2].Value);
                }
            }
            return (arrayOfURL.Count != 0);
        }

        private static bool isValidURL(string url)
        {
        Uri result = null;
            if (Uri.TryCreate(url, UriKind.Absolute, out result))
            {
                if (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        #endregion
    }


