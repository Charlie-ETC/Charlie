using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.UI;
using Charlie.Twitter;
using Charlie.Apiai;
using System.Threading.Tasks;

public class ActionUploadPicture : FsmStateAction
{
    FsmGameObject photoGameObject;
    
    public async override void OnEnter()
    {
        Texture2D texture = GameObject.Find("PhotoTaken").GetComponent<RawImage>().texture as Texture2D;
        byte[] jpegData = ImageConversion.EncodeToJPG(texture, 80);

        Debug.Log("[ActionUploadPicture] Uploading picture to Twitter");
        Media media = await TwitterService.Instance.UploadMedia(jpegData);
        Debug.Log($"[ActionUploadPicture] Media uploaded with ID {media.mediaIdString}");

        // Obtain the user profile from the context.
        string caption = await GenerateCaption();
        TwitterService.Instance.TweetWithMedia(caption, new string[1] { media.mediaIdString });
    }

    private async Task<string> GenerateCaption()
    {
        List<Context> contexts = await ApiaiService.Instance.GetContexts(DictationMonitor.Instance.apiaiSessionId);
        Context userContext = contexts.Find(context => context.name == "user");
        if (userContext != null && userContext.parameters != null)
        {
            string name = GetUserContextParam(userContext, "name");
            string favoriteCity = GetUserContextParam(userContext, "favorite_city");
            string favoriteCountry = GetUserContextParam(userContext, "geo_country");
            string location = null;
            if (favoriteCity != null)
            {
                location = favoriteCity;
            }
            else
            {
                location = favoriteCountry;
            }

            if (name != null && location != null)
            {
                return $"I met {name} who likes {location}!";
            }
            else if (name != null)
            {
                return $"I met {name} who is a fun person to be around!";
            }
            else if (location != null)
            {
                return $"I met Pumpkin who likes {location}!";
            }
            else
            {
                return "Hello there!";
            }
        }

        return "Someone visited me today!";
    }

    private string GetUserContextParam(Context context, string key)
    {
        return context.parameters.ContainsKey(key) ? context.parameters[key] as string : null;
    }
}
