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
        int l; // common length for seed and f
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
            this.tmp = new int[l];  // at start it is a copy of seed. After every getOnBit method is updating
            this.seed = new int[l]; // we need remember seed when we call restart method
            for (int i = 0; i < l; i++)
            {
                if (f[i].Equals('1')) { indexes.Add(i); } // determine which digits will be taking to XOR operation
                seed[i] = tmp[i] = Program.toInt(s[i]); // ascii 49 -> 1, 48 -> 0
            }
        }

        public void reset() // we recover to state at begining
        {
            for (int i=0; i<l; i++)
            {
                tmp[i] = seed[i]; 
            }
        }

        public int getOneBitStandard() // generate one bit
        {
            int result;
            result = xor();
            updateTmp(result); // add our determinated xor on begining tmp and shift right rest of tmp
            return result;
        }

        public int getOneBitExtended() // generate one bit, second method - without updating tmp
        {
            int result;
            result = xor();
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

        public void updateTmp(int xor)
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
            string fileName = "test";
            GeneratorLFSR generatorLFSR;
            byte[] fileBytes;
            byte[] encodedBytes;
            byte[] decodedBytes;
            BinaryWriter br;
            bool b = true;
            string action = "";

            s = "1101";
            f = "1001";
            //setFunctionAndSeed(ref f, ref s);

            generatorLFSR = new GeneratorLFSR(f, s);

            // testing, generate four bits 
            //Console.Write("{0,26}", "Y: ");
            //Console.WriteLine("{0}{1}{2}{3}", generatorLFSR.getOneBit(), generatorLFSR.getOneBit(), generatorLFSR.getOneBit(), generatorLFSR.getOneBit());


            while (b)
            {
                Console.WriteLine("*****************************************************************************************\n"
                                 + "f:           " + f + "\n"
                                 + "seed:        " + s + "\n"
                                 + "file:        " + fileName + "\n"
                                 + "last action: " + action + "\n"
                                 + "Choose action: change function and seed(c) | change file(cf) | 2e | 2d | 3e | 3d | q");
                action = Console.ReadLine();
                switch (action)
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
                        // reading all bytes from file
                        fileBytes = File.ReadAllBytes(System.IO.Directory.GetCurrentDirectory() + @"\" + fileName + ".bin");
                        // encoding
                        encodedBytes = exercise3encode(fileBytes, generatorLFSR);
                        // write all bytes into the file
                        br = new BinaryWriter(new FileStream(System.IO.Directory.GetCurrentDirectory() + @"\" + fileName + "encoded.bin", FileMode.Create));
                        br.Write(encodedBytes);
                        br.Close();
                        break;

                    case "3d":
                        // reading all bytes from file
                        fileBytes = File.ReadAllBytes(System.IO.Directory.GetCurrentDirectory() + @"\" + fileName + "encoded.bin");
                        // decoding
                        decodedBytes = exercise3decode(fileBytes, generatorLFSR);
                        // write all bytes into the file
                        br = new BinaryWriter(new FileStream(System.IO.Directory.GetCurrentDirectory() + @"\" + fileName + ".bin", FileMode.Create));
                        br.Write(decodedBytes);
                        br.Close();
                        break;

                    case "q":
                        b = false;
                        break;

                }
            }


            Console.ReadKey();

        }

        
        #region configuration methods

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

        public static int toInt(char x)
        {
            return Convert.ToInt32(x - '0');
        }

        public static char toChar(int c)
        {
            return Convert.ToChar(c + '0');
        }
        #endregion

        #region exercise 2

        private static byte[] exercise2encode(byte[] fileBytes, GeneratorLFSR generatorLFSR)
        {
            generatorLFSR.reset();

            byte[] encodedBytes = new byte[fileBytes.Length];
            StringBuilder s = new StringBuilder();

            for (int i = 0; i < fileBytes.Length; i++) // for every byte in bytes array
            {
                s.Clear();
                s.Insert(0, Convert.ToString(fileBytes[i], 2).PadLeft(8, '0')); // we have string of 8 digits representing one byte. Example 00001100
                s = encodeByte(s, generatorLFSR); // encode all byte

                encodedBytes[i] = Convert.ToByte(s.ToString(), 2);
            }

            return encodedBytes;
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
                s = encodeByte(s, generatorLFSR);

                decodedBytes[i] = Convert.ToByte(s.ToString(), 2);
            }


            return decodedBytes;

        }

       

        private static StringBuilder encodeByte(StringBuilder s, GeneratorLFSR generatorLFSR)
        {

            for(int i=0; i<s.Length; i++) // makes XOR with xor and x for all digits
            {
                s[i] = toChar(toInt(s[i])^generatorLFSR.getOneBitStandard());
            }

            return s;
        }

        
        #endregion

        #region exercise 3
        private static byte[] exercise3encode(byte[] fileBytes, GeneratorLFSR generatorLFSR)
        {
            generatorLFSR.reset();

            byte[] encodedBytes = new byte[fileBytes.Length];
            StringBuilder s = new StringBuilder();

            for (int i = 0; i < fileBytes.Length; i++) // for every byte in file
            {
                s.Clear();
                s.Insert(0, Convert.ToString(fileBytes[i], 2).PadLeft(8, '0')); // we have string of 8 digits representing one byte. Example 00001100
                s = encodeByte3(s, generatorLFSR);

                encodedBytes[i] = Convert.ToByte(s.ToString(), 2);
            }

            return encodedBytes;
        }

        private static byte[] exercise3decode(byte[] fileBytes, GeneratorLFSR generatorLFSR)
        {
            generatorLFSR.reset();

            byte[] decodedBytes = new byte[fileBytes.Length];
            StringBuilder s = new StringBuilder();

            for (int i = 0; i < fileBytes.Length; i++) // for every byte in file
            {
                s.Clear();
                s.Insert(0, Convert.ToString(fileBytes[i], 2).PadLeft(8, '0')); // we have string of 8 digits representing one byte. Example 00001100
                s = decodeByte(s, generatorLFSR);

                decodedBytes[i] = Convert.ToByte(s.ToString(), 2);
            }


            return decodedBytes;
        }

        private static StringBuilder encodeByte3(StringBuilder s, GeneratorLFSR generatorLFSR)
        {

            for (int i = 0; i < s.Length; i++) // makes XOR for all digits
            {

                s[i] = toChar(toInt(s[i])^generatorLFSR.getOneBitExtended());
                generatorLFSR.updateTmp(toInt(s[i]));
            }

            return s;
        }
    
        private static StringBuilder decodeByte(StringBuilder s, GeneratorLFSR generatorLFSR)
        {
            int y;
            for (int i = 0; i < s.Length; i++) // makes XOR for all digits
            {
                y = toInt(s[i]);
                s[i] = toChar((y^generatorLFSR.getOneBitExtended()));
                generatorLFSR.updateTmp(y);
            }

            return s;
        }

        #endregion
    }
}
