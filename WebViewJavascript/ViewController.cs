using Foundation;
using System;
using UIKit;
using WebKit;

namespace WebViewJavascript
{
    public partial class ViewController : UIViewController, IWKScriptMessageHandler
    {
        private string mNativeToWebHandler = "jsMessageHandler";
        private string mWebPageName = "mWebPageName";
        private string mWebPageExtension = "html";

        
        public ViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            sendDataButton.TouchUpInside += SendsendButtonPressedDataButton_TouchUpInside;

            var urls = NSBundle.MainBundle.GetUrlsForResourcesWithExtension(mWebPageName, mWebPageExtension);
            var url = urls[0];//always is first element
            webView.Configuration.Preferences.JavaScriptEnabled = true;
            webView.LoadFileUrl(url, url.RemoveLastPathComponent());

            var contentController = new WKUserContentController();
            webView.Configuration.UserContentController = contentController;
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
