using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using UnityEditor;
using UnityEngine;

public class CFGoogleDriveApi : MonoBehaviour
{
    private const string JsonPath = "Assets/cfkey_GoogleServiceAccount.json";
    
    void Start()
    {
        var credentialJson = AssetDatabase.LoadAssetAtPath<TextAsset>(JsonPath);
        if (credentialJson != null)
        {
            Debug.Log(credentialJson?.text);
            var credential = GoogleCredential.FromJson(credentialJson.text)
                .CreateScoped(new[] { DriveService.ScopeConstants.DriveReadonly });
            
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });
            
            var request = service.Files.List();
            var response = request.Execute();
            foreach (var file in response.Files)
            {
                Debug.Log($"File: {file.Name} ({file.Id})");
            }
        }
    }

    void Update()
    {
        
    }
}
