namespace WPF_HTTP_SERVER.HTTP
{
    /// <summary>
    /// 
    /// </summary>
    public enum TextTypes
    {
        html,
        plain,
        css,
        javascript
    }

    public enum ApplicationTypes
    {
        json,
        xml,
        x_www_form_urlencoded,
        pdf,
        javascript,
        octet_stream
    }

    public enum ImageTypes
    {
        jpeg,
        png,
        gif,
        webp
    }

    public enum AudioTypes
    {
        mpeg,
        ogg,
        mp4,
        webm
    }

    public static class ContentTypes
    {
        public static Dictionary<TextTypes, string> TextTypes = new()
        {
            { HTTP.TextTypes.html,       "text/html" },
            { HTTP.TextTypes.plain,      "text/plain" },
            { HTTP.TextTypes.css,        "text/css" },
            { HTTP.TextTypes.javascript, "text/javascript" }
        };

        public static Dictionary<ApplicationTypes, string> ApplicationTypes = new()
        {
            { HTTP.ApplicationTypes.json,                    "application/json" },
            { HTTP.ApplicationTypes.xml,                     "application/xml" },
            { HTTP.ApplicationTypes.x_www_form_urlencoded,   "application/x-www-form-urlencoded" },
            { HTTP.ApplicationTypes.pdf,                     "application/pdf" },
            { HTTP.ApplicationTypes.javascript,              "application/javascript" },
            { HTTP.ApplicationTypes.octet_stream,            "application/octet-stream" }
        };

        public static Dictionary<ImageTypes, string> ImageTypes = new()
        {
            { HTTP.ImageTypes.jpeg, "image/jpeg" },
            { HTTP.ImageTypes.png,  "image/png" },
            { HTTP.ImageTypes.gif,  "image/gif" },
            { HTTP.ImageTypes.webp, "image/webp" }
        };

        public static Dictionary<AudioTypes, string> AudioTypes = new()
        {
            { HTTP.AudioTypes.mpeg, "audio/mpeg" },
            { HTTP.AudioTypes.ogg,  "audio/ogg" },
            { HTTP.AudioTypes.mp4,  "audio/mp4" },
            { HTTP.AudioTypes.webm, "audio/webm" }
        };
    }
}
