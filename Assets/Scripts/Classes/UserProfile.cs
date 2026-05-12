[System.Serializable]
public class UserProfile
{
    public string guid;
    public string name;
    public string urlImage;
    public Settings settings;

    public UserProfile(string name, string urlImage, Settings settings)
    {
        this.name = name;
        this.urlImage = urlImage;
        this.settings = settings;
    }
}