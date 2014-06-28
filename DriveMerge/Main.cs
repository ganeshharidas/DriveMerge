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
        //
        // We should use single threaded apartment to be able to initialize ActiveX controls
        // (which is required by Browser class). 
        // GHTODO: Find out if it is enough to set to this on just the CWebBrowser class.
        //
        [STAThread]
        static void Main (string[] args)
        {
            CConfig.Initialize();

            //
            // GHTODO: Need to implement embedded web authorization type.
            //
            CAuthorize.Authorize (EAuthType.Web);

            //
            // Create the service.
            // Does it launch a process?
            //
            BaseClientService.Initializer InitHelper = new BaseClientService.Initializer ();
            InitHelper.HttpClientInitializer = CAuthorize.GetCredential ();
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
    }
}
