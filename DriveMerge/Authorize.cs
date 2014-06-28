using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Windows.Forms;

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
        static string UserName;
        static string Pass;

        static CAuthorize ()
        {
            UserName = "ganeshharidas@gmail.com";
            Pass = "****";
        }

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
            CEmbeddedWebAuthorizationBroker.Authorize (UserName, Pass);
        }


    }

    class CWebBrowser
    {
        WebBrowser Browser;

        public CWebBrowser ()
        { 
            Browser = new WebBrowser ();
            Browser.DocumentCompleted += OnDocumentLoadComplete;
        }

        private void OnDocumentLoadComplete (Object Sender, WebBrowserDocumentCompletedEventArgs Event)
        {
            WebBrowser Internal = (WebBrowser) Sender;
            Application.ExitThread ();
        }

        public void Navigate (string Uri)
        {
            Thread NavigateThread = new Thread (() => NavigateInternal (Uri));

            //
            // We should use single threaded apartment to be able to initialize ActiveX controls
            // (which is required by Browser class). 
            //
            NavigateThread.SetApartmentState (ApartmentState.STA);
            NavigateThread.Start ();

            //
            // Wait for the load event.
            // Navigate thread uses the MessageLoop. And we ask it to exit only when document load event
            // is complete.
            //
            NavigateThread.Join ();
        }

        private void NavigateInternal (String Uri)
        {
            Browser.Navigate (Uri);
            Application.Run ();
        }

        public HtmlDocument GetDocument ()
        {
            if (Browser != null)
            {
                return Browser.Document;
            }
            else
            {
                return null;
            }
        }
    }

    class CEmbeddedWebAuthorizationBroker 
    {
        static CWebBrowser Browser;

        //
        // Login page elements.
        //
        static HtmlElement EmailField;
        static string EmailFieldId;
        static HtmlElement PassField;
        static string PassFieldId;
        static HtmlElement SignInButton;
        static string SignInButtonId;

        //
        // Permission page elements.
        //
        static HtmlElement AcceptButton;
        static string AcceptButtonId;

        //
        // This is what we are working for.
        // 
        static string AuthCode;

        static CEmbeddedWebAuthorizationBroker ()
        {
            Browser = new CWebBrowser ();
            EmailFieldId = "Email";
            PassFieldId = "Passwd";
            SignInButtonId = "signIn";
            AcceptButtonId = "submit_approve_access";
        }




        public static bool Authorize (string UserName, string Pass)
        {
            //
            // Setup the webserver to handle redirects.
            //
            CWebServer WebServer = new CWebServer ();
            WebServer.Start (8080);

            //
            // Navigate to Auth page.
            //
            Browser.Navigate (GetAuthUri ());

            //
            // Auth page will redirect to sign in page if already not logged in.
            //
            HtmlDocument Page = Browser.GetDocument ();
            if (IsDocLoginPage (Page) == true)
            {
                //
                // Fill in login details. And click Sign in.
                //
                EmailField.SetAttribute ("value", UserName);
                PassField.SetAttribute ("value", Pass);
                SignInButton.InvokeMember ("Click");

                //
                // Now reload the auth page. Since we are already signed in, we should get to the auth page directly.
                //
                Browser.Navigate(GetAuthUri());
                Page = Browser.GetDocument ();
            }
            
            if (IsDocPermissionsPage (Page) == true)
            {
                AcceptButton.InvokeMember ("Click");

                //
                // Do the processing to get auth code.
                //
                AuthCode = GetAuthCode ();

                //
                // Set Page to null. This is the expected state var to track failures.
                //
                Page = null;
            }

            if (Page != null)
            {
                return false;
            }
            else
            {
                if (GetAccessToken () == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        static bool GetAccessToken ()
        {
            //
            // GHTODO: Do the token call.
            //
            return false;
        }

        static bool IsDocLoginPage (HtmlDocument Page)
        {
            EmailField = Page.GetElementById (EmailFieldId);
            PassField = Page.GetElementById (PassFieldId);
            SignInButton = Page.GetElementById (SignInButtonId);

            //
            // GHTODO: Check for type also?
            //
            if (EmailField != null &&
                PassField != null &&
                SignInButton != null)
            {
                return true;
            }
            else
            {
                EmailField = null;
                PassField = null;
                SignInButton = null;
                return false;
            }
        }

        static bool IsDocPermissionsPage (HtmlDocument Page)
        {
            AcceptButton = Page.GetElementById(AcceptButtonId);

            //
            // GHTODO: Check for type also?
            //
            if (AcceptButton != null)
            {
                return true;
            }
            else
            {
                AcceptButton = null;
                return false;
            }
        }

        static string GetAuthUri ()
        {
            return "https://accounts.google.com/o/oauth2/auth?response_type=code&client_id=643577310687-cmciajj2klrd7ead464duos1lp49l0m4.apps.googleusercontent.com&redirect_uri=http://localhost:8080&scope=https://www.googleapis.com/auth/drive&state=ganesh&include_granted_scopes=true";
        }

        static string GetAuthCode ()
        {
            if (AuthCode != null)
            {
                return AuthCode;
            }
            else
            {
                //
                // GHTODO: Process response from auth call.
                // This may involve getting auth code from title bar or from a localhost webbrowser.
                //
                return "";
            }
        }
    }
}
