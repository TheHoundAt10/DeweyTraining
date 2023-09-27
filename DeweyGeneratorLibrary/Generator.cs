using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Colby van Staden ST10081889 POE Part 1

namespace DeweyGeneratorLibrary
{
    public class Generator
    {
        private List<string> deweyNumbers = new List<string>();
        private Random random = new Random();

        public List<string> GenerateRandomDeweyNumbers(int count)
        {
            for (int i = 0; i < count; i++)
            {
                // Generate a random Dewey Decimal number between 000 and 999
                string deweyNumber = random.Next(0, 1000).ToString("D3");
                string deweyDec = random.Next(0, 1000).ToString("D3");

                string deweyDecimalNumber = deweyNumber + "." + deweyDec;

                deweyNumbers.Add(deweyDecimalNumber);
            }

            return deweyNumbers;
        }
    }
}
//oooooooooooooooooooooooooooooooooooooO EoF Ooooooooooooooooooooooooooooooooooooooooooooo