using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using static Hate.Tools;

namespace PasswordGen
{
    class Program
    {
        static void Main(string[] args)
        {
            int wordCount = 0,
                numberCount = 0,
                specialCount = 0;
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
            if (args.Length == 0)
            {
                Console.WriteLine("passGen");
                Console.WriteLine("Usage: passGen <wordCount> [<numberCount> [<specialCount>]] [args]");
                Console.WriteLine();
                Console.WriteLine("wordCount:   The number of words to use in the password. Minimum 1.");
                Console.WriteLine("numberCount: The number of numbers to use in the password. Minimum 0.");
                Console.WriteLine("specialCount: The number of special characters (!,.?\'\"`~@#$%^&*-_=+\\/|()[]{}<>) to use in the password. Minimum 0.");
                Console.WriteLine("args:        Include the names of any of the following after your initial numeric argument(s).");
                Console.WriteLine("      leet:  Instead of including the numbers and special characters at the end of the password, replace random letters in the password with them." + Environment.NewLine +
                    "WARNING: If there are fewer leetable letters in the password than numbers specified in numberCount and specialCount, either numberCount or specialCount will be reduced to the maximum number of leetable letters, and the other will be reduced to zero.");
                Console.WriteLine("      caps:  Capitalize the first letter of each word.");
                Console.WriteLine("      camel: Capitalize the first letter of each word, except the first word.");
                Console.WriteLine("      leetcaps: capitalized letters will be leeted, as well as lowercase letters.");
                return;
            }
            else if (!int.TryParse(args[0], out wordCount) || wordCount < 1)
            {
                throw new ArgumentException("The argument was invalid, or was out of range.", "wordCount");
            }
            else if (args.Length <= 1 || !int.TryParse(args[1], out numberCount) || numberCount < 0)
            {
                numberCount = 0;
            }
            else if (args.Length <= 2 || !int.TryParse(args[2], out specialCount) || specialCount < 0)
            {
                specialCount = 0;
            }
            bool leet = args.Contains("leet") | args.Contains("leetcaps"),
            caps = args.Contains("caps"),
            camel = args.Contains("camel"),
            leetcaps = args.Contains("leetcaps");

            string[] nouns = File.ReadAllLines("nouns.txt"),
            adjectives = File.ReadAllLines("adjectives.txt"),
            both = new string[nouns.Length + adjectives.Length];
            adjectives.CopyTo(both, 0);
            nouns.CopyTo(both, adjectives.Length);

            Random rng = new Random();
            Dictionary<char, char> leetChars = new Dictionary<char, char>()
            {
                { 'a', '4' },
                { 'b', '6' },
                { 'e', '3' },
                { 'i', '1' },
                { 'l', '1' },
                { 'o', '0' },
                { 's', '5' },
                { 't', '7' },
                { 'y', '7' }
            };
            Dictionary<char, char> leetSpecialChars = new Dictionary<char, char>()
            {
                { 'a', '@' },
                { 'c', '(' },
                { 'i', '!' },
                { 'k', '{' },
                { 'l', '|' },
                { 'p', '?' },
                { 's', '$' }
            };
            char[] specialChars = "!,.?\'\"`~@#$%^&*-_=+\\/|()[]{}<>".ToCharArray();

            string password = both.RandomItem();
            wordCount--;
            if (caps)
            {
                password = password.Capitalize();
            }
            if (caps || camel)
            {
                for (int i = 0; i < wordCount; i++)
                {
                    password += nouns.RandomItem().Capitalize();
                }
            }
            else
            {
                for (int i = 0; i < wordCount; i++)
                {
                    password += nouns.RandomItem();
                }
            }

            if (leet)
            {
                int specialLeetableChars = password.Count(x => leetSpecialChars.ContainsKey(leetcaps ? x.ToString().ToLower()[0] : x));
                if (specialLeetableChars <= specialCount)
                {
                    for (int i = 0; i < password.Length; i++)
                    {
                        char charValue = leetcaps ? password[i].ToString().ToLower()[0] : password[i];
                        if (leetSpecialChars.ContainsKey(charValue))
                        {
                            password = password.ReplaceChar(i, leetSpecialChars[charValue].ToString());
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < specialCount; i++)
                    {
                        int charLoc = rng.Next(password.Length - 1);
                        char charValue = leetcaps ? password[charLoc].ToString().ToLower()[0] : password[charLoc];
                        if (leetSpecialChars.ContainsKey(charValue))
                        {
                            password = password.ReplaceChar(charLoc, leetSpecialChars[charValue].ToString());
                        }
                        else
                        {
                            i--;
                        }
                    }
                }

                int leetableChars = password.Count(x => leetChars.ContainsKey(leetcaps ? x.ToString().ToLower()[0] : x));
                if (leetableChars <= numberCount)
                {
                    for (int i = 0; i < password.Length; i++)
                    {
                        char charValue = leetcaps ? password[i].ToString().ToLower()[0] : password[i];
                        if (leetChars.ContainsKey(charValue))
                        {
                            password = password.ReplaceChar(i, leetChars[charValue].ToString());
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < numberCount; i++)
                    {
                        int charLoc = rng.Next(password.Length - 1);
                        char charValue = leetcaps ? password[charLoc].ToString().ToLower()[0] : password[charLoc];
                        if (leetChars.ContainsKey(charValue))
                        {
                            password = password.ReplaceChar(charLoc, leetChars[charValue].ToString());
                        }
                        else
                        {
                            i--;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < numberCount; i++)
                {
                    password += rng.Next(0, 9).ToString();
                }
                for (int i = 0; i < numberCount; i++)
                {
                    password += specialChars.RandomItem();
                }
            }

            Console.WriteLine(password);
        }
    }
}
