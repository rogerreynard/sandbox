using Sitecore.Mvc.Pipelines;
using Sitecore.Mvc.Presentation;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

namespace Foundation.PageExtender.ResponseFilter
{
    internal class PageExtenderResponseFilter : Stream
    {
        private readonly MemoryStream _internalStream;

        private string _extendersHtml;

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => true;

        public virtual Encoding Encoding
        {
            get
            {
                var currentOrNull = PageContext.CurrentOrNull;
                return currentOrNull == null ? Encoding.UTF8 : currentOrNull.RequestContext.HttpContext.Response.ContentEncoding;
            }
        }

        public virtual string ExtendersHtml
        {
            get
            {
                var html = _extendersHtml;
                if (string.IsNullOrEmpty(html))
                {
                    html = _extendersHtml = GetExtendersHtml();
                }
                return html;
            }
            set
            {
                Sitecore.Diagnostics.Assert.IsNotNull(value, "value");
                _extendersHtml = value;
            }
        }

        public override long Length => 0L;

        public override long Position { get; set; }

        public virtual Stream ResponseStream { get; }

        public PageExtenderResponseFilter(Stream stream)
        {
            Sitecore.Diagnostics.Assert.ArgumentNotNull(stream, "stream");
            ResponseStream = stream;
            _internalStream = new MemoryStream();
        }

        public override void Close()
        {
            ResponseStream.Close();
            _internalStream.Close();
        }

        public override void Flush()
        {
            var array = _internalStream.ToArray();
            var current = HttpContext.Current;
            if (current != null)
            {
                var lastError = current.Server.GetLastError();
                if (lastError != null)
                {
                    Sitecore.Diagnostics.Log.SingleWarn("Page extenders were not added, because an error occurred during the request execution", this);
                    TransmitData(array);
                    return;
                }
            }
            if (string.IsNullOrEmpty(ExtendersHtml))
            {
                TransmitData(array);
                return;
            }
            var text = Encoding.GetString(array);
            text = AddExtendersHtml(text);
            var bytes = Encoding.GetBytes(text);
            TransmitData(bytes);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            Sitecore.Diagnostics.Assert.ArgumentNotNull(buffer, "buffer");
            return ResponseStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return ResponseStream.Seek(offset, origin);
        }

        public override void SetLength(long length)
        {
            ResponseStream.SetLength(length);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Sitecore.Diagnostics.Assert.ArgumentNotNull(buffer, "buffer");
            _internalStream.Write(buffer, offset, count);
        }

        protected virtual string AddExtendersHtml(string html)
        {
            Sitecore.Diagnostics.Assert.ArgumentNotNull(html, "html");
            //var regex = new Regex("(<BODY\\b[^>]*?>)", RegexOptions.IgnoreCase);
           // html = regex.Replace(html, "$1" + ExtendersHtml, 1);
            var regex = new Regex("(</BODY>)", RegexOptions.IgnoreCase);
            html = regex.Replace(html, ExtendersHtml + "\n$1", 1);
            return html;
        }

        protected virtual string GetExtendersHtml()
        {
            var htmlTextWriter = new HtmlTextWriter(new StringWriter());
            var renderPageExtendersArgs = new RenderPageExtendersArgs(htmlTextWriter);
            PipelineService.Get().RunPipeline<RenderPageExtendersArgs>("PageExtender.RenderPageExtenders", renderPageExtendersArgs);
            return !renderPageExtendersArgs.IsRendered ? string.Empty : htmlTextWriter.InnerWriter.ToString();
        }

        protected virtual void TransmitData(byte[] data)
        {
            Sitecore.Diagnostics.Assert.ArgumentNotNull(data, "data");
            ResponseStream.Write(data, 0, data.Length);
            ResponseStream.Flush();
            _internalStream.SetLength(0L);
            Position = 0L;
        }
    }
}