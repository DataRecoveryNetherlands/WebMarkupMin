﻿using System.Collections.Generic;
using System.IO;
using System.Text;

using Xunit;

namespace WebMarkupMin.Core.Test.Xml
{
	public class MinificationTests : FileSystemTestsBase
	{
		private readonly string _xmlFilesDirectoryPath;


		public MinificationTests()
		{
			_xmlFilesDirectoryPath = Path.GetFullPath(Path.Combine(_baseDirectoryPath, @"../SharedFiles/xml/"));
		}


		#region Removing BOM

		[Fact]
		public void RemovingBomAtStartIsCorrect()
		{
			// Arrange
			var minifier = new XmlMinifier(new XmlMinificationSettings(true));

			string inputFilePath = Path.Combine(_xmlFilesDirectoryPath, "xml-document-with-bom-at-start.xml");
			byte[] inputBytes = File.ReadAllBytes(inputFilePath);
			string inputContent = Encoding.UTF8.GetString(inputBytes);

			string targetOutputFilePath = Path.Combine(_xmlFilesDirectoryPath, "xml-document-without-bom.xml");
			byte[] targetOutputBytes = File.ReadAllBytes(targetOutputFilePath);

			// Act
			string outputContent = minifier.Minify(inputContent).MinifiedContent;
			byte[] outputBytes = Encoding.UTF8.GetBytes(outputContent);

			// Assert
			Assert.Equal(targetOutputBytes, outputBytes);
		}

		[Fact]
		public void RemovingBomFromBodyTagIsCorrect()
		{
			// Arrange
			var minifier = new XmlMinifier(new XmlMinificationSettings(true));

			string inputFilePath = Path.Combine(_xmlFilesDirectoryPath, "xml-document-with-bom-in-tag.xml");
			byte[] inputBytes = File.ReadAllBytes(inputFilePath);
			string inputContent = Encoding.UTF8.GetString(inputBytes);

			string targetOutputFilePath = Path.Combine(_xmlFilesDirectoryPath, "xml-document-without-bom.xml");
			byte[] targetOutputBytes = File.ReadAllBytes(targetOutputFilePath);

			// Act
			string outputContent = minifier.Minify(inputContent).MinifiedContent;
			byte[] outputBytes = Encoding.UTF8.GetBytes(outputContent);

			// Assert
			Assert.Equal(targetOutputBytes, outputBytes);
		}

		#endregion

		#region Ignoring fragments of markup

		[Fact]
		public void IgnoringFragmentsOfMarkupIsCorrect()
		{
			// Arrange
			var minifier = new XmlMinifier(new XmlMinificationSettings(true)
			{
				MinifyWhitespace = true
			});

			const string input1 = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n"+
				"<menu>\n" +
				"	<dish>\n" +
				"		<name>Belgian Waffles</name>\n" +
				"		<price>$6.20</price>\n" +
				"		<description>Two of our famous Belgian Waffles with plenty of real maple syrup</description>\n" +
				"	</dish>\n" +
				"<!--wmm:ignore-->\n" +
				"	<dish>\n" +
				"		<name>Berry-Berry Belgian Waffles</name>\n" +
				"		<price>$9.20</price>\n" +
				"		<description>Light Belgian waffles covered with an assortment of fresh berries and whipped cream</description>\n" +
				"	</dish>\n" +
				"	<dish>\n" +
				"		<name>French Toast</name>\n" +
				"		<price>$5.50</price>\n" +
				"		<description>Thick slices made from our homemade sourdough bread</description>\n" +
				"	</dish>\n" +
				"<!--/wmm:ignore-->\n" +
				"	<dish>\n" +
				"		<name>Homestyle Breakfast</name>\n" +
				"		<price>$7.20</price>\n" +
				"		<description>Two eggs, bacon or sausage, toast, and our ever-popular hash browns</description>\n" +
				"	</dish>\n" +
				"<!--wmm:ignore-->\n" +
				"	<dish>\n" +
				"		<name>Strawberry Belgian Waffles</name>\n" +
				"		<price>$8.20</price>\n" +
				"		<description>Light Belgian waffles covered with strawberries and whipped cream</description>\n" +
				"	</dish>\n" +
				"<!--/wmm:ignore-->\n" +
				"</menu>"
				;
			const string targetOutput1 = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
				"<menu>" +
				"<dish>" +
				"<name>Belgian Waffles</name>" +
				"<price>$6.20</price>" +
				"<description>Two of our famous Belgian Waffles with plenty of real maple syrup</description>" +
				"</dish>\n" +
				"	<dish>\n" +
				"		<name>Berry-Berry Belgian Waffles</name>\n" +
				"		<price>$9.20</price>\n" +
				"		<description>Light Belgian waffles covered with an assortment of fresh berries and whipped cream</description>\n" +
				"	</dish>\n" +
				"	<dish>\n" +
				"		<name>French Toast</name>\n" +
				"		<price>$5.50</price>\n" +
				"		<description>Thick slices made from our homemade sourdough bread</description>\n" +
				"	</dish>\n" +
				"<dish>" +
				"<name>Homestyle Breakfast</name>" +
				"<price>$7.20</price>" +
				"<description>Two eggs, bacon or sausage, toast, and our ever-popular hash browns</description>" +
				"</dish>\n" +
				"	<dish>\n" +
				"		<name>Strawberry Belgian Waffles</name>\n" +
				"		<price>$8.20</price>\n" +
				"		<description>Light Belgian waffles covered with strawberries and whipped cream</description>\n" +
				"	</dish>\n\n" +
				"</menu>"
				;

			const string input2 = "<!--wmm:ignore--><!--/wmm:ignore-->";
			const string targetOutput2 = "";

			const string input3 = "<!--wmm:ignore-->";

			const string input4 = "<row RoleId=\"4\" RoleName=\"Administrator\"/>\n" +
				"<!--wmm:ignore--><row RoleId=\"5\" RoleName=\"Contributor\"/>\n" +
				"<row RoleId=\"6\" RoleName=\"Editor\"/>"
				;

			const string input5 = "<!--/wmm:ignore-->";

			const string input6 = "<row RoleId=\"4\" RoleName=\"Administrator\"/>\n" +
				"<row RoleId=\"5\" RoleName=\"Contributor\"/><!--/wmm:ignore-->\n" +
				"<row RoleId=\"6\" RoleName=\"Editor\"/>"
				;

			// Act
			string output1 = minifier.Minify(input1).MinifiedContent;
			string output2 = minifier.Minify(input2).MinifiedContent;
			IList<MinificationErrorInfo> errors3 = minifier.Minify(input3).Errors;
			IList<MinificationErrorInfo> errors4 = minifier.Minify(input4).Errors;
			IList<MinificationErrorInfo> errors5 = minifier.Minify(input5).Errors;
			IList<MinificationErrorInfo> errors6 = minifier.Minify(input6).Errors;

			// Assert
			Assert.Equal(targetOutput1, output1);
			Assert.Equal(targetOutput2, output2);

			Assert.Equal(1, errors3.Count);
			Assert.Equal(1, errors3[0].LineNumber);
			Assert.Equal(1, errors3[0].ColumnNumber);

			Assert.Equal(1, errors4.Count);
			Assert.Equal(2, errors4[0].LineNumber);
			Assert.Equal(1, errors4[0].ColumnNumber);

			Assert.Equal(1, errors5.Count);
			Assert.Equal(1, errors5[0].LineNumber);
			Assert.Equal(1, errors5[0].ColumnNumber);

			Assert.Equal(1, errors6.Count);
			Assert.Equal(2, errors6[0].LineNumber);
			Assert.Equal(41, errors6[0].ColumnNumber);
		}

		#endregion

		#region Processing DOCTYPE declaration

		[Fact]
		public void ProcessingDoctypeIsCorrect()
		{
			// Arrange
			var minifier = new XmlMinifier(new XmlMinificationSettings(true));

			const string input1 = "<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.0//EN\"\n" +
				"	\"http://www.w3.org/TR/2001/REC-SVG-20010904/DTD/svg10.dtd\">";
			const string targetOutput1 = "<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.0//EN\"" +
				" \"http://www.w3.org/TR/2001/REC-SVG-20010904/DTD/svg10.dtd\">";

			const string input2 = "<!DOCTYPE\n" +
				"    svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\"\n" +
				"    \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">"
				;
			const string targetOutput2 = "<!DOCTYPE" +
				" svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\"" +
				" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">"
				;

			const string input3 = "<!DOCTYPE math SYSTEM \r\n" +
				"	\"http://www.w3.org/Math/DTD/mathml1/mathml.dtd\">";
			const string targetOutput3 = "<!DOCTYPE math SYSTEM" +
				" \"http://www.w3.org/Math/DTD/mathml1/mathml.dtd\">";

			const string input4 = "<!DOCTYPE math PUBLIC \"-//W3C//DTD MathML 2.0//EN\"	\n" +
				"    \"http://www.w3.org/Math/DTD/mathml2/mathml2.dtd\">";
			const string targetOutput4 = "<!DOCTYPE math PUBLIC \"-//W3C//DTD MathML 2.0//EN\"" +
				" \"http://www.w3.org/Math/DTD/mathml2/mathml2.dtd\">";

			const string input5 = "<!DOCTYPE recipe>";
			const string targetOutput5 = input5;

			const string input6 = "<!DOCTYPE\r\nrecipe>";
			const string targetOutput6 = "<!DOCTYPE recipe>";

			const string input7 = "<!DOCTYPE\trecipe>";
			const string targetOutput7 = "<!DOCTYPE recipe>";

			// Act
			string output1 = minifier.Minify(input1).MinifiedContent;
			string output2 = minifier.Minify(input2).MinifiedContent;
			string output3 = minifier.Minify(input3).MinifiedContent;
			string output4 = minifier.Minify(input4).MinifiedContent;
			string output5 = minifier.Minify(input5).MinifiedContent;
			string output6 = minifier.Minify(input6).MinifiedContent;
			string output7 = minifier.Minify(input7).MinifiedContent;

			// Assert
			Assert.Equal(targetOutput1, output1);
			Assert.Equal(targetOutput2, output2);
			Assert.Equal(targetOutput3, output3);
			Assert.Equal(targetOutput4, output4);
			Assert.Equal(targetOutput5, output5);
			Assert.Equal(targetOutput6, output6);
			Assert.Equal(targetOutput7, output7);
		}

		#endregion

		#region Encoding attribute values

		[Fact]
		public void EncodingAttributeValuesIsCorrect()
		{
			// Arrange
			var minifier = new XmlMinifier(new XmlMinificationSettings(true));

			const string input1 = "<product url=\"/product.asp?id=12&category=5&returnUrl=%2Fdefault.asp\"></product>";
			const string targetOutput1 = "<product url=\"/product.asp?id=12&amp;category=5&amp;returnUrl=%2Fdefault.asp\"></product>";

			const string input2 = "<product url='/product.asp?id=12&category=5&returnUrl=%2Fdefault.asp'></product>";
			const string targetOutput2 = "<product url=\"/product.asp?id=12&amp;category=5&amp;returnUrl=%2Fdefault.asp\"></product>";

			const string input3 = "<article description=\'Знаменитая статья Артемия Лебедева \"Паранойя оптимизатора\"\'/>";
			const string targetOutput3 = "<article description=\"Знаменитая статья Артемия Лебедева &quot;Паранойя оптимизатора&quot;\"/>";

			const string input4 = "<article description=\"Знаменитая статья Артемия Лебедева 'Паранойя оптимизатора'\"/>";
			const string targetOutput4 = input4;

			const string input5 = "<article description=\"Знаменитая статья Артемия Лебедева <Паранойя оптимизатора>\"/>";
			const string targetOutput5 = "<article description=\"Знаменитая статья Артемия Лебедева &lt;Паранойя оптимизатора&gt;\"/>";

			const string input6 = "<minifiers>\n" +
				"	<add displayName='Douglas Crockford&apos;s JS Minifier'/>\n" +
				"	<add displayName='Microsoft Ajax JS Minifier'/>\n" +
				"	<add displayName='YUI JS Minifier'/>\n" +
				"</minifiers>"
				;
			const string targetOutput6 = "<minifiers>\n" +
				"	<add displayName=\"Douglas Crockford's JS Minifier\"/>\n" +
				"	<add displayName=\"Microsoft Ajax JS Minifier\"/>\n" +
				"	<add displayName=\"YUI JS Minifier\"/>\n" +
				"</minifiers>"
				;

			// Act
			string output1 = minifier.Minify(input1).MinifiedContent;
			string output2 = minifier.Minify(input2).MinifiedContent;
			string output3 = minifier.Minify(input3).MinifiedContent;
			string output4 = minifier.Minify(input4).MinifiedContent;
			string output5 = minifier.Minify(input5).MinifiedContent;
			string output6 = minifier.Minify(input6).MinifiedContent;

			// Assert
			Assert.Equal(targetOutput1, output1);
			Assert.Equal(targetOutput2, output2);
			Assert.Equal(targetOutput3, output3);
			Assert.Equal(targetOutput4, output4);
			Assert.Equal(targetOutput5, output5);
			Assert.Equal(targetOutput6, output6);
		}

		#endregion

		#region Whitespace minification

		[Fact]
		public void WhitespaceMinificationIsCorrect()
		{
			// Arrange
			var keepingWhitespaceMinifier = new XmlMinifier(new XmlMinificationSettings(true) { MinifyWhitespace = false });
			var removingWhitespaceMinifier = new XmlMinifier(new XmlMinificationSettings(true) { MinifyWhitespace = true });

			const string input1 = " \n   <?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
				"<?xml-stylesheet type=\"text/xsl\" href=\"http://feeds.example.com/feed-rss.xslt\"?>\n" +
				"<rss version=\"2.0\">\n" +
				"	<channel>\n" +
				"		<!-- Channel properties -->\n" +
				"		<title>    RSS          Title          </title>\n" +
				"		<description>	This    is  an example   of an   RSS feed	</description>\n" +
				"		<link>http://www.example.com/rss</link>\n" +
				"		<lastBuildDate>Mon, 07 Sep 2013 00:01:00 +0000</lastBuildDate>\n" +
				"		<pubDate>Mon, 07 Sep 2012 16:45:00 +0000</pubDate>\n" +
				"		<ttl>1800</ttl>\n" +
				"		<!-- /Channel properties -->\n" +
				"		<!-- Item list -->\n" +
				"		<item>\n" +
				"			<title>		Example    entry  </title>\n" +
				"			<description>\n" +
				"			<![CDATA[\n" +
				"			<p>Here is some text containing an description.</p>\n" +
				"			]]>\n" +
				"			</description>\n" +
				"			<link>http://www.example.com/2012/09/01/my-article</link>\n" +
				"			<guid>97357194-40ea-4a19-9941-bc208521b8ce</guid>\n" +
				"			<pubDate>Mon, 07 Sep 2012 16:45:00 +0000</pubDate>\n" +
				"		</item>\n" +
				"		<!-- /Item list -->\n" +
				"	</channel>\n" +
				"</rss>\t   \n "
				;
			const string targetOutput1A = input1;
			const string targetOutput1B = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
				"<?xml-stylesheet type=\"text/xsl\" href=\"http://feeds.example.com/feed-rss.xslt\"?>" +
				"<rss version=\"2.0\">" +
				"<channel>\n" +
				"		<!-- Channel properties -->\n" +
				"		<title>    RSS          Title          </title>" +
				"<description>	This    is  an example   of an   RSS feed	</description>" +
				"<link>http://www.example.com/rss</link>" +
				"<lastBuildDate>Mon, 07 Sep 2013 00:01:00 +0000</lastBuildDate>" +
				"<pubDate>Mon, 07 Sep 2012 16:45:00 +0000</pubDate>" +
				"<ttl>1800</ttl>\n" +
				"		<!-- /Channel properties -->\n" +
				"		<!-- Item list -->\n" +
				"		<item>" +
				"<title>		Example    entry  </title>" +
				"<description>\n" +
				"			<![CDATA[\n" +
				"			<p>Here is some text containing an description.</p>\n" +
				"			]]>\n" +
				"			</description>" +
				"<link>http://www.example.com/2012/09/01/my-article</link>" +
				"<guid>97357194-40ea-4a19-9941-bc208521b8ce</guid>" +
				"<pubDate>Mon, 07 Sep 2012 16:45:00 +0000</pubDate>" +
				"</item>\n" +
				"		<!-- /Item list -->\n" +
				"	</channel>" +
				"</rss>"
				;


			const string input2 = "  \n\n  <?xml version=\"1.0\"?>\n" +
				"<soap:Envelope xmlns:soap=\"http://www.w3.org/2003/05/soap-envelope\">\n" +
				"	<soap:Header>\n" +
				"	</soap:Header>\n" +
				"	<soap:Body>\n" +
				"		<m:GetStockPrice xmlns:m=\"http://www.example.com/stock\">\n" +
				"			<m:StockName>MSFT</m:StockName>\n" +
				"		</m:GetStockPrice>\n" +
				"	</soap:Body>\n" +
				"</soap:Envelope>	   \n  "
				;
			const string targetOutput2A = input2;
			const string targetOutput2B = "<?xml version=\"1.0\"?>" +
				"<soap:Envelope xmlns:soap=\"http://www.w3.org/2003/05/soap-envelope\">" +
				"<soap:Header>\n" +
				"	</soap:Header>" +
				"<soap:Body>" +
				"<m:GetStockPrice xmlns:m=\"http://www.example.com/stock\">" +
				"<m:StockName>MSFT</m:StockName>" +
				"</m:GetStockPrice>" +
				"</soap:Body>" +
				"</soap:Envelope>"
				;

			const string input3 = "<?xml version=\"1.0\" standalone=\"no\"?>\n" +
				"<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\" " +
				"\"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">\n" +
				"<math xmlns=\"http://www.w3.org/1998/Math/MathML\">\n" +
				"	<mrow>\n" +
				"		<mi>a</mi>\n" +
				"		<mo>&InvisibleTimes;</mo>\n" +
				"		<msup>\n" +
				"			<mi>x</mi>\n" +
				"			<mn>2</mn>\n" +
				"		</msup>\n" +
				"		<mo>+</mo>\n" +
				"		<mi>b</mi>\n" +
				"		<mo>&InvisibleTimes; </mo>\n" +
				"		<mi>x</mi>\n" +
				"		<mo>+</mo>\n" +
				"		<mi>c</mi>\n" +
				"	</mrow>\n" +
				"</math>"
				;
			const string targetOutput3A = input3;
			const string targetOutput3B = "<?xml version=\"1.0\" standalone=\"no\"?>" +
				"<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\" " +
				"\"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">" +
				"<math xmlns=\"http://www.w3.org/1998/Math/MathML\">" +
				"<mrow>" +
				"<mi>a</mi>" +
				"<mo>&InvisibleTimes;</mo>" +
				"<msup>" +
				"<mi>x</mi>" +
				"<mn>2</mn>" +
				"</msup>" +
				"<mo>+</mo>" +
				"<mi>b</mi>" +
				"<mo>&InvisibleTimes; </mo>" +
				"<mi>x</mi>" +
				"<mo>+</mo>" +
				"<mi>c</mi>" +
				"</mrow>" +
				"</math>"
				;

			const string input4 = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
				"<!DOCTYPE math PUBLIC \"-//W3C//DTD MathML 2.0//EN\" " +
				"\"http://www.w3.org/Math/DTD/mathml2/mathml2.dtd\">\n" +
				"<mrow>\n" +
				"	a &InvisibleTimes; <msup>x 2</msup>\n" +
				"	+ b &InvisibleTimes; x\n" +
				"	+ c\n" +
				"</mrow>"
				;
			const string targetOutput4A = input4;
			const string targetOutput4B = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
				"<!DOCTYPE math PUBLIC \"-//W3C//DTD MathML 2.0//EN\" " +
				"\"http://www.w3.org/Math/DTD/mathml2/mathml2.dtd\">" +
				"<mrow>\n" +
				"	a &InvisibleTimes; <msup>x 2</msup>\n" +
				"	+ b &InvisibleTimes; x\n" +
				"	+ c\n" +
				"</mrow>"
				;

			const string input5 = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n\n" +
				"<feed xmlns=\"http://www.w3.org/2005/Atom\">\n\n" +
				"	<title>Some feed title...</title>\n" +
				"	<subtitle> 	 </subtitle>\n" +
				"	<link href=\"http://example.org/feed/\" rel=\"self\"/>\n" +
				"	<link href=\"http://example.org/\"/>\n" +
				"	<id>urn:uuid:447d7106-3653-407c-ba8b-776f001b9d30</id>\n" +
				"	<updated>2013-04-12T19:35:07Z</updated>\n\n" +
				"	<entry>\n" +
				"		<title>Some entry title...</title>\n" +
				"		<link href=\"http://example.org/2003/12/13/atom03\"/>\n" +
				"		<link rel=\"alternate\" type=\"text/html\" href=\"http://example.org/2003/12/13/atom03.html\"/>\n" +
				"		<link rel=\"edit\" href=\"http://example.org/2003/12/13/atom03/edit\"/>\n" +
				"		<id>urn:uuid:af2cb1de-6a68-4af2-aa87-f575898a96e2</id>\n" +
				"		<updated>2013-04-12T19:35:07Z</updated>\n" +
				"		<summary>Some text...</summary>\n" +
				"		<author>\n" +
				"			<name>Vasya Pupkin</name>\n" +
				"			<email>vasya.pupkin@example.com</email>\n" +
				"		</author>\n" +
				"	</entry>\n" +
				"</feed>\n"
				;
			const string targetOutput5A = input5;
			const string targetOutput5B = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
				"<feed xmlns=\"http://www.w3.org/2005/Atom\">" +
				"<title>Some feed title...</title>" +
				"<subtitle> 	 </subtitle>" +
				"<link href=\"http://example.org/feed/\" rel=\"self\"/>" +
				"<link href=\"http://example.org/\"/>" +
				"<id>urn:uuid:447d7106-3653-407c-ba8b-776f001b9d30</id>" +
				"<updated>2013-04-12T19:35:07Z</updated>" +
				"<entry>" +
				"<title>Some entry title...</title>" +
				"<link href=\"http://example.org/2003/12/13/atom03\"/>" +
				"<link rel=\"alternate\" type=\"text/html\" href=\"http://example.org/2003/12/13/atom03.html\"/>" +
				"<link rel=\"edit\" href=\"http://example.org/2003/12/13/atom03/edit\"/>" +
				"<id>urn:uuid:af2cb1de-6a68-4af2-aa87-f575898a96e2</id>" +
				"<updated>2013-04-12T19:35:07Z</updated>" +
				"<summary>Some text...</summary>" +
				"<author>" +
				"<name>Vasya Pupkin</name>" +
				"<email>vasya.pupkin@example.com</email>" +
				"</author>" +
				"</entry>" +
				"</feed>"
				;

			// Act
			string output1A = keepingWhitespaceMinifier.Minify(input1).MinifiedContent;
			string output1B = removingWhitespaceMinifier.Minify(input1).MinifiedContent;

			string output2A = keepingWhitespaceMinifier.Minify(input2).MinifiedContent;
			string output2B = removingWhitespaceMinifier.Minify(input2).MinifiedContent;

			string output3A = keepingWhitespaceMinifier.Minify(input3).MinifiedContent;
			string output3B = removingWhitespaceMinifier.Minify(input3).MinifiedContent;

			string output4A = keepingWhitespaceMinifier.Minify(input4).MinifiedContent;
			string output4B = removingWhitespaceMinifier.Minify(input4).MinifiedContent;

			string output5A = keepingWhitespaceMinifier.Minify(input5).MinifiedContent;
			string output5B = removingWhitespaceMinifier.Minify(input5).MinifiedContent;

			// Assert
			Assert.Equal(targetOutput1A, output1A);
			Assert.Equal(targetOutput1B, output1B);

			Assert.Equal(targetOutput2A, output2A);
			Assert.Equal(targetOutput2B, output2B);

			Assert.Equal(targetOutput3A, output3A);
			Assert.Equal(targetOutput3B, output3B);

			Assert.Equal(targetOutput4A, output4A);
			Assert.Equal(targetOutput4B, output4B);

			Assert.Equal(targetOutput5A, output5A);
			Assert.Equal(targetOutput5B, output5B);
		}

		#endregion

		#region Processing XML comments

		[Fact]
		public void RemovingXmlCommentsIsCorrect()
		{
			// Arrange
			var keepingXmlCommentsMinifier = new XmlMinifier(
				new XmlMinificationSettings(true) { RemoveXmlComments = false });
			var removingXmlCommentsMinifier = new XmlMinifier(
				new XmlMinificationSettings(true) { RemoveXmlComments = true });

			const string input1 = "<!---->";
			const string targetOutput1A = input1;
			const string targetOutput1B = "";

			const string input2 = "<!-- -->";
			const string targetOutput2A = input2;
			const string targetOutput2B = "";

			const string input3 = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
				"<rss version=\"2.0\">" +
				"<channel>" +
				"<!-- Channel properties -->" +
				"<title>RSS Title</title>" +
				"<description>This is an example of an RSS feed</description>" +
				"<link>http://www.example.com/rss</link>" +
				"<lastBuildDate>Mon, 07 Sep 2013 00:01:00 +0000</lastBuildDate>" +
				"<pubDate>Mon, 07 Sep 2012 16:45:00 +0000</pubDate>" +
				"<ttl>1800</ttl>" +
				"<!-- /Channel properties -->" +
				"<!-- Item list -->" +
				"<item>" +
				"<title>Example <!-- Somme comment... -->entry</title>" +
				"<description>" +
				"<![CDATA[" +
				"<p>Here is some text<!-- Somme other comment... --> containing an description.</p>" +
				"]]>" +
				"</description>" +
				"<link>http://www.example.com/2012/09/01/my-article</link>" +
				"<guid>97357194-40ea-4a19-9941-bc208521b8ce</guid>" +
				"<pubDate>Mon, 07 Sep 2012 16:45:00 +0000</pubDate>" +
				"</item>" +
				"<!-- /Item list -->" +
				"</channel>" +
				"</rss>"
				;
			const string targetOutput3A = input3;
			const string targetOutput3B = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
				"<rss version=\"2.0\">" +
				"<channel>" +
				"<title>RSS Title</title>" +
				"<description>This is an example of an RSS feed</description>" +
				"<link>http://www.example.com/rss</link>" +
				"<lastBuildDate>Mon, 07 Sep 2013 00:01:00 +0000</lastBuildDate>" +
				"<pubDate>Mon, 07 Sep 2012 16:45:00 +0000</pubDate>" +
				"<ttl>1800</ttl>" +
				"<item>" +
				"<title>Example entry</title>" +
				"<description>" +
				"<![CDATA[" +
				"<p>Here is some text<!-- Somme other comment... --> containing an description.</p>" +
				"]]>" +
				"</description>" +
				"<link>http://www.example.com/2012/09/01/my-article</link>" +
				"<guid>97357194-40ea-4a19-9941-bc208521b8ce</guid>" +
				"<pubDate>Mon, 07 Sep 2012 16:45:00 +0000</pubDate>" +
				"</item>" +
				"</channel>" +
				"</rss>"
				;

			const string input4 = "<!--wmm:ignore-->Some text...<!--/wmm:ignore-->";
			const string targetOutput4A = "Some text...";
			const string targetOutput4B = targetOutput4A;

			// Act
			string output1A = keepingXmlCommentsMinifier.Minify(input1).MinifiedContent;
			string output1B = removingXmlCommentsMinifier.Minify(input1).MinifiedContent;

			string output2A = keepingXmlCommentsMinifier.Minify(input2).MinifiedContent;
			string output2B = removingXmlCommentsMinifier.Minify(input2).MinifiedContent;

			string output3A = keepingXmlCommentsMinifier.Minify(input3).MinifiedContent;
			string output3B = removingXmlCommentsMinifier.Minify(input3).MinifiedContent;

			string output4A = keepingXmlCommentsMinifier.Minify(input4).MinifiedContent;
			string output4B = removingXmlCommentsMinifier.Minify(input4).MinifiedContent;

			// Assert
			Assert.Equal(targetOutput1A, output1A);
			Assert.Equal(targetOutput1B, output1B);

			Assert.Equal(targetOutput2A, output2A);
			Assert.Equal(targetOutput2B, output2B);

			Assert.Equal(targetOutput3A, output3A);
			Assert.Equal(targetOutput3B, output3B);

			Assert.Equal(targetOutput4A, output4A);
			Assert.Equal(targetOutput4B, output4B);
		}

		#endregion

		#region Empty tag rendering

		[Fact]
		public void EmptyTagRenderingIsCorrect()
		{
			// Arrange
			var emptyTagWithSlashMinifier = new XmlMinifier(
				new XmlMinificationSettings(true) { RenderEmptyTagsWithSpace = false });
			var emptyTagWithSpaceAndSlashMinifier = new XmlMinifier(
				new XmlMinificationSettings(true) { RenderEmptyTagsWithSpace = true });

			const string input1 = "<?xml version=\"1.0\" standalone=\"no\"?>" +
				"<!DOCTYPE svg PUBLIC \" -//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">" +
				"<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\" width=\"220\" height=\"220\">" +
				"<circle cx=\"110\" cy=\"110\" r=\"100\" fill=\"#fafaa2\" stroke=\"#000\" />" +
				"<rect x=\"10\" y=\"10\" width=\"200\" height=\"200\" fill=\"#fafaa2\" stroke=\"#000\"/>" +
				"</svg>"
				;
			const string targetOutput1A = "<?xml version=\"1.0\" standalone=\"no\"?>" +
				"<!DOCTYPE svg PUBLIC \" -//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">" +
				"<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\" width=\"220\" height=\"220\">" +
				"<circle cx=\"110\" cy=\"110\" r=\"100\" fill=\"#fafaa2\" stroke=\"#000\"/>" +
				"<rect x=\"10\" y=\"10\" width=\"200\" height=\"200\" fill=\"#fafaa2\" stroke=\"#000\"/>" +
				"</svg>"
				;
			const string targetOutput1B = "<?xml version=\"1.0\" standalone=\"no\"?>" +
				"<!DOCTYPE svg PUBLIC \" -//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">" +
				"<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\" width=\"220\" height=\"220\">" +
				"<circle cx=\"110\" cy=\"110\" r=\"100\" fill=\"#fafaa2\" stroke=\"#000\" />" +
				"<rect x=\"10\" y=\"10\" width=\"200\" height=\"200\" fill=\"#fafaa2\" stroke=\"#000\" />" +
				"</svg>"
				;

			// Act
			string output1A = emptyTagWithSlashMinifier.Minify(input1).MinifiedContent;
			string output1B = emptyTagWithSpaceAndSlashMinifier.Minify(input1).MinifiedContent;

			// Assert
			Assert.Equal(targetOutput1A, output1A);
			Assert.Equal(targetOutput1B, output1B);
		}

		#endregion

		#region Collapsing tags without content

		[Fact]
		public void CollapsingTagsWithoutContentIsCorrect()
		{
			// Arrange
			var notCollapsingMinifier = new XmlMinifier(
				new XmlMinificationSettings(true) { CollapseTagsWithoutContent = false });
			var collapsingMinifier = new XmlMinifier(
				new XmlMinificationSettings(true) { CollapseTagsWithoutContent = true });

			const string input1 = "<node></node>";
			const string targetOutput1A = input1;
			const string targetOutput1B = "<node/>";

			const string input2 = "<node>  \t \n  \n</node>";

			const string input3 = "<row RoleId=\"4\" RoleName=\"Administrator\"></row>\n" +
				"<row RoleId=\"5\" RoleName=\"Contributor\"></row>\n" +
				"<row RoleId=\"6\" RoleName=\"Editor\"></row>"
				;
			const string targetOutput3A = input3;
			const string targetOutput3B = "<row RoleId=\"4\" RoleName=\"Administrator\"/>\n" +
				"<row RoleId=\"5\" RoleName=\"Contributor\"/>\n" +
				"<row RoleId=\"6\" RoleName=\"Editor\"/>"
				;

			const string input4 = "<row RoleId=\"4\" RoleName=\"Administrator\"> </row>\n" +
				"<row RoleId=\"5\" RoleName=\"Contributor\">  \t  </row>\n" +
				"<row RoleId=\"6\" RoleName=\"Editor\"> \n  \n </row>"
				;

			// Act
			string output1A = notCollapsingMinifier.Minify(input1).MinifiedContent;
			string output1B = collapsingMinifier.Minify(input1).MinifiedContent;

			string output2A = notCollapsingMinifier.Minify(input2).MinifiedContent;
			string output2B = collapsingMinifier.Minify(input2).MinifiedContent;

			string output3A = notCollapsingMinifier.Minify(input3).MinifiedContent;
			string output3B = collapsingMinifier.Minify(input3).MinifiedContent;

			string output4A = notCollapsingMinifier.Minify(input4).MinifiedContent;
			string output4B = collapsingMinifier.Minify(input4).MinifiedContent;

			// Assert
			Assert.Equal(targetOutput1A, output1A);
			Assert.Equal(targetOutput1B, output1B);

			Assert.Equal(input2, output2A);
			Assert.Equal(input2, output2B);

			Assert.Equal(targetOutput3A, output3A);
			Assert.Equal(targetOutput3B, output3B);

			Assert.Equal(input4, output4A);
			Assert.Equal(input4, output4B);
		}

		#endregion

		#region Applying of all optimizations

		[Fact]
		public void ApplyingOfAllOptimizationsIsCorrect()
		{
			// Arrange
			var emptyTagWithSlashMinifier = new XmlMinifier(
				new XmlMinificationSettings(true)
				{
					MinifyWhitespace = true,
					RemoveXmlComments = true,
					CollapseTagsWithoutContent = true,
					RenderEmptyTagsWithSpace = false
				});
			var emptyTagWithSpaceAndSlashMinifier = new XmlMinifier(
				new XmlMinificationSettings(true)
				{
					MinifyWhitespace = true,
					RemoveXmlComments = true,
					CollapseTagsWithoutContent = true,
					RenderEmptyTagsWithSpace = true
				});

			const string input1 = "  \n	\n  <?xml	 version=\"1.0\"	encoding=\'utf-8\' \n ?>\n" +
				"<xsl:stylesheet  xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\"	version=\'1.0\'>\n" +
				"	<xsl:output   method=\"xml\"   indent=\"yes\"   />\n\n" +
				"	<!-- List of persons template -->\n" +
				"	<xsl:template   match=\"/persons\"  >\n" +
				"		<root>\n" +
				"			<xsl:apply-templates  select=\"person\"><!-- Apply the Person template --></xsl:apply-templates>\n" +
				"		</root>\n" +
				"	</xsl:template>\n" +
				"	<!-- /List of persons template -->\n\n" +
				"	<!-- Person template -->\n" +
				"	<xsl:template    match=\"person\"  >\n" +
				"		<name   username=\"{@username}\"  >\n" +
				"			<!-- Name of person -->\n" +
				"			<xsl:value-of     select=\"name\"   />\n" +
				"			<!-- /Name of person -->\n" +
				"			<xsl:if    test=\"position() != last()\"  >\n" +
				"				<xsl:text>, </xsl:text>\n" +
				"			</xsl:if>\n" +
				"		</name>\n" +
				"	</xsl:template>\n" +
				"	<!-- /Person template -->\n\n" +
				"</xsl:stylesheet>  \n	 \n"
				;
			const string targetOutput1A = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
				"<xsl:stylesheet xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" version=\"1.0\">" +
				"<xsl:output method=\"xml\" indent=\"yes\"/>" +
				"<xsl:template match=\"/persons\">" +
				"<root>" +
				"<xsl:apply-templates select=\"person\"/>" +
				"</root>" +
				"</xsl:template>" +
				"<xsl:template match=\"person\">" +
				"<name username=\"{@username}\">" +
				"<xsl:value-of select=\"name\"/>" +
				"<xsl:if test=\"position() != last()\">" +
				"<xsl:text>, </xsl:text>" +
				"</xsl:if>" +
				"</name>" +
				"</xsl:template>" +
				"</xsl:stylesheet>"
				;
			const string targetOutput1B = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
				"<xsl:stylesheet xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" version=\"1.0\">" +
				"<xsl:output method=\"xml\" indent=\"yes\" />" +
				"<xsl:template match=\"/persons\">" +
				"<root>" +
				"<xsl:apply-templates select=\"person\" />" +
				"</root>" +
				"</xsl:template>" +
				"<xsl:template match=\"person\">" +
				"<name username=\"{@username}\">" +
				"<xsl:value-of select=\"name\" />" +
				"<xsl:if test=\"position() != last()\">" +
				"<xsl:text>, </xsl:text>" +
				"</xsl:if>" +
				"</name>" +
				"</xsl:template>" +
				"</xsl:stylesheet>"
				;

			// Act
			string output1A = emptyTagWithSlashMinifier.Minify(input1).MinifiedContent;
			string output1B = emptyTagWithSpaceAndSlashMinifier.Minify(input1).MinifiedContent;

			// Assert
			Assert.Equal(targetOutput1A, output1A);
			Assert.Equal(targetOutput1B, output1B);
		}

		#endregion
	}
}