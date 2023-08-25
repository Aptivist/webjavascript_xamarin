using Foundation;
using System;
using System.Linq;
using UIKit;
using WebKit;

namespace WebViewJavascript
{
    public partial class ViewController : UIViewController, IWKScriptMessageHandler
    {
        private string mNativeToWebHandler = "jsMessageHandler";
        private string mWebPageName = "sampleweb";
        private string mWebPageExtension = "html";

        
        public ViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            sendDataButton.TouchUpInside += SendsendButtonPressedDataButton_TouchUpInside;

            var url = NSBundle.MainBundle.GetUrlForResource(mWebPageName, mWebPageExtension);
            webView.Configuration.Preferences.JavaScriptEnabled = true;
            webView.LoadFileUrl(url, url);

            var contentController = new WKUserContentController();
            webView.Configuration.UserContentController = contentController;

            //other way
            //var script = new WKUserScript(new NSString(JavaScriptFunction), WKUserScriptInjectionTime.AtDocumentEnd, false);
            //contentController.AddUserScript(script);
            webView.Configuration.UserContentController.AddScriptMessageHandler(this, mNativeToWebHandler);
        }

        private void SendsendButtonPressedDataButton_TouchUpInside(object sender, EventArgs e)
        {
            var data = inputData.Text;
            webView.EvaluateJavaScript($"document.getElementById('inputField').value='({data})'", OnEvaluationResult);
        }

        private void OnEvaluationResult(NSObject result, NSError error)
        {
            Console.WriteLine($"result: {result}");
            Console.WriteLine($"error: {error}");
        }

        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
            if (message.Name == mNativeToWebHandler)
            {
                var body = message.Body as NSString;
                inputData.Text = body.ToString();
            }
        }
    }
}
