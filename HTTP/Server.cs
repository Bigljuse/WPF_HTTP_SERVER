using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using WPF_HTTP_SERVER.DataBase;
using WPF_HTTP_SERVER.DataBase.Models;
using Python.Runtime;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Linq.Expressions;

namespace WPF_HTTP_SERVER.HTTP
{
    public enum Status
    {
        Off,
        Listening,
        Error,
    }

    public static partial class Server
    {
        public static string Ip { get; private set; } = "";
        public static string Port { get; private set; } = "";
        public static Uri Uri { get; private set; } = new Uri("http://127.0.0.1:8888/");
        public static string PythonDllPath { get; private set; } = "";
        public static Status Status { get; private set; } = Status.Off;
        public static string ErrorMessage { get; private set; } = "";
    }

    public static partial class Server
    {
        private static HttpListener _HTTPListener = new HttpListener();
        private static object _lockObject = new object();
    }

    public static partial class Server
    {
        public static void SetServer(string ip, string port)
        {
            Ip = ip;
            Port = port;
            Uri = new Uri($"http://{Ip}:{Port}/");

            _HTTPListener.Prefixes.Clear();
            _HTTPListener.Prefixes.Add(Uri.ToString());
        }

        public static async void Start()
        {
            ErrorMessage = string.Empty;
            Status = Status.Listening;

            _HTTPListener.Start();

            try
            {
                //var result = listener.BeginGetContext(new AsyncCallback(ProcessRequest), listener);
                while (Status == Status.Listening)
                {
                    var result = await _HTTPListener.GetContextAsync();
                    await ProcessRequest(result);
                }
            }
            catch (HttpListenerException exception)
            {
                HttpListenerExceptionHandler(exception);
            }
            catch (ObjectDisposedException exception)
            {
                HttpListenerExceptionHandler(exception);
            }

            _HTTPListener.Stop();
            //_HTTPListener.Close();
        }

        public static void Stop()
        {
            try
            {
                Status = Status.Off;
            }
            catch (Exception ex)
            {
                Status = Status.Error;
                ErrorMessage = ex.Message;
            }
        }

        public static void SetPythnDllPath(string path)
        {
            PythonDllPath = path;
        }
    }

    public static partial class Server
    {
        private async static Task ProcessRequest(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            if (request.HttpMethod == HTTPMethods.GET.ToString())
                await HandleGETRequest(context);
            else if (request.HttpMethod == HTTPMethods.POST.ToString())
                await HandlePOSTRequest(context);
            else if (request.HttpMethod == HTTPMethods.PUT.ToString())
                return;
            else if (request.HttpMethod == HTTPMethods.DELETE.ToString())
                return;
            else if (request.HttpMethod == HTTPMethods.OPTIONS.ToString())
                return;
            else if (request.HttpMethod == HTTPMethods.PATCH.ToString())
                return;
            else if (request.HttpMethod == HTTPMethods.TRACE.ToString())
                return;
            else if (request.HttpMethod == HTTPMethods.HEAD.ToString())
                HandleHEADRequest(context);
            else
            {
                //await response.OutputStream.FlushAsync();
                ErrorMessage = "Unrecognized HTTP method detected.";
                response.Close();
                return;
            }

            response.Close();
        }

        private static void HandleHEADRequest(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            response.StatusCode = (int)HttpStatusCode.OK;
            response.ContentType = ContentTypes.TextTypes[TextTypes.plain];
            response.ContentLength64 = 0;

            if (request.Headers.Get("Type") == "Prediction_Generation_Image")
            {
                response.Headers.Add("Type", "Prediction_Generation_Image");
                if (request.Headers.Get("Type_Stage") == "Generating")
                {
                    response.Headers.Add("Type_Stage", "Generating");
                    //string[] args = new string[0];
                    string pythonText = File.ReadAllText("Test.py");
                    StartPythonGeneration(pythonText);
                }
                return;
            }
        }

        private async static Task HandleGETRequest(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            var headerTypeValue = request.Headers.Get("Type");

            if (headerTypeValue == "Predictions")
            {
                response.Headers.Add("Type", "Predictions");

                PredictionModel[] predictions = await DataBaseMethods.Getpredictions();
                string outputContent = JsonSerializer.Serialize(predictions);
                await SendOutputContent(response.OutputStream, outputContent);
                return;
            }
            if (headerTypeValue == "Prediction_Generation_Image")
            {
                response.Headers.Add("Type", "Prediction_Generation_Image");
                if (request.Headers.Get("Type_Stage") == "Result")
                {
                    response.Headers.Add("Type_Stage", "Result");
                    string contentData = await SerializeImage("Image.png");
                    //string outputContent = JsonSerializer.Serialize(contentData);
                    await SendOutputContent(response.OutputStream, contentData);
                    return;
                }
            }
        }

        private async static Task HandlePOSTRequest(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            if (request.Headers.Get("Type") == "Authorization")
            {
                response.Headers.Add("Type", "Authorization");

                string inputContent = await ReadInputStream(request.InputStream);// .GetValues("User").ToArray();
                var user = JsonSerializer.Deserialize<UserModel>(inputContent);

                bool exist = false;
                if (user is not null)
                    exist = await DataBaseMethods.CheckUser(user);

                ResultModel result = new ResultModel() { Result = exist };
                string outputContent = JsonSerializer.Serialize<ResultModel>(result);
                await SendOutputContent(response.OutputStream, outputContent);
            }
            if (request.Headers.Get("Type") == "Prediction_Generation_Image")
            {
                response.Headers.Add("Type", "Prediction_Generation_Image");

                var imageArray = await ReadInputImageStream(request.InputStream);
                File.WriteAllBytes("Image.png", imageArray);
                response.StatusCode = 200;
            }
        }

        private static void HttpListenerExceptionHandler(HttpListenerException exception)
        {
            if (exception.ErrorCode == 995)
                Status = Status.Off;
            else
                Status = Status.Error;

            ErrorMessage = exception.Message;
        }

        private static void HttpListenerExceptionHandler(ObjectDisposedException exception)
        {
            Status = Status.Off;
            ErrorMessage = exception.Message;
        }

        private async static Task<string> ReadInputStream(Stream stream)
        {
            string content = "";

            using (var streamReader = new StreamReader(stream))
            {
                content = await streamReader.ReadToEndAsync();
            }

            return content;
        }

        private async static Task<byte[]> ReadInputImageStream(Stream stream)
        {
            byte[] imageArray;

            using (var streamReader = new StreamReader(stream))
            {
                var imageText = await streamReader.ReadToEndAsync();
                imageArray = Convert.FromBase64String(imageText);
            }

            return imageArray;
        }

        private async static Task SendOutputContent(Stream outputStream, string outputContent)
        {
            using (outputStream)
            {
                var buffer = Encoding.UTF8.GetBytes(outputContent);
                await outputStream.WriteAsync(buffer);
                await outputStream.FlushAsync();
            }
        }

        private async static Task<string> SerializeImage(string path)
        {
            var fileData = await File.ReadAllBytesAsync(path);
            var imageData = Convert.ToBase64String(fileData);
            return imageData;
        }

        private static void StartPythonGeneration(string code)
        {
            Task.Run(() =>
            {
                lock (_lockObject)
                {
                    try
                    {
                        Runtime.PythonDLL = Settings.Settings.Default.PythondllPath;
                        PythonEngine.Initialize();
                        PythonEngine.Exec(code);
                        PythonEngine.Shutdown();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        PythonEngine.Shutdown();
                    }
                }
            });
        }
    }
}
