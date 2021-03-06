using OnlineSalesCore.Options;
using HtmlAgilityPack;
using Microsoft.Extensions.Options;

namespace OnlineSalesCore.Services
{
    public class HtmlComposer : IHtmlComposer
    {
        private HtmlDocument _doc;
        //Keeps track of important nodes
        private HtmlNode _body;
        private HtmlNode _head;
        private HtmlNode _html;
        public HtmlComposer()
        {
            Reset();
        }
        public void AppendText(string tag, string text)
        {
            var textNode = _doc.CreateTextNode(text);
            var p = _doc.CreateElement(tag);
            p.AppendChild(textNode);
            _body.AppendChild(p);
        }
        private void Init()
        {
            _html = _doc.CreateElement("html");
            _body = _doc.CreateElement("body");
            _head = _doc.CreateElement("head");
            _html.AppendChild(_head);
            _html.AppendChild(_body);
            _doc.DocumentNode.AppendChild(_html);

        }
        public override string ToString()
        {
            var html = _doc.DocumentNode.WriteTo();
            Reset();
            return html;
        }

        public void Reset()
        {
             _doc = new HtmlAgilityPack.HtmlDocument();
            Init();
        }
    }
}