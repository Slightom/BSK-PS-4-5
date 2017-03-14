using System;
using System.Collections.Generic;
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
            this.l = s.Length;
            this.tmp = new int[l];
            this.seed = new int[l]; // to be frank it is unnecessary. We need tmp which is updated after every generating bit

            for (int i = 0; i < l; i++)
            {
                if (f[i].Equals('1'))   { indexes.Add(i); } // determine which digits will be taking to XOR operation
                seed[i] = tmp[i] = (int)Char.GetNumericValue(s[i]); // ascii 49 -> 1
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
            int l, i;
            string s, f; // seed, polynomial function
            GeneratorLFSR generatorLFSR;

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

            generatorLFSR = new GeneratorLFSR(f, s);

            // testing, generate four bits 
            Console.Write("{0,26}", "Y: ");
            Console.WriteLine("{0}{1}{2}{3}", generatorLFSR.getOneBit(), generatorLFSR.getOneBit(), generatorLFSR.getOneBit(), generatorLFSR.getOneBit());

        }


    }
}
