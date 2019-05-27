using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;

namespace RequireJsCore
{
    public class JavascriptBuilder
    {
        private const string Type = "application/javascript";

        private readonly TagBuilder scriptTag = new TagBuilder("script");

        private readonly StringBuilder content = new StringBuilder();

        private bool hasNewLine = false;

        public bool TagHasType { get; set; }

        public string Render()
        {
            scriptTag.InnerHtml.AppendHtml(content.ToString());

            var script = string.Empty;

            using (var writer = new StringWriter())
            {
                scriptTag.WriteTo(writer, HtmlEncoder.Default);
                
                script = writer.ToString();
            }

            //return scriptTag.ToString();
            return script;
        }

        public string RenderStatement()
        {
            if (TagHasType) {
                scriptTag.MergeAttribute("type", Type);
            }

            scriptTag.InnerHtml.Append(string.Empty);

            return scriptTag.ToString();
        }

        public void AddAttributesToStatement(string key, string value)
        {
            scriptTag.MergeAttribute(key, value);
        }

        public void AddStatement(string statement)
        {
            if (!hasNewLine)
            {
                content.AppendLine();
                hasNewLine = true;
            }

            content.AppendLine(statement);
        }
    }
}
