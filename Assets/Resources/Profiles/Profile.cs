using UnityEngine;

public class Profile
{
    string name;
    string id;
    Configuration configuration;
    
    public Profile(string name, string id, Configuration configuration)
    {
        this.name = name;
        this.id = id;
        this.configuration = configuration;
    }
}
