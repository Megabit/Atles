﻿using Atles.Infrastructure.Commands;
using Docs.Attributes;

namespace Atles.Domain.Models.Sites.Commands
{
    /// <summary>
    /// Request to update the details of a site.
    /// </summary>
    [DocRequest(typeof(Site))]
    public class UpdateSite : CommandBase
    {
        /// <summary>
        /// The new tile of the site.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The new theme for the public site.
        /// </summary>
        public string Theme { get; set; }

        /// <summary>
        /// The new Cascading Style Sheet for the public site.
        /// </summary>
        public string Css { get; set; }

        /// <summary>
        /// The new language ISO code of the site.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// The new content of the privacy page.
        /// </summary>
        public string Privacy { get; set; }

        /// <summary>
        /// The new content of the terms page.
        /// </summary>
        public string Terms { get; set; }

        /// <summary>
        /// The script for the head section of the HTML document.
        /// </summary>
        public string HeadScript { get; set; }
    }
}
