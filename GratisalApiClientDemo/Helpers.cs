using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GratisalApiClientDemo
{
    static class Helpers
    {
        static Stopwatch sw = new Stopwatch();

        public static void Log(this string _string)
        {
            if (!sw.IsRunning) sw.Start();
            Console.WriteLine($"{sw.Elapsed.ToString(@"ss\.ff")} // {_string}");
        }

        public enum Gender
        {
            Male,
            Female
        }

        public static string GenerateRandomIdentitynumber(Gender gender)
        {
            var weights = new[] { 4, 3, 2, 7, 6, 5, 4, 3, 2, 1 };
            var fromdaterange = new DateTime(1920, 01, 01);
            var todaterange = new DateTime(1999, 12, 31);

            var result = string.Empty;

            var randomDays = new Random().Next(0, (todaterange - fromdaterange).Days);

            var randomDate = todaterange.AddDays(-randomDays).Date;

            while (true)
            {
                var serialNumber = new Random().Next(100, 400);

                result = randomDate.ToString("ddMMyy") + serialNumber;

                var weightsum = 0;
                for (var i = 0; i < weights.Length - 1; i++)
                {
                    weightsum += weights[i] * Convert.ToInt32(result.Substring(i, 1));
                }

                var modulus = weightsum % 11;

                switch (gender)
                {
                    default:
                    case Gender.Male:
                        if (modulus % 2 == 0 && modulus != 0)
                            return result + (11 - modulus).ToString();
                        break;
                    case Gender.Female:
                        if (modulus % 2 == 1 && modulus != 1)
                            return result + (11 - modulus).ToString();
                        break;
                }
            }
        }
    }
}
