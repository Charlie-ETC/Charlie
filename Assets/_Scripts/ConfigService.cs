using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigService : MonoBehaviour {
    private Config selectedConfig;

    void Awake () {
        Config[] configs = Resources.LoadAll<Config>("Config");
        if (configs.Length == 0)
        {
            Debug.LogError("Unable to locate any configurations! You must " +
                "create one in Resources/Config");
            return;
        }

        // Determine the appropriate config.
        Config localConfig = Array.Find(configs, item => item.name == "local");
        Config defaultConfig = Array.Find(configs, item => item.name == "default");

        if (!localConfig)
        {
            if (!defaultConfig)
            {
                Debug.LogError("Unable to locate default configuration. You " +
                    "must create one in Resources/Config");
                return;
            }

            Debug.Log("Selecting default config", this);
            selectedConfig = defaultConfig;
        }

        Debug.Log("Selecting local config", this);
        selectedConfig = localConfig;
    }

    public Config SelectedConfig()
    {
        return selectedConfig;
    }
}
