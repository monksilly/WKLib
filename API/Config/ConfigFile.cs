using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
            LoadFromDisk();
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
    
    public void LoadFromDisk()
    {
        JObject obj = null;

        if (File.Exists(FilePath))
        {
            try
            {
                var json = File.ReadAllText(FilePath);
                if (!string.IsNullOrWhiteSpace(json))
                    obj = JObject.Parse(json);
            }
            catch
            {
                // if there was an error, just dont load anything
                obj = null;
            }
        }

        // Clear cache
        parsedCache.Clear();
        
        if (obj != null)
        {
            foreach (var prop in obj.Properties())
                parsedCache[prop.Name] = prop.Value.DeepClone();
        }

        // Set values
        lock (registeredValues)
        {
            foreach (var value in registeredValues)
            {
                if (parsedCache.TryGetValue(value.Key, out var token))
                {
                    value.LoadFromJToken(token);
                }
                else
                {
                    value.ResetToDefaultValue();
                }
            }
        }
    }
    
    // one caller may enter immediately and at most one caller can hold it at a time
    readonly SemaphoreSlim saveLock = new SemaphoreSlim(1, 1);

    public async Task SaveAsync()
    {
        await saveLock.WaitAsync().ConfigureAwait(false);
        try
        {
            var settings = CoreSettings.Instance.JsonSerializerSettings;
            var serializer = JsonSerializer.Create(settings);

            // Root object that will become the saved JSON
            var root = new JObject();

            lock (registeredValues)
            {
                foreach (var v in registeredValues)
                {
                    // Don't save default values
                    if (object.Equals(v.GetBoxedValue(), v.GetBoxedDefaultValue())) 
                        continue;

                    // produce a JToken for the value using the serializer
                    JToken valueToken = v.GetBoxedValue() == null
                        ? JValue.CreateNull()
                        : JToken.FromObject(v.GetBoxedValue(), serializer);

                    // split path and ensure nested objects exist
                    var parts = v.Key.Split('.');
                    JObject current = root;
                    for (int i = 0; i < parts.Length - 1; i++)
                    {
                        var part = parts[i];
                        if (current.TryGetValue(part, out var child) && child is JObject childObj)
                        {
                            current = childObj;
                        }
                        else
                        {
                            var newObj = new JObject();
                            current[part] = newObj;
                            current = newObj;
                        }
                    }

                    // set the final property
                    current[parts[^1]] = valueToken;
                }
            }

            // if root has no properties, write empty object {}
            var text = root.HasValues
                ? root.ToString(Formatting.Indented)
                : "{}";

            // Ensure directory exists
            var dir = Path.GetDirectoryName(FilePath);
            if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            await File.WriteAllTextAsync(FilePath, text).ConfigureAwait(false);

            // Update parsedCache to match saved JSON (top-level entries)
            parsedCache.Clear();
            if (root.HasValues)
            {
                foreach (var p in root.Properties())
                    parsedCache[p.Name] = p.Value.DeepClone();
            }
        }
        finally
        {
            saveLock.Release();
        }
    }

    public void SaveSync() => SaveAsync().GetAwaiter().GetResult();
}