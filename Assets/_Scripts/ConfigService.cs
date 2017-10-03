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
            Debug.LogError("[ConfigService] Unable to locate any configurations! You must " +
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
                Debug.LogError("[ConfigService] Unable to locate default configuration. You " +
                    "must create one in Resources/Config");
                return;
            }

            Debug.Log("[ConfigService] Selecting default config", this);
            selectedConfig = defaultConfig;
        }

        Debug.Log("[ConfigService] Selecting local config", this);
        selectedConfig = localConfig;
    }

    public Config SelectedConfig()
    {
        return selectedConfig;
    }
}
