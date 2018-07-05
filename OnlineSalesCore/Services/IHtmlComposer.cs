using HtmlAgilityPack;
using Microsoft.Extensions.Options;

namespace OnlineSalesCore.Services
{
    public interface IHtmlComposer
    {
        void AppendText(string tag, string text);
        void Reset();
    }
}