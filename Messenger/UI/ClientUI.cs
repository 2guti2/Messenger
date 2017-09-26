using System;
using System.Collections.Generic;

namespace UI
{
    public class ClientUI
    {
        public static string WelcomeMessage()
        {
            return Resources.Welcome;
        }

        public static string Title(List<int> notifications = null, string username = null)
        {
            string t = "";
            if (username != null) t += "You are connected as : " + username + "\n";
            if (notifications != null)
                t += "You have " + notifications[0] + " friendship requests pending.\n";
            t += " __  __\n";
            t += "|  \\/  |\n";
            t += "| \\  / | ___  ___ ___  ___ _ __   __ _  ___ _ __ \n";
            t += "| |\\/| |/ _ \\/ __/ __|/ _ \\ '_ \\ / _` |/ _ \\ '__|\n";
            t += "| |  | |  __/\\__ \\__ \\  __/ | | | (_| |  __/ | \n";
            t += "|_|  |_|\\___||___/___/\\___|_| |_|\\__, |\\___|_|\n";
            t += "                                  __/ |\n";
            t += "                                 |___/ \n";

            return t;
        }

        public static string CallToAction()
        {
            return Resources.PressKeyToContinue;
        }

        public static string Connecting()
        {
            return Resources.ConnectingToServer;
        }

        public static string LoginTitle()
        {
            return "Log In: ";
        }

        public static string InsertUsername()
        {
            return "Insert Username: ";
        }

        public static string InsertPassword()
        {
            return "Insert Password: ";
        }

        public static string InvalidCredentials()
        {
            return "Wrong username or password";
        }

        public static string TheseAreTheConnectedUsers()
        {
            return "This are the connected users:";
        }

        public static string LoginSuccessful()
        {
            return "Logged in successfully";
        }

        public static string PromptUsername()
        {
            return "Enter a username";
        }
        public static void Clear()
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }
    }
}