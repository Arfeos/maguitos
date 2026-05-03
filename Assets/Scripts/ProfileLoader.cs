using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ProfileLoader : MonoBehaviour
{
    IProfileService profileService;
    List<UserProfile> profiles;
    [SerializeField]GameObject cardPrefab;
    void Start()
    {
        profileService= AppContainer.Get<IProfileService>();
        profiles = profileService.GetProfiles();
        if (profiles.Count <= 0) {
            Debug.Log("no hay perfiles");
            return;
        }
        foreach (UserProfile profiledata in profiles)
        {
            var card = Instantiate(cardPrefab, transform).GetComponent<CardUI>();
            card.Setup(profiledata);
        }
       
    }

}
