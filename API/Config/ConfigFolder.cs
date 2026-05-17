using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using UnityEngine;
using WKLib.Core.Config;
using WKLib.Utilities;

namespace WKLib.API.Config;

public class ConfigFolder
{
    public string BasePath { get; private set; }
    public string FolderName { get; private set; }
    
    private List<ConfigFolder> subFolders = new List<ConfigFolder>();
    private List<ConfigFile> files = new List<ConfigFile>();
    
    #region Constructors
    public ConfigFolder(string folderName)
    { 
        if (string.IsNullOrWhiteSpace(folderName))
            throw new ArgumentNullException(nameof(folderName));
        
        FolderName = folderName;
        BasePath = Path.Combine(Paths.ConfigPath, folderName); // Default bepinex config path
        if (!Directory.Exists(BasePath))
        {
            Directory.CreateDirectory(BasePath);
            return;
        }

        LoadFiles();
    }
    
    public ConfigFolder(ConfigFolder parent, string folderName)
    { 
        if (string.IsNullOrWhiteSpace(folderName))
            throw new ArgumentNullException(nameof(folderName));
        
        FolderName = folderName;
        BasePath = Path.Combine(parent.BasePath, folderName);
        if (!Directory.Exists(BasePath))
        {
            Directory.CreateDirectory(BasePath);
            return;
        }

        LoadFiles();
    }
    #endregion

    #region SubFolder
    public ConfigFolder GetSubFolder(string folderName)
    {
        if (string.IsNullOrWhiteSpace(folderName))
            return null;
        
        lock (subFolders)
        {
            // If subFolders exist then return
            var exFolder = subFolders.Find(cFolder => string.Equals(cFolder.FolderName, folderName));
            if (exFolder != null)
                return exFolder;
            
            return null;
        }
    }
    
    public ConfigFolder CreateSubFolder(string folderName)
    {
        if (string.IsNullOrWhiteSpace(folderName))
            return null;
        
        lock (subFolders)
        {
            // If subFolder already exists then return null
            var exFolder = subFolders.Find(cFolder => string.Equals(cFolder.FolderName, folderName));
            if (exFolder != null)
                return null;
            
            var path = Path.Combine(BasePath, folderName);
            var newConfigFolder = new ConfigFolder(path);
            subFolders.Add(newConfigFolder);
            return newConfigFolder;
        }
    }
    
    public ConfigFolder GetOrCreateSubFolder(string folderName)
    {
        if (string.IsNullOrWhiteSpace(folderName))
            return null;
        
        lock (subFolders)
        {
            // If subFolders exist then return
            var exFolder = subFolders.Find(cFolder => string.Equals(cFolder.FolderName, folderName));
            if (exFolder != null)
                return exFolder;
            
            var path = Path.Combine(BasePath, folderName);
            var newConfigFolder = new ConfigFolder(path);
            subFolders.Add(newConfigFolder);
            return newConfigFolder;
        }
    }

    public bool RegisterConfigFolder(string folderName)
    {
        if (string.IsNullOrWhiteSpace(folderName))
            return false;
        
        lock (subFolders)
        {
            // If subFolders exist then return
            var exFolder = subFolders.Find(cFolder => string.Equals(cFolder.FolderName, folderName));
            if (exFolder != null)
                return false;
            
            var path = Path.Combine(BasePath, folderName);
            var configFolder = new ConfigFolder(path);
            subFolders.Add(configFolder);
            return true;
        }
    }
    #endregion

    #region ConfigFile
    public ConfigFile GetConfigFile(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return null;
        
        lock (files)
        {
            // Try to find existing file
            var exFile = files.Find(cFile => string.Equals(cFile.FileName, fileName));
            if (exFile != null)
                return exFile;

            return null;
        }
    }
    
    public ConfigFile CreateConfigFile(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return null;
        
        lock (files)
        {
            // Try to find existing file
            var exFile = files.Find(cFile => string.Equals(cFile.FileName, fileName));
            if (exFile != null)
                return null;

            var newConfigFile = new ConfigFile(this, fileName);
            files.Add(newConfigFile);
            return newConfigFile;
        }
    }
    
    public ConfigFile GetOrCreateConfigFile(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return null;
        
        lock (files)
        {
            // Try to find existing file
            var exFile = files.Find(cFile => string.Equals(cFile.FileName, fileName));
            if (exFile != null)
                return exFile;

            var newConfigFile = new ConfigFile(this, fileName);
            files.Add(newConfigFile);
            return newConfigFile;
        }
    }

    public bool RegisterConfigFile(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return false;
        
        lock (files)
        {
            // If file exists then return
            var exFile = files.Find(cFile => string.Equals(cFile.FullFileName, fileName));
            if (exFile != null)
            {
                WKLog.Error($"Existing config file, {fileName} is already registered.");
                return false;
            }
            
            var newConfigFile = new ConfigFile(this, fileName);
            files.Add(newConfigFile);
            return true;
        }
    }
    #endregion

    public ConfigFolder LoadFiles()
    {
        // Load existing JSON files
        string[] jsonFiles = Directory.GetFiles(BasePath, "*.json", SearchOption.TopDirectoryOnly);
        foreach (var jsonFile in jsonFiles)
        {
            if (string.IsNullOrWhiteSpace(jsonFile))
                continue;
            
            RegisterConfigFile(Path.GetFileName(jsonFile));
        }
        
        string[] folders = Directory.GetDirectories(BasePath, "*", SearchOption.TopDirectoryOnly);
        foreach (var folder in folders)
        {
            if (string.IsNullOrWhiteSpace(folder))
                continue;
            
            RegisterConfigFolder(folder);
        }

        return this;
    }

    public List<ConfigFolder> GetConfigFolders()
    {
        lock (subFolders)
            return new List<ConfigFolder>(subFolders);
    }

    public List<ConfigFile> GetConfigFiles()
    {
        lock (files)
            return new List<ConfigFile>(files);
    }

    // Includes subdirectories
    public IEnumerable<ConfigFile> EnumerateFiles()
    {
        lock (files) 
            foreach (var f in files) 
                yield return f;
        
        lock (subFolders) 
            foreach (var sf in subFolders) 
            foreach (var f in sf.EnumerateFiles()) 
                yield return f;
    }
}