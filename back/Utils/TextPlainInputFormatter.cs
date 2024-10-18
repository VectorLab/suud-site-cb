using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace suud_site_cb.Utils;

public class TextPlainInputFormatter : InputFormatter
{
    public TextPlainInputFormatter()
    {
        SupportedMediaTypes.Add("text/plain");
    }

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context){
        try{
            var request = context.HttpContext.Request;
            Encoding? encoding = null;
            if (null != request.ContentType){
                encoding = GetEncoding(request.ContentType);
            }

            string? content;
            if (null == encoding){
                using var reader = new StreamReader(request.Body, detectEncodingFromByteOrderMarks: true);
                content = await reader.ReadToEndAsync();
            }else{
                using var reader = new StreamReader(request.Body, encoding);
                content = await reader.ReadToEndAsync();
            }

            if (null != content){
                return await InputFormatterResult.SuccessAsync(content);
            }
        }catch (Exception){}
        return await InputFormatterResult.FailureAsync();
    }

    private static Encoding? GetEncoding(string contentType)
    {
        if (string.IsNullOrEmpty(contentType))
        {
            return null;
        }

        var mediaType = MediaTypeHeaderValue.Parse(contentType);
        var charset = mediaType.CharSet;
        if (string.IsNullOrEmpty(charset))
        {
            return null;
        }
        return Encoding.GetEncoding(charset);
    }

    protected override bool CanReadType(Type type)
    {
        return type == typeof(string);
    }
}
