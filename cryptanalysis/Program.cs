using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Stuff to add:
 * file selection
 * 
 */


namespace cryptanalysis
{
    class Program
    {
        public static String actualLetterFreqString = "etaoinshrdlcumwfgypbvkjxqz";
        public static String actualFirstLetterFreqString = "tashwiobmfcldpnegryuvjkqxz";
        public static String alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static Dictionary<String, int> digraph = new Dictionary<string,int>();
        public static Dictionary<String, int> trigraph = new Dictionary<string, int>();
        public static char[] match = new char[26];
        public static Dictionary<String, char[]> save = new Dictionary<String, char[]>();
        public static int[] letterFreq = new int[26];
        public static int[] firstLetterFreq = new int[26];
        public static String text = "";
        public static char[] actualLetterFreqArray = new char[26];
        public static char[] actualFirstLetterFreqArray = new char[26];
        public static char[] alphabetArray = new char[26];
        public static char[] breakdown;
        public static Boolean exit = false;
        public static int[] shiftAmount;

        static void Main(string[] args)
        {
            int[][] test = new int[2][];
            test[0] = new int[2];
            test[1] = new int[2];
            test[0][0] = 0;
            test[0][1] = 1;
            test[1][0] = 1;
            test[1][1] = 0;
            
            initArrays();
            Console.Write("Enter File Name: ");
            init(Console.ReadLine());       
            Console.WriteLine("Enter command: freq, map, unmap, digraph, trigraph, detectshift, keyword, load, save, list, help, exit, reinit, optimalShift, printwordcombos, ic");
            
            while(!exit)
            {
                String cmd = Console.ReadLine().ToLower();
                command(cmd);
            }
        }

        private static void init(String fileName)
        {
            String testing = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            Console.WriteLine(testing + "\\crypt\\" + fileName + ".txt");
            try
            {
                text = System.IO.File.ReadAllText(testing + "\\crypt\\" + fileName + ".txt");
            }
            catch (Exception e)
            {
                Console.WriteLine("No file exists: " + e.Message);
            }
            text = text.ToUpper();
            breakdown = new char[text.Length];
            int cur = 0;
            bool firstLetter = true;
            firstLetterFreq = new int[26];
            letterFreq = new int[26];
            foreach (char c in text)
            {
                // mayce change to c - 65 >= 0  && < 26
                if (c != ' ' && c != '\n' && c != '\r' && c != '.')
                {
                    breakdown[cur] = c;
                    cur += 1;

                    if (firstLetter)
                    {
                        firstLetterFreq[c - 65] += 1;
                    }
                    firstLetter = false;
                }
                else
                {
                    firstLetter = true;
                }

            }
            int pos;
            //level frequency
            foreach (char c in breakdown)
            {
                if (c != '\0')
                {
                    pos = c - 65;
                    letterFreq[pos] += 1;
                }
            }
        }

        public static void currentDecode(String text)
        {
            text = text.Replace("\n", "");
            text = text.Replace("\r", "");
            foreach (char c in text)
            {
                
                bool notFound = true;
                if (c == ' ')
                {
                    Console.Write(' ');
                    notFound = false;
                }
                for (int i = 0; i < 26 && notFound; i++)
                {
                    if (match[i] == c)
                    {
                        notFound = false;
                        Console.Write(alphabet[i]);
                    }
                }
                if (notFound)
                {
                    Console.Write('_');
                }
            }
            Console.WriteLine();
        }

        public static void map(String crypt, String plain)
        {
            crypt = crypt.ToUpper();
            plain = plain.ToUpper();
            char[] cryptArray = crypt.ToCharArray();
            char[] plainArray = plain.ToCharArray();
            for (int i = 0; i < cryptArray.Length; i++)
            {
                int index = Convert.ToChar(plainArray[i]) - 65;
                match[index] = Convert.ToChar(cryptArray[i]);
            }

        }

        public static void createDigraph(String text, int cutoff = 0)
        {
            digraph = new Dictionary<string,int>();
            text = text.Replace(" ","_");
            text = text.Replace("\n", "");
            text = text.Replace("\r", "");
            int length = text.Length;
            for (int i = 0; i < length - 1; i++)
            {
                String sub = text.Substring(i, 2);
                if(!digraph.ContainsKey(sub))
                {
                    digraph.Add(sub, 1);
                }
                else
                {
                    digraph[sub] += 1;
                }
            }

            List<String> tuples = digraph.Keys.ToList();
            int maxDigraph = 0;
            foreach (String x in tuples)
            {
                if (digraph[x] != 1)
                {
                    if (digraph[x] > maxDigraph)
                    {
                        maxDigraph = digraph[x];
                    }
                    //Console.WriteLine(x + "  " + digraph[x]);

                }
            }
            while (maxDigraph > cutoff)
            {
                foreach (String x in tuples)
                {                
                    if (digraph[x] == maxDigraph)
                    {
                        Console.WriteLine(x + "  " + digraph[x]);
                    }
                }
                maxDigraph -= 1;
            }
            

        }

        private static void initArrays()
        {
            int cur = 0;
            foreach (char c in actualLetterFreqString)
            {
                actualLetterFreqArray[cur] = c;
                cur += 1;
            }
            cur = 0;
            
            foreach (char c in actualFirstLetterFreqString)
            {
                actualFirstLetterFreqArray[cur] = c;
                cur += 1;
            }
            cur = 0;
            
            foreach (char c in alphabet)
            {
                alphabetArray[cur] = c;
                cur += 1;
            }
        }

        public static void createTrigraph(String text, int cutoff = 0)
        {
            trigraph = new Dictionary<string, int>();
            text = text.Replace(" ", "_");
            text = text.Replace("\n", "");
            text = text.Replace("\r", "");
            int length = text.Length;
            for (int i = 0; i < length - 2; i++)
            {
                String sub = text.Substring(i, 3);
                if (!trigraph.ContainsKey(sub))
                {
                    trigraph.Add(sub, 1);
                }
                else
                {
                    trigraph[sub] += 1;
                }
            }

            List<String> tuples = trigraph.Keys.ToList();
            int maxtrigraph = 0;
            foreach (String x in tuples)
            {
                if (trigraph[x] != 1)
                {
                    if (trigraph[x] > maxtrigraph)
                    {
                        maxtrigraph = trigraph[x];
                    }
                    //Console.WriteLine(x + "  " + trigraph[x]);

                }
            }
            while (maxtrigraph > cutoff)
            {
                foreach (String x in tuples)
                {
                    if (trigraph[x] == maxtrigraph)
                    {
                        Console.WriteLine(x + "  " + trigraph[x]);
                    }
                }
                maxtrigraph -= 1;
            }


        }

        public static void printValue(int[] val)
        {
            Console.WriteLine("A  B  C  D  E  F  G  H  I  J  K  L  M  N  O  P  Q  R  S  T  U  V  W  X  Y  Z");
            foreach(int x in val)
            {
                if (x < 10)
                    Console.Write(x + "  ");
                else
                    Console.Write(x + " ");
            }
            Console.Write("\n");
            
        }

        //returns the letters in order of the frequency 
        public static void pairByFrequency(int[] freqArray, char[] actualFrequency)
        {
            int max = 0;
            String inOrder = "";
            String crypt = "";
            int done = 0;
            foreach (int x in freqArray)
            {
                if (x > max)
                {
                    max = x;
                }
            }
            while(max >= 0)
            {
                
                char[] cur = new char[26];
                int num = 0;
                
                for (int i = 0; i < 26; i++)
                {
                    
                    if(freqArray[i] == max)
                    {
                        cur[num] = (char)(i + 65);
                        num += 1;
                        
                    }
                }
                max = max - 1;
                
                if(num == 1)
                {
                    inOrder += actualFrequency[done] + " ";
                    crypt += cur[0] + " ";
                }
                else
                {
                    if (num != 0)
                    {
                        inOrder += "{";
                        crypt += "{";
                        for (int i = 0; i < num; i++)
                        {
                            inOrder += actualFrequency[done + i] + ",";
                            crypt += cur[i] + ",";
                        }
                        inOrder += "} ";
                        crypt += "} ";
                    }
                }
                done += num;
            }

            Console.WriteLine("Actual: "+ inOrder);
            Console.WriteLine("Crypto: " + crypt);

            
        }

        //used for keyword encoding
        public static void createCol(int cols, String cypher, String plain)
        {
            int overflow = 26 % cols;
            int fullCol = 26 / cols;
            int pos = 0;
            String[] rows;
            if (overflow != 0)
            {
                rows = new String[(fullCol + 1)*2];

            }
            else
            {
                rows = new String[fullCol * 2];
            }
            for (int i = 0; i < rows.Length; i++)
            {
                rows[i] = "";
            }
            for (int i = 0; i < overflow; i++ )
            {
                for (int j = 0; j < rows.Length; j = j+2)
                {
                    rows[j] += alphabet.Substring(pos, 1) + " ";
                    rows[j + 1] += cypher.Substring(pos, 1) + " ";
                    pos += 1;
                }
            }
            for (int i = 0; i < (cols - overflow); i++)
            {
                for (int j = 0; j < rows.Length-2; j = j+2)
                {
                    rows[j] += alphabet.Substring(pos, 1) + " ";
                    rows[j + 1] += cypher.Substring(pos, 1) + " ";
                    pos += 1;
                }
            }
            for (int i = 0; i < rows.Length; i=i+2)
            {
                Console.WriteLine(rows[i]);
                Console.WriteLine(rows[i+1]);
            }

        }

        public static int[][] detectShift2(int keywordLength, char[] text)
        {
            int[][] comparison = new int[keywordLength][];
            char[][] breakdown = new char[keywordLength][];
            for (int i = 0; i < keywordLength; i++)
            {
                breakdown[i] = new char[(text.Length / keywordLength) + 1];
                comparison[i] = new int[26];
            }
            int listNum = 0;
            int divTemp = 0;
            for (int i = 0; i < text.Length; i++)
            {
                listNum = i % keywordLength;
                if (i == 0)
                    divTemp = 0;
                else
                    divTemp = i / keywordLength;
                breakdown[listNum][divTemp] = text[i];
            }
            for (int i = 0; i < keywordLength; i++)
            {
                comparison[i] = frequencyList(breakdown[i]);
                
                
            }
            Console.WriteLine("Keywork length: " + keywordLength);
            for (int i = 0; i < keywordLength; i++)
            {
                Console.WriteLine("Row: " + i);
                printValue(comparison[i]);
                Console.WriteLine();
                Console.WriteLine();
            }

            return comparison;
        }

        public static char[][] detectShift(int keywordLength, char[] text, int numKeys)
        {
            char[][] comparison = new char[keywordLength][];
            char[][] breakdown = new char[keywordLength][];
            for(int i = 0; i < keywordLength; i++)
            {
                breakdown[i] = new char[(text.Length/keywordLength) + 1];
                comparison[i] = new char[26];
            }
            int listNum = 0;
            int divTemp = 0;
            for (int i = 0; i < text.Length; i++)
            {
                listNum = i % keywordLength;
                if (i == 0)
                    divTemp = 0;
                else
                    divTemp = i/keywordLength;
                breakdown[listNum][divTemp] = text[i];
            }
            for (int i = 0; i < keywordLength; i++)
            {
                int[] temp = frequencyList(breakdown[i]);
                int max = 0;
                int done = 0;
                char symbol = 'A';
                bool change = false;
                foreach(int x in temp)
                {
                    if (max < x)
                    {
                        max = x;
                    }
                }
                while (done < numKeys)
                {
                    for (int j = 0; j < 26; j++)
                    {
                        if (temp[j] == max)
                        {
                            temp[j] = 0;
                            done += 1;
                            change = true;
                            comparison[i][j] = symbol;
                        }
                        
                    }
                    if(change == true)
                    {
                        symbol = Convert.ToChar(symbol + 1);
                        change = false;
                    }
                    max = max - 1;
                }
            }
            Console.WriteLine("Keywork length: " + keywordLength);
            for(int i =0; i < keywordLength; i++)
            {
                Console.WriteLine("Row: " + i);
                Console.WriteLine(alphabet);
                foreach (char x in comparison[i])
                {
                    Console.Write(x);
                }
                Console.WriteLine();
                Console.WriteLine();
            }

            return comparison;
            
        }

        public static int[] frequencyList(char[] text)
        {
            int[] freq = new int[26];
            int pos = 0;
            foreach (char c in text)
            {
                if (c != '\0')
                {
                    pos = c - 65;
                    freq[pos] += 1;
                }
            }
            return freq;
        }

        public static int[] smallestVarianceShift(int[][] freq)
        {
            int[] basis = freq[0];
            int[] shift = new int[freq.Length];
            shift[0] = 0;
            int shiftValue = 0;
            int min = -1;
            for (int j = 1; j < freq.Length; j++)
            {
                for (int i = 1; i < freq[0].Length; i++)
                {
                    for (int k = 1; k < freq[0].Length; k++)
                    {
                        shiftValue += (basis[k] + freq[j][(i + k)%freq[0].Length]);
                    }
                    if (shiftValue < min || min == -1)
                    {
                        Console.WriteLine("Shift Value: " + shiftValue + " on: " + i);
                        shift[j] = i;
                        min = shiftValue;
                    }
                    shiftValue = 0;
                    
                }
                min = -1;
            }
            return shift;
        }

        public static void IC(String text)
        {
            for (int i = 4; i <= 6; i++)
            {
                Console.WriteLine("Split: " + i);
                int[][] ds2 = detectShift2(i,text.ToCharArray());
                foreach(int[] x in ds2)
                {
                    Console.WriteLine(ICHelper(x));
                }
                Console.WriteLine("--------------------");

            }
        }

        private static double ICHelper(int[] freq)
        {
            int total = (freq.Sum());
            double x = (total * (total - 1));
            x = 1 / x;
            int sum = 0;
            for (int i = 0; i < freq.Length; i++)
            {
                sum += freq[i] * (freq[i] - 1); 
            }
            double tbr = x*sum;
            //Console.WriteLine("X: " + x + "\n" + "Sum: " + sum + "\n" + "TBR: " + tbr + "\n");
            return tbr;
        }

        private static void command(String cmd)
        {
            if (cmd == "exit")
            {
                exit = true;
            }
            else
            {
                if (cmd == "freq")
                {
                    Console.WriteLine("Letter Frequency:");
                    printValue(letterFreq);
                    Console.WriteLine("First Letter Frequency");
                    printValue(firstLetterFreq);
                    Console.WriteLine("\nFrequency Matching");
                    pairByFrequency(letterFreq, actualLetterFreqArray);
                    Console.WriteLine("\nFirst Letter Frequency Matching");
                    pairByFrequency(firstLetterFreq, actualFirstLetterFreqArray);
                    Console.WriteLine("\n----------");
                }
                else
                {
                    if (cmd.IndexOf("digraph") == 0)
                    {
                        Console.WriteLine("\nDigraph");
                        String[] split = cmd.Split();
                        if (split.Length == 1)
                        {
                            createDigraph(text);
                        }
                        else
                        {
                            createDigraph(text, Convert.ToInt32(split[1]));
                        }
                        Console.WriteLine("\n----------");
                    }
                    else
                    {
                        if (cmd.IndexOf("keyword") == 0)
                        {
                            String[] split = cmd.Split();
                            int start = 4;
                            int end = 11;
                            if (split.Length != 1)
                            {
                                start = Convert.ToInt32(split[1]);
                                end = start;
                            }
                            String matchString = "";
                            for (int j = 0; j < 26; j++)
                            {
                                matchString += match[j];
                            }
                            for (int i = start; i <= end; i++)
                            {
                                Console.WriteLine("\n" + i + " col\n");

                                createCol(i, matchString, null);
                                Console.WriteLine("\n----------");
                            }
                        }
                        else
                        {
                            if (cmd.IndexOf("map") == 0)
                            {
                                String[] split = cmd.Split(' ');
                                map(split[1], split[2]);
                                Console.WriteLine("Plain text:  " + alphabet);
                                Console.Write("Cypher text: ");
                                foreach (char c in match)
                                {
                                    Console.Write(c);
                                }
                                Console.WriteLine("\n----------");
                            }
                            else
                            {
                                if (cmd == "decode")
                                {
                                    currentDecode(text);
                                    Console.WriteLine("\n----------");
                                }
                                else
                                {
                                    if (cmd.IndexOf("save") == 0)
                                    {
                                        String[] split = cmd.Split(' ');
                                        char[] temp = (char[])match.Clone();
                                        if (save.ContainsKey(split[1]))
                                        {
                                            save[split[1]] = temp;
                                        }
                                        else
                                        {
                                            save.Add(split[1], temp);
                                        }
                                        Console.WriteLine("\n----------");
                                    }
                                    else
                                    {
                                        if (cmd == "list")
                                        {
                                            List<String> saves = save.Keys.ToList();
                                            if (saves.Count == 0)
                                            {
                                                Console.WriteLine("No saves");
                                            }
                                            else
                                            {
                                                Console.WriteLine("Saves: ");
                                                foreach (String x in saves)
                                                {
                                                    Console.WriteLine("Name: " + x + "\n" + alphabet + "\n");
                                                    for (int i = 0; i < 26; i++)
                                                    {
                                                        Console.Write(save[x][i]);
                                                    }
                                                    Console.Write('\n');
                                                }
                                            }
                                            Console.WriteLine("\n----------");
                                        }
                                        else
                                        {
                                            if (cmd.IndexOf("load") == 0)
                                            {
                                                String[] split = cmd.Split(' ');
                                                match = save[split[1]];
                                                Console.WriteLine("\n----------");
                                            }
                                            else
                                                if (cmd.IndexOf("unmap") == 0)
                                                {
                                                    String[] split = cmd.Split(' ');
                                                    String temp = split[1].ToUpper();
                                                    for (int i = 0; i < split[1].Length; i++)
                                                    {
                                                        match[Convert.ToChar(temp[i]) - 65] = ' ';
                                                    }
                                                    Console.WriteLine("Plain text:  " + alphabet);
                                                    Console.Write("Cypher text: ");
                                                    foreach (char c in match)
                                                    {
                                                        Console.Write(c);
                                                    }
                                                    Console.WriteLine("\n----------");
                                                }
                                                else
                                                {
                                                    if (cmd.IndexOf("trigraph") == 0)
                                                    {
                                                        Console.WriteLine("\nTrigraph");
                                                        String[] split = cmd.Split();
                                                        if (split.Length == 1)
                                                        {
                                                            createTrigraph(text);
                                                        }
                                                        else
                                                        {
                                                            createTrigraph(text, Convert.ToInt32(split[1]));
                                                        }
                                                        Console.WriteLine("\n----------");
                                                    }
                                                    else
                                                    {
                                                        if (cmd == "detectshift")
                                                        {

                                                            detectShift2(3, breakdown);
                                                            Console.WriteLine("\n----------");
                                                            detectShift2(4, breakdown);
                                                            Console.WriteLine("\n----------");
                                                            detectShift2(5, breakdown);
                                                            Console.WriteLine("\n----------");
                                                            detectShift2(6, breakdown);
                                                            Console.WriteLine("\n----------");
                                                        }
                                                        else
                                                        {
                                                            if (cmd == "help")
                                                            {
                                                                Console.WriteLine("Exit - Exits the program\n");
                                                                Console.WriteLine("Freq - The frequency of both the first letters and overall letters in the input text\n");
                                                                Console.WriteLine("Digraph - shows all two letter combinations and their frequencies that show up more than once.\n");
                                                                Console.WriteLine("Trigraph - shows all three letter frequencies that show up more than once.\n");
                                                                Console.WriteLine("Keyword - creates keyword tables of length 4 to 10.\n");
                                                                Console.WriteLine("Map - (format: map cyperLetter plainTextLetter) used to add a mapping from a cyper letter to a plaintext letter\n");
                                                                Console.WriteLine("Unmap - (format: unmap cyperLetter plainTextLetter) used to remove a mapping from a cyper letter to a plaintext letter\n");
                                                                Console.WriteLine("Decode - using the mapping shows the translation of the inputtext\n");
                                                                Console.WriteLine("Save - (format: save saveName) save a current mapping so you can revert after a test.\n");
                                                                Console.WriteLine("Load - (format: load saveName) load an old mapping to revert to it.\n");
                                                                Console.WriteLine("Detectshift - frequency histogram for shift keywords of possible length 3 to 5.\n");
                                                                Console.WriteLine("optimalShift - finds the smallest variance of all possible shifts. Not always correct due to equal variance at times.\n");
                                                                Console.WriteLine("PrintWordCombos - (format: printwordcombos num) Prints all possible shift combinations of (optimalshift num) \n");
                                                                Console.WriteLine("Help - your using it!\n");
                                                            }
                                                            else
                                                            {
                                                                if (cmd == "print")
                                                                {
                                                                    Console.WriteLine(text);
                                                                    Console.WriteLine("\n----------");
                                                                }
                                                                else
                                                                {
                                                                    if (cmd == "show")
                                                                    {
                                                                        Console.WriteLine("Plain text:  " + alphabet);
                                                                        Console.Write("Cypher text: ");
                                                                        foreach (char c in match)
                                                                        {
                                                                            Console.Write(c);
                                                                        }
                                                                        Console.WriteLine();
                                                                        Console.WriteLine("\n----------");
                                                                    }
                                                                    else
                                                                    {
                                                                        if (cmd.IndexOf("hardsave") == 0)
                                                                        {
                                                                            String[] split = cmd.Split(' ');
                                                                            String toSave = "";
                                                                            for (int i = 0; i < 26; i++)
                                                                            {
                                                                                toSave += match[i];
                                                                            }
                                                                            System.IO.File.WriteAllText(@"C:\Users\Bill\Desktop\crypt\" + split[1], toSave);
                                                                        }
                                                                        else
                                                                        {
                                                                            if (cmd.IndexOf("hardload") == 0)
                                                                            {
                                                                                String[] split = cmd.Split(' ');
                                                                                String data = System.IO.File.ReadAllText(@"C:\Users\Bill\Desktop\crypt\" + split[1]);
                                                                                char[] dataArray = data.ToCharArray();
                                                                                for (int i = 0; i < 26; i++)
                                                                                {
                                                                                    match[i] = dataArray[i];
                                                                                }
                                                                                Console.WriteLine("\n----------");
                                                                            }
                                                                            else
                                                                            {
                                                                                if (cmd.IndexOf("reinit") == 0)
                                                                                {
                                                                                    String[] split = cmd.Split(' ');
                                                                                    init(split[1]);
                                                                                    Console.WriteLine("\n----------");
                                                                                }
                                                                                else
                                                                                {
                                                                                    if(cmd.IndexOf("optimalshift") == 0)
                                                                                    {
                                                                                        String[] split = cmd.Split(' ');
                                                                                        shiftAmount = smallestVarianceShift(detectShift2(Convert.ToInt32(split[1]), breakdown));
                                                                                        Console.WriteLine("Least variable shift");
                                                                                        foreach (int x in shiftAmount)
                                                                                        {
                                                                                            Console.WriteLine(x);
                                                                                        }

                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (cmd.IndexOf("printwordcombos") == 0)
                                                                                        {
                                                                                            char temp;
                                                                                            String[] split = cmd.Split(' ');
                                                                                            if (shiftAmount == null)
                                                                                            {
                                                                                                if (split.Length != 0)
                                                                                                {
                                                                                                    shiftAmount = smallestVarianceShift(detectShift2(Convert.ToInt32(split[1]), breakdown));
                                                                                                    for (int i = 0; i < 26; i++)
                                                                                                    {
                                                                                                        Char basis = 'A';
                                                                                                        for (int j = 0; j < shiftAmount.Length; j++)
                                                                                                        {
                                                                                                            Console.Write(Convert.ToChar((basis + shiftAmount[j] + i) % 26 + 'A'));
                                                                                                        }
                                                                                                        Console.WriteLine();
                                                                                                    }
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    Console.WriteLine("No number specified and no previous call to optimal shift");
                                                                                                }
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (shiftAmount.Length != Convert.ToInt32(split[1]))
                                                                                                {
                                                                                                    shiftAmount = smallestVarianceShift(detectShift2(Convert.ToInt32(split[1]), breakdown));

                                                                                                }
                                                                                                for (int i = 0; i < 26; i++)
                                                                                                {
                                                                                                    Char basis = 'A';
                                                                                                    for (int j = 0; j < shiftAmount.Length; j++)
                                                                                                    {
                                                                                                        Console.Write(Convert.ToChar((basis + shiftAmount[j] + i) % 26 + 'A'));
                                                                                                    }
                                                                                                    Console.WriteLine();
                                                                                                }

                                                                                            }


                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (cmd == "ic")
                                                                                            {
                                                                                                IC(text);
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                Console.WriteLine("Invalid command");
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }

                                                                                
                                                                            }
                                                                        }
                                                                    }
                                                                }

                                                            }
                                                        }
                                                    }
                                                }
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
