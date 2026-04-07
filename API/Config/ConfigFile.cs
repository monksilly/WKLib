using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using WKLib.Core.Config;

namespace WKLib.API.Config;

public class ConfigFile
{
    public string FilePath { get; private set; }
    public string FullFileName { get; private set; }
    public string FileName { get; private set; } //Filename without extension

    private Dictionary<string, JToken> parsedCache = new Dictionary<string, JToken>();
    private List<ConfigValueBase> registeredValues = new List<ConfigValueBase>();

    #region Constructors
    public ConfigFile(string filePath)
    {
        Initialize(filePath);
    }

    public ConfigFile(ConfigFolder configFolder, string fileName)
    {
        if (configFolder == null)
            throw new ArgumentNullException(nameof(configFolder));

        Initialize(Path.Combine(configFolder.BasePath, fileName));
    }
    
    public ConfigFile(WKLibAPI API, string fileName)
    {
        if (API == null)
            throw new ArgumentNullException(nameof(API));
        
        if (API.ConfigFolder == null)
            throw new ArgumentNullException(nameof(API.ConfigFolder));

        Initialize(Path.Combine(API.ConfigFolder.BasePath, fileName));
    }

    //TODO: Disallow bad filenames and ".."
    private void Initialize(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentNullException(nameof(filePath));

        FilePath = filePath;
        FullFileName = Path.GetFileName(FilePath);
        FileName = Path.GetFileNameWithoutExtension(FilePath);
        
        // Try load file
        try
        {
            //LoadFromDisk();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load config file ({filePath}): {ex.Message}");
        }
    }
    #endregion

    #region ValueParsing
    // We get 2 strings like "MyConfig.Element" and "MyConfig.Element2" and we save like
    // MyConfig
    // - Element: value
    // - Element2: value
    private bool TryGetParsedTokenByPath(string path, out JToken token)
    {
        token = null;
        if (string.IsNullOrEmpty(path)) 
            return false;

        var parts = path.Split('.');
        if (parts.Length <= 0)
            return false;

        // first segment must be a top-level property in parsedCache
        if (!parsedCache.TryGetValue(parts[0], out var current) || current == null)
            return false;

        token = current;
        for (int i = 1; i < parts.Length; i++)
        {
            if (token is JObject obj 
                && obj.TryGetValue(parts[i], out var child))
            {
                token = child;
            }
            else
            {
                token = null;
                return false;
            }
        }

        return token != null;
    }
    
    public bool TryGetParsedToken(string key, out JToken token)
    {
        token = null;
        if (string.IsNullOrEmpty(key)) 
            return false;
        
        if (TryGetParsedTokenByPath(key, out token))
            return true;
        
        return false;
    }

    public bool TryGetParsedValueObject(string key, out object value)
    {
        value = null;
        
        if (TryGetParsedToken(key, out var token))
        {
            if (token == null || token.Type == JTokenType.Null) 
                return false;

            try
            {
                var settings = CoreSettings.Instance.JsonSerializerSettings;
                var json = token.ToString(Formatting.None);
                value = JsonConvert.DeserializeObject(json, settings);
            }
            catch
            {
                return false;
            }

            return true;
        }

        return false;
    }
    #endregion
    
    public ConfigValueBase AddOrUpdateConfigValue<T>(string key, T currentValue, T defaultValue, bool loadOnCreation)
    {
        if (string.IsNullOrEmpty(key))
            return null;
        
        lock (registeredValues)
        {
            var exConfigValue = registeredValues.Find(cValue => string.Equals(cValue.Key, key));
            if (exConfigValue != null)
            {
                if (exConfigValue is ConfigValue<T> typed)
                {
                    typed.Value = currentValue;   // set current value
                }
                
                return exConfigValue;
            }
            
            var newConfigValue = new ConfigValue<T>(this, key, currentValue, defaultValue, loadOnCreation);
            registeredValues.Add(newConfigValue);
            return newConfigValue;
        }
    }
    
    public void RegisterConfigValue(ConfigValueBase configValue)
    {
        if (string.IsNullOrEmpty(configValue.Key))
            return;
        
        lock (registeredValues)
        {
            var exConfigValue = registeredValues.Find(cValue => string.Equals(cValue.Key, configValue.Key));
            if (exConfigValue != null)
            {
                Debug.LogError($"Existing config key, {configValue.Key} is already registered.");
                return;
            }
            
            registeredValues.Add(configValue);
        }
    }
}