using System;
using System.Net;
using System.Threading.Tasks;

namespace http_listener
{
    public class Listener
    {
        public Listener()
        {
            HttpListener = new HttpListener();
        }

        public void Start()
        {
            HttpListener.Prefixes.Add("http://localhost:8086/");
            HttpListener.Start();
            HttpListener.BeginGetContext(BeginProcessAsync, null);
        }

        private async void BeginProcessAsync(IAsyncResult asyncResult)
        {
            if (!HttpListener.IsListening)
                return;

            HttpListener.BeginGetContext(BeginProcessAsync, null);
            var context = HttpListener.EndGetContext(asyncResult);

            var request = context.Request;
            if (request.ContentType != "application/x-grobuf")
            {
                Console.WriteLine($"ContentType: '{request.ContentType}', ContentLength: {request.ContentLength64}");
                HttpListener.Stop();
                return;
            }

            var buffer = System.Text.Encoding.UTF8.GetBytes("<HTML><BODY> Hello world!</BODY></HTML>");
            context.Response.ContentLength64 = buffer.Length;
            var output = context.Response.OutputStream;
            await output.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
            output.Close();
        }

        public HttpListener HttpListener { get; }
    }

    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var listener = new Listener();
            listener.Start();
            while (listener.HttpListener.IsListening)
                await Task.Delay(500).ConfigureAwait(false);
        }
    }
}