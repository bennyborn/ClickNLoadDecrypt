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

/*
 * base64 jdownloader logo
 * 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
 * */

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

            /*
            Dispatcher.Invoke(
                DispatcherPriority.Normal
                , new Action<string>(setLinks)
                , context.Request.RawUrl
            );
            */

            // build response data
            HttpListenerResponse response = context.Response;
            string responseString = "<HTML><BODY>application should now be visible</BODY></HTML>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;

            // output response
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        public void setLinks(string links)
        {
            this.Activate();
            txtLinks.AppendText("\r\n" + links);
            updateCopyBtn(null, null);
            testDecrypt(null, null);
        }

        private void dragWindow(object sender, MouseButtonEventArgs e)
        {
            /*
             * seems to throw an error
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
             */
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

        private void testDecrypt(object sender, RoutedEventArgs e)
        {

            Debug.WriteLine("test javascript");
            var engine = new Jurassic.ScriptEngine();
            Debug.WriteLine(engine.Evaluate("(function (){a=new Array('3934','3537','3038','3632','3135','3833','3135','3731');var n='';for(var i=7;i>=0;i--)n=n+a[i].split('').reverse().join('').split('').reverse().join('').split('').reverse().join('');return n.split('').reverse().join('');})()"));
            

            Debug.WriteLine("test decrypting");
            String links = decryptLinks("6c4e57663861746d5176654c4f52476a", "i+22MYLELxtBTOhhXvvxh8NxD5l3/H4hzfuwkAiLjSLzlSOXKaRfkIr8FSX6GOsDZzu61hnVBNJLnMBEY5i2dPUB3SBGAAc6nUaeeCV4xq7L65/SAzbVWjQUQgfmuG4jebWa5CYL1u83llJJnfacW8h2GCiGlgOFQEWYDsgTp6cBri0OkxcHg7tj5kZykEQ+rG+ncuf9xQAZrmUwVksND83q+gUbLziEA70w2jHwJ1Zz7v73iU+B7Ingj4jdV8zUvglBBGXnZ/8onMIPS3vQiqUlpXtNgMPE2cbP6YTlu4RXUqoklFWGaGg6ENTdKm/zrjdOmSKWofXeqm+uPW/6jY8TvTACNUNGEs09D3VSKsDA3tkkXIggy82W86BhjgE3M5XWmNs3yl+sxT7bOKVpJ8JuIyfEWploFQAJXJuqjb6OJiRDnqBc8KfOiWBkNLQ7N6qccPv8UEZXFcodgVD2/g==");
            txtLinks.Text = links;

        }
    }
}
