using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Protocol
{
    public class Serializer
    {
        private static readonly char[] separators = {'|', '%', '.'};

        public static string Serialize(object[] objectArray, int level = 0)
        {
            var serializedObjects = new string[objectArray.Length];
            for (var i = 0; i < objectArray.Length; i++)
            {
                if (objectArray[i] is Array)
                    serializedObjects[i] = Serialize((object[]) objectArray[i], level + 1);
                else
                    serializedObjects[i] = (string) objectArray[i];
            }
            
            var levelSeparator = level == 0 ? separators[0].ToString() : "";
            return levelSeparator + string.Join(separators[level].ToString(), serializedObjects) + levelSeparator;
        }

        public static string[][][] DeSerialize(string serializedObj)
        {
            // Regex stops matching when it finds a backslash 
            // so it matches only the unescaped separators
            var trimmedObject = serializedObj.Trim('|');
            var firstLevelObjects = Regex.Split(trimmedObject, @"(?<!\\)\|");
            var secondLevelObjects = new string[firstLevelObjects.Length][];
            for (var i = 0; i < firstLevelObjects.Length; i++)
            {
                secondLevelObjects[i] = Regex.Split(firstLevelObjects[i], @"(?<!\\)\%");
            }
            var thirdLevelObjects = new string[firstLevelObjects.Length][][];
            for (var i = 0; i < firstLevelObjects.Length; i++)
            {
                thirdLevelObjects[i] = new string[secondLevelObjects[i].Length][];
                for (var j = 0; j < secondLevelObjects[i].Length; j++)
                {
                    var serializedObject = secondLevelObjects[i][j];
                    thirdLevelObjects[i][j] = Regex.Split(serializedObject, @"(?<!\\)\.");
                }
            }

            return thirdLevelObjects;
        }
    }
}