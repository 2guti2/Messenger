using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI
{
    public class ClientUI
    {
        public static string WelcomeMessage()
        {
            return Resources.Welcome;
        }

        public static string Title()
        {
            string t = "";
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
    }
}
