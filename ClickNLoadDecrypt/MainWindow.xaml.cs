using MahApps.Metro.Controls;
using System;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace ClickNLoadDecrypt
{

    public partial class MainWindow : MetroWindow
    {

        protected HttpListener Listener;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Listener = new HttpListener();
            this.Listener.Prefixes.Add("http://*:9666/");
            this.Listener.Start();
            Debug.WriteLine("Server started...");
            IAsyncResult result = this.Listener.BeginGetContext(new AsyncCallback(WebRequestCallback), this.Listener);
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Listener.Close();
            this.Listener = null;
        }

        protected void WebRequestCallback(IAsyncResult result)
        {
            HttpListenerContext context = this.Listener.EndGetContext(result);
            this.Listener.BeginGetContext(new AsyncCallback(WebRequestCallback), this.Listener);

            this.ProcessRequest(context);
        }

        protected void ProcessRequest(HttpListenerContext context)
        {

            Debug.WriteLine(context.Request.RawUrl);

            // build response data
            HttpListenerResponse response = context.Response;
            string responseString = "";

            response.StatusCode = 200;
            response.Headers.Add("Content-Type: text/html");

            // crossdomain.xml
            if (context.Request.RawUrl == "/crossdomain.xml")
            {
                responseString = "<?xml version=\"1.0\"?>"
                    + "<!DOCTYPE cross-domain-policy SYSTEM \"http://www.macromedia.com/xml/dtds/cross-domain-policy.dtd\">"
                    + "<cross-domain-policy>"
                    + "<allow-access-from domain=\"*\" />"
                    + "</cross-domain-policy>";

            } else if( context.Request.RawUrl == "/jdcheck.js" )
            {
                responseString = "jdownloader=true; var version='18507';";

            }
            else if (context.Request.RawUrl.StartsWith("/flash"))
            {
                if (context.Request.RawUrl.Contains("addcrypted2"))
                {
                    System.IO.Stream body = context.Request.InputStream;
                    System.IO.StreamReader reader = new System.IO.StreamReader(body, context.Request.ContentEncoding);

                    String requestBody = System.Web.HttpUtility.UrlDecode(reader.ReadToEnd());

                    // get encrypted data
                    Regex rgxData = new Regex("crypted=(.*?)(&|$)");
                    String data = rgxData.Match(requestBody).Groups[1].ToString();

                    // get encrypted pass
                    Regex rgxPass = new Regex("jk=(.*?){(.*?)}(&|$)");
                    String pass = rgxPass.Match(requestBody).Groups[2].ToString();

                    var jsEngine = new Jurassic.ScriptEngine();
                    pass = jsEngine.Evaluate("(function (){" + pass + "})()").ToString();

                    // show decrypted links
                    Dispatcher.BeginInvoke(
                        new Action<Object>((sender) => { showDecryptedLinks(pass,data); })
                    ,   new object[] { this } 
                    );

                    responseString = "success\r\n";
                }
                else
                {
                    responseString = "JDownloader";
                }
            }
            else
            {
                response.StatusCode = 400;
            }

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;

            // output response
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        private void updateCopyBtn(object sender, TextChangedEventArgs e)
        {
            if ( txtLinks.Text.Length > 0 )
                btnCopy.IsEnabled = true;
            else
                btnCopy.IsEnabled = false;
        }

        private void copyLinksToClipboard(object sender, RoutedEventArgs e)
        {
            System.Windows.Clipboard.SetText( txtLinks.Text );
        }

        private void showDecryptedLinks( String key, String data ) {

            String links = decryptLinks(key, data);

            this.Activate();
            txtLinks.Text = links;
            updateCopyBtn(null, null);
        }

        private string decryptLinks(String key, String data)
        {
            // decode key
            key = key.ToUpper();

            String decKey = "";
            for (int i = 0; i < key.Length; i += 2)
            {
                decKey += (char)Convert.ToUInt16(key.Substring(i, 2), 16);
            }

            // decode data
            byte[] dataByte = Convert.FromBase64String(data);

            // decrypt that shit!
            RijndaelManaged rDel = new RijndaelManaged();
            System.Text.ASCIIEncoding aEc = new System.Text.ASCIIEncoding();

            rDel.Key = aEc.GetBytes(decKey);
            rDel.IV = aEc.GetBytes(decKey);
            rDel.Mode = CipherMode.CBC;

            rDel.Padding = PaddingMode.None;
            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(dataByte, 0, dataByte.Length);

            String rawLinks = aEc.GetString(resultArray);

            // replace empty paddings
            Regex rgx = new Regex("\u0000+$");
            String cleanLinks = rgx.Replace(rawLinks, "");

            // replace newlines
            rgx = new Regex("\n+");
            cleanLinks = rgx.Replace(cleanLinks, "\r\n");

            return cleanLinks;
        }
    }
}
