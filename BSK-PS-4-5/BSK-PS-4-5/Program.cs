using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSK_PS_4_5
{

    class GeneratorLFSR
    {
        int l; // seed and f length
        List<int> indexes; // tells for us which digits are taking to XOR operation
        int[] tmp;
        int[] seed;

        public GeneratorLFSR(String f, String s)
        {
            indexes = new List<int>();
            init(f, s);           
        }

        public void init(String f, String s)
        {
            indexes.Clear();

            this.l = s.Length;
            this.tmp = new int[l];
            this.seed = new int[l]; // to be frank it is unnecessary. We need tmp which is updated after every generating bit

            for (int i = 0; i < l; i++)
            {
                if (f[i].Equals('1')) { indexes.Add(i); } // determine which digits will be taking to XOR operation
                seed[i] = tmp[i] = (int)Char.GetNumericValue(s[i]); // ascii 49 -> 1
            }
        }

        public void reset() // we recover to state at begining
        {
            for (int i=0; i<l; i++)
            {
                tmp[i] = seed[i]; 
            }
        }

        public int getOneBit() // generate one bit
        {
            int result;
            result = xor();
            generateNewTmp(result); // add our determinated xor on begining and shift right tmp
            return result;
        }

        private int xor()
        {
            int result = tmp[indexes[0]]; // we take first digit from collection we taking to xor
            for (int i = 1; i < indexes.Count; i++)
            {
                result ^= tmp[indexes[i]];
            }
            return result;
        }

        private void generateNewTmp(int xor)
        {
            for (int i = l - 1; i > 0; i--) // for digits from 1 to (l-1), shift right
            {
                tmp[i] = tmp[i - 1];
            }
            tmp[0] = xor; // set new - xor - value on begining
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            int i;
            string s="", f=""; // seed, polynomial function
            string fileName = "test3";
            GeneratorLFSR generatorLFSR;
            byte[] fileBytes;
            byte[] encodedBytes;
            byte[] decodedBytes;
            BinaryWriter br;
            bool b = true;


            setFunctionAndSeed(ref f, ref s);

            generatorLFSR = new GeneratorLFSR(f, s);

            // testing, generate four bits 
            //Console.Write("{0,26}", "Y: ");
            //Console.WriteLine("{0}{1}{2}{3}", generatorLFSR.getOneBit(), generatorLFSR.getOneBit(), generatorLFSR.getOneBit(), generatorLFSR.getOneBit());


            while (b)
            {
                Console.WriteLine("*****************************************************************************************\n"
                                 + "f:    " + f + "\n"
                                 + "seed: " + s + "\n"
                                 + "file: " + fileName + "\n"
                                 + "Choose action: change function and seed(c) | change file(cf) | 2e | 2d | 3e | 3d | q");
                switch (Console.ReadLine())
                {
                    case "c":
                        setFunctionAndSeed(ref f, ref s);
                        generatorLFSR.init(f, s);
                        break;

                    case "cf":
                        setFile(ref fileName);
                        break;

                    case "2e":
                        // reading all bytes from file
                        fileBytes = File.ReadAllBytes(System.IO.Directory.GetCurrentDirectory() + @"\" + fileName + ".bin");
                        // encoding
                        encodedBytes = exercise2encode(fileBytes, generatorLFSR);
                        // write all bytes into the file
                        br = new BinaryWriter(new FileStream(System.IO.Directory.GetCurrentDirectory() + @"\" + fileName + "encoded.bin", FileMode.Create));
                        br.Write(encodedBytes);
                        br.Close();
                        break;
                    case "2d":
                        // reading all bytes from file
                        fileBytes = File.ReadAllBytes(System.IO.Directory.GetCurrentDirectory() + @"\" + fileName + "encoded.bin");
                        // decoding
                        decodedBytes = exercise2decode(fileBytes, generatorLFSR);
                        // write all bytes into the file
                        br = new BinaryWriter(new FileStream(System.IO.Directory.GetCurrentDirectory() + @"\" + fileName + ".bin", FileMode.Create));
                        br.Write(decodedBytes);
                        br.Close();
                        break;
                    case "3e":
                      
                        break;
                    case "3d":

                        break;

                    case "q":
                        b = false;
                        break;

                }
            }


            Console.ReadKey();

        }

        private static byte[] exercise2decode(byte[] fileBytes, GeneratorLFSR generatorLFSR)
        {
            generatorLFSR.reset();

            byte[] decodedBytes = new byte[fileBytes.Length];
            StringBuilder s = new StringBuilder();

            for (int i = 0; i < fileBytes.Length; i++)
            {
                s.Clear();
                s.Insert(0, Convert.ToString(fileBytes[i], 2).PadLeft(8, '0')); // we have string of 8 digits representing one byte. Example 00001100
                s = updateS(s, generatorLFSR);

                decodedBytes[i] = Convert.ToByte(s.ToString(), 2);
            }

            return decodedBytes;

        }

        private static void setFile(ref string fileName)
        {
            Console.WriteLine("Podaj nazwę pliku(bez rozszerzenia): ");
            fileName = Console.ReadLine();
        }

        private static void setFunctionAndSeed(ref string f, ref string s)
        {
            int l;
            Console.Write("{0,26}", "Podaj funkcję wielomianu: ");
            f = Console.ReadLine();
            l = f.Length;


            Console.Write("{0,26}", "Podaj seed: ");
            s = Console.ReadLine();
            while (s.Length != l)
            {
                Console.Write("Podana długość ciągu seed różni się od długości funkcji wielomianowej!\n"
                                 + "{0,26}", "Podaj seed: ");
                s = Console.ReadLine();
            }
        }


        private static byte[] exercise2encode(byte[] fileBytes, GeneratorLFSR generatorLFSR)
        {
            byte[] encodedBytes = new byte[fileBytes.Length];
            StringBuilder s = new StringBuilder();

            for(int i=0; i<fileBytes.Length; i++)
            {
                s.Clear();
                s.Insert(0, Convert.ToString(fileBytes[i], 2).PadLeft(8, '0')); // we have string of 8 digits representing one byte. Example 00001100
                s = updateS(s, generatorLFSR);

                encodedBytes[i] = Convert.ToByte(s.ToString(),2);
            }

            return encodedBytes;
        }

        private static StringBuilder updateS(StringBuilder s, GeneratorLFSR generatorLFSR)
        {
            for(int i=0; i<s.Length; i++) // makes XOR for all digits
            {
                s[i] = (char)((int)s[i] ^ generatorLFSR.getOneBit()); 
            }

            return s;
        }

    }
}
