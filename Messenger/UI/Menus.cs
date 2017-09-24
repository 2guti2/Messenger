using System;
using System.Collections.Generic;

namespace UI
{
    public static class Menus
    {
        public static string SelectRequest(string[][] requests)
        {
            Console.WriteLine("Friend Requests");
            for (var i = 0; i < requests.Length; i++)
            {
                Console.WriteLine(i + 1 + " - " + requests[i][1]);
            }
            int option = Input.SelectOption("Select a request to accept", 1, requests.Length);

            return requests[option - 1][0];
        }

        public static int MainMenu(List<string> options)
        {
            for (int i = 0; i < options.Count; i++)
            {
                Console.WriteLine(i + 1 + " - " + options[i]);
            }

            return Input.SelectOption("Select an option", 1, options.Count);
        }
    }
}