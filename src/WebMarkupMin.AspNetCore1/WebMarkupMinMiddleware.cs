﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

using WebMarkupMin.AspNet.Common;
using WebMarkupMin.AspNet.Common.Compressors;
using WebMarkupMin.Core;
using WebMarkupMin.Core.Utilities;
using AspNetCommonStrings = WebMarkupMin.AspNet.Common.Resources.Strings;

namespace WebMarkupMin.AspNetCore1
{
	/// <summary>
	/// WebMarkupMin middleware
	/// </summary>
	public class WebMarkupMinMiddleware
	{
		/// <summary>
		/// The next middleware in the pipeline
		/// </summary>
		private readonly RequestDelegate _next;

		/// <summary>
		/// WebMarkupMin configuration
		/// </summary>
		private readonly WebMarkupMinOptions _options;

		/// <summary>
		/// List of markup minification manager
		/// </summary>
		private readonly IList<IMarkupMinificationManager> _minificationManagers;

		/// <summary>
		/// HTTP compression manager
		/// </summary>
		private readonly IHttpCompressionManager _compressionManager;


		/// <summary>
		/// Constructs a instance of WebMarkupMin middleware
		/// </summary>
		/// <param name="next">The next middleware in the pipeline</param>
		/// <param name="options">WebMarkupMin options</param>
		/// <param name="services">The list of services</param>
		public WebMarkupMinMiddleware(RequestDelegate next,
			IOptions<WebMarkupMinOptions> options,
			IServiceProvider services)
		{
			if (next == null)
			{
				throw new ArgumentNullException("next");
			}

			if (options == null)
			{
				throw new ArgumentNullException("options");
			}

			if (services == null)
			{
				throw new ArgumentNullException("services");
			}

			_next = next;
			_options = options.Value;

			var minificationManagers = new List<IMarkupMinificationManager>();

			var htmlMinificationManager = services.GetService<IHtmlMinificationManager>();
			if (htmlMinificationManager != null)
			{
				minificationManagers.Add(htmlMinificationManager);
			}

			var xhtmlMinificationManager = services.GetService<IXhtmlMinificationManager>();
			if (xhtmlMinificationManager != null)
			{
				minificationManagers.Add(xhtmlMinificationManager);
			}

			var xmlMinificationManager = services.GetService<IXmlMinificationManager>();
			if (xmlMinificationManager != null)
			{
				minificationManagers.Add(xmlMinificationManager);
			}

			_minificationManagers = minificationManagers;

			var compressionManager = services.GetService<IHttpCompressionManager>();
			if (compressionManager != null)
			{
				_compressionManager = compressionManager;
			}
		}


		public async Task Invoke(HttpContext context)
		{
			bool useMinification = _options.IsMinificationEnabled() && _minificationManagers.Count > 0;
			bool useCompression = _options.IsCompressionEnabled() && _compressionManager != null;

			if (!useMinification && !useCompression)
			{
				await _next.Invoke(context);
				return;
			}

			HttpRequest request = context.Request;
			HttpResponse response = context.Response;

			using (var cachedStream = new MemoryStream())
			{
				Stream originalStream = response.Body;
				response.Body = cachedStream;

				try
				{
					await _next.Invoke(context);
				}
				catch (Exception)
				{
					response.Body = originalStream;
					cachedStream.Clear();

					throw;
				}

				byte[] cachedBytes = cachedStream.ToArray();
				int cachedByteCount = cachedBytes.Length;
				bool isProcessed = false;

				response.Body = originalStream;
				cachedStream.Clear();

				if (cachedByteCount == 0)
				{
					return;
				}

				if (response.StatusCode == 200)
				{
					string httpMethod = request.Method;
					string contentType = response.ContentType;
					string mediaType = null;
					Encoding encoding = null;

					if (contentType != null)
					{
						MediaTypeHeaderValue mediaTypeHeader;

						if (MediaTypeHeaderValue.TryParse(contentType, out mediaTypeHeader))
						{
							mediaType = mediaTypeHeader.MediaType.ToLowerInvariant();
							encoding = mediaTypeHeader.Encoding;
						}
					}

					encoding = encoding ?? Encoding.GetEncoding(0);

					string currentUrl = request.Path.Value;
					QueryString queryString = request.QueryString;
					if (queryString.HasValue)
					{
						currentUrl += queryString.Value;
					}

					string content = encoding.GetString(cachedBytes);
					string processedContent = content;
					IHeaderDictionary responseHeaders = response.Headers;
					bool isEncodedContent = responseHeaders.IsEncodedContent();
					Action<string, string> appendHttpHeader = (key, value) =>
					{
						responseHeaders.Append(key, new StringValues(value));
					};

					if (useMinification && _options.IsAllowableResponseSize(cachedByteCount))
					{
						foreach (IMarkupMinificationManager minificationManager in _minificationManagers)
						{
							if (minificationManager.IsSupportedHttpMethod(httpMethod)
								&& mediaType != null && minificationManager.IsSupportedMediaType(mediaType)
								&& minificationManager.IsProcessablePage(currentUrl))
							{
								if (isEncodedContent)
								{
									throw new InvalidOperationException(
										string.Format(
											AspNetCommonStrings.MarkupMinificationIsNotApplicableToEncodedContent,
											responseHeaders["Content-Encoding"]
										)
									);
								}

								IMarkupMinifier minifier = minificationManager.CreateMinifier();

								MarkupMinificationResult minificationResult = minifier.Minify(processedContent,
									currentUrl, encoding, minificationManager.GenerateStatistics);
								if (minificationResult.Errors.Count == 0)
								{
									processedContent = minificationResult.MinifiedContent;
									if (_options.IsPoweredByHttpHeadersEnabled())
									{
										minificationManager.AppendPoweredByHttpHeader(appendHttpHeader);
									}

									isProcessed = true;
								}
							}

							if (isProcessed)
							{
								break;
							}
						}
					}

					if (useCompression && !isEncodedContent
						&& _compressionManager.IsSupportedHttpMethod(httpMethod)
						&& _compressionManager.IsSupportedMediaType(mediaType))
					{
						byte[] processedBytes = encoding.GetBytes(processedContent);

						using (var inputStream = new MemoryStream(processedBytes))
						using (var outputStream = new MemoryStream())
						{
							string acceptEncoding = request.Headers["Accept-Encoding"];
							ICompressor compressor = _compressionManager.CreateCompressor(acceptEncoding);

							using (Stream compressedStream = compressor.Compress(outputStream))
							{
								await inputStream.CopyToAsync(compressedStream);
							}

							byte[] compressedBytes = outputStream.ToArray();
							int compressedByteCount = compressedBytes.Length;

							outputStream.Clear();
							inputStream.Clear();

							responseHeaders["Content-Length"] = compressedByteCount.ToString();
							compressor.AppendHttpHeaders(appendHttpHeader);
							await originalStream.WriteAsync(compressedBytes, 0, compressedByteCount);
						}

						isProcessed = true;
					}
					else
					{
						if (isProcessed)
						{
							byte[] processedBytes = encoding.GetBytes(processedContent);
							int processedByteCount = processedBytes.Length;

							responseHeaders["Content-Length"] = processedByteCount.ToString();
							await originalStream.WriteAsync(processedBytes, 0, processedByteCount);
						}
					}
				}

				if (!isProcessed)
				{
					await originalStream.WriteAsync(cachedBytes, 0, cachedByteCount);
				}
			}
		}
	}
}