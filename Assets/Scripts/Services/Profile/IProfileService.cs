using System.Collections.Generic;


public interface IProfileService
{
    void LoadProfiles();

    List<UserProfile> GetProfiles();   

    void SelectProfile(UserProfile profile);

    void CreateProfile(string name, string urlImage, Settings settings);

    void DeleteProfile(string guid);   

    void UpdateProfile(UserProfile profile);
}