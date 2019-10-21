using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Zadacha();

            Console.ReadLine();
        }
        static void Zadacha()
        {
            double km = 0;
            double benzin = 100;

            for (int i = 50; i > 0; i--)
            {
                km = km + benzin / i;

                

                Console.WriteLine("motosiklov = {0}   baki po= {1}    {2} km", i, benzin, km);
            }

        }
    }
}
