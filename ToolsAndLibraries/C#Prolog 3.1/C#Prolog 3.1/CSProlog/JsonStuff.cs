using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

/* JSON Grammar

Grammar for JSON data, following http://www.json.org/
and compliant with http://www.ietf.org/rfc/rfc4627

"Start Symbol" = <Json>
"Case Sensitive" = True
"Character Mapping" = 'Unicode'

! ------------------------------------------------- Sets

{Unescaped} = {All Valid} - {&1 .. &19} - ["\]
{Hex} = {Digit} + [ABCDEFabcdef]
{Digit9} = {Digit} - [0]

! ------------------------------------------------- Terminals

Number = '-'?('0'|{Digit9}{Digit}*)('.'{Digit}+)?([Ee][+-]?{Digit}+)?
String = '"'({Unescaped}|'\'(["\/bfnrt]|'u'{Hex}{Hex}{Hex}{Hex}))*'"'

! ------------------------------------------------- Rules

<Json> ::= <Object>
         | <Array>

<Object> ::= '{' '}'
           | '{' <Members> '}'

<Members> ::= <Pair>
            | <Pair> ',' <Members>

<Pair> ::= Label ':' <Value>

<Label> ::= String

<Value> ::= String
          | Number
          | <Object>
          | <Array>
          | true
          | false
          | null

<Array> ::= '[' ']'
          | '[' <Elements> ']'

<Elements> ::= <Value>
             | <Value> ',' <Elements>

*/

namespace Prolog
{
  partial class Engine
  {
    public static void JsonToXml (BaseTerm jsonTerm, string root, ref string fileNameOrXmlString,
      string[] attributes)
    {
      // convert the JSON term to an equivalent term in which JSON-term 
      // functors are replaced by names that will appear as tag names
      XmlTextWriter xwr = null;
      StringWriter sw = new StringWriter ();
      bool contentWritten = false;

      try
      {
        if (fileNameOrXmlString == null) // return flat XmlString
        {
          xwr = new XmlTextWriter (sw);
          xwr.Formatting = Formatting.None;
        }
        else // write to file
        {
          xwr = new XmlTextWriter (fileNameOrXmlString, Encoding.UTF8);
          xwr.Formatting = Formatting.Indented;
          xwr.Indentation = 2;
          xwr.IndentChar = ' '; // default
        }

        xwr.WriteStartElement (root);

        if (jsonTerm.FunctorToString == PrologParser.CURL)
          DoJsonObject (xwr, attributes, jsonTerm, ref contentWritten);
        else if (jsonTerm is AltListTerm) // if {...} has been declared a last with wrap( '{', |, '}')
          DoJsonObject (xwr, attributes, (AltListTerm)jsonTerm, ref contentWritten);
        else if (jsonTerm.IsProperList)
          DoJsonArray (xwr, attributes, null, ((ListTerm)jsonTerm), ref contentWritten);
        else
          IO.Error ("Unable to convert term to XML:\r\n{0}", jsonTerm);

        xwr.WriteEndElement ();
      }
      finally
      {
        if (fileNameOrXmlString == null)
          fileNameOrXmlString = sw.ToString ();
        else if (xwr != null)
          xwr.Close ();
      }
    }


    static void DoJsonObject (XmlTextWriter xwr, string [] attributes, AltListTerm t, ref bool contentWritten)
    {
      if (t.IsEmptyList) return;

      DoJsonPair (xwr, attributes, t.Arg (0), ref contentWritten);
      DoJsonObject (xwr, attributes, (AltListTerm)t.Arg (1), ref contentWritten);
    }


    static void DoJsonObject (XmlTextWriter xwr, string [] attributes, BaseTerm t, ref bool contentWritten)
    {
      foreach (BaseTerm a in t.Args) DoJsonPair (xwr, attributes, a, ref contentWritten);
    }


    static void DoJsonArray (XmlTextWriter xwr, string [] attributes, string label, ListTerm t, ref bool contentWritten)
    {
      if (t.IsEmptyList) return;

      DoJsonValue (xwr, attributes, label, t.Arg (0), ref contentWritten);
      DoJsonArray (xwr, attributes, label, (ListTerm)t.Arg (1), ref contentWritten);
    }


    static void DoJsonPair (XmlTextWriter xwr, string [] attributes, BaseTerm t, ref bool contentWritten)
    {
      if (!(t.FunctorToString == ":" && t.Arity == 2))
        IO.Error ("Unable to convert term to XML:\r\n{0}", t);

      string label = t.Arg (0).FunctorToString;

      if (!contentWritten && t.Arg (1).Arity == 0 && attributes != null && attributes.Contains (label))
        xwr.WriteAttributeString (label, t.Arg (1).FunctorToString);
      else
      {
        xwr.WriteStartElement (label);
        contentWritten = false;
        DoJsonValue (xwr, attributes, label, t.Arg (1), ref contentWritten);
        xwr.WriteEndElement ();
        contentWritten = true;
      }
    }


    static void DoJsonValue (XmlTextWriter xwr, string [] attributes, string label, BaseTerm t, ref bool contentWritten)
    {
      string functor = t.FunctorToString;

      if (functor == PrologParser.CURL)
        DoJsonObject (xwr, attributes, t, ref contentWritten);
      else if (t is AltListTerm) // if {...} has been declared a last with wrap( '{', |, '}')
        DoJsonObject (xwr, attributes, (AltListTerm)t, ref contentWritten);
      else if (t.IsProperList)
        DoJsonArray (xwr, attributes, label, (ListTerm)t, ref contentWritten);
      else if (t.IsNumber || t.Arity == 0)
      {
        xwr.WriteStartElement ("value");
        string value = t.ToString ();
        Regex re = new Regex ("@{(?<id>.*)}");
        Match m = re.Match (value);

        //if (m.Success)
        //{
        //  string id = m.Groups ["id"].Captures [0].Value;
        //  xwr.WriteAttributeString (label, id);
        //}
        //else
          xwr.WriteString (value);

        xwr.WriteEndElement ();
      }
      else
        IO.Error ("Unable to convert term to XML:\r\n{0}", t);
    }


    // JSON formatting

    class JsonTextBuffer : List<KeyValuePair<string, string>>
    {
      StringBuilder sb; // sealed, cannot inheritate from
      const int DELTA = 2;
      int level;
      int maxAttrLen;
      bool? commaBeforeBuff = null;
      string Indentation { get { return new string (' ', level * DELTA); } }

      public JsonTextBuffer ()
      {
        sb = new StringBuilder ();
        level = 0;
        maxAttrLen = 0;
      }


      public void EmitAttrValuePair (string attr, string value, bool first)
      {
        AppendPossibleCommaAndNewLine (first);
        sb.AppendFormat ("{0}{1} : {2}", Indentation, attr.Enquote ('"'), value);

        if (commaBeforeBuff == null) commaBeforeBuff = !first; // remember first value;

        maxAttrLen = Math.Max (attr.Length, maxAttrLen);
        Add (new KeyValuePair<string, string> (attr, value));
      }


      public void EmitOpenBracket (string attrName, char c)
      {
        if (attrName != null)
          sb.AppendLine ("{0}{1} :", Indentation, attrName.Enquote ('"'));

        sb.AppendFormat ("{0}{1}", Indentation, c);
        level++;
      }


      public void EmitCloseBracket (char c)
      {
        level--;
        sb.AppendFormat ("\r\n{0}{1}", Indentation, c);
      }


      public void AppendPossibleCommaAndNewLine (bool first)
      {
        if (!first) sb.Append (',');

        sb.AppendLine ();
      }


      public bool FlushBuffer (bool first)
      {
        //if (Count == 0) return (commaBeforeBuff == false);

        //if (commaBeforeBuff == true) sb.Append (',');

        foreach (KeyValuePair<string, string> kv in this)
        {
          string attr = kv.Key;

          AppendPossibleCommaAndNewLine (first);
          sb.AppendFormat ("{0}{1}{2} : {3}",
            Indentation, attr.Enquote ('"'), new string (' ', maxAttrLen-attr.Length), kv.Value);
        }

        Clear ();
        maxAttrLen = 0;
        //commaBeforeBuff = null;

        return true;
      }


      public override string ToString ()
      {
        return sb.ToString ();
      }
    }


    public static string FormatJson (BaseTerm jsonTerm)
    {
      JsonTextBuffer result = new JsonTextBuffer ();

      if (jsonTerm.FunctorToString == PrologParser.CURL)
        DoJsonObject0 (result, null, jsonTerm, true);
      else if (jsonTerm is AltListTerm) // if {...} has been declared a last with wrap( '{', |, '}')
        DoJsonObject0 (result, null, (AltListTerm)jsonTerm, true);
      else if (jsonTerm.IsProperList)
        DoJsonArray0 (result, null, ((ListTerm)jsonTerm), true);
      else
        IO.Error ("Error in JSON term:\r\n{0}", jsonTerm);

      return result.ToString ();
    }


    static void DoJsonObject0 (JsonTextBuffer avb, string attrName, BaseTerm t, bool first)
    {
      avb.AppendPossibleCommaAndNewLine (first); // is this entire {}-last the first element?
      avb.EmitOpenBracket (attrName, '{');
      DoJsonObject (avb, t, true);
      avb.EmitCloseBracket ('}');
    }


    static void DoJsonArray0 (JsonTextBuffer avb, string attrName, ListTerm t, bool first)
    {
      avb.AppendPossibleCommaAndNewLine (first); // is this entire []-last the first element?
      avb.EmitOpenBracket (attrName, '[');
      DoJsonArray (avb, t, true);
      avb.EmitCloseBracket (']');
    }

    
    static void DoJsonObject (JsonTextBuffer avb, AltListTerm t, bool first)
    {
      if (t.IsEmptyList) return;

      DoJsonPair (avb, t.Arg (0), first);
      DoJsonObject (avb, (AltListTerm)t.Arg (1), false);
    }

    static void DoJsonObject (JsonTextBuffer avb, BaseTerm t, bool first)
    {
      foreach (BaseTerm a in t.Args)
      {
        DoJsonPair (avb, a, first);
        first = false;
      }
    }


    static void DoJsonArray (JsonTextBuffer avb, ListTerm t, bool first)
    {
      if (t.IsEmptyList) return;

      DoJsonValue (avb, null, t.Arg (0), first);
      DoJsonArray (avb, (ListTerm)t.Arg (1), false);
    }


    static void DoJsonPair (JsonTextBuffer avb, BaseTerm t, bool first)
    {
      if (!(t.FunctorToString == ":" && t.Arity == 2))
        IO.Error ("Not a JSON-term:\r\n{0}", t);

      DoJsonValue (avb, t.Arg (0).FunctorToString, t.Arg (1), first);
    }


    static void DoJsonValue (JsonTextBuffer avb, string attrName, BaseTerm t, bool first)
    {
      string functor = t.FunctorToString;

      if (functor == PrologParser.CURL)
        DoJsonObject0 (avb, attrName, t, first); // is this the first object of a last?
      else if (t is AltListTerm) // if {...} has been declared a last with wrap( '{', |, '}')
        DoJsonObject0 (avb, attrName, (AltListTerm)t, first);
      else if (t.IsProperList)
        DoJsonArray0 (avb, attrName, (ListTerm)t, first);
      else if (t.IsNumber || t.Arity == 0)
        avb.EmitAttrValuePair (attrName, t.ToString (), first);
      else
        IO.Error ("Not a JSON-term:\r\n{0}", t);
    }
  }
}
