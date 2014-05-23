//#define fullyTagged // i.e. use ELEMENT and TEXT (see below) to encapsulate elements and texts
/*-----------------------------------------------------------------------------------------

  C#Prolog -- Copyright (C) 2007-2013 John Pool -- j.pool@ision.nl

  This library is free software; you can redistribute it and/or modify it under the terms of
  the GNU General Public License as published by the Free Software Foundation; either version
  2 of the License, or any later version.

  This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
  without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
  See the GNU General Public License for details, or enter 'license.' at the command prompt.

-------------------------------------------------------------------------------------------*/

using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Globalization;

namespace Prolog
{
  public partial class Engine
  {
    #region Node

    const string CDATA        = "'cdata$'";
    const string COMMENT      = "'comment$'";
    const string INSTRUCTIONS = "'instructions$'";
    const string XMLDECL      = "'xmldecl$'";
    const string XMLDOCUMENT  = "'xmldocument$'";
    const NumberStyles styleAllowDecPnt = NumberStyles.AllowDecimalPoint;
    const NumberStyles styleAllowFloat = NumberStyles.Float;
#if fullyTagged
    const string ELEMENT      = "'element$'";
    const string TEXT         = "'text$'";
#endif

    public class Node
    {
      XmlNodeType type;
      string name;
      string text; // contains prefix in case of element
      List<KeyValuePair<string, string>> attributes;
      List<Node> childNodes;

      public Node ()
      {
        attributes = new List<KeyValuePair<string, string>> ();
        childNodes = new List<Node> ();
      }

      public Node (string nodeName)
      {
        name = nodeName;
        attributes = new List<KeyValuePair<string, string>> ();
        childNodes = new List<Node> ();
      }

      public XmlNodeType Type { get { return type; } }
      public string TagName { get { return name; } set { name = value; } }
      public string Text { get { return text; } set { text = value; } }
      public List<Node> ChildNodes { get { return childNodes; } }
      public List<KeyValuePair<string, string>> Attributes { get { return attributes; } }

      public string AttributeValue (string attributeName)
      {
        foreach (KeyValuePair<string, string> kv in attributes)
          if (kv.Key == attributeName) return kv.Value;

        return null;
      }

      public void AddAttribute (string attributeName, string attributeValue)
      {
        attributes.Insert (0,
          new KeyValuePair<string, string> (attributeName.ToAtom (), attributeValue.EscapeDoubleQuotes ()));
      }


      // Conversion of an XML-structure (in a string or in a file) to a Prolog BaseTerm
      public static BaseTerm XmlToTerm (BaseTerm settings, string s, bool inFile)
      {
        XmlTextReader xrd = null;
        StreamReader sr = null;
        Encoding encoding = GetEncodingFromString ("UTF-8");
        Node result;
        string settingValue = null;

        // get settings -- currently, only 'encoding' is recognized
        if (settings != null)
          foreach (BaseTerm setting in (ListTerm)settings) // traverse ...
          {
            string settingName = setting.FunctorToString;

            if (setting.Arity == 1)
              settingValue = setting.Arg (0).FunctorToString;
            else
              IO.Error ("Illegal setting in xml_term/3: '{0}'", setting);

            switch (settingName)
            {
              // Expected string or file encoding. Superseded by explicit encoding attribute setting found in xml
              case "encoding":
                encoding = GetEncodingFromString (settingValue); // default is UTF-8
                break;
              default:
                IO.Error ("Unknown setting in xml_term/3: '{0}'", setting);
                break;
            }
          }

        try
        {
          if (inFile)
          {
            sr = new StreamReader (s, encoding);
            xrd = new XmlTextReader (sr);
          }
          else
            xrd = new XmlTextReader (new StringReader (s));

          //xrd.ProhibitDtd = true; // new in the .NET Framework version 2.0
          xrd.Namespaces = false;
          result = new Node ();
          result.TagName = "<root>";
          result.type = XmlNodeType.Element;

          result.ToNode (xrd, 0); // first, create an intermediate representation (a Node) containing the XML structure
        }
        catch (Exception e)
        {
          string source = inFile ? string.Format (" file '{0}'", s) : null;

          throw new ApplicationException (
            string.Format ("Error in XML input{0}. Message was:\r\n{1}", source, e.Message));
        }
        finally
        {
          if (sr != null) sr.Close ();
          if (xrd != null) xrd.Close ();
        }

        return result.ToTerm (); // Convert the Node to a Prolog BaseTerm
      }


      public void ToNode (XmlTextReader reader, int level)
      {
        Node child; // essentially: in the loop, add new nodes to this.childNodes

        while (reader.Read ())
        {
          //IO.WriteLine ("Read name={0} value={1} reader.NodeType={2} Level={3}", reader.Name, reader.Value.Trim (), reader.NodeType, level);
          switch (reader.NodeType)
          {
            case XmlNodeType.Element:  // create a new subelement
              bool isEmpty = reader.IsEmptyElement;
              child = new Node ();
              child.type = XmlNodeType.Element;
              child.name = reader.LocalName;
              child.text = reader.Prefix;

              while (reader.MoveToNextAttribute ())
                child.AddAttribute (reader.Name, reader.Value);

              if (isEmpty)
              {
                childNodes.Insert (0, child);

                continue;
              }

              child.ToNode (reader, level + 1);
              childNodes.Insert (0, child);

              break;
            case XmlNodeType.Attribute:
              this.AddAttribute (reader.Name, reader.Value);
              break;
            case XmlNodeType.EndElement:
              return;
            case XmlNodeType.Comment:
            case XmlNodeType.Text:
            case XmlNodeType.CDATA:
              child = new Node ();
              child.type = reader.NodeType;
              child.Text = reader.Value;
              childNodes.Insert (0, child);
              break;
            case XmlNodeType.ProcessingInstruction:
              child = new Node ();
              child.type = reader.NodeType;
              child.name = reader.Name;
              child.text = reader.Value;
              childNodes.Insert (0, child);
              break;
            case XmlNodeType.XmlDeclaration:
              while (reader.MoveToNextAttribute ())
                this.AddAttribute (reader.Name, reader.Value);
              break;
            case XmlNodeType.Document:
            case XmlNodeType.DocumentFragment:
            case XmlNodeType.DocumentType:
            case XmlNodeType.EndEntity:
            case XmlNodeType.Entity:
            case XmlNodeType.EntityReference:
            case XmlNodeType.None:
            case XmlNodeType.Notation:
            case XmlNodeType.SignificantWhitespace:
            case XmlNodeType.Whitespace:
              // ignore
              break;
            default:
              throw new ApplicationException (
                string.Format ("*** Unhandled XmlTextReader.NodeType: {0}", reader.NodeType));
          }
        }

        return;
      }


      // ToTerm results in term xml( Pre, Element, Post), where Pre and Post are lists with comments
      // and/or processing instructions that come before/after the top-level XML element.
      public BaseTerm ToTerm ()
      {
        BaseTerm pre = ListTerm.EMPTYLIST;
        BaseTerm post = ListTerm.EMPTYLIST;
        BaseTerm t = null;
        Node topEl = null;

        foreach (Node n in childNodes) // array was constructed in reverse order, so top-level XML-element is entry 0.
        {
          switch (n.type)
          {
            case XmlNodeType.Element:
              topEl = n;
              break;
            case XmlNodeType.Comment:
              t = new CompoundTerm (COMMENT, new StringTerm (n.Text));

              if (topEl == null)
                post = new ListTerm (t, post);
              else
                pre = new ListTerm (t, pre);

              break;
            case XmlNodeType.ProcessingInstruction:
              t = new CompoundTerm (INSTRUCTIONS, new AtomTerm (n.name.ToAtom ()),
                    new AtomTerm (n.text.Trim ()));
              if (topEl == null)
                post = new ListTerm (t, post);
              else
                pre = new ListTerm (t, pre);

              break;
          }
        }

        BaseTerm xmlDecl = ListTerm.EMPTYLIST;

        foreach (KeyValuePair<string, string> kv in Attributes) // XML Declaration (acually a PI) was stored in Attributes
        {
          BaseTerm pair = new OperatorTerm (EqualOpDescr, new AtomTerm (kv.Key), new AtomTerm (kv.Value));
          xmlDecl = new ListTerm (pair, xmlDecl); // [pre, arg2, ...]
        }

        if (xmlDecl != ListTerm.EMPTYLIST) // enhance term with XML Declaration
        {
          xmlDecl = new CompoundTerm (XMLDECL, xmlDecl);
          pre = new ListTerm (xmlDecl, pre);
        }

        BaseTerm content = ToTermEx (topEl); // Now process top-level Element

        return new CompoundTerm (XMLDOCUMENT, new BaseTerm [] { pre, content, post });
      }


      static BaseTerm ToTermEx (Node root)
      {
        BaseTerm [] args = new BaseTerm [3];
#if fullyTagged
        args [0] = new AtomTerm (root.TagName.ToAtom ());
#else
        string tagName = root.TagName.ToAtom ();
#endif
        args [1] = ListTerm.EMPTYLIST;
        Decimal d;

        foreach (KeyValuePair<string, string> kv in root.Attributes) // XML Declaration
        {
          BaseTerm pair;

          if (Decimal.TryParse (kv.Value, styleAllowDecPnt, Utils.CIC, out d))
            pair =  new OperatorTerm (EqualOpDescr, new AtomTerm (kv.Key), new DecimalTerm (d));
          else
            pair =  new OperatorTerm (EqualOpDescr, new AtomTerm (kv.Key), new StringTerm (kv.Value));

          args [1] = new ListTerm (pair, args [1]);
        }

        args [2] = ListTerm.EMPTYLIST;

        if (root.ChildNodes.Count > 0)
        {
          foreach (Node n in root.ChildNodes)
          {
            BaseTerm e;
            e = null;
            switch (n.type)
            {
              case XmlNodeType.Element:
                e = ToTermEx (n);
                break;
              case XmlNodeType.Comment:
                e = new CompoundTerm (COMMENT, new StringTerm (n.text.Trim ().EscapeDoubleQuotes ()));
                break;
              case XmlNodeType.Text:
                if (Decimal.TryParse (n.text, styleAllowDecPnt, Utils.CIC, out d))
#if fullyTagged
                  e = new CompoundTerm (TEXT, new DecimalTerm (d));
                else
                  e = new CompoundTerm (TEXT, new StringTerm (n.text.Trim ().EscapeDoubleQuotes ()));
#else
                  e = new DecimalTerm (d);
                else
                  e = new StringTerm (n.text.Trim ().EscapeDoubleQuotes ());
#endif
                break;
              case XmlNodeType.CDATA:
                e = new CompoundTerm (CDATA, new StringTerm (n.text.Trim ().EscapeDoubleQuotes ()));
                break;
              case XmlNodeType.ProcessingInstruction:
                e = new CompoundTerm ("processing_instruction", new AtomTerm (n.name.ToAtom ()),
                              new StringTerm (n.text.Trim ().EscapeDoubleQuotes ()));
                break;
              case XmlNodeType.SignificantWhitespace:
              case XmlNodeType.Whitespace:
                break;
              default:
                break;
            }
            if (e != null) args [2] = new ListTerm (e, args [2]);
          }
        }

#if fullyTagged
        return new CompoundTerm (ELEMENT, args);
#else
        return new CompoundTerm (tagName, args [1], args [2]);
#endif
      }


      // Conversion of a Prolog BaseTerm to an XML-structure (in a string or in a file)
      public static void TermToXml (BaseTerm settings, BaseTerm xmlTerm, ref string fileNameOrXmlString)
      // xmlTerm = xmldocument( [<xmlprolog>?], element (...), [<misc>?])
      {
        // get settings
        bool isXChars = true; // not used
        bool isFormat = true;
        bool isRemPrf = false; // not used
        bool isIndent = true;
        bool isDtdChk = false; // not used
        Encoding encoding = null;
        string settingValue = null; // value of setting

        if (settings != null)
          foreach (BaseTerm setting in (ListTerm)settings) // traverse settings
          {
            string settingName = setting.FunctorToString;

            if (setting.Arity == 1)
              settingValue = setting.Arg (0).FunctorToString;
            else
              IO.Error ("Illegal setting in xml_term/3: '{0}'", setting);

            switch (settingName)
            {
              case "extended_characters": // Use the extended character entities for XHTML (default true)
                isXChars = (settingValue == "true");
                break;
              case "format": // Strip layouts when no character data appears between elements.
                isFormat = (settingValue == "true");
                break;
              case "remove_attribute_prefixes": // Remove namespace prefixes from attributes when it
                // is the same as the prefix of the parent element
                isRemPrf = (settingValue == "true");
                break;
              case "indent": // Indent the element content (2 spaces)
                isIndent = (settingValue == "true");
                break;
              case "encoding": // Encoding to appear in XML-declaration
                encoding = GetEncodingFromString (settingValue);
                break;
              case "check_dtd": // Read the referenced DTD
                isDtdChk = (settingValue == "true");
                break;
              default:
                IO.Error ("Unknown setting in xml_term/3: '{0}'", setting);
                break;
            }
          }

        XmlTextWriter xwr = null;
        StringWriter sw = new StringWriter ();

        try
        {
          BaseTerm t0 = xmlTerm.Arg (0);

          if (fileNameOrXmlString == null) // return flat XmlString
          {
            xwr = new XmlTextWriter (sw);
            xwr.Formatting = Formatting.None;
          }
          else // write to file
          {
            // get the encoding from the term
            if (encoding == null)
              encoding = GetEncodingFromTerm (t0, Encoding.UTF8); // if not provided use Encoding.UTF8

            xwr = new XmlTextWriter (fileNameOrXmlString, encoding);
            xwr.Formatting = isFormat ? Formatting.Indented : Formatting.None;
            xwr.Indentation = 2;
            xwr.IndentChar = ' '; // default
            xwr.Namespaces = true;
            ContentTermToXml (xwr, t0);
          }

          ElementTermToXml (xwr, xmlTerm.Arg (1)); // top-level element
        }
        catch
        {
          IO.Error ("Unable to convert term to XML:\r\n{0}", xmlTerm);
        }
        finally
        {
          if (fileNameOrXmlString == null)
            fileNameOrXmlString = sw.ToString ();
          else if (xwr != null)
            xwr.Close ();
        }
      }


      static void ContentTermToXml (XmlTextWriter xwr, BaseTerm list) // process an element content, i.e. a t
      {
        while (list != ListTerm.EMPTYLIST) // traverse ...
        {
          BaseTerm e = list.Arg (0);
          string type = e.FunctorToString;

#if !fullyTagged
          if (e is StringTerm)
            xwr.WriteString (((StringTerm)e).Value);
          else if (e is DecimalTerm)
            xwr.WriteString (((DecimalTerm)e).FunctorToString);
          else if (e is CompoundTerm)
            ElementTermToXml (xwr, e);
          else
#endif
            switch (type)
            {
              case XMLDECL:
                xwr.WriteStartDocument (true);
                break;
#if fullyTagged
            case ELEMENT:
              if (!ElementTermToXml (xwr, e)) return false;
              break;
            case TEXT:
              xwr.WriteString (e.Arg (0).FunctorToString);
              break;
#endif
              case CDATA:
                xwr.WriteCData (e.Arg (0).FunctorToString);
                break;
              case COMMENT:
                xwr.WriteComment (e.Arg (0).FunctorToString);
                break;
              case INSTRUCTIONS:
                xwr.WriteProcessingInstruction (e.Arg (0).FunctorToString, e.Arg (1).ToString ());
                break;
              default:
                IO.Error ("ContentTermToXml -- unhandled type: {0} ({1})",
                  e.GetType ().Name, type);
                break;
            }

          list = list.Arg (1);
        }
      }


      static void ElementTermToXml (XmlTextWriter xwr, BaseTerm e) // process an element( <tag>, <attributes>, <content>)
      {
#if fullyTagged
        int ft = 1;
#else
        int ft = 0;
#endif
        // open tag
        if (e.Arity == 1 + ft && e.FunctorToString == XMLDECL)
        {
          xwr.WriteStartDocument ();
        }
        else if (e.Arity == 2 + ft)
        {
#if fullyTagged        
          xwr.WriteStartElement (e.Arg (0).ToString ().Dequoted ());
#else
          xwr.WriteStartElement (e.FunctorToString.Dequoted ());
#endif

          // attributes
          BaseTerm le = e.Arg (ft); // t with attribute-value pairs
          while (le != ListTerm.EMPTYLIST)
          {
            BaseTerm av = le.Arg (0); // BaseTerm: attr = value
            xwr.WriteAttributeString (av.Arg (0).FunctorToString.Dequoted (), av.Arg (1).FunctorToString);
            le = le.Arg (1);
          }

          // content
          ContentTermToXml (xwr, e.Arg (1 + ft));
          xwr.WriteEndElement ();
        }
        else
          IO.Error ("Unexpected element encountered:\r\n{0}", e);
      }


      // 'xmldocument$'(['xmldecl$'([version=1.0,encoding=ISO-8859-1])], ...]), [])
      static Encoding GetEncodingFromTerm (BaseTerm t, Encoding defEnc)
      {
        if (t.Arity == 0 || (t = t.Arg (0)).FunctorToString != XMLDECL)
          return defEnc;

        if (!((t = t.Arg (0)) is ListTerm)) // attributes last, find encoding
          return defEnc;

        foreach (BaseTerm b in (ListTerm)t)
        {
          if (!(b is OperatorTerm) || !((OperatorTerm)b).HasBinaryOperator ("="))
            return defEnc;

          OperatorTerm ot = (OperatorTerm)b;

          if (ot.Arg (0).FunctorToString == "encoding")
            return GetEncodingFromString (ot.Arg (1).FunctorToString);
        }

        return defEnc;
      }


      new public string ToString ()
      {
        StringBuilder sb = new StringBuilder ();
        ToStringEx (this, sb, 0);

        return sb.ToString ();
      }


      static void ToStringEx (Node root, StringBuilder sb, int depth)
      {
        sb.Append (Spaces (depth) + "<" + root.TagName);

        foreach (KeyValuePair<string, string> kv in root.Attributes)
          sb.Append (String.Format (@" {0}=""{1}""", kv.Key, kv.Value));

        sb.Append (">" + root.Text);

        if (root.ChildNodes.Count > 0)
        {
          sb.Append (Environment.NewLine);

          foreach (Node n in root.ChildNodes)
          {
            switch (n.type)
            {
              case XmlNodeType.Element:
                ToStringEx (n, sb, depth + 2);
                break;
              case XmlNodeType.Comment:
                sb.Append ("<!--" + n.text + "-->");
                break;
              case XmlNodeType.Text:
                sb.Append ("[" + n.text.Trim () + "]");
                break;
              case XmlNodeType.CDATA:
              case XmlNodeType.ProcessingInstruction:
              case XmlNodeType.SignificantWhitespace:
              case XmlNodeType.Whitespace:
                break;
              default:
                break;
            }
          }
          sb.Append (String.Format ("{0}</{1}>{2}", Spaces (depth), root.TagName, Environment.NewLine));
        }
        else
          sb.Append ("</" + root.TagName + ">" + Environment.NewLine);
      }


      static Encoding GetEncodingFromString (string enc) // string may be numeric or symbolic
      {
        int encNo;
        enc = enc.Dequoted ();

        try
        {
          if (int.TryParse (enc, out encNo))
            return Encoding.GetEncoding (encNo);

          switch (enc.ToUpper ())
          {
            case "ASMO-708": return Encoding.GetEncoding (708);    // Arabic (ASMO 708)
            case "BIG5": return Encoding.GetEncoding (950);   // Chinese Traditional (Big5)
            case "CP1025": return Encoding.GetEncoding (21025);  // IBM EBCDIC (Cyrillic Serbian-Bulgarian)
            case "CP866": return Encoding.GetEncoding (866);    // Cyrillic (DOS)
            case "CP875": return Encoding.GetEncoding (875);    // IBM EBCDIC (Greek Modern)
            case "CSISO2022JP": return Encoding.GetEncoding (50221);  // Japanese (JIS-Allow 1 byte Kana)
            case "DOS-720": return Encoding.GetEncoding (720);    // Arabic (DOS)
            case "DOS-862": return Encoding.GetEncoding (862);    // Hebrew (DOS)
            case "EUC-CN": return Encoding.GetEncoding (51936);  // Chinese Simplified (EUC)
            case "EUC-JP": return Encoding.GetEncoding (20932);  // Japanese (JIS 0208-1990 and 0212-1990)
            case "EUC-KR": return Encoding.GetEncoding (51949);  // Korean (EUC)
            case "GB18030": return Encoding.GetEncoding (54936);  // Chinese Simplified (GB18030)
            case "GB2312": return Encoding.GetEncoding (936);    // Chinese Simplified (GB2312)
            case "HZ-GB-2312": return Encoding.GetEncoding (52936);  // Chinese Simplified (HZ)
            case "IBM-THAI": return Encoding.GetEncoding (20838);  // IBM EBCDIC (Thai)
            case "IBM00858": return Encoding.GetEncoding (858);    // OEM Multilingual Latin I
            case "IBM00924": return Encoding.GetEncoding (20924);  // IBM Latin-1
            case "IBM01047": return Encoding.GetEncoding (1047);   // IBM Latin-1
            case "IBM01140": return Encoding.GetEncoding (1140);   // IBM EBCDIC (US-Canada-Euro)
            case "IBM01141": return Encoding.GetEncoding (1141);   // IBM EBCDIC (Germany-Euro)
            case "IBM01142": return Encoding.GetEncoding (1142);   // IBM EBCDIC (Denmark-Norway-Euro)
            case "IBM01143": return Encoding.GetEncoding (1143);   // IBM EBCDIC (Finland-Sweden-Euro)
            case "IBM01144": return Encoding.GetEncoding (1144);   // IBM EBCDIC (Italy-Euro)
            case "IBM01145": return Encoding.GetEncoding (1145);   // IBM EBCDIC (Spain-Euro)
            case "IBM01146": return Encoding.GetEncoding (1146);   // IBM EBCDIC (UK-Euro)
            case "IBM01147": return Encoding.GetEncoding (1147);   // IBM EBCDIC (France-Euro)
            case "IBM01148": return Encoding.GetEncoding (1148);   // IBM EBCDIC (International-Euro)
            case "IBM01149": return Encoding.GetEncoding (1149);   // IBM EBCDIC (Icelandic-Euro)
            case "IBM037": return Encoding.GetEncoding (37);     // IBM EBCDIC (US-Canada)
            case "IBM1026": return Encoding.GetEncoding (1026);   // IBM EBCDIC (Turkish Latin-5)
            case "IBM273": return Encoding.GetEncoding (20273);  // IBM EBCDIC (Germany)
            case "IBM277": return Encoding.GetEncoding (20277);  // IBM EBCDIC (Denmark-Norway)
            case "IBM278": return Encoding.GetEncoding (20278);  // IBM EBCDIC (Finland-Sweden)
            case "IBM280": return Encoding.GetEncoding (20280);  // IBM EBCDIC (Italy)
            case "IBM284": return Encoding.GetEncoding (20284);  // IBM EBCDIC (Spain)
            case "IBM285": return Encoding.GetEncoding (20285);  // IBM EBCDIC (UK)
            case "IBM290": return Encoding.GetEncoding (20290);  // IBM EBCDIC (Japanese katakana)
            case "IBM297": return Encoding.GetEncoding (20297);  // IBM EBCDIC (France)
            case "IBM420": return Encoding.GetEncoding (20420);  // IBM EBCDIC (Arabic)
            case "IBM423": return Encoding.GetEncoding (20423);  // IBM EBCDIC (Greek)
            case "IBM424": return Encoding.GetEncoding (20424);  // IBM EBCDIC (Hebrew)
            case "IBM437": return Encoding.GetEncoding (437);    // OEM United States
            case "IBM500": return Encoding.GetEncoding (500);    // IBM EBCDIC (International)
            case "IBM737": return Encoding.GetEncoding (737);    // Greek (DOS)
            case "IBM775": return Encoding.GetEncoding (775);    // Baltic (DOS)
            case "IBM850": return Encoding.GetEncoding (850);    // Western European (DOS)
            case "IBM852": return Encoding.GetEncoding (852);    // Central European (DOS)
            case "IBM855": return Encoding.GetEncoding (855);    // OEM Cyrillic
            case "IBM857": return Encoding.GetEncoding (857);    // Turkish (DOS)
            case "IBM860": return Encoding.GetEncoding (860);    // Portuguese (DOS)
            case "IBM861": return Encoding.GetEncoding (861);    // Icelandic (DOS)
            case "IBM863": return Encoding.GetEncoding (863);    // French Canadian (DOS)
            case "IBM864": return Encoding.GetEncoding (864);    // Arabic (864)
            case "IBM865": return Encoding.GetEncoding (865);    // Nordic (DOS)
            case "IBM869": return Encoding.GetEncoding (869);    // Greek, Modern (DOS)
            case "IBM870": return Encoding.GetEncoding (870);    // IBM EBCDIC (Multilingual Latin-2)
            case "IBM871": return Encoding.GetEncoding (20871);  // IBM EBCDIC (Icelandic)
            case "IBM880": return Encoding.GetEncoding (20880);  // IBM EBCDIC (Cyrillic Russian)
            case "IBM905": return Encoding.GetEncoding (20905);  // IBM EBCDIC (Turkish)
            case "ISO-2022-JP": return Encoding.GetEncoding (50220);  // Japanese (JIS)
            case "ISO-2022-KR": return Encoding.GetEncoding (50225);  // Korean (ISO)
            case "ISO-8859-1": return Encoding.GetEncoding (28591);  // Western European (ISO)
            case "ISO-8859-13": return Encoding.GetEncoding (28603);  // Estonian (ISO)
            case "ISO-8859-15": return Encoding.GetEncoding (28605);  // Latin 9 (ISO)
            case "ISO-8859-2": return Encoding.GetEncoding (28592);  // Central European (ISO)
            case "ISO-8859-3": return Encoding.GetEncoding (28593);  // Latin 3 (ISO)
            case "ISO-8859-4": return Encoding.GetEncoding (28594);  // Baltic (ISO)
            case "ISO-8859-5": return Encoding.GetEncoding (28595);  // Cyrillic (ISO)
            case "ISO-8859-6": return Encoding.GetEncoding (28596);  // Arabic (ISO)
            case "ISO-8859-7": return Encoding.GetEncoding (28597);  // Greek (ISO)
            case "ISO-8859-8": return Encoding.GetEncoding (28598);  // Hebrew (ISO-Visual)
            case "ISO-8859-8-I": return Encoding.GetEncoding (38598);  // Hebrew (ISO-Logical)
            case "ISO-8859-9": return Encoding.GetEncoding (28599);  // Turkish (ISO)
            case "JOHAB": return Encoding.GetEncoding (1361);   // Korean (Johab)
            case "KOI8-R": return Encoding.GetEncoding (20866);  // Cyrillic (KOI8-R)
            case "KOI8-U": return Encoding.GetEncoding (21866);  // Cyrillic (KOI8-U)
            case "KS_C_5601-1987": return Encoding.GetEncoding (949);    // Korean
            case "MACINTOSH": return Encoding.GetEncoding (10000);  // Western European (Mac)
            case "SHIFT_JIS": return Encoding.GetEncoding (932);    // Japanese (Shift-JIS)
            case "UNICODEFFFE": return Encoding.GetEncoding (1201);   // Unicode (Big-Endian)
            case "US-ASCII": return Encoding.GetEncoding (20127);  // US-ASCII
            case "UTF-16": return Encoding.GetEncoding (1200);   // Unicode
            case "UTF-32": return Encoding.GetEncoding (65005);  // Unicode (UTF-32)
            case "UTF-32BE": return Encoding.GetEncoding (65006);  // Unicode (UTF-32 Big-Endian)
            case "UTF-7": return Encoding.GetEncoding (65000);  // Unicode (UTF-7)
            case "UTF-8": return Encoding.GetEncoding (65001);  // Unicode (UTF-8)
            case "WINDOWS-1250": return Encoding.GetEncoding (1250);   // Central European (Windows)
            case "WINDOWS-1251": return Encoding.GetEncoding (1251);   // Cyrillic (Windows)
            case "WINDOWS-1252": return Encoding.GetEncoding (1252);   // Western European (Windows)
            case "WINDOWS-1253": return Encoding.GetEncoding (1253);   // Greek (Windows)
            case "WINDOWS-1254": return Encoding.GetEncoding (1254);   // Turkish (Windows)
            case "WINDOWS-1255": return Encoding.GetEncoding (1255);   // Hebrew (Windows)
            case "WINDOWS-1256": return Encoding.GetEncoding (1256);   // Arabic (Windows)
            case "WINDOWS-1257": return Encoding.GetEncoding (1257);   // Baltic (Windows)
            case "WINDOWS-1258": return Encoding.GetEncoding (1258);   // Vietnamese (Windows)
            case "WINDOWS-874": return Encoding.GetEncoding (874);    // Thai (Windows)
            case "X-CHINESE-CNS": return Encoding.GetEncoding (20000);  // Chinese Traditional (CNS)
            case "X-CHINESE-ETEN": return Encoding.GetEncoding (20002);  // Chinese Traditional (Eten)
            case "X-CP20001": return Encoding.GetEncoding (20001);  // TCA Taiwan
            case "X-CP20003": return Encoding.GetEncoding (20003);  // IBM5550 Taiwan
            case "X-CP20004": return Encoding.GetEncoding (20004);  // TeleText Taiwan
            case "X-CP20005": return Encoding.GetEncoding (20005);  // Wang Taiwan
            case "X-CP20261": return Encoding.GetEncoding (20261);  // T.61
            case "X-CP20269": return Encoding.GetEncoding (20269);  // ISO-6937
            case "X-CP20936": return Encoding.GetEncoding (20936);  // Chinese Simplified (GB2312-80)
            case "X-CP20949": return Encoding.GetEncoding (20949);  // Korean Wansung
            case "X-CP50227": return Encoding.GetEncoding (50227);  // Chinese Simplified (ISO-2022)
            case "X-EUROPA": return Encoding.GetEncoding (29001);  // Europa
            case "X-IA5": return Encoding.GetEncoding (20105);  // Western European (IA5)
            case "X-IA5-GERMAN": return Encoding.GetEncoding (20106);  // German (IA5)
            case "X-IA5-NORWEGIAN": return Encoding.GetEncoding (20108);  // Norwegian (IA5)
            case "X-IA5-SWEDISH": return Encoding.GetEncoding (20107);  // Swedish (IA5)
            case "X-ISCII-AS": return Encoding.GetEncoding (57006);  // ISCII Assamese
            case "X-ISCII-BE": return Encoding.GetEncoding (57003);  // ISCII Bengali
            case "X-ISCII-DE": return Encoding.GetEncoding (57002);  // ISCII Devanagari
            case "X-ISCII-GU": return Encoding.GetEncoding (57010);  // ISCII Gujarati
            case "X-ISCII-KA": return Encoding.GetEncoding (57008);  // ISCII Kannada
            case "X-ISCII-MA": return Encoding.GetEncoding (57009);  // ISCII Malayalam
            case "X-ISCII-OR": return Encoding.GetEncoding (57007);  // ISCII Oriya
            case "X-ISCII-PA": return Encoding.GetEncoding (57011);  // ISCII Punjabi
            case "X-ISCII-TA": return Encoding.GetEncoding (57004);  // ISCII Tamil
            case "X-ISCII-TE": return Encoding.GetEncoding (57005);  // ISCII Telugu
            case "X-MAC-ARABIC": return Encoding.GetEncoding (10004);  // Arabic (Mac)
            case "X-MAC-CE": return Encoding.GetEncoding (10029);  // Central European (Mac)
            case "X-MAC-CHINESESIMP": return Encoding.GetEncoding (10008);  // Chinese Simplified (Mac)
            case "X-MAC-CHINESETRAD": return Encoding.GetEncoding (10002);  // Chinese Traditional (Mac)
            case "X-MAC-CROATIAN": return Encoding.GetEncoding (10082);  // Croatian (Mac)
            case "X-MAC-CYRILLIC": return Encoding.GetEncoding (10007);  // Cyrillic (Mac)
            case "X-MAC-GREEK": return Encoding.GetEncoding (10006);  // Greek (Mac)
            case "X-MAC-HEBREW": return Encoding.GetEncoding (10005);  // Hebrew (Mac)
            case "X-MAC-ICELANDIC": return Encoding.GetEncoding (10079);  // Icelandic (Mac)
            case "X-MAC-JAPANESE": return Encoding.GetEncoding (10001);  // Japanese (Mac)
            case "X-MAC-KOREAN": return Encoding.GetEncoding (10003);  // Korean (Mac)
            case "X-MAC-ROMANIAN": return Encoding.GetEncoding (10010);  // Romanian (Mac)
            case "X-MAC-THAI": return Encoding.GetEncoding (10021);  // Thai (Mac)
            case "X-MAC-TURKISH": return Encoding.GetEncoding (10081);  // Turkish (Mac)
            case "X-MAC-UKRAINIAN": return Encoding.GetEncoding (10017);  // Ukrainian (Mac)
          }
        }
        catch
        {
        }

        IO.Warning ("Unknown encoding '{0}' -- UTF-8 used instead", enc);

        return Encoding.GetEncoding (65001);  // UTF-8
      }
    }
    #endregion Node
  }
}
