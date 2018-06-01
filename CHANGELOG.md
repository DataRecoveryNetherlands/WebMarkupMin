Change log
==========

## v2.4.3 - June 1, 2018
 * In WebMarkupMin.NUglify added support of the NUglify version 1.5.11

## v2.4.2 - August 16, 2017
 * Created the WebMarkupMin.AspNetCore2 module, that contains middleware for ASP.NET Core 2.0
 * In WebMarkupMin.NUglify added support of the NUglify version 1.5.8

## v2.4.1 - June 30, 2017
 * In WebMarkupMin.NUglify added support of the NUglify version 1.5.6

## v2.4.0 - May 7, 2017
 * Added support of .NET Core 1.0.4
 * HTML and XHTML minifiers now support processing of CDATA sections outside the `script` and `style` tags
 * In `IMarkupMinificationManager` interface, `MarkupMinificationOptionsBase` class, `IHttpCompressionManager` interface and `HttpCompressionOptions` class was added a new property - `SupportedHttpMethods` (default `GET`)
 * In WebMarkupMin.AspNet4.Mvc and WebMarkupMin.AspNet4.WebForms now, by default, only the `GET` requests are minified and compressed (this behavior can be changed by using the `SupportedHttpMethods` property)
 * In `IHttpCompressionManager` interface and `HttpCompressionOptions` class was added two new properties - `IncludedPages` (default empty list) and `ExcludedPages` (default empty list)
 * In ASP.NET 4.X Extensions fixed a error of filtering media-types, which led to incorrect usage of HTTP compression

## v2.3.0 - March 7, 2017
 * Downgraded .NET Framework version from 4.5.1 to 4.5
 * Added support of .NET Core 1.0.3
 * Fixed a [error #31](https://github.com/Taritsyn/WebMarkupMin/issues/31) “Perfomance is very slow when a HTML comment is inside a JavaScript block”
 * Fixed a error in `SourceCodeNavigator` class
 * In WebMarkupMin.NUglify added support of the NUglify version 1.5.5
 * In `IMarkupMinificationManager` interface and `MarkupMinificationOptionsBase` class was added a new property - `GenerateStatistics` (default `false`)
 * From `IHttpCompressionManager` interface was removed `IsSupportedMediaType` method
 * In `IHttpCompressionManager` interface and `HttpCompressionOptions` class was added a new property - `SupportedMediaTypePredicate` (default `null`)

## v2.2.5 - December 22, 2016
 * In WebMarkupMin.AspNetCore1 fixed a error due to which instead of the status code pages displayed an empty content

## v2.2.4 - November 26, 2016
 * Added the ability to ignore fragments of markup by using the ignoring comment tags (`<!--wmm:ignore--><!--/wmm:ignore-->`)

## v2.2.3 - November 23, 2016
 * All exceptions made serializable
 * Fixed a [error #21](https://github.com/Taritsyn/WebMarkupMin/issues/21) “Remove redundant attributes, except input”

## v2.2.2 - November 11, 2016
 * Fixed a [error #18](https://github.com/Taritsyn/WebMarkupMin/issues/18) “Why is SavedGzipInBytes a decimal?”
 * Fixed a [error #20](https://github.com/Taritsyn/WebMarkupMin/issues/20) “Adding WebMarkupMin with a ServiceStack .Net Core enabled project fails”
 * Added the ability to specify a level of GZip or Deflate compression (while available only in ASP.NET Core applications)

## v2.2.1 - September 30, 2016
 * In WebMarkupMin.AspNetCore1 fixed a error “Stream does not support writing”

## v2.2.0 - September 27, 2016
 * Downgraded .NET Framework version from 4.5.2 to 4.5.1
 * Added support of .NET Core 1.0.1
 * Fixed a [error #13](https://github.com/Taritsyn/WebMarkupMin/issues/13) “HttpCompression Not Checking for Already Compressed Content”
 * Fixed a [error #14](https://github.com/Taritsyn/WebMarkupMin/issues/14) “HttpCompression Algorithm Priority”
 * Fixed a [error #15](https://github.com/Taritsyn/WebMarkupMin/issues/15) “Check for Content-Length in response headers”

## v2.1.1 - September 7, 2016
 * Fixed a [error #12](https://github.com/Taritsyn/WebMarkupMin/issues/12) “HTTP modules cause forms button do post back error”

## v2.1.0 - July 19, 2016
 * In configuration settings of HTML/XHTML minifier was changed type of `ProcessableScriptTypeCollection` and `CustomAngularDirectiveCollection` properties from `IEnumerable<string>` to `ISet<string>`
 * In configuration settings of HTML minifier was changed type of `PreservableOptionalTagCollection` property from `IEnumerable<string>` to `ISet<string>`
 * In configuration settings of HTML/XHTML minifier was changed a default value of `ProcessableScriptTypeList` property from `""` to `"text/html"`
 * In `CrockfordJsMinifier` was optimized memory usage
 * In ASP.NET 4.X Extensions was changed a mechanism of using default instances of loggers, factories and managers

## v2.0.2 - July 12, 2016
 * Added module based on the [NUglify](http://github.com/xoofx/NUglify)
 * In WebMarkupMin.MsAjax and WebMarkupMin.Yui was made refactoring

## v2.0.1 - July 9, 2016
 * Optimized memory usage
 * Fixed a [error #10](https://github.com/Taritsyn/WebMarkupMin/issues/10) “Crash parsing invalid comment block”

## v2.0.0 - June 28, 2016
 * Added support of .NET Core and ASP.NET Core 1.0 RTM
 * Was made refactoring

## v2.0.0 RC 9 - June 13, 2016
 * `rb` and `rtc` tags are now considered as optional end tags
 * In configuration settings of HTML minifier was added one new property - `PreservableOptionalTagList` (default is empty)
 * Fixed a [error #8](https://github.com/Taritsyn/WebMarkupMin/issues/8) “MarkupMinificationException when having nested SVG element inside an SVG”
 * Fixed a [error #9](https://github.com/Taritsyn/WebMarkupMin/issues/9) “&lt;div&gt;${{something}}&lt;/div&gt; incorrectly minified”

## v2.0.0 RC 8 - May 19, 2016
 * Added support of .NET Core and ASP.NET Core 1.0 RC 2
 * WebMarkupMin.AspNet5 package has been replaced by WebMarkupMin.AspNetCore1 package

## v2.0.0 RC 7 - May 10, 2016
 * Improved a safe whitespace minification mode

## v2.0.0 RC 6 - April 2, 2016
 * In configuration settings of HTML minifier was added one new property - `PreserveCase` (default `false`)
 * Fixed a errors that occurred when processing of Angular 2, Aurelia and Polymer templates

## v2.0.0 RC 5 - March 24, 2016
 * Fixed a [error #6](https://github.com/Taritsyn/WebMarkupMin/issues/6) “Incompatible with DeveloperExceptionPageMiddleware (ASP.NET 5)”

## v2.0.0 RC 4 - March 17, 2016
 * Fixed a [error #3](https://github.com/Taritsyn/WebMarkupMin/issues/3) “NullReferenceException with FileContentResult in ASP.vNext RC1”

## v2.0.0 RC 3 - February 23, 2016
 * Now in WebMarkupMin.AspNet4.Mvc and WebMarkupMin.AspNet4.WebForms the responses with status codes are not equal to 200 is not minified and compressed

## v2.0.0 RC 2 - December 5, 2015
 * Now during minification removes the byte order mark (BOM)

## v2.0.0 RC 1 - November 20, 2015
 * Added support of .NET Core and ASP.NET 5 RC 1

## v2.0.0 Beta 5 - October 17, 2015
 * Added support of ASP.NET 5 Beta 8

## v2.0.0 Beta 4 - September 4, 2015
 * Added support of ASP.NET 5 Beta 7

## v2.0.0 Beta 3 - September 1, 2015
 * In WebMarkupMin.AspNet4.Mvc fixed a error “Filtering is not allowed.”, that caused by joint usage of the WebMarkupMin's action filters and the nopCommerce's widgets

## v2.0.0 Beta 2 - August 24, 2015
 * Was made refactoring
 * In WebMarkupMin.AspNet4.Mvc now the `CompressContentAttribute` action filter can be applied to the controllers

## v2.0.0 Beta 1 - August 13, 2015
 * .NET Core Libraries (CoreFX) have been upgraded to stable versions
 * Added support of xUnit.net 2.1 Beta 4
 * Fixed a problems with the NuGet package restore

## v2.0.0 Alpha 2 - July 31, 2015
 * Added support of ASP.NET 5 Beta 6
 * Now during HTTP compression the deflate algorithm has a higher priority than the gzip algorithm

## v2.0.0 Alpha 1 - July 17, 2015
 * Removed dependency on `System.Configuration.dll` (no longer supported configuration by using the `Web.config` and `App.config` files)
 * In WebMarkupMin.Core package added support of DNX 4.5.1 and DNX Core 5.0
 * In WebMarkupMin.MsAjax and WebMarkupMin.Yui packages added support of DNX 4.5.1
 * WebMarkupMin.Web package was split into 2 packages: WebMarkupMin.AspNet4.Common and WebMarkupMin.AspNet4.HttpModules
 * WebMarkupMin.Mvc package has been replaced by WebMarkupMin.AspNet4.Mvc package
 * WebMarkupMin.WebForms package has been replaced by WebMarkupMin.AspNet4.WebForms package
 * Created WebMarkupMin.AspNet5 package, that contains middleware for ASP.NET 5
 * WebMarkupMin.ConfigurationIntelliSense package is no longer required for the current version of WebMarkupMin