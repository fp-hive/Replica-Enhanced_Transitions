using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;
//using UnityEngine.UI;
using System;
using TMPro;
using SimpleJSON;
using SunCalcNet;

public class WeatherManager : MonoBehaviour
{
    public WeatherEffects weatherEffects;

    //public GameObject infoPanel;
    //private TMP_Text infoPanelString;

    public SunCalc SunCalc;

    private string systemTime;
    private string currentIP;

    public string currentCountry;
    public string currentCity;

    public string wantedCity = "Hagenberg+im+Mühlkreis";

    public string retrievedCountry;
    public string retrievedCity;

    [SerializeField] private string retrievedLon;
    [SerializeField] private string retrievedLat;

    private string retrievedUnixTime;
    private DateTime unixTime;

    private string retrievedTimeZone;
    private DateTime convertedTime;

    [SerializeField] private string timeString;

    private string timeHour;
    private string timeMinute;

    private int intHour;
    private int intMinute;

    private double timePercentage;

    private int conditionID;
    public string cloudLevel;
    public string conditionName;

    /*
    public string conditionImage;
    public Texture2D myTexture;
    public Sprite mySprite;
    public Image conImage;
    */

    // Start is called before the first frame update
    void Start()
    {
        //infoPanelString = infoPanel.GetComponent<TMP_Text>();

        StartCoroutine(GetIPAddress());
        StartCoroutine(GetWeather());
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateTimePanel();
        float currentTime = (float)intHour + (float)(intMinute / 100);
    }

    void GetSystemTime()
    {
        systemTime = System.DateTime.Now.ToString("HH:mm:ss - dd MMMM, yyyy");
    }

    /*
    void UpdateTimePanel()
    {
        infoPanelString.text = retrievedCity + ", " + retrievedCountry +
    "\n" + timeString +
    "\nCloud Amount: " + cloudLevel;
    "\nWeather Condition: " + conditionName +
    }
    */


    IEnumerator GetIPAddress()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://checkip.dyndns.org");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string result = www.downloadHandler.text;

            // This results in a string similar to this: <html><head><title>Current IP Check</title></head><body>Current IP Address: 123.123.123.123</body></html>
            // where 123.123.123.123 is your external IP Address.
            //  Debug.Log("" + result);

            string[] a = result.Split(':'); // Split into two substrings -> one before : and one after. 
            string a2 = a[1].Substring(1);  // Get the substring after the :
            string[] a3 = a2.Split('<');    // Now split to the first HTML tag after the IP address.
            string a4 = a3[0];              // Get the substring before the tag.

            Debug.Log("External IP Address = " + a4);
            currentIP = a4;
        }
    }


    IEnumerator GetWeather()
    {
        UnityWebRequest cityRequest = UnityWebRequest.Get("http://www.geoplugin.net/json.gp?ip=" + currentIP); //get our location info
        yield return cityRequest.SendWebRequest();

        if (cityRequest.error == null || cityRequest.error == "")
        {
            var N = JSON.Parse(cityRequest.downloadHandler.text);
            currentCity = N["geoplugin_city"].Value;
            currentCountry = N["geoplugin_countryName"].Value;
        }

        else
        {
            Debug.Log("WWW error: " + cityRequest.error);
        }

        UnityWebRequest weatherRequest = UnityWebRequest.Get("http://api.openweathermap.org/data/2.5/weather?q=" + wantedCity + "&APPID=70b823b79f12361bd42e152ee7e2c08e"); //get our weather
        yield return weatherRequest.SendWebRequest();
        Debug.Log(weatherRequest.downloadHandler.text);

        if (weatherRequest.error == null || weatherRequest.error == "")
        {
            var N = JSON.Parse(weatherRequest.downloadHandler.text);


            retrievedCountry = N["sys"]["country"].Value; //get the country
            retrievedCity = N["name"].Value; //get the city

            string temp = N["main"]["temp"].Value; //get the temperature
            float tempTemp; //variable to hold the parsed temperature
            float.TryParse(temp, out tempTemp); //parse the temperature
            float finalTemp = Mathf.Round((tempTemp - 273.0f) * 10) / 10; //holds the actual converted temperature

            int.TryParse(N["weather"][0]["id"].Value, out conditionID); //get the current condition ID
                                                                        //conditionName = N["weather"][0]["main"].Value; //get the current condition Name
            conditionName = N["weather"][0]["main"].Value; //get the current condition Description
            //conditionImage = N["weather"][0]["icon"].Value; //get the current condition Image

            cloudLevel = N["clouds"]["all"].Value; //get amount of clouds in percent

            retrievedLon = N["coord"]["lon"].Value;
            retrievedLat = N["coord"]["lat"].Value;

            retrievedUnixTime = N["dt"].Value;
            retrievedTimeZone = N["timezone"];

            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Local);
            unixTime = dtDateTime.AddSeconds(Int32.Parse(retrievedUnixTime));
            convertedTime = unixTime.AddSeconds(Int32.Parse(retrievedTimeZone)).ToLocalTime();

            unixTime = new DateTime();

            timeString = convertedTime.ToString("HH:mm:ss - dd MMMM, yyyy");

            timeHour = convertedTime.ToString("HH");
            timeMinute = convertedTime.ToString("mm");

            intHour = Int32.Parse(timeHour);
            intMinute = Int32.Parse(timeMinute);

            timePercentage = ((double)intHour * 60 + (double)intMinute) / 1440 * 100;

        }
        else
        {
            Debug.Log("WWW error: " + weatherRequest.error);
        }

        /*
        UnityWebRequest spriteTex = UnityWebRequestTexture.GetTexture("http://openweathermap.org/img/wn/" + conditionImage + "@2x.png");
        yield return spriteTex.SendWebRequest();

        
        myTexture = ((DownloadHandlerTexture)spriteTex.downloadHandler).texture;
        mySprite = Sprite.Create(myTexture, new Rect(0.0f, 0.0f, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
        conImage.sprite = mySprite;
        */

        weatherEffects.DeactivateAllEffects();
        weatherEffects.SetWeatherEffect();
        weatherEffects.SetClouds();

        SunCalc.RotateSun(convertedTime, Convert.ToDouble(retrievedLat, new CultureInfo("en-US")), Convert.ToDouble(retrievedLon, new CultureInfo("en-US")));
    }
}

