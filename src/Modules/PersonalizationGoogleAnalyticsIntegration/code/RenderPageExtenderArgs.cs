﻿using System.IO;
using Sitecore.Mvc.Pipelines;

namespace Sitecore.PersonalizationGoogleAnalyticsIntegration
{
    public class RenderPageExtendersArgs : MvcPipelineArgs
    {
        public bool IsRendered { get; set; }

        public TextWriter Writer { get; }

        public RenderPageExtendersArgs(TextWriter writer)
        {
            Sitecore.Diagnostics.Assert.ArgumentNotNull(writer, "writer");
            Writer = writer;
        }
    }
}