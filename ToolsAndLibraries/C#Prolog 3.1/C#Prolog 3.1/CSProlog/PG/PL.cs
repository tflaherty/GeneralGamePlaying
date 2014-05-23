//#define LL1_tracing
namespace Prolog
{
  using System;
  using System.IO;
  using System.Text;
  using System.Xml;
  using System.Collections;
  using System.Collections.Generic;
  using System.Collections.Specialized;
  using System.Globalization;
  using System.Threading;
  using System.Diagnostics;
  using System.Security.Principal;

/* _______________________________________________________________________________________________
  |                                                                                               |
  |  C#Prolog -- Copyright (C) 2007 John Pool -- j.pool@ision.nl                                  |
  |                                                                                               |
  |  This library is free software; you can redistribute it and/or modify it under the terms of   |
  |  the GNU General Public License as published by the Free Software Foundation; either version  |
  |  2 of the License, or any later version.                                                      |
  |                                                                                               |
  |  This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;    |
  |  without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.    |
  |  See the GNU General Public License for details, or enter 'license.' at the command prompt.   |
  |_______________________________________________________________________________________________|
*/

// Parser Generator version 4.0 -- Date/Time: 12-12-12 17:31:18


  public partial class Engine
  {
    #region PrologParser
    public partial class PrologParser : BaseParser<OpDescrTriplet>
    {
      public static readonly string VersionTimeStamp = "2012-12-12 17:31:18";
      
      
      #region Terminal definition
      
      /* The following constants are defined in BaseParser.cs:
      const int Undefined = 0;
      const int Comma = 1;
      const int LeftParen = 2;
      const int RightParen = 3;
      const int Identifier = 4;
      const int IntLiteral = 5;
      const int ppDefine = 6;
      const int ppUndefine = 7;
      const int ppIf = 8;
      const int ppIfNot = 9;
      const int ppIfDef = 10;
      const int ppIfNDef = 11;
      const int ppElse = 12;
      const int ppEndIf = 13;
      const int RealLiteral = 14;
      const int ImagLiteral = 15;
      const int StringLiteral = 16;
      const int CharLiteral = 17;
      const int CommentStart = 18;
      const int CommentSingle = 19;
      const int EndOfLine = 20;
      const int ANYSYM = 21;
      const int EndOfInput = 22;
      */
      const int Operator = 23;
      const int Atom = 24;
      const int VerbatimStringStart = 25;
      const int Dot = 26;
      const int Anonymous = 27;
      const int CutSym = 28;
      const int ImpliesSym = 29;
      const int PromptSym = 30;
      const int DCGArrowSym = 31;
      const int BuiltinCSharp = 32;
      const int LSqBracket = 33;
      const int RSqBracket = 34;
      const int LCuBracket = 35;
      const int RCuBracket = 36;
      const int VBar = 37;
      const int OpSym = 38;
      const int WrapSym = 39;
      const int BuiltinSym = 40;
      const int ProgramSym = 41;
      const int ReadingSym = 42;
      const int EnsureLoaded = 43;
      const int Discontiguous = 44;
      const int StringStyle = 45;
      const int AllDiscontiguous = 46;
      const int Module = 47;
      const int ListPatternOpen = 48;
      const int ListPatternClose = 49;
      const int EllipsisSym = 50;
      const int SubtreeSym = 51;
      const int NegateSym = 52;
      const int PlusSym = 53;
      const int TimesSym = 54;
      const int QuestionMark = 55;
      const int QuestionMarks = 56;
      const int TrySym = 57;
      const int CatchSym = 58;
      const int WrapOpen = 59;
      const int WrapClose = 60;
      const int AltListOpen = 61;
      const int AltListClose = 62;
      const int Slash = 63;
      const int VerbatimStringLiteral = 64;
      // Total number of terminals:
      public const int terminalCount = 65;
      
      public static void FillTerminalTable (BaseTrie terminalTable)
      {
        terminalTable.Add (Undefined, "Undefined");
        terminalTable.Add (Comma, "Comma", ",");
        terminalTable.Add (LeftParen, "LeftParen", "(");
        terminalTable.Add (RightParen, "RightParen", ")");
        terminalTable.Add (Identifier, "Identifier");
        terminalTable.Add (IntLiteral, "IntLiteral");
        terminalTable.Add (ppDefine, "ppDefine", "#define");
        terminalTable.Add (ppUndefine, "ppUndefine", "#undefine");
        terminalTable.Add (ppIf, "ppIf", "#if");
        terminalTable.Add (ppIfNot, "ppIfNot", "#ifnot");
        terminalTable.Add (ppIfDef, "ppIfDef", "#ifdef");
        terminalTable.Add (ppIfNDef, "ppIfNDef", "#ifndef");
        terminalTable.Add (ppElse, "ppElse", "#else");
        terminalTable.Add (ppEndIf, "ppEndIf", "#endif");
        terminalTable.Add (RealLiteral, "RealLiteral");
        terminalTable.Add (ImagLiteral, "ImagLiteral");
        terminalTable.Add (StringLiteral, "StringLiteral");
        terminalTable.Add (CharLiteral, "CharLiteral");
        terminalTable.Add (CommentStart, "CommentStart", "/*");
        terminalTable.Add (CommentSingle, "CommentSingle", "%");
        terminalTable.Add (EndOfLine, "EndOfLine");
        terminalTable.Add (ANYSYM, "ANYSYM");
        terminalTable.Add (EndOfInput, "EndOfInput");
        terminalTable.Add (Operator, "Operator");
        terminalTable.Add (Atom, "Atom");
        terminalTable.Add (VerbatimStringStart, "VerbatimStringStart", @"@""");
        terminalTable.Add (Dot, "Dot");
        terminalTable.Add (Anonymous, "Anonymous", "_");
        terminalTable.Add (CutSym, "CutSym", "!");
        terminalTable.Add (ImpliesSym, "ImpliesSym", ":-");
        terminalTable.Add (PromptSym, "PromptSym", "?-");
        terminalTable.Add (DCGArrowSym, "DCGArrowSym", "-->");
        terminalTable.Add (BuiltinCSharp, "BuiltinCSharp", ":==");
        terminalTable.Add (LSqBracket, "LSqBracket", "[");
        terminalTable.Add (RSqBracket, "RSqBracket", "]");
        terminalTable.Add (LCuBracket, "LCuBracket", "{");
        terminalTable.Add (RCuBracket, "RCuBracket", "}");
        terminalTable.Add (VBar, "VBar", "|");
        terminalTable.Add (OpSym, "OpSym", "op");
        terminalTable.Add (WrapSym, "WrapSym", "wrap");
        terminalTable.Add (BuiltinSym, "BuiltinSym", "&builtin");
        terminalTable.Add (ProgramSym, "ProgramSym", "&program");
        terminalTable.Add (ReadingSym, "ReadingSym", "&reading");
        terminalTable.Add (EnsureLoaded, "EnsureLoaded", "ensure_loaded");
        terminalTable.Add (Discontiguous, "Discontiguous", "discontiguous");
        terminalTable.Add (StringStyle, "StringStyle", "stringstyle");
        terminalTable.Add (AllDiscontiguous, "AllDiscontiguous", "alldiscontiguous");
        terminalTable.Add (Module, "Module", "module");
        terminalTable.Add (ListPatternOpen, "ListPatternOpen", "[!");
        terminalTable.Add (ListPatternClose, "ListPatternClose", "!]");
        terminalTable.Add (EllipsisSym, "EllipsisSym", "..");
        terminalTable.Add (SubtreeSym, "SubtreeSym", @"\");
        terminalTable.Add (NegateSym, "NegateSym", "~");
        terminalTable.Add (PlusSym, "PlusSym", "+");
        terminalTable.Add (TimesSym, "TimesSym", "*");
        terminalTable.Add (QuestionMark, "QuestionMark", "?");
        terminalTable.Add (QuestionMarks, "QuestionMarks", "??");
        terminalTable.Add (TrySym, "TrySym", "TRY");
        terminalTable.Add (CatchSym, "CatchSym", "CATCH");
        terminalTable.Add (WrapOpen, "WrapOpen");
        terminalTable.Add (WrapClose, "WrapClose");
        terminalTable.Add (AltListOpen, "AltListOpen");
        terminalTable.Add (AltListClose, "AltListClose");
        terminalTable.Add (Slash, "Slash");
        terminalTable.Add (VerbatimStringLiteral, "VerbatimStringLiteral");
      }
      
      #endregion Terminal definition

      #region Constructor
      public PrologParser (Engine engine)
      {
        this.engine = engine;
        ps = engine.Ps;
        terminalTable = engine.terminalTable;
        opTable = engine.OpTable;
        symbol = new Symbol (this);
        streamInPrefix = "";
        streamInPreLen = 0;
        AddReservedOperators ();
      }

      public PrologParser ()
      {
        FillTerminalTable (terminalTable);
        symbol = new Symbol (this);
        streamInPrefix = "";
        streamInPreLen = 0;
        AddReservedOperators ();
      }
      #endregion constructor

      #region NextSymbol, GetSymbol
      protected override bool GetSymbol (TerminalSet followers, bool done, bool genXCPN)
      {
        string s;

        if (symbol.IsProcessed) NextSymbol ();

        symbol.SetProcessed (done);
        if (parseAnyText || followers.IsEmpty ()) return true;

        if (syntaxErrorStat) return false;

        if (symbol.Terminal == ANYSYM || followers.Contains (symbol.Terminal)) return true;

        switch (symbol.Terminal)
        {
          case EndOfLine:
            if (seeEndOfLine) s = "<EndOfLine>"; else goto default;
            s = "<EndOfLine>";
            break;
          case EndOfInput:
            s = "<EndOfInput>";
            break;
          default:
            s = String.Format ("\"{0}\"", symbol.ToString ());
            break;
        }

        s = String.Format ("*** Unexpected symbol: {0}{1}*** Expected one of: {2}", s,
                           Environment.NewLine, terminalTable.TerminalImageSet (followers));
        if (genXCPN)
          SyntaxError = s;
        else
          errorMessage = s;

        return true;
      }
      #endregion NextSymbol, GetSymbol

      #region PARSER PROCEDURES
      public override void RootCall ()
      {
        PrologCode (new TerminalSet (terminalCount, EndOfInput));
      }


      public override void Delegates ()
      {
        
      }

      
      #region PrologCode
      void PrologCode (TerminalSet _TS)
      {
        #if LL1_tracing
        ReportParserProcEntry ("PrologCode");
        #endif
        SetCommaAsSeparator (false); // Comma-role only if comma is separating arguments
        terminalTable [OP] = OpSym;
        terminalTable [WRAP] = WrapSym;
        inQueryMode = false;
        try {
        SeeEndOfLine = false;
        terminalTable [ELLIPSIS] = Operator;
        terminalTable [SUBTREE] = Operator;
        terminalTable [STRINGSTYLE] = Atom;
        if (terminalTable [NEGATE] == NegateSym) terminalTable [NEGATE] = Atom;
        GetSymbol (_TS.Union (terminalCount, LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral, StringLiteral,
                                             Operator, Atom, Anonymous, CutSym, LSqBracket, LCuBracket, OpSym, WrapSym,
                                             BuiltinSym, ProgramSym, ReadingSym, ListPatternOpen, TrySym, WrapOpen, WrapClose,
                                             AltListOpen, AltListClose, VerbatimStringLiteral), false, true);
        if (symbol.IsMemberOf (LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral, StringLiteral, Operator, Atom,
                               Anonymous, CutSym, LSqBracket, LCuBracket, OpSym, WrapSym, BuiltinSym, ProgramSym, ReadingSym,
                               ListPatternOpen, TrySym, WrapOpen, WrapClose, AltListOpen, AltListClose, VerbatimStringLiteral))
        {
          GetSymbol (new TerminalSet (terminalCount, LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral, StringLiteral,
                                                     Operator, Atom, Anonymous, CutSym, LSqBracket, LCuBracket, OpSym, WrapSym,
                                                     BuiltinSym, ProgramSym, ReadingSym, ListPatternOpen, TrySym, WrapOpen,
                                                     WrapClose, AltListOpen, AltListClose, VerbatimStringLiteral), false, true);
          if (symbol.Terminal == BuiltinSym)
          {
            symbol.SetProcessed ();
            Initialize ();
            Predefineds (_TS);
          }
          else if (symbol.Terminal == ProgramSym)
          {
            symbol.SetProcessed ();
            Initialize ();
            Program (_TS);
          }
          else if (symbol.Terminal == ReadingSym)
          {
            symbol.SetProcessed ();
            PrologTerm (new TerminalSet (terminalCount, Dot), out readTerm);
            GetSymbol (new TerminalSet (terminalCount, Dot), true, true);
          }
          else
          {
            engine.EraseVariables ();
            inQueryMode = true;
            GetSymbol (new TerminalSet (terminalCount, LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral,
                                                       StringLiteral, Operator, Atom, Anonymous, CutSym, LSqBracket, LCuBracket,
                                                       OpSym, WrapSym, ListPatternOpen, TrySym, WrapOpen, WrapClose,
                                                       AltListOpen, AltListClose, VerbatimStringLiteral), false, true);
            if (symbol.IsMemberOf (LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral, StringLiteral, Operator, Atom,
                                   Anonymous, CutSym, LSqBracket, LCuBracket, ListPatternOpen, TrySym, WrapOpen, WrapClose,
                                   AltListOpen, AltListClose, VerbatimStringLiteral))
            {
              terminalTable [OP] = Atom;
              terminalTable [WRAP] = Atom;
              SetReservedOperators (true);
              Query (new TerminalSet (terminalCount, Dot), out queryNode);
            }
            else if (symbol.Terminal == OpSym)
            {
              OpDefinition (new TerminalSet (terminalCount, Dot), true);
              queryNode = null;
            }
            else
            {
              WrapDefinition (new TerminalSet (terminalCount, Dot));
              queryNode = null;
            }
            GetSymbol (new TerminalSet (terminalCount, Dot), true, true);
          }
        }
        } finally { Terminate (); }
        #if LL1_tracing
        ReportParserProcExit ("PrologCode");
        #endif
      }
      #endregion
      
      #region Program
      void Program (TerminalSet _TS)
      {
        #if LL1_tracing
        ReportParserProcEntry ("Program");
        #endif
        bool firstReport = true;
        while (true)
        {
          GetSymbol (_TS.Union (terminalCount, LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral, StringLiteral,
                                               Operator, Atom, Anonymous, CutSym, ImpliesSym, PromptSym, LSqBracket, LCuBracket,
                                               ListPatternOpen, TrySym, WrapOpen, WrapClose, AltListOpen, AltListClose,
                                               VerbatimStringLiteral), false, true);
          if (symbol.IsMemberOf (LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral, StringLiteral, Operator, Atom,
                                 Anonymous, CutSym, ImpliesSym, PromptSym, LSqBracket, LCuBracket, ListPatternOpen, TrySym,
                                 WrapOpen, WrapClose, AltListOpen, AltListClose, VerbatimStringLiteral))
          {
            ClauseNode (_TS.Union (terminalCount, LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral, StringLiteral,
                                                  Operator, Atom, Anonymous, CutSym, ImpliesSym, PromptSym, LSqBracket,
                                                  LCuBracket, ListPatternOpen, TrySym, WrapOpen, WrapClose, AltListOpen,
                                                  AltListClose, VerbatimStringLiteral), ref firstReport);
          }
          else
            break;
        }
        #if LL1_tracing
        ReportParserProcExit ("Program");
        #endif
      }
      #endregion
      
      #region ClauseNode
      void ClauseNode (TerminalSet _TS, ref bool firstReport)
      {
        #if LL1_tracing
        ReportParserProcEntry ("ClauseNode");
        #endif
        BaseTerm head;
        TermNode body = null;
        ClauseNode c;
        engine.EraseVariables ();
        int lineNo = symbol.LineNo;
        GetSymbol (new TerminalSet (terminalCount, LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral, StringLiteral,
                                                   Operator, Atom, Anonymous, CutSym, ImpliesSym, PromptSym, LSqBracket,
                                                   LCuBracket, ListPatternOpen, TrySym, WrapOpen, WrapClose, AltListOpen,
                                                   AltListClose, VerbatimStringLiteral), false, true);
        if (symbol.IsMemberOf (LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral, StringLiteral, Operator, Atom,
                               Anonymous, CutSym, LSqBracket, LCuBracket, ListPatternOpen, TrySym, WrapOpen, WrapClose,
                               AltListOpen, AltListClose, VerbatimStringLiteral))
        {
          PrologTerm (new TerminalSet (terminalCount, Dot, ImpliesSym, DCGArrowSym), out head);
          if (!head.IsCallable)
            IO.Error ("Illegal predicate head: {0}", head.ToString ());
          if (engine.predTable.Predefineds.Contains (head.Key) | head.Precedence >= 1000)
            IO.Error ("Predefined predicate or operator '{0}' cannot be redefined", head.FunctorToString);
          GetSymbol (new TerminalSet (terminalCount, Dot, ImpliesSym, DCGArrowSym), false, true);
          if (symbol.IsMemberOf (ImpliesSym, DCGArrowSym))
          {
            GetSymbol (new TerminalSet (terminalCount, ImpliesSym, DCGArrowSym), false, true);
            if (symbol.Terminal == ImpliesSym)
            {
              symbol.SetProcessed ();
              Query (new TerminalSet (terminalCount, Dot), out body);
            }
            else
            {
              symbol.SetProcessed ();
              BaseTerm t;
              readingDcgClause = true;
              PrologTerm (new TerminalSet (terminalCount, Dot), out t);
              readingDcgClause = false;
              body = t.ToDCG (ref head);
            }
          }
          c = new ClauseNode (head, body);
          engine.ReportSingletons (c, lineNo - 1, ref firstReport);
          ps.AddClause (c);
        }
        else if (symbol.Terminal == PromptSym)
        {
          symbol.SetProcessed ();
          bool m = inQueryMode;
          bool o = isReservedOperatorSetting;
          try {
          inQueryMode = true;
          SetReservedOperators (true);
          Query (new TerminalSet (terminalCount, Dot), out queryNode);
          IO.Error ("'?-' querymode in file not yet supported");
          } finally {
          inQueryMode = m;
          SetReservedOperators (o);
          }
        }
        else
        {
          symbol.SetProcessed ();
          terminalTable [STRINGSTYLE] = StringStyle;
          terminalTable.Add (Module, "Module", "module");
          terminalTable.Add (Discontiguous, "Discontiguous", "discontiguous");
          int saveNegate = terminalTable [NEGATE];
          try {
          GetSymbol (new TerminalSet (terminalCount, Atom, LSqBracket, OpSym, WrapSym, EnsureLoaded, Discontiguous, StringStyle,
                                                     AllDiscontiguous, Module), false, true);
          if (symbol.Terminal == OpSym)
          {
            OpDefinition (new TerminalSet (terminalCount, Dot), true);
          }
          else if (symbol.Terminal == WrapSym)
          {
            WrapDefinition (new TerminalSet (terminalCount, Dot));
          }
          else if (symbol.Terminal == EnsureLoaded)
          {
            symbol.SetProcessed ();
            GetSymbol (new TerminalSet (terminalCount, LeftParen), true, true);
            GetSymbol (new TerminalSet (terminalCount, Operator, Atom), false, true);
            if (symbol.Terminal == Atom)
            {
              symbol.SetProcessed ();
            }
            else
            {
              symbol.SetProcessed ();
            }
            string fileName = Utils.ExtendedFileName (symbol.ToString ().ToLower (), ".pl");
            if (Globals.ConsultedFiles [fileName] == null)
            {
              ps.Consult (fileName);
              Globals.ConsultedFiles [fileName] = true;
            }
            GetSymbol (new TerminalSet (terminalCount, RightParen), true, true);
          }
          else if (symbol.Terminal == Discontiguous)
          {
            symbol.SetProcessed ();
            BaseTerm t;
            PrologTerm (new TerminalSet (terminalCount, Dot), out t);
            ps.SetDiscontiguous (t);
          }
          else if (symbol.Terminal == StringStyle)
          {
            symbol.SetProcessed ();
            BaseTerm t;
            PrologTerm (new TerminalSet (terminalCount, Dot), out t);
            engine.SetStringStyle (t);
          }
          else if (symbol.Terminal == AllDiscontiguous)
          {
            symbol.SetProcessed ();
            ps.SetDiscontiguous (true);
          }
          else if (symbol.Terminal == Module)
          {
            symbol.SetProcessed ();
            GetSymbol (new TerminalSet (terminalCount, LeftParen), true, true);
            try {
            SetCommaAsSeparator (true);
            GetSymbol (new TerminalSet (terminalCount, Operator, Atom), false, true);
            if (symbol.Terminal == Atom)
            {
              symbol.SetProcessed ();
            }
            else
            {
              symbol.SetProcessed ();
            }
            ps.SetModuleName (symbol.ToString ());
            GetSymbol (new TerminalSet (terminalCount, Comma), true, true);
            } finally {
            SetCommaAsSeparator (false);
            }
            BaseTerm t;
            PrologTerm (new TerminalSet (terminalCount, RightParen), out t);
            GetSymbol (new TerminalSet (terminalCount, RightParen), true, true);
          }
          else if (symbol.Terminal == LSqBracket)
          {
            symbol.SetProcessed ();
            int lines = 0;
            int files = 0;
            try {
            while (true)
            {
              GetSymbol (new TerminalSet (terminalCount, Operator, Atom), false, true);
              if (symbol.Terminal == Atom)
              {
                symbol.SetProcessed ();
              }
              else
              {
                symbol.SetProcessed ();
              }
              string fileName = Utils.FileNameFromSymbol (symbol.ToString (), ".pl");
              SetCommaAsSeparator (false);
              lines += ps.Consult (fileName);
              files++;
              SetCommaAsSeparator (true);
              GetSymbol (new TerminalSet (terminalCount, Comma, RSqBracket), false, true);
              if (symbol.Terminal == Comma)
              {
                symbol.SetProcessed ();
              }
              else
                break;
            }
              if (files > 1) IO.Message ("Grand total is {0} lines", lines);
            } finally {
            SetCommaAsSeparator (false);
            }
            GetSymbol (new TerminalSet (terminalCount, RSqBracket), true, true);
          }
          else
          {
            SimpleDirective (new TerminalSet (terminalCount, Dot));
          }
          } finally {
          terminalTable.Remove ("module");
          terminalTable.Remove ("discontiguous");
          terminalTable [ELLIPSIS] = Atom;
          terminalTable [NEGATE] = saveNegate;
          terminalTable [STRINGSTYLE] = Atom;
          terminalTable [SLASH] = Operator;
          terminalTable [SUBTREE] = Operator;
          }
        }
        GetSymbol (new TerminalSet (terminalCount, Dot), true, true);
        #if LL1_tracing
        ReportParserProcExit ("ClauseNode");
        #endif
      }
      #endregion
      
      #region SimpleDirective
      void SimpleDirective (TerminalSet _TS)
      {
        #if LL1_tracing
        ReportParserProcEntry ("SimpleDirective");
        #endif
        GetSymbol (new TerminalSet (terminalCount, Atom), true, true);
        string directive = symbol.ToString ();
        bool spaceAfter = symbol.IsFollowedByLayoutChar;
        string argument = null;
        int arity = -1;
        terminalTable [SLASH] = Slash;
        GetSymbol (_TS.Union (terminalCount, LeftParen), false, true);
        if (symbol.Terminal == LeftParen)
        {
          symbol.SetProcessed ();
          GetSymbol (new TerminalSet (terminalCount, IntLiteral, StringLiteral, Operator, Atom), false, true);
          if (symbol.IsMemberOf (Operator, Atom))
          {
            if (spaceAfter)
              IO.Error ("Illegal space between directive '{0}' and left parenthesis", directive);
            GetSymbol (new TerminalSet (terminalCount, Operator, Atom), false, true);
            if (symbol.Terminal == Atom)
            {
              symbol.SetProcessed ();
            }
            else
            {
              symbol.SetProcessed ();
            }
            argument = symbol.ToString ();
            GetSymbol (new TerminalSet (terminalCount, RightParen, Slash), false, true);
            if (symbol.Terminal == Slash)
            {
              symbol.SetProcessed ();
              GetSymbol (new TerminalSet (terminalCount, IntLiteral), true, true);
              arity = symbol.ToInt ();
            }
          }
          else
          {
            GetSymbol (new TerminalSet (terminalCount, IntLiteral, StringLiteral), false, true);
            if (symbol.Terminal == StringLiteral)
            {
              symbol.SetProcessed ();
            }
            else
            {
              symbol.SetProcessed ();
            }
            argument = symbol.ToString ().Dequoted ();
          }
          GetSymbol (new TerminalSet (terminalCount, RightParen), true, true);
        }
        ps.HandleSimpleDirective (directive, argument, arity);
        #if LL1_tracing
        ReportParserProcExit ("SimpleDirective");
        #endif
      }
      #endregion
      
      #region OpDefinition
      void OpDefinition (TerminalSet _TS, bool user)
      {
        #if LL1_tracing
        ReportParserProcEntry ("OpDefinition");
        #endif
        string name;
        string assoc;
        try {
        SetCommaAsSeparator (true);
        GetSymbol (new TerminalSet (terminalCount, OpSym), true, true);
        int prec;
        GetSymbol (new TerminalSet (terminalCount, LeftParen), true, true);
        GetSymbol (new TerminalSet (terminalCount, IntLiteral, CutSym), false, true);
        if (symbol.Terminal == IntLiteral)
        {
          symbol.SetProcessed ();
          prec = symbol.ToInt ();
        }
        else
        {
          symbol.SetProcessed ();
          prec = -1;
        }
        GetSymbol (new TerminalSet (terminalCount, Comma), true, true);
        GetSymbol (new TerminalSet (terminalCount, Operator, Atom), false, true);
        if (symbol.Terminal == Atom)
        {
          symbol.SetProcessed ();
        }
        else
        {
          symbol.SetProcessed ();
        }
        assoc = symbol.ToString ();
        GetSymbol (new TerminalSet (terminalCount, Comma), true, true);
        GetSymbol (new TerminalSet (terminalCount, LeftParen, Operator, Atom, LSqBracket, OpSym, WrapSym, EnsureLoaded,
                                                   Discontiguous, StringStyle, AllDiscontiguous, Module, WrapOpen, WrapClose),
                   false, true);
        if (symbol.Terminal == LSqBracket)
        {
          symbol.SetProcessed ();
          while (true)
          {
            PotentialOpName (new TerminalSet (terminalCount, Comma, RSqBracket), out name);
            if (prec == -1)
              RemovePrologOperator (assoc, name, user);
            else
              AddPrologOperator (prec, assoc, name, user);
            GetSymbol (new TerminalSet (terminalCount, Comma, RSqBracket), false, true);
            if (symbol.Terminal == Comma)
            {
              symbol.SetProcessed ();
            }
            else
              break;
          }
          GetSymbol (new TerminalSet (terminalCount, RSqBracket), true, true);
        }
        else
        {
          PotentialOpName (new TerminalSet (terminalCount, RightParen), out name);
          if (prec == -1)
            RemovePrologOperator (assoc, name, user);
          else
            AddPrologOperator (prec, assoc, name, user);
        }
        GetSymbol (new TerminalSet (terminalCount, RightParen), true, true);
        } finally {
        SetCommaAsSeparator (false);
        }
        #if LL1_tracing
        ReportParserProcExit ("OpDefinition");
        #endif
      }
      #endregion
      
      #region WrapDefinition
      void WrapDefinition (TerminalSet _TS)
      {
        #if LL1_tracing
        ReportParserProcEntry ("WrapDefinition");
        #endif
        // wrapClose is set to the reverse of wrapOpen if only one argument is supplied.
        string wrapOpen;
        string wrapClose = null;
        bool useAsList = false;
        try {
        SetCommaAsSeparator (true);
        GetSymbol (new TerminalSet (terminalCount, WrapSym), true, true);
        GetSymbol (new TerminalSet (terminalCount, LeftParen), true, true);
        PotentialOpName (new TerminalSet (terminalCount, Comma, RightParen), out wrapOpen);
        GetSymbol (new TerminalSet (terminalCount, Comma, RightParen), false, true);
        if (symbol.Terminal == Comma)
        {
          symbol.SetProcessed ();
          GetSymbol (new TerminalSet (terminalCount, LeftParen, RightParen, Operator, Atom, VBar, OpSym, WrapSym, EnsureLoaded,
                                                     Discontiguous, StringStyle, AllDiscontiguous, Module, WrapOpen, WrapClose),
                     false, true);
          if (symbol.IsMemberOf (LeftParen, Operator, Atom, VBar, OpSym, WrapSym, EnsureLoaded, Discontiguous, StringStyle,
                                 AllDiscontiguous, Module, WrapOpen, WrapClose))
          {
            GetSymbol (new TerminalSet (terminalCount, LeftParen, Operator, Atom, VBar, OpSym, WrapSym, EnsureLoaded,
                                                       Discontiguous, StringStyle, AllDiscontiguous, Module, WrapOpen,
                                                       WrapClose), false, true);
            if (symbol.Terminal == VBar)
            {
              symbol.SetProcessed ();
              useAsList = true;
              GetSymbol (new TerminalSet (terminalCount, Comma, RightParen), false, true);
              if (symbol.Terminal == Comma)
              {
                symbol.SetProcessed ();
                PotentialOpName (new TerminalSet (terminalCount, RightParen), out wrapClose);
                wrapClose = symbol.ToString ();
              }
            }
            else
            {
              PotentialOpName (new TerminalSet (terminalCount, RightParen), out wrapClose);
              wrapClose = symbol.ToString ();
            }
          }
        }
        if (wrapClose == null) wrapClose = wrapOpen.Mirror ();
        GetSymbol (new TerminalSet (terminalCount, RightParen), true, true);
        AddBracketPair (wrapOpen, wrapClose, useAsList);
        } finally {
        SetCommaAsSeparator (false);
        }
        #if LL1_tracing
        ReportParserProcExit ("WrapDefinition");
        #endif
      }
      #endregion
      
      #region PotentialOpName
      void PotentialOpName (TerminalSet _TS, out string name)
      {
        #if LL1_tracing
        ReportParserProcEntry ("PotentialOpName");
        #endif
        GetSymbol (new TerminalSet (terminalCount, LeftParen, Operator, Atom, OpSym, WrapSym, EnsureLoaded, Discontiguous,
                                                   StringStyle, AllDiscontiguous, Module, WrapOpen, WrapClose), false, true);
        if (symbol.IsMemberOf (Operator, Atom, OpSym, WrapSym, EnsureLoaded, Discontiguous, StringStyle, AllDiscontiguous,
                               Module, WrapOpen, WrapClose))
        {
          GetSymbol (new TerminalSet (terminalCount, Operator, Atom, OpSym, WrapSym, EnsureLoaded, Discontiguous, StringStyle,
                                                     AllDiscontiguous, Module, WrapOpen, WrapClose), false, true);
          if (symbol.Terminal == Atom)
          {
            symbol.SetProcessed ();
          }
          else if (symbol.Terminal == Operator)
          {
            symbol.SetProcessed ();
          }
          else if (symbol.Terminal == WrapOpen)
          {
            symbol.SetProcessed ();
          }
          else if (symbol.Terminal == WrapClose)
          {
            symbol.SetProcessed ();
          }
          else
          {
            ReservedWord (_TS);
          }
          name = symbol.ToString ();
        }
        else
        {
          symbol.SetProcessed ();
          PotentialOpName (new TerminalSet (terminalCount, RightParen), out name);
          GetSymbol (new TerminalSet (terminalCount, RightParen), true, true);
        }
        #if LL1_tracing
        ReportParserProcExit ("PotentialOpName");
        #endif
      }
      #endregion
      
      #region ReservedWord
      void ReservedWord (TerminalSet _TS)
      {
        #if LL1_tracing
        ReportParserProcEntry ("ReservedWord");
        #endif
        GetSymbol (new TerminalSet (terminalCount, OpSym, WrapSym, EnsureLoaded, Discontiguous, StringStyle, AllDiscontiguous,
                                                   Module), false, true);
        if (symbol.Terminal == OpSym)
        {
          symbol.SetProcessed ();
        }
        else if (symbol.Terminal == WrapSym)
        {
          symbol.SetProcessed ();
        }
        else if (symbol.Terminal == EnsureLoaded)
        {
          symbol.SetProcessed ();
        }
        else if (symbol.Terminal == Discontiguous)
        {
          symbol.SetProcessed ();
        }
        else if (symbol.Terminal == StringStyle)
        {
          symbol.SetProcessed ();
        }
        else if (symbol.Terminal == AllDiscontiguous)
        {
          symbol.SetProcessed ();
        }
        else
        {
          symbol.SetProcessed ();
        }
        #if LL1_tracing
        ReportParserProcExit ("ReservedWord");
        #endif
      }
      #endregion
      
      #region Predefineds
      void Predefineds (TerminalSet _TS)
      {
        #if LL1_tracing
        ReportParserProcEntry ("Predefineds");
        #endif
        do
        {
          Predefined (_TS.Union (terminalCount, LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral, StringLiteral,
                                                Operator, Atom, Anonymous, CutSym, ImpliesSym, LSqBracket, LCuBracket,
                                                ListPatternOpen, TrySym, WrapOpen, WrapClose, AltListOpen, AltListClose,
                                                VerbatimStringLiteral));
          GetSymbol (_TS.Union (terminalCount, LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral, StringLiteral,
                                               Operator, Atom, Anonymous, CutSym, ImpliesSym, LSqBracket, LCuBracket,
                                               ListPatternOpen, TrySym, WrapOpen, WrapClose, AltListOpen, AltListClose,
                                               VerbatimStringLiteral), false, true);
        } while (! (_TS.Contains (symbol.Terminal)) );
        #if LL1_tracing
        ReportParserProcExit ("Predefineds");
        #endif
      }
      #endregion
      
      #region Predefined
      void Predefined (TerminalSet _TS)
      {
        #if LL1_tracing
        ReportParserProcEntry ("Predefined");
        #endif
        BaseTerm head;
        bool     opt  = true;
        TermNode body = null;
        engine.EraseVariables ();
        GetSymbol (new TerminalSet (terminalCount, LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral, StringLiteral,
                                                   Operator, Atom, Anonymous, CutSym, ImpliesSym, LSqBracket, LCuBracket,
                                                   ListPatternOpen, TrySym, WrapOpen, WrapClose, AltListOpen, AltListClose,
                                                   VerbatimStringLiteral), false, true);
        if (symbol.Terminal == ImpliesSym)
        {
          symbol.SetProcessed ();
          GetSymbol (new TerminalSet (terminalCount, Atom, OpSym, WrapSym), false, true);
          if (symbol.Terminal == OpSym)
          {
            OpDefinition (new TerminalSet (terminalCount, Dot), false);
          }
          else if (symbol.Terminal == WrapSym)
          {
            WrapDefinition (new TerminalSet (terminalCount, Dot));
          }
          else
          {
            SimpleDirective (new TerminalSet (terminalCount, Dot));
          }
        }
        else
        {
          PrologTerm (new TerminalSet (terminalCount, Dot, ImpliesSym, DCGArrowSym, BuiltinCSharp), out head);
          GetSymbol (new TerminalSet (terminalCount, Dot, ImpliesSym, DCGArrowSym, BuiltinCSharp), false, true);
          if (symbol.IsMemberOf (ImpliesSym, DCGArrowSym, BuiltinCSharp))
          {
            GetSymbol (new TerminalSet (terminalCount, ImpliesSym, DCGArrowSym, BuiltinCSharp), false, true);
            if (symbol.Terminal == BuiltinCSharp)
            {
              symbol.SetProcessed ();
              GetSymbol (new TerminalSet (terminalCount, Operator, Atom), false, true);
              if (symbol.Terminal == Atom)
              {
                symbol.SetProcessed ();
              }
              else
              {
                symbol.SetProcessed ();
              }
              ps.AddPredefined (new ClauseNode (head, new TermNode (symbol.ToString ())));
              opt = false;
            }
            else if (symbol.Terminal == ImpliesSym)
            {
              symbol.SetProcessed ();
              Query (new TerminalSet (terminalCount, Dot), out body);
              ps.AddPredefined (new ClauseNode (head, body));
              opt = false;
            }
            else
            {
              symbol.SetProcessed ();
              BaseTerm term;
              readingDcgClause = true;
              PrologTerm (new TerminalSet (terminalCount, Dot), out term);
              readingDcgClause = false;
              body = term.ToDCG (ref head);
              ps.AddPredefined (new ClauseNode (head, body));
              opt = false;
            }
          }
          if (opt) ps.AddPredefined (new ClauseNode (head, null));
        }
        GetSymbol (new TerminalSet (terminalCount, Dot), true, true);
        #if LL1_tracing
        ReportParserProcExit ("Predefined");
        #endif
      }
      #endregion
      
      #region Query
      void Query (TerminalSet _TS, out TermNode body)
      {
        #if LL1_tracing
        ReportParserProcEntry ("Query");
        #endif
        BaseTerm t = null;
        PrologTerm (_TS, out t);
        body = t.ToGoalList ();
        #if LL1_tracing
        ReportParserProcExit ("Query");
        #endif
      }
      #endregion
      
      #region PrologTerm
      void PrologTerm (TerminalSet _TS, out BaseTerm t)
      {
        #if LL1_tracing
        ReportParserProcEntry ("PrologTerm");
        #endif
        bool saveStatus = SetCommaAsSeparator (false);
        PrologTermEx (_TS, out t);
        SetCommaAsSeparator (saveStatus);
        #if LL1_tracing
        ReportParserProcExit ("PrologTerm");
        #endif
      }
      #endregion
      
      #region PrologTermEx
      void PrologTermEx (TerminalSet _TS, out BaseTerm t)
      {
        #if LL1_tracing
        ReportParserProcEntry ("PrologTermEx");
        #endif
        string functor;
        OpDescrTriplet triplet;
        bool spaceAfter;
        TokenSeqToTerm tokenSeqToTerm = new TokenSeqToTerm (opTable);
        do
        {
          triplet = null;
          BaseTerm [] args = null;
          GetSymbol (new TerminalSet (terminalCount, LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral, StringLiteral,
                                                     Operator, Atom, Anonymous, CutSym, LSqBracket, LCuBracket, ListPatternOpen,
                                                     TrySym, WrapOpen, WrapClose, AltListOpen, AltListClose,
                                                     VerbatimStringLiteral), false, true);
          if (symbol.Terminal == Operator)
          {
            symbol.SetProcessed ();
            spaceAfter = symbol.IsFollowedByLayoutChar;
            triplet = symbol.Payload;
            bool commaAsSeparator = !spaceAfter && tokenSeqToTerm.PrevTokenWasOperator;
            GetSymbol (_TS.Union (terminalCount, LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral, StringLiteral,
                                                 Operator, Atom, Anonymous, CutSym, LSqBracket, LCuBracket, ListPatternOpen,
                                                 TrySym, WrapOpen, WrapClose, AltListOpen, AltListClose, VerbatimStringLiteral),
                       false, true);
            if (symbol.Terminal == LeftParen)
            {
              symbol.SetProcessed ();
              ArgumentList (new TerminalSet (terminalCount, RightParen), out args, commaAsSeparator);
              GetSymbol (new TerminalSet (terminalCount, RightParen), true, true);
            }
            if (args == null)
              tokenSeqToTerm.Add (triplet); // single operator
            else if (commaAsSeparator)
              tokenSeqToTerm.AddOperatorFunctor (triplet, args); // operator as functor with >= 1 args
            else
            {
              tokenSeqToTerm.Add (triplet);
              tokenSeqToTerm.Add (args [0]);
            }
          }
          else
          {
            GetSymbol (new TerminalSet (terminalCount, LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral,
                                                       StringLiteral, Atom, Anonymous, CutSym, LSqBracket, LCuBracket,
                                                       ListPatternOpen, TrySym, WrapOpen, WrapClose, AltListOpen, AltListClose,
                                                       VerbatimStringLiteral), false, true);
            if (symbol.Terminal == Atom)
            {
              symbol.SetProcessed ();
              functor = symbol.ToString ();
              spaceAfter = symbol.IsFollowedByLayoutChar;
              GetSymbol (_TS.Union (terminalCount, LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral, StringLiteral,
                                                   Operator, Atom, Anonymous, CutSym, LSqBracket, LCuBracket, ListPatternOpen,
                                                   TrySym, WrapOpen, WrapClose, AltListOpen, AltListClose,
                                                   VerbatimStringLiteral), false, true);
              if (symbol.Terminal == LeftParen)
              {
                symbol.SetProcessed ();
                ArgumentList (new TerminalSet (terminalCount, RightParen), out args, true);
                GetSymbol (new TerminalSet (terminalCount, RightParen), true, true);
              }
              tokenSeqToTerm.AddFunctorTerm (functor, spaceAfter, args);
            }
            else if (symbol.Terminal == LeftParen)
            {
              symbol.SetProcessed ();
              bool saveStatus = SetCommaAsSeparator (false);
              PrologTermEx (new TerminalSet (terminalCount, RightParen), out t);
              SetCommaAsSeparator (saveStatus);
              tokenSeqToTerm.Add (t);
              GetSymbol (new TerminalSet (terminalCount, RightParen), true, true);
            }
            else if (symbol.Terminal == Identifier)
            {
              GetIdentifier (_TS.Union (terminalCount, LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral,
                                                       StringLiteral, Operator, Atom, Anonymous, CutSym, LSqBracket, LCuBracket,
                                                       ListPatternOpen, TrySym, WrapOpen, WrapClose, AltListOpen, AltListClose,
                                                       VerbatimStringLiteral), out t);
              tokenSeqToTerm.Add (t);
            }
            else if (symbol.Terminal == Anonymous)
            {
              symbol.SetProcessed ();
              tokenSeqToTerm.Add (new AnonymousVariable ());
            }
            else if (symbol.Terminal == CutSym)
            {
              symbol.SetProcessed ();
              tokenSeqToTerm.Add (new Cut (0));
            }
            else if (symbol.Terminal == AltListOpen)
            {
              AltList (_TS.Union (terminalCount, LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral, StringLiteral,
                                                 Operator, Atom, Anonymous, CutSym, LSqBracket, LCuBracket, ListPatternOpen,
                                                 TrySym, WrapOpen, WrapClose, AltListOpen, AltListClose, VerbatimStringLiteral), 
                      out t);
              tokenSeqToTerm.Add (t);
            }
            else if (symbol.Terminal == LSqBracket)
            {
              List (_TS.Union (terminalCount, LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral, StringLiteral,
                                              Operator, Atom, Anonymous, CutSym, LSqBracket, LCuBracket, ListPatternOpen,
                                              TrySym, WrapOpen, WrapClose, AltListOpen, AltListClose, VerbatimStringLiteral), 
                   out t);
              tokenSeqToTerm.Add (t);
            }
            else if (symbol.IsMemberOf (IntLiteral, RealLiteral))
            {
              GetSymbol (new TerminalSet (terminalCount, IntLiteral, RealLiteral), false, true);
              if (symbol.Terminal == IntLiteral)
              {
                symbol.SetProcessed ();
              }
              else
              {
                symbol.SetProcessed ();
              }
              tokenSeqToTerm.Add (new DecimalTerm (symbol.ToDecimal ()));
            }
            else if (symbol.Terminal == ImagLiteral)
            {
              symbol.SetProcessed ();
              tokenSeqToTerm.Add (new ComplexTerm (0, symbol.ToImaginary ()));
            }
            else if (symbol.Terminal == StringLiteral)
            {
              symbol.SetProcessed ();
              string s = symbol.ToUnquoted ();
              s = ConfigSettings.ResolveEscapes ? s.Unescaped () : s.Replace ("\"\"", "\"");
              tokenSeqToTerm.Add (engine.NewIsoOrCsStringTerm (s));
            }
            else if (symbol.Terminal == VerbatimStringLiteral)
            {
              symbol.SetProcessed ();
              string s = symbol.ToUnquoted ();
              s = s.Replace ("\"\"", "\"");
              tokenSeqToTerm.Add (engine.NewIsoOrCsStringTerm (s));
            }
            else if (symbol.Terminal == LCuBracket)
            {
              DCGBracketList (_TS.Union (terminalCount, LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral,
                                                        StringLiteral, Operator, Atom, Anonymous, CutSym, LSqBracket,
                                                        LCuBracket, ListPatternOpen, TrySym, WrapOpen, WrapClose, AltListOpen,
                                                        AltListClose, VerbatimStringLiteral), out t);
              tokenSeqToTerm.Add (t);
            }
            else if (symbol.Terminal == WrapOpen)
            {
              symbol.SetProcessed ();
              string wrapOpen = symbol.ToString ();
              GetSymbol (_TS.Union (terminalCount, LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral, StringLiteral,
                                                   Operator, Atom, Anonymous, CutSym, LSqBracket, LCuBracket, ListPatternOpen,
                                                   TrySym, WrapOpen, WrapClose, AltListOpen, AltListClose,
                                                   VerbatimStringLiteral), false, true);
              if (symbol.IsMemberOf (LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral, StringLiteral, Operator, Atom,
                                     Anonymous, CutSym, LSqBracket, LCuBracket, ListPatternOpen, TrySym, WrapOpen, WrapClose,
                                     AltListOpen, AltListClose, VerbatimStringLiteral))
              {
                string wrapClose = engine.WrapTable.FindCloseBracket (wrapOpen);
                bool saveStatus = SetCommaAsSeparator (false);
                ArgumentList (new TerminalSet (terminalCount, WrapClose), out args, true);
                SetCommaAsSeparator (saveStatus);
                GetSymbol (new TerminalSet (terminalCount, WrapClose), true, true);
                if (symbol.ToString () != wrapClose)
                  IO.Error ("Illegal wrapper close token: got '{0}' expected '{1}'",
                             symbol.ToString (), wrapClose);
                tokenSeqToTerm.Add (new WrapperTerm (wrapOpen, wrapClose, args));
              }
              if (args == null) tokenSeqToTerm.Add (new AtomTerm (wrapOpen.ToAtom ()));
            }
            else if (symbol.IsMemberOf (WrapClose, AltListClose))
            {
              GetSymbol (new TerminalSet (terminalCount, WrapClose, AltListClose), false, true);
              if (symbol.Terminal == WrapClose)
              {
                symbol.SetProcessed ();
              }
              else
              {
                symbol.SetProcessed ();
              }
              string orphanCloseBracket = symbol.ToString ();
              tokenSeqToTerm.Add (new AtomTerm (orphanCloseBracket.ToAtom ()));
            }
            else if (symbol.Terminal == ListPatternOpen)
            {
              symbol.SetProcessed ();
              ListPatternMembers (new TerminalSet (terminalCount, ListPatternClose), out t);
              GetSymbol (new TerminalSet (terminalCount, ListPatternClose), true, true);
              tokenSeqToTerm.Add (t);
            }
            else
            {
              TryCatchClause (_TS.Union (terminalCount, LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral,
                                                        StringLiteral, Operator, Atom, Anonymous, CutSym, LSqBracket,
                                                        LCuBracket, ListPatternOpen, TrySym, WrapOpen, WrapClose, AltListOpen,
                                                        AltListClose, VerbatimStringLiteral), tokenSeqToTerm, out t);
            }
          }
          GetSymbol (_TS.Union (terminalCount, LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral, StringLiteral,
                                               Operator, Atom, Anonymous, CutSym, LSqBracket, LCuBracket, ListPatternOpen,
                                               TrySym, WrapOpen, WrapClose, AltListOpen, AltListClose, VerbatimStringLiteral),
                     false, true);
        } while (! (_TS.Contains (symbol.Terminal)) );
        tokenSeqToTerm.ConstructPrefixTerm (out t);
        #if LL1_tracing
        ReportParserProcExit ("PrologTermEx");
        #endif
      }
      #endregion
      
      #region GetIdentifier
      void GetIdentifier (TerminalSet _TS, out BaseTerm t)
      {
        #if LL1_tracing
        ReportParserProcEntry ("GetIdentifier");
        #endif
        GetSymbol (new TerminalSet (terminalCount, Identifier), true, true);
        string id = symbol.ToString ();
        t = engine.GetVariable (id);
        if (t == null)
        {
          t = new NamedVariable (id);
          engine.SetVariable (t, id);
        }
        else
          engine.RegisterVarNonSingleton (id);
        #if LL1_tracing
        ReportParserProcExit ("GetIdentifier");
        #endif
      }
      #endregion
      
      #region ArgumentList
      void ArgumentList (TerminalSet _TS, out BaseTerm [] args, bool commaIsSeparator)
      {
        #if LL1_tracing
        ReportParserProcEntry ("ArgumentList");
        #endif
        bool b = isReservedOperatorSetting;
        List<BaseTerm> argList = new List<BaseTerm> ();
        BaseTerm a;
        bool saveStatus = SetCommaAsSeparator (commaIsSeparator);
        SetReservedOperators (true);
        while (true)
        {
          PrologTermEx (_TS.Union (terminalCount, Comma), out a);
          argList.Add (a);
          GetSymbol (_TS.Union (terminalCount, Comma), false, true);
          if (symbol.Terminal == Comma)
          {
            symbol.SetProcessed ();
          }
          else
            break;
        }
        SetCommaAsSeparator (saveStatus);
        SetReservedOperators (b);
        args = argList.ToArray ();
        #if LL1_tracing
        ReportParserProcExit ("ArgumentList");
        #endif
      }
      #endregion
      
      #region ListPatternMembers
      void ListPatternMembers (TerminalSet _TS, out BaseTerm t)
      {
        #if LL1_tracing
        ReportParserProcEntry ("ListPatternMembers");
        #endif
        bool b = isReservedOperatorSetting;
        List<SearchTerm> searchTerms;
        bool saveStatus = SetCommaAsSeparator (true);
        int saveEllipsis = terminalTable [ELLIPSIS];
        int saveNegate = terminalTable [NEGATE];
        int saveSubtree = terminalTable [SUBTREE];
        SetReservedOperators (true);
        bool isRangeVar;
        bool lastWasRange = false;
        List<ListPatternElem> rangeTerms = new List<ListPatternElem> ();
        try {
        bool isSearchTerm = false;
        BaseTerm RangeVar = null;
        BaseTerm minLenTerm;
        BaseTerm maxLenTerm;
        BaseTerm altListName = null;
        minLenTerm = maxLenTerm = new DecimalTerm (0);
        searchTerms = null;
        bool isSingleVar;
        bool isNegSearch = false;
        while (true)
        {
          terminalTable [ELLIPSIS] = EllipsisSym;
          terminalTable [NEGATE] = NegateSym;
          terminalTable [SUBTREE] = SubtreeSym;
          GetSymbol (new TerminalSet (terminalCount, LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral, StringLiteral,
                                                     Operator, Atom, Anonymous, CutSym, LSqBracket, LCuBracket, ListPatternOpen,
                                                     EllipsisSym, NegateSym, TrySym, WrapOpen, WrapClose, AltListOpen,
                                                     AltListClose, VerbatimStringLiteral), false, true);
          if (symbol.IsMemberOf (LCuBracket, EllipsisSym))
          {
            if (lastWasRange)
            {
              rangeTerms.Add (new ListPatternElem (minLenTerm, maxLenTerm, RangeVar, null, null, false, false));
              RangeVar = null;
            }
            Range (_TS.Union (terminalCount, Comma), out minLenTerm, out maxLenTerm);
            lastWasRange = true;
          }
          else
          {
            isRangeVar = false;
            AlternativeTerms (_TS.Union (terminalCount, Comma, LCuBracket, EllipsisSym), 
                             saveEllipsis, saveNegate, saveSubtree, out searchTerms, out altListName, out isSingleVar, out isNegSearch
                             );
            isSearchTerm = true;
            GetSymbol (_TS.Union (terminalCount, Comma, LCuBracket, EllipsisSym), false, true);
            if (symbol.IsMemberOf (LCuBracket, EllipsisSym))
            {
              if (!isSingleVar) IO.Error ("Range specifier may be preceded by a variable only");
              if (lastWasRange)
                rangeTerms.Add (new ListPatternElem (minLenTerm, maxLenTerm, RangeVar, null, null, false, false));
              Range (_TS.Union (terminalCount, Comma), out minLenTerm, out maxLenTerm);
              isRangeVar = true;
              lastWasRange = true;
              isSearchTerm = false;
            }
            if (isRangeVar)
              RangeVar = searchTerms [0].term;
            else
              lastWasRange = false;
          }
          if (isSearchTerm)
          {
            rangeTerms.Add (new ListPatternElem (minLenTerm, maxLenTerm, RangeVar, altListName, searchTerms, isNegSearch, false));
            isSearchTerm = false;
            RangeVar = null;
            altListName = null;
            searchTerms = null;
            minLenTerm = maxLenTerm = new DecimalTerm (0);
          }
          GetSymbol (_TS.Union (terminalCount, Comma), false, true);
          if (symbol.Terminal == Comma)
          {
            symbol.SetProcessed ();
          }
          else
            break;
        }
        if (lastWasRange) rangeTerms.Add (new ListPatternElem (minLenTerm, maxLenTerm, RangeVar, null, null, false, false));
        t = new ListPatternTerm (rangeTerms.ToArray ());
        } finally {
        SetCommaAsSeparator (saveStatus);
        terminalTable [ELLIPSIS] = saveEllipsis;
        terminalTable [NEGATE] = saveNegate;
        terminalTable [SUBTREE] = saveSubtree;
        SetReservedOperators (b);
        }
        #if LL1_tracing
        ReportParserProcExit ("ListPatternMembers");
        #endif
      }
      #endregion
      
      #region AlternativeTerms
      void AlternativeTerms (TerminalSet _TS, 
                            int saveEllipsis, int saveNegate, int saveSubtree, out List<SearchTerm> searchTerms, 
                            out BaseTerm altListName, out bool isSingleVar, out bool isNegSearch)
      {
        #if LL1_tracing
        ReportParserProcEntry ("AlternativeTerms");
        #endif
        searchTerms = new List<SearchTerm> ();
        BaseTerm t;
        altListName = null;
        bool first = true;
        DownRepFactor downRepFactor = null;
        isNegSearch = false;
        while (true)
        {
          GetSymbol (new TerminalSet (terminalCount, LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral, StringLiteral,
                                                     Operator, Atom, Anonymous, CutSym, LSqBracket, LCuBracket, ListPatternOpen,
                                                     NegateSym, TrySym, WrapOpen, WrapClose, AltListOpen, AltListClose,
                                                     VerbatimStringLiteral), false, true);
          if (symbol.Terminal == NegateSym)
          {
            if (isNegSearch) IO.Error ("Only one '~' allowed (which will apply to the entire alternatives list)");
            GetSymbol (new TerminalSet (terminalCount, NegateSym), true, true);
            isNegSearch = true;
          }
          //terminalTable [ELLIPSIS] = saveEllipsis;
          //terminalTable [NEGATE]   = saveNegate;
          //terminalTable [SUBTREE]  = saveSubtree;
          PrologTermEx (_TS.Union (terminalCount, CutSym, VBar), out t);
          //terminalTable [ELLIPSIS] = EllipsisSym;
          //terminalTable [NEGATE]   = NegateSym;
          //terminalTable [SUBTREE]  = SubtreeSym;
          if (!first) searchTerms.Add (new SearchTerm (downRepFactor, t));
          if (t is AnonymousVariable)
            IO.Warning ("Anonymous variable in alternatives list makes it match anything");
          GetSymbol (_TS.Union (terminalCount, CutSym, VBar), false, true);
          if (symbol.IsMemberOf (CutSym, VBar))
          {
            GetSymbol (new TerminalSet (terminalCount, CutSym, VBar), false, true);
            if (symbol.Terminal == VBar)
            {
              symbol.SetProcessed ();
              if (first) searchTerms.Add (new SearchTerm (downRepFactor, t));
            }
            else
            {
              symbol.SetProcessed ();
              if (first)
              {
                if (t is Variable)
                {
                  if (isNegSearch)
                    IO.Error ("'~' not allowed before alternatives list name");
                  else
                    altListName = t;
                }
                else
                  IO.Error ("Variable expected before !");
                first = false;
              }
              else
                IO.Error ("Only one ! allowed for alternatives list");
            }
          }
          else
            break;
        }
        if (first) searchTerms.Add (new SearchTerm (downRepFactor, t));
        isSingleVar = (searchTerms.Count == 1 && searchTerms [0].term is Variable);
        #if LL1_tracing
        ReportParserProcExit ("AlternativeTerms");
        #endif
      }
      #endregion
      
      #region Range
      void Range (TerminalSet _TS, out BaseTerm minLenTerm, out BaseTerm maxLenTerm)
      {
        #if LL1_tracing
        ReportParserProcEntry ("Range");
        #endif
        try
        {
          savePlusSym = terminalTable [PLUSSYM];
          saveTimesSym = terminalTable [TIMESSYM];
          saveQuestionMark = terminalTable [QUESTIONMARK];
          terminalTable [PLUSSYM] = PlusSym;
          terminalTable [TIMESSYM] = TimesSym;
          terminalTable [QUESTIONMARK] = QuestionMark;
        GetSymbol (new TerminalSet (terminalCount, LCuBracket, EllipsisSym), false, true);
        if (symbol.Terminal == LCuBracket)
        {
          int minLen = 0;
          int maxLen = 0;
          minLenTerm = maxLenTerm = DecimalTerm.ZERO;
          GetSymbol (new TerminalSet (terminalCount, LCuBracket), true, true);
          GetSymbol (_TS.Union (terminalCount, Comma, Identifier, IntLiteral, PlusSym, TimesSym, QuestionMark), false, true);
          if (symbol.IsMemberOf (Comma, Identifier, IntLiteral, RCuBracket))
          {
            GetSymbol (new TerminalSet (terminalCount, Comma, Identifier, IntLiteral, RCuBracket), false, true);
            if (symbol.IsMemberOf (Identifier, IntLiteral))
            {
              GetSymbol (new TerminalSet (terminalCount, Identifier, IntLiteral), false, true);
              if (symbol.Terminal == IntLiteral)
              {
                symbol.SetProcessed ();
                minLen = maxLen = symbol.ToInt ();
                minLenTerm = maxLenTerm = new DecimalTerm (minLen);
              }
              else
              {
                GetIdentifier (new TerminalSet (terminalCount, Comma, RCuBracket), out minLenTerm);
                maxLenTerm = minLenTerm;
              }
            }
            GetSymbol (new TerminalSet (terminalCount, Comma, RCuBracket), false, true);
            if (symbol.Terminal == Comma)
            {
              symbol.SetProcessed ();
              maxLen = Infinite;
              maxLenTerm = new DecimalTerm (Infinite);
              GetSymbol (new TerminalSet (terminalCount, Identifier, IntLiteral, RCuBracket), false, true);
              if (symbol.IsMemberOf (Identifier, IntLiteral))
              {
                GetSymbol (new TerminalSet (terminalCount, Identifier, IntLiteral), false, true);
                if (symbol.Terminal == IntLiteral)
                {
                  symbol.SetProcessed ();
                  if (minLen > (maxLen = symbol.ToInt ()))
                    IO.Error ("Range lower bound {0} not allowed to be greater than range upper bound {1}", minLen, maxLen);
                  maxLenTerm = new DecimalTerm (maxLen);
                }
                else
                {
                  GetIdentifier (new TerminalSet (terminalCount, RCuBracket), out maxLenTerm);
                }
              }
            }
          }
          else if (symbol.Terminal == TimesSym)
          {
            symbol.SetProcessed ();
            minLenTerm = new DecimalTerm (0);
            maxLenTerm = new DecimalTerm (Infinite);
          }
          else if (symbol.Terminal == PlusSym)
          {
            symbol.SetProcessed ();
            minLenTerm = new DecimalTerm (1);
            maxLenTerm = new DecimalTerm (Infinite);
          }
          else if (symbol.Terminal == QuestionMark)
          {
            symbol.SetProcessed ();
            minLenTerm = new DecimalTerm (0);
            maxLenTerm = new DecimalTerm (1);
          }
          GetSymbol (new TerminalSet (terminalCount, RCuBracket), true, true);
        }
        else
        {
          symbol.SetProcessed ();
          minLenTerm = new DecimalTerm (0);
          maxLenTerm = new DecimalTerm (Infinite);
        }
        }
        finally
        {
          terminalTable [PLUSSYM] = savePlusSym;
          terminalTable [TIMESSYM] = saveTimesSym;
          terminalTable [QUESTIONMARK] = saveQuestionMark;
        }
        #if LL1_tracing
        ReportParserProcExit ("Range");
        #endif
      }
      #endregion
      
      #region TryCatchClause
      void TryCatchClause (TerminalSet _TS, TokenSeqToTerm tokenSeqToTerm, out BaseTerm t)
      {
        #if LL1_tracing
        ReportParserProcEntry ("TryCatchClause");
        #endif
        GetSymbol (new TerminalSet (terminalCount, TrySym), true, true);
        bool nullClass = false;
        tokenSeqToTerm.Add (new TryOpenTerm ());
        tokenSeqToTerm.Add (CommaOpTriplet);
        GetSymbol (new TerminalSet (terminalCount, LeftParen), true, true);
        PrologTermEx (new TerminalSet (terminalCount, RightParen), out t);
        GetSymbol (new TerminalSet (terminalCount, RightParen), true, true);
        tokenSeqToTerm.Add (t);
        tokenSeqToTerm.Add (CommaOpTriplet);
        List<string> ecNames = new List<string> ();
        int catchSeqNo = 0;
        do
        {
          GetSymbol (new TerminalSet (terminalCount, CatchSym), true, true);
          if (nullClass)
            IO.Error ("No CATCH-clause allowed after CATCH-clause without exception class");
          string exceptionClass = null;
          BaseTerm msgVar = null;
          GetSymbol (new TerminalSet (terminalCount, LeftParen, Identifier, IntLiteral, Atom), false, true);
          if (symbol.IsMemberOf (Identifier, IntLiteral, Atom))
          {
            GetSymbol (new TerminalSet (terminalCount, Identifier, IntLiteral, Atom), false, true);
            if (symbol.IsMemberOf (IntLiteral, Atom))
            {
              bool saveStatus = SetCommaAsSeparator (true);
              GetSymbol (new TerminalSet (terminalCount, IntLiteral, Atom), false, true);
              if (symbol.Terminal == Atom)
              {
                symbol.SetProcessed ();
              }
              else
              {
                symbol.SetProcessed ();
              }
              if (ecNames.Contains (exceptionClass = symbol.ToString ()))
                IO.Error ("Duplicate exception class name '{0}'", exceptionClass);
              else
               ecNames.Add (exceptionClass);
              GetSymbol (new TerminalSet (terminalCount, Comma, LeftParen, Identifier), false, true);
              if (symbol.IsMemberOf (Comma, Identifier))
              {
                GetSymbol (new TerminalSet (terminalCount, Comma, Identifier), false, true);
                if (symbol.Terminal == Comma)
                {
                  symbol.SetProcessed ();
                }
                GetIdentifier (new TerminalSet (terminalCount, LeftParen), out msgVar);
              }
              SetCommaAsSeparator (saveStatus);
            }
            else
            {
              GetIdentifier (new TerminalSet (terminalCount, LeftParen), out msgVar);
            }
          }
          nullClass = nullClass || (exceptionClass == null);
          if (msgVar == null) msgVar = new AnonymousVariable ();
          tokenSeqToTerm.Add (new CatchOpenTerm (exceptionClass, msgVar, catchSeqNo++));
          tokenSeqToTerm.Add (CommaOpTriplet);
          t = null;
          GetSymbol (new TerminalSet (terminalCount, LeftParen), true, true);
          GetSymbol (new TerminalSet (terminalCount, LeftParen, RightParen, Identifier, IntLiteral, RealLiteral, ImagLiteral,
                                                     StringLiteral, Operator, Atom, Anonymous, CutSym, LSqBracket, LCuBracket,
                                                     ListPatternOpen, TrySym, WrapOpen, WrapClose, AltListOpen, AltListClose,
                                                     VerbatimStringLiteral), false, true);
          if (symbol.IsMemberOf (LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral, StringLiteral, Operator, Atom,
                                 Anonymous, CutSym, LSqBracket, LCuBracket, ListPatternOpen, TrySym, WrapOpen, WrapClose,
                                 AltListOpen, AltListClose, VerbatimStringLiteral))
          {
            PrologTermEx (new TerminalSet (terminalCount, RightParen), out t);
          }
          GetSymbol (new TerminalSet (terminalCount, RightParen), true, true);
          if (t != null)
          {
            tokenSeqToTerm.Add (t);
            tokenSeqToTerm.Add (CommaOpTriplet);
          }
          GetSymbol (_TS.Union (terminalCount, CatchSym), false, true);
        } while (! (_TS.Contains (symbol.Terminal)) );
        tokenSeqToTerm.Add (TC_CLOSE);
        #if LL1_tracing
        ReportParserProcExit ("TryCatchClause");
        #endif
      }
      #endregion
      
      #region OptionalPrologTerm
      void OptionalPrologTerm (TerminalSet _TS, out BaseTerm t)
      {
        #if LL1_tracing
        ReportParserProcEntry ("OptionalPrologTerm");
        #endif
        t = null;
        GetSymbol (_TS.Union (terminalCount, LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral, StringLiteral,
                                             Operator, Atom, Anonymous, CutSym, LSqBracket, LCuBracket, ListPatternOpen, TrySym,
                                             WrapOpen, WrapClose, AltListOpen, AltListClose, VerbatimStringLiteral), false,
                   true);
        if (symbol.IsMemberOf (LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral, StringLiteral, Operator, Atom,
                               Anonymous, CutSym, LSqBracket, LCuBracket, ListPatternOpen, TrySym, WrapOpen, WrapClose,
                               AltListOpen, AltListClose, VerbatimStringLiteral))
        {
          PrologTerm (new TerminalSet (terminalCount, Dot), out t);
          GetSymbol (new TerminalSet (terminalCount, Dot), true, true);
        }
        #if LL1_tracing
        ReportParserProcExit ("OptionalPrologTerm");
        #endif
      }
      #endregion
      
      #region List
      void List (TerminalSet _TS, out BaseTerm term)
      {
        #if LL1_tracing
        ReportParserProcEntry ("List");
        #endif
        BaseTerm afterBar = null;
        terminalTable [OP] = Atom;
        terminalTable [WRAP] = Atom;
        BaseTerm [] elements = null;
        GetSymbol (new TerminalSet (terminalCount, LSqBracket), true, true);
        GetSymbol (new TerminalSet (terminalCount, LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral, StringLiteral,
                                                   Operator, Atom, Anonymous, CutSym, LSqBracket, RSqBracket, LCuBracket,
                                                   ListPatternOpen, TrySym, WrapOpen, WrapClose, AltListOpen, AltListClose,
                                                   VerbatimStringLiteral), false, true);
        if (symbol.IsMemberOf (LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral, StringLiteral, Operator, Atom,
                               Anonymous, CutSym, LSqBracket, LCuBracket, ListPatternOpen, TrySym, WrapOpen, WrapClose,
                               AltListOpen, AltListClose, VerbatimStringLiteral))
        {
          ArgumentList (new TerminalSet (terminalCount, RSqBracket, VBar), out elements, true);
          GetSymbol (new TerminalSet (terminalCount, RSqBracket, VBar), false, true);
          if (symbol.Terminal == VBar)
          {
            symbol.SetProcessed ();
            PrologTerm (new TerminalSet (terminalCount, RSqBracket), out afterBar);
          }
        }
        terminalTable [OP] = OpSym;
        terminalTable [WRAP] = WrapSym;
        GetSymbol (new TerminalSet (terminalCount, RSqBracket), true, true);
        term = (afterBar == null) ? new ListTerm () : afterBar;
        if (elements != null) term = ListTerm.ListFromArray (elements, term);
        #if LL1_tracing
        ReportParserProcExit ("List");
        #endif
      }
      #endregion
      
      #region AltList
      void AltList (TerminalSet _TS, out BaseTerm term)
      {
        #if LL1_tracing
        ReportParserProcEntry ("AltList");
        #endif
        BaseTerm afterBar = null;
        terminalTable [OP] = Atom;
        terminalTable [WRAP] = Atom;
        BaseTerm [] elements = null;
        GetSymbol (new TerminalSet (terminalCount, AltListOpen), true, true);
        string altListOpen = symbol.ToString ();
        string altListClose = engine.AltListTable.FindCloseBracket (altListOpen);
        GetSymbol (new TerminalSet (terminalCount, LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral, StringLiteral,
                                                   Operator, Atom, Anonymous, CutSym, LSqBracket, LCuBracket, ListPatternOpen,
                                                   TrySym, WrapOpen, WrapClose, AltListOpen, AltListClose,
                                                   VerbatimStringLiteral), false, true);
        if (symbol.IsMemberOf (LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral, StringLiteral, Operator, Atom,
                               Anonymous, CutSym, LSqBracket, LCuBracket, ListPatternOpen, TrySym, WrapOpen, WrapClose,
                               AltListOpen, AltListClose, VerbatimStringLiteral))
        {
          ArgumentList (new TerminalSet (terminalCount, VBar, AltListClose), out elements, true);
          GetSymbol (new TerminalSet (terminalCount, VBar, AltListClose), false, true);
          if (symbol.Terminal == VBar)
          {
            symbol.SetProcessed ();
            PrologTerm (new TerminalSet (terminalCount, AltListClose), out afterBar);
          }
        }
        terminalTable [OP] = OpSym;
        terminalTable [WRAP] = WrapSym;
        GetSymbol (new TerminalSet (terminalCount, AltListClose), true, true);
        if (symbol.ToString () != altListClose)
          IO.Error ("Illegal alternative list close token: got '{0}' expected '{1}'",
                     symbol.ToString (), altListClose);
        term = (afterBar == null) ? new AltListTerm (altListOpen, altListClose) : afterBar;
        if (elements != null)
          term = AltListTerm.ListFromArray (altListOpen, altListClose, elements, term);
        #if LL1_tracing
        ReportParserProcExit ("AltList");
        #endif
      }
      #endregion
      
      #region DCGBracketList
      void DCGBracketList (TerminalSet _TS, out BaseTerm term)
      {
        #if LL1_tracing
        ReportParserProcEntry ("DCGBracketList");
        #endif
        terminalTable [OP] = Atom;
        terminalTable [WRAP] = Atom;
        BaseTerm [] elements = null;
        GetSymbol (new TerminalSet (terminalCount, LCuBracket), true, true);
        GetSymbol (new TerminalSet (terminalCount, LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral, StringLiteral,
                                                   Operator, Atom, Anonymous, CutSym, LSqBracket, LCuBracket, RCuBracket,
                                                   ListPatternOpen, TrySym, WrapOpen, WrapClose, AltListOpen, AltListClose,
                                                   VerbatimStringLiteral), false, true);
        if (symbol.IsMemberOf (LeftParen, Identifier, IntLiteral, RealLiteral, ImagLiteral, StringLiteral, Operator, Atom,
                               Anonymous, CutSym, LSqBracket, LCuBracket, ListPatternOpen, TrySym, WrapOpen, WrapClose,
                               AltListOpen, AltListClose, VerbatimStringLiteral))
        {
          ArgumentList (new TerminalSet (terminalCount, RCuBracket), out elements, true);
        }
        GetSymbol (new TerminalSet (terminalCount, RCuBracket), true, true);
        term = BaseTerm.NULLCURL;
        if (elements != null)
          if (readingDcgClause)
            for (int i = elements.Length-1; i >= 0; i--)
              term = new DcgTerm (elements [i], term);
          else
            term = new CompoundTerm (CURL, elements);
        #if LL1_tracing
        ReportParserProcExit ("DCGBracketList");
        #endif
      }
      #endregion
      
      
      #endregion PARSER PROCEDURES
    }
    #endregion PrologParser
  }
}
