using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using SunCalcNet.Internal;
using SunCalcNet.Model;
using SunCalcNet.Utils;
using System;
using System.Collections.Generic;

namespace SunCalcNet
{
    public class SunCalc : MonoBehaviour
    {
        public Light sun;
        private SunPosition sunPos;

        /// <summary>
        /// Calculates sun position for a given date and latitude/longitude.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <returns></returns>
        public static SunPosition GetSunPosition(DateTime date, double lat, double lng)
        {
            var lw = Constants.Rad * -lng;
            var phi = Constants.Rad * lat;
            var d = date.ToDays();

            var sunCoords = Sun.GetEquatorialCoords(d);
            var h = Position.GetSiderealTime(d, lw) - sunCoords.RightAscension;

            var azimuth = Position.GetAzimuth(h, phi, sunCoords.Declination);
            var altitude = Position.GetAltitude(h, phi, sunCoords.Declination);

            return new SunPosition(azimuth, altitude);
        }

        /// <summary>
        /// Calculates phases of the sun for a single day and latitude/longitude
        /// and optionally the observer height (in meters) relative to the horizon
        /// </summary>
        /// <param name="date"></param>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static IEnumerable<SunPhase> GetSunPhases(DateTime date, double lat, double lng, double height = 0)
        {
            var lw = Constants.Rad * -lng;
            var phi = Constants.Rad * lat;

            var dh = SunTime.GetObserverAngle(height);
            
            var d = date.ToDays();

            var n = SunTime.GetJulianCycle(d, lw);
            var ds = SunTime.GetApproxTransit(0, lw, n);

            var m = Sun.GetMeanAnomaly(ds);
            var l = Sun.GetEclipticLongitude(m);
            var dec = Position.GetDeclination(l, 0);

            var jnoon = SunTime.GetSolarTransitJ(ds, m, l);
            var solarNoon = jnoon.FromJulian();
            var nadir = (jnoon - 0.5).FromJulian();

            var sunPhaseCol = new List<SunPhase>
            {
                new SunPhase(SunPhaseName.SolarNoon, solarNoon),
                new SunPhase(SunPhaseName.Nadir, nadir)
            };

            foreach (var sunPhase in SunPhaseAngle.List)
            {
                var h0 = (sunPhase.Angle + dh) * Constants.Rad;
                var jset = SunTime.GetSetJ(h0, lw, phi, dec, n, m, l);

                if (double.IsNaN(jset) || double.IsInfinity(jset))
                {
                    continue;
                }

                var jrise = jnoon - (jset - jnoon);
                sunPhaseCol.Add(new SunPhase(sunPhase.RiseName, jrise.FromJulian()));
                sunPhaseCol.Add(new SunPhase(sunPhase.SetName, jset.FromJulian()));
            }

            return sunPhaseCol;
        }

        public void RotateSun(DateTime date, double lat, double lng)
        {
            sunPos = GetSunPosition(date, lat, lng);
            Debug.Log(date + ", " + lat + ", " + lng);
            sun.transform.rotation = Quaternion.Euler((float)(sunPos.Altitude*Mathf.Rad2Deg), (float)(sunPos.Azimuth*Mathf.Rad2Deg)+180, 0);
        }
    }
}