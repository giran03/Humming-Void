using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsFlickerController : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField] bool enableFlicker;
    [SerializeField] bool onOffBehaviour;
    [SerializeField] float minWaitTime;
    [SerializeField] float maxWaitTime;
    [SerializeField] float minLightIntensity;

    bool turnOffAllLights;
    Light bulb;
    Material lightMat;
    float defaultIntensity;
    bool changeIntensity;
    bool turnOffBulb;

    private void Start()
    {
        bulb = GetComponent<Light>();
        lightMat = GetComponent<Renderer>().material;

        defaultIntensity = bulb.intensity;

        if (!enableFlicker) return;

        if (!onOffBehaviour)
            StartCoroutine(LightsFlickerIntensity());
        else
            StartCoroutine(LightsFlickerOnOff());
    }

    private void FixedUpdate()
    {
        // flip light switch once
        if (turnOffAllLights)
        {
            bulb.enabled = false;
            lightMat.SetColor("_EmissionColor", Color.white * 0f);
        }
        
        if (turnOffAllLights) return;
        // flip the light switch continuously
        if (!turnOffBulb)
        {
            bulb.enabled = true;
            lightMat.SetColor("_EmissionColor", Color.white * 3f);
        }
        else
        {
            bulb.enabled = false;
            lightMat.SetColor("_EmissionColor", Color.white * 0f);
        }

        if (onOffBehaviour) return;
        // Change intensity only
        if (!changeIntensity)
        {
            bulb.intensity = defaultIntensity;
            lightMat.SetColor("_EmissionColor", Color.white * 3f);
        }
        else
        {
            bulb.intensity = minLightIntensity;
            lightMat.SetColor("_EmissionColor", Color.white * 1f);
        }
    }

    public void FlipLightSwitch() => turnOffAllLights = !turnOffAllLights;

    IEnumerator LightsFlickerIntensity()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
            if (turnOffAllLights) break;
            changeIntensity = !changeIntensity;
        }
    }

    IEnumerator LightsFlickerOnOff()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
            if (turnOffAllLights) break;
            turnOffBulb = !turnOffBulb;
        }
    }
}
