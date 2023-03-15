using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class WeatherEffects : MonoBehaviour
{
    public WeatherManager weatherManager;

    public GameObject clearSky;
    public Volume clouds;
    public GameObject drizzle;
    public GameObject rain;
    public GameObject thunderstorm;
    public GameObject snow;

    [SerializeField] private int cloudLevel;


    public void ActivateClearSky()
    {
        clearSky.SetActive(true);
    }

    public void ActivateDrizzle()
    {
        drizzle.SetActive(true);
    }

    public void ActivateRain()
    {
        rain.SetActive(true);
    }

    public void ActivateThunderstorm()
    {
        DeactivateAllEffects();
        thunderstorm.SetActive(true);
    }

    public void ActivateSnow()
    {
        DeactivateAllEffects();
        snow.SetActive(true);
    }

    public void DeactivateAllEffects()
    {
        clearSky.SetActive(false);
        //clouds.SetActive(false);
        drizzle.SetActive(false);
        rain.SetActive(false);
        thunderstorm.SetActive(false);
        snow.SetActive(false);
    }

    public void SetClouds()
    {
        VolumeProfile profile = clouds.sharedProfile;
        cloudLevel = int.Parse(weatherManager.cloudLevel);

        if (!profile.TryGet<VolumetricClouds>(out var VC))
        {
            VC = profile.Add<VolumetricClouds>(true);
        }

        if (cloudLevel < 11)
        {
            VC.cloudPreset.value = VolumetricClouds.CloudPresets.Custom;
        }
        else if (cloudLevel >= 11 && cloudLevel < 30)
        {
            VC.cloudPreset.value = VolumetricClouds.CloudPresets.Sparse;
        }
        else if (cloudLevel >= 30 && cloudLevel < 70)
        {
            VC.cloudPreset.value = VolumetricClouds.CloudPresets.Cloudy;
        }
        else if (cloudLevel >= 70 && cloudLevel < 90)
        {
            VC.cloudPreset.value = VolumetricClouds.CloudPresets.Stormy;
        }
        else if (cloudLevel >= 90)
        {
            VC.cloudPreset.value = VolumetricClouds.CloudPresets.Overcast;
        }

    }

    public void SetWeatherEffect()
    {

        switch (weatherManager.conditionName)
        {
            case "Clear":
                ActivateClearSky();
                break;
            case "Drizzle":
                ActivateDrizzle();
                break;
            case "Rain":
                ActivateRain();
                break;
            case "Thunderstorm":
                ActivateThunderstorm();
                break;
            case "Snow":
                ActivateSnow();
                break;
            default:
                ActivateClearSky();
                break;
        }
    }
}
