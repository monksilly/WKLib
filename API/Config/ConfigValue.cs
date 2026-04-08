using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WKLib.Core.Config;
using static WKLib.API.Config.ConfigUtility;

namespace WKLib.API.Config;

public abstract class ConfigValueBase
{
    public string Key { get; }
    protected ConfigFile File { get; }

    protected ConfigValueBase(ConfigFile file, string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException(nameof(key));
        
        File = file ?? throw new ArgumentNullException(nameof(file));
        Key = key;
        File.RegisterConfigValue(this);
    }
    
    public abstract object GetBoxedValue();
    public abstract object GetBoxedDefaultValue();
    public abstract void LoadFromJToken(JToken token);
    public abstract void ResetToDefaultValue();
}

public class ConfigValue<T> : ConfigValueBase
{
    private T value;
    public T Value

    {
        get => value;
        set => this.value = value;
    }
    
    private T defaultValue;

    public T DefaultValue
    {
        get => defaultValue; 
        private set => this.defaultValue = value;
    }

    public ref T RefValue => ref this.value;
    public static implicit operator T(ConfigValue<T> cfg) => cfg.Value;

    #region Constructors
    public ConfigValue(ConfigFile file, string key, T defaultValue = default,  bool loadOnCreation = true)
        : base(file, key)
    {
        Initialize(file, key, defaultValue, defaultValue, loadOnCreation);
    }
    
    public ConfigValue(ConfigFile file, string key, T currentValue, T defaultValue, bool loadOnCreation)
        : base(file, key)
    {
        Initialize(file, key, currentValue, defaultValue, loadOnCreation);
    }

    public ConfigValue(WKLibAPI API, string key, T defaultValue = default,  bool loadOnCreation = true)
        : base(API.DefaultConfigFile, key)
    {
        if (API == null)
            throw new ArgumentNullException(nameof(API));
        
        if (API.ConfigFolder == null)
            throw new ArgumentNullException(nameof(API.ConfigFolder));
        
        Initialize(API.DefaultConfigFile, key, defaultValue, defaultValue, loadOnCreation);
    }
    
    internal ConfigValue(string key, T defaultValue = default,  bool loadOnCreation = true) // WKLib only
        : base(CoreSettings.Instance.DefaultConfigFile, key)
    {
        Initialize(CoreSettings.Instance.DefaultConfigFile, key, defaultValue, defaultValue, loadOnCreation);
    }
    #endregion
    
    private void Initialize(ConfigFile file, string key, T currentValue, T defaultValue, bool loadOnCreation = true)
    {
        Value = CloneIfPossible(currentValue);
        DefaultValue = CloneIfPossible(defaultValue);
        
        if (!loadOnCreation)
            return;
        
        // Try load value
        if (file.TryGetParsedToken(key, out var token))
        {
            try
            {
                LoadFromJToken(token);
            }
            catch
            {
                
            }
        }
    }

    public void SetDefaultValue(T newDefaultValue)
    {
        DefaultValue = CloneIfPossible(newDefaultValue);
    }

    public override object GetBoxedValue() => value;
    public override object GetBoxedDefaultValue() => defaultValue;

    //TODO: Custom serializer
    public override void LoadFromJToken(JToken token)
    {
        if (token == null || token.Type == JTokenType.Null) 
            return;
        
        try
        {
            var settings = CoreSettings.Instance.JsonSerializerSettings;
            var json = token.ToString(Formatting.None);
            value = JsonConvert.DeserializeObject<T>(json, settings);
        }
        catch
        {
            //Ignore parse errors
        }
    }
    
    public override void ResetToDefaultValue() { Value = DefaultValue; }
}