﻿using Microsoft.Extensions.Options;

using WebMarkupMin.AspNet.Common;
using WebMarkupMin.Core.Loggers;

namespace WebMarkupMin.AspNetCore1
{
	/// <summary>
	/// XHTML minification manager
	/// </summary>
	public sealed class XhtmlMinificationManager : XhtmlMinificationManagerBase
	{
		/// <summary>
		/// Constructs a instance of XHTML minification manager
		/// </summary>
		/// <param name="logger">Logger</param>
		/// <param name="options">XHTML minification options</param>
		public XhtmlMinificationManager(ILogger logger, IOptions<XhtmlMinificationOptions> options)
		{
			Logger = logger;

			XhtmlMinificationOptions minificationOptions = options.Value;
			MinificationSettings = minificationOptions.MinificationSettings;
			SupportedHttpMethods = minificationOptions.SupportedHttpMethods;
			SupportedMediaTypes = minificationOptions.SupportedMediaTypes;
			IncludedPages = minificationOptions.IncludedPages;
			ExcludedPages = minificationOptions.ExcludedPages;
			GenerateStatistics = minificationOptions.GenerateStatistics;
			JsMinifierFactory = minificationOptions.JsMinifierFactory;
			CssMinifierFactory = minificationOptions.CssMinifierFactory;
		}
	}
}