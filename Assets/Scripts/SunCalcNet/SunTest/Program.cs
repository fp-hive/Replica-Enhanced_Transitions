using SunCalcNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunTest
{
    class Program
    {
        static void Main(string[] args)
        {

            //Arrange
            var date = DateTime.Now;
            var lat = 48.0;
            var lng = 13.0;

            //Act
            var sunPosition = SunCalc.GetSunPosition(date, lat, lng);

            double azimuthInDegrees = ConvertRadiansToDegrees(sunPosition.Azimuth);
            double elevationInDegress = ConvertRadiansToDegrees(sunPosition.Altitude);

            //Test
            Console.WriteLine("Azimuth: " + azimuthInDegrees);
            Console.WriteLine("Elevation: " + elevationInDegress);
            Console.ReadKey();

        }

        public static double ConvertRadiansToDegrees(double radians)
        {
            double degrees = (180 / Math.PI) * radians;
            degrees = degrees < 0 ? degrees + 180 : degrees;
            return (degrees);
        }
    }
}
