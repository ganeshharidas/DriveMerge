using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.Threading.Tasks;

using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace DriveMerge
{
    class CMain
    {
        static ClientSecrets APISecrets;
        static UserCredential Cred;

        static void Main (string[] args)
        {
            //
            // Setup API secret.
            // This is from enabling Drive API for "DriveMerge" project in Google Developer Console
            // for developer account ganeshharidas@gmail.com. Key type - "Installed Application".
            // GHTODO: Tied to personal account only temporarily. Need to figure out a clean user based way for this.
            //
            APISecrets = new ClientSecrets ();
            APISecrets.ClientId = "643577310687-cmciajj2klrd7ead464duos1lp49l0m4.apps.googleusercontent.com";
            APISecrets.ClientSecret = "xg9KL7qweTQkA07ou5rf0-CX";

            //
            // Get user credentials and authenticate.
            // GHTODO: This currently opens a web browser with credentials page, authenticates and then 
            // returns back to the application. OAuth is cool for keeping it simple.
            // But we are going to be multiple accounts here. Manually logging in for each user isnt very good.
            // Need to figure out a way to login from the application itself.
            // 
            UserCredential Cred = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    APISecrets,
                    new[] { DriveService.Scope.Drive },
                    "user",
                    CancellationToken.None)
                .Result;

            //
            // Create the service.
            // Does it launch a process?
            //
            BaseClientService.Initializer InitHelper = new BaseClientService.Initializer ();
            InitHelper.HttpClientInitializer = Cred;
            InitHelper.ApplicationName = "DriveMerge";
            DriveService Drive = new DriveService(InitHelper);
            
            //
            // Get the file list.
            //
            int iFileCount = 0; 
            FilesResource.ListRequest Req = Drive.Files.List();
            do
            {
                FileList List = Req.Execute();
                iFileCount += List.Items.Count();
                foreach (File Item in List.Items)
                {
                    if (Item.Title.Length != 0)
                    {
                        Console.WriteLine("File: " + Item.Title + ", Modifier: " + Item.LastModifyingUserName);
                    }
                }

                //
                // Go to next page.
                //
                Req.PageToken = List.NextPageToken;
            } 
            while (String.IsNullOrEmpty(Req.PageToken) == false);

            Console.WriteLine("Found " + iFileCount + " files, good sir.");

            //
            // Yes, I am a C++ developer.
            //
            System.Console.ReadLine ();
        }

        private static async void Authenticate (bool bRefresh)
        {
            if (bRefresh && Cred != null)
            {
                await Cred.RefreshTokenAsync(CancellationToken.None);
            }
           
           Cred = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                   APISecrets,
                   new[] { DriveService.Scope.Drive },
                   "user",
                   CancellationToken.None,
                   new FileDataStore("Drive.Sample.Credentals"));
        }
    }
    
    
}
