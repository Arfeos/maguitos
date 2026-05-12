using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ProfileService : IProfileService
{
    private List<UserProfile> profiles = new List<UserProfile>();
    private UserProfile selectedProfile;
    IUIService uiService;
    private string folderPath => Application.persistentDataPath + "/profiles";

    public void LoadProfiles()
    {
        profiles.Clear();

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            return;
        }

        string[] files = Directory.GetFiles(folderPath, "*.json");

        foreach (var file in files)
        {
            string json = File.ReadAllText(file);

            UserProfile p = JsonUtility.FromJson<UserProfile>(json);

            // GUID = nombre del archivo
            p.guid = Path.GetFileNameWithoutExtension(file);

            profiles.Add(p);
        }
    }


    public List<UserProfile> GetProfiles()
    {
        LoadProfiles();
        return profiles;
    }

    public void CreateProfile(string name, Settings settings, string urlImage = "")
    {
        string guid = System.Guid.NewGuid().ToString();

        UserProfile newProfile = new UserProfile(name, urlImage, settings);
        newProfile.guid = guid;

        string json = JsonUtility.ToJson(newProfile, true);

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string path = Path.Combine(folderPath, guid + ".json");

        File.WriteAllText(path, json);

        profiles.Add(newProfile);
    }

    public void DeleteProfile(string guid)
    {
        string path = Path.Combine(folderPath, guid + ".json");

        if (File.Exists(path))
            File.Delete(path);

        profiles.RemoveAll(p => p.guid == guid);

        if (selectedProfile != null && selectedProfile.guid == guid)
            selectedProfile = null;
    }

    public void SelectProfile(UserProfile profile)
    {
        if(uiService == null)
            uiService = AppContainer.Get<IUIService>();
        selectedProfile = profile;
        Languages lang= (Languages) selectedProfile.settings.language;
        uiService.changeLanguage(lang);
    }


    public void UpdateProfile(UserProfile profile)
    {
        if (profile == null || string.IsNullOrEmpty(profile.guid))
            return;

        string path = Path.Combine(folderPath, profile.guid + ".json");

        string json = JsonUtility.ToJson(profile, true);

        File.WriteAllText(path, json);
    }
    public UserProfile getSelectedProfile() => selectedProfile;
}