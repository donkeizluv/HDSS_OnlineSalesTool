using HtmlAgilityPack;
using Microsoft.Extensions.Options;

namespace OnlineSalesCore.Service
{
    public interface IHtmlComposer
    {
        void AppendText(string tag, string text);
    }
}