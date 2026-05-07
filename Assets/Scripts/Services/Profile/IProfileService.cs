using System.Collections.Generic;


public interface IProfileService
{
    void LoadProfiles();

    List<UserProfile> GetProfiles();   

    void SelectProfile(UserProfile profile);

    void CreateProfile(string name, Settings settings, string urlImage ="");

    void DeleteProfile(string guid);

    void UpdateProfile(UserProfile profile);
    public UserProfile getSelectedProfile();
}