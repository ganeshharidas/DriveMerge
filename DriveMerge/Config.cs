using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace DriveMerge
{
    class CConfig
    {
        static ClientSecrets APISecrets;

        public static ClientSecrets GetSecrets ()
        {
            return APISecrets;
        }

        public static void Initialize()
        {
            //
            // Setup API secret.
            // This is from enabling Drive API for "DriveMerge" project in Google Developer Console
            // for developer account ganeshharidas@gmail.com. Key type - "Installed Application".
            // GHTODO: Tied to personal account only temporarily. Need to figure out a clean user based way for this.
            //
            APISecrets = new ClientSecrets();
            APISecrets.ClientId = "643577310687-cmciajj2klrd7ead464duos1lp49l0m4.apps.googleusercontent.com";
            APISecrets.ClientSecret = "xg9KL7qweTQkA07ou5rf0-CX";
        }
    }
}
