using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;

using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace DriveMerge
{
    enum EAuthType
    {
        Web = 1,
        Embedded
    };

    class CAuthorize
    {
        static UserCredential Cred;

        public static UserCredential GetCredential()
        {
            return Cred;
        }

        public static void Authorize (EAuthType AuthType = EAuthType.Embedded)
        {
            if (AuthType == EAuthType.Web)
            {
                WebAuthorize ();
            }
            else if (AuthType == EAuthType.Embedded)
            {
                EmbeddedWebAuthorize ();
            }
            else 
            {
                
            }
        }

        static async void WebAuthorize ()
        {
            Cred = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                CConfig.GetSecrets (),
                new[] { DriveService.Scope.Drive },
                "user",
                CancellationToken.None);
        }

        static void EmbeddedWebAuthorize()
        {
            
        }
    }
}
