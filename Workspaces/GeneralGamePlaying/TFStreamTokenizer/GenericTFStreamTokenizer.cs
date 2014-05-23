using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace TFStreamTokenizer
{
    public enum KIFTokenType
    {
        None = 0,
        EndOfFile = 1,

        Error,
        Letter,
        Digit,
        WhiteSpace,
        Comment,
        DoubleQuote,
        Sign,
        Exp,
        Dot,
        NonDigitNonWhiteSpace,

        NonWhiteSpace,
        NonEndOfLine,
        EndOfLine,
        Word, // = InitialChar WordTail, = InitialChar, WordTail = WordChar WordTail, WordTail = WordChar
        Number,
        Alphabetic,
        Comma,
        Upper,
        Lower,

        InitialChar,
        WordChar,
        Character,
        Special,
        LeftParen,
        RightParen,
        QuotedString, // = " Characters ", = " "
        SingleQuote,
        String,

        NonParenNonWhiteSpace,
        ImplicationToRight,
        ImplicationToLeft,
        Equivalence,
        Equal,

        GreaterThan,
        LessThan,
        Variable,
        QuestionMark,
        Ampersand,

        Any = 64
    }

    public class TestToken
    {
        public enum TokenType 
        {
            foo,
            goo
        } ;

        static public TokenType TokenType_None = (TokenType)Enum.GetValues(typeof(TokenType)).GetValue(0);
        static public TokenType TokenType_EndOfFile = (TokenType)Enum.GetValues(typeof(TokenType)).GetValue(1);

        public TokenType TheTokenType { get; set; }
        public string Lexeme { get; set; }
        public double? NVal { get; set; }

        public TestToken()
        {
            TheTokenType = TokenType_None;
            Lexeme = String.Empty;
            NVal = null;
        }

        public override string ToString()
        {
            return TheTokenType + " (\"" + Lexeme + "\")";
        }
    }

    public class Foo<TToken> where TToken : TestToken
    {
        public TToken AToken;
    }

    public class KIFFoo : Foo<TestToken>
    {
        
    }

    abstract public class GenericTFStreamTokenizer<TStateType, TTokenType>
        // C# 4.0 still doesn't support generic constraints of type Enum so 
        // using these three interfaces implemented by enums and struct are as close as I can get right now
        where TStateType : struct, IComparable, IFormattable, IConvertible
        where TTokenType : struct, IComparable, IFormattable, IConvertible
    {
        public delegate Token StateAccept();

        public class Token
        {
            static public TTokenType TokenType_None = TokenType_None = (TTokenType)Enum.GetValues(typeof(TTokenType)).GetValue(0);
            static public TTokenType TokenType_EndOfFile = TokenType_None = (TTokenType)Enum.GetValues(typeof(TTokenType)).GetValue(1);

            public TTokenType TheTokenType { get; set; }
            public string Lexeme { get; set; }
            public double? NVal { get; set; }

            public Token()
            {
                TheTokenType = TokenType_None;
                Lexeme = String.Empty;
                NVal = null;
            }

            public override string ToString()
            {
                return TheTokenType + " (\"" + Lexeme + "\")";
            }
        }

        public class CharacterTableEntry
        {
            public char TheChar;
            public BitArray TokenTypes = new BitArray(TokenCount + 1);  // does this need to be + 1?

            public CharacterTableEntry(char theChar)
            {
                TheChar = theChar;
            }
        }

        public class StateTableEntry
        {
            public TStateType StateType { get; set; }
            public StateAccept TheStateAccept { get; set; }

            public StateTableEntry(TStateType stateType, StateAccept stateAccept = null)
            {
                StateType = stateType;
                TheStateAccept = stateAccept;
            }
        }

        public class StateTransitionTableEntry
        {
            public TStateType CurrentStateType { get; set; }
            public TTokenType InputTokenType { get; set; }
            public TStateType NextStateType { get; set; }

            public StateTransitionTableEntry(TStateType stateType, TTokenType inputTokenType, TStateType nextStateType)
            {
                CurrentStateType = stateType;
                InputTokenType = inputTokenType;
                NextStateType = nextStateType;
            }
        }

        public string Lexeme { get; set; }
        public double? NVal { get; set; }
        public bool EOLIsSignificant { get; set; }

        protected StreamReader _sr;
        protected int _peekedInputChar;
        protected CharacterTableEntry _peekedCharTableEntry;
        protected readonly StringBuilder _workingTokenBuffer = new StringBuilder();
        //protected int _currInputPos;  // position of input character to process
        //protected int _tokenStartPos; // first position of this next token we're parsing
        protected TStateType _currStateType;
        protected StateAccept _mostRecentStateAccept;
        protected int? _mostRecentStateAcceptPos;
        protected CharacterTableEntry[] _characterTable;
        protected List<StateTransitionTableEntry> _stateTransitionTable;
        protected List<StateTableEntry> _stateTable;

        static protected TTokenType TokenType_None;
        static protected TTokenType TokenType_EndOfFile;
        static protected TStateType StateType_Start;
        static protected int TokenCount;

        public GenericTFStreamTokenizer(StreamReader sr, TTokenType tokenType_None, TTokenType tokenType_EndOfFile, TStateType stateType_Start, int tokenCount)
        {
            //TokenType_None = (TTokenType)Enum.GetValues(typeof(TTokenType)).GetValue(0);
            TokenType_None = tokenType_None;
            TokenType_EndOfFile = tokenType_EndOfFile;
            StateType_Start = stateType_Start;
            TokenCount = tokenCount;

            _sr = sr;
            //_currInputPos = 0;
            //_tokenStartPos = 0;
            EOLIsSignificant = false;
            _currStateType = StateType_Start;
            //_mostRecentStateAccept = null;
            //_mostRecentStateAcceptPos = null;

            InitializeCharacterTable();
            InitializeKIFStateTable();
            InitializeKIFStateTransitionTable();
        }

        abstract protected void InitializeCharacterTable();
        abstract protected void InitializeKIFStateTable();
        abstract protected void InitializeKIFStateTransitionTable();

        public Token NextToken()
        {
            while (!_sr.EndOfStream)
            {
                _peekedInputChar = _sr.Peek();
                _peekedCharTableEntry = _characterTable.Where(n => n.TheChar == _peekedInputChar).First();

                var query =
                    _stateTransitionTable.Where(
//                        n => EqualityComparer<TStateType>.Default.Equals(n.CurrentStateType, _currStateType));
                        n => n.CurrentStateType.Equals(_currStateType));
                bool foundATransition = false;
                if (query.Any())
                {

                    foreach (StateTransitionTableEntry stateTransitionTableEntry in query)
                    {
                        // we have to do the (int)(dynamic) here because we can't make the constraints enums so 
                        // the compiler doesn't know this can be converted to an int in the normal way
//                      if (_peekedCharTableEntry.TokenTypes[(int)(dynamic)stateTransitionTableEntry.InputTokenType])
                        if (_peekedCharTableEntry.TokenTypes[stateTransitionTableEntry.InputTokenType.ToInt32(CultureInfo.CurrentCulture)])
                        {
                            foundATransition = true;

                            _currStateType = stateTransitionTableEntry.NextStateType;
//                            var currStateTableEntry = _stateTable.Where(n => EqualityComparer<TStateType>.Default.Equals(n.StateType, _currStateType)).First();
                            var currStateTableEntry = _stateTable.Where(n => n.StateType.Equals(_currStateType)).First();
                            _workingTokenBuffer.Append((char)_sr.Read());


                            if (currStateTableEntry.TheStateAccept != null)
                            {
                                _mostRecentStateAccept = currStateTableEntry.TheStateAccept;
                                _mostRecentStateAcceptPos = _workingTokenBuffer.Length;
                            }

                            break;
                        }
                    }
                }

                if (!foundATransition)
                {
                    if (_mostRecentStateAccept == null || _mostRecentStateAcceptPos == null)
                    {
                        // Discard the current lexeme and input character and reset to start state.
                        // Unlike what "Compiler Design In C' says, this is not necessarily an error.
                        // For instance, this can is used to swallow white space in the Start state.
                        _workingTokenBuffer.Clear();
                        _sr.Read();
                        _currStateType = StateType_Start;
                    }
                    else
                    {
                        _workingTokenBuffer.Length = _mostRecentStateAcceptPos.Value;
                        return _mostRecentStateAccept();
                    }
                    //return new Token() { TheTokenType = TokenType.Error, Lexeme = Lexeme };
                }

                //if (_returnTokenType != null)
                //{
                //    return new Token() { TheTokenType = _returnTokenType.Value, Lexeme = Lexeme, NVal = _returnTokenType.Value == TokenType.Number ? NVal : null };
                //}
            }

            if (_mostRecentStateAccept == null || _mostRecentStateAcceptPos == null)
            {
                return new Token() { TheTokenType = TokenType_EndOfFile, Lexeme = String.Empty };
            }
            else
            {
                _workingTokenBuffer.Length = _mostRecentStateAcceptPos.Value;
                var retToken = _mostRecentStateAccept();
                _mostRecentStateAccept = null;
                return retToken;
            }
        }
    }


    public class TFKIFStreamTokenizer : GenericTFStreamTokenizer<TFKIFStreamTokenizer.KIFStateType, TFKIFStreamTokenizer.KIFTokenType>
    {
        public enum KIFStateType
        {
            Start,
            Digit1,
            Digit2,
            Digit3,
            DigitN,
            DigitComma1,
            DigitComma2,
            DigitComma3,
            DigitComma4,
            Float,
            Sign,
            Exp,
            ExpSign,
            ExpDigit,
            Word,
            String,
            QuotedStringStart,
            QuotedStringEnd,
            LeftParen,
            RightParen,
            Equal,
            EqualOrGreaterThan, // =>
            LessThan,
            LessThanOrEqual,    // <=
            Equivalence,        // <=>
            QuestionMark,
            Ampersand,
            Variable,
            Comment,
            Error            
        }

        public enum KIFTokenType
        {
            Error,
            Letter,
            Digit,
            WhiteSpace,
            Comment,
            DoubleQuote,
            Sign,
            Exp,
            Dot,
            NonDigitNonWhiteSpace,

            NonWhiteSpace,
            NonEndOfLine,
            EndOfLine,
            EndOfFile,
            Word, // = InitialChar WordTail, = InitialChar, WordTail = WordChar WordTail, WordTail = WordChar
            Number,
            Alphabetic,
            Comma,
            Upper,
            Lower,

            InitialChar,
            WordChar,
            Character,
            Special,
            LeftParen,
            RightParen,
            QuotedString, // = " Characters ", = " "
            SingleQuote,
            String,
            None,

            NonParenNonWhiteSpace,
            ImplicationToRight,
            ImplicationToLeft,
            Equivalence,
            Equal,
            GreaterThan,
            LessThan,
            Variable,
            QuestionMark,
            Ampersand,

            Any = 64
        }

        public TFKIFStreamTokenizer(StreamReader sr) : base(sr, KIFTokenType.None, KIFTokenType.EndOfFile, KIFStateType.Start, Enum.GetValues(typeof(KIFTokenType)).Length)
        {
        }

        protected sealed override void InitializeCharacterTable()
        {
            _characterTable = new CharacterTableEntry[256];

            for (int i = 0; i < 256; i++)
            {
                var entry = new CharacterTableEntry((char)i);

                entry.TokenTypes[(int)KIFTokenType.Any] = true;
                entry.TokenTypes[(int)KIFTokenType.NonWhiteSpace] = true;
                entry.TokenTypes[(int)KIFTokenType.NonDigitNonWhiteSpace] = true;
                entry.TokenTypes[(int)KIFTokenType.NonParenNonWhiteSpace] = true;
                entry.TokenTypes[(int)KIFTokenType.NonEndOfLine] = true;

                if ((0x00 <= i && i <= 0x20) || i == 0x7F)
                {
                    entry.TokenTypes[(int)KIFTokenType.NonWhiteSpace] = false;
                    entry.TokenTypes[(int)KIFTokenType.NonDigitNonWhiteSpace] = false;
                    entry.TokenTypes[(int)KIFTokenType.NonParenNonWhiteSpace] = false;

                    entry.TokenTypes[(int)KIFTokenType.WhiteSpace] = true;
                    if (i == 0x0A)
                    {
                        entry.TokenTypes[(int)KIFTokenType.EndOfLine] = true;
                        entry.TokenTypes[(int)KIFTokenType.NonEndOfLine] = false;
                    }
                }

                if ((0x41 <= i && i <= 0x5A) || (0x61 <= i && i <= 0x7A))     // A-Z, a-z
                {
                    entry.TokenTypes[(int)KIFTokenType.Letter] = true;
                    entry.TokenTypes[(int)KIFTokenType.Alphabetic] = true;

                    if (0x41 <= i && i <= 0x5A)
                    {
                        entry.TokenTypes[(int)KIFTokenType.Upper] = true;
                    }
                    else
                    {
                        entry.TokenTypes[(int)KIFTokenType.Lower] = true;
                    }
                }

                if (0x30 <= i && i <= 0x39)  // 0-9
                {
                    entry.TokenTypes[(int)KIFTokenType.NonDigitNonWhiteSpace] = false;
                    entry.TokenTypes[(int)KIFTokenType.Digit] = true;
                }

                if (i == '!' || i == '$' || i == '%' || i == '&' || i == '*' || i == '+' || i == '-' || i == '.' || i == '/' || i == '<' || i == '=' || i == '>' || i == '?' || i == '@' || i == '_' || i == '~')
                {
                    entry.TokenTypes[(int)KIFTokenType.Special] = true;
                }

                if (i == 0x27) // single quote
                {
                    entry.TokenTypes[(int)KIFTokenType.SingleQuote] = true;
                }

                if (i == 0x22) // double quote
                {
                    entry.TokenTypes[(int)KIFTokenType.DoubleQuote] = true;
                }

                if (i == '?')
                {
                    entry.TokenTypes[(int)KIFTokenType.QuestionMark] = true;
                }

                if (i == '@')
                {
                    entry.TokenTypes[(int)KIFTokenType.Ampersand] = true;
                }

                if (i == '.')
                {
                    entry.TokenTypes[(int)KIFTokenType.Dot] = true;
                }

                if (i == ',')
                {
                    entry.TokenTypes[(int)KIFTokenType.Comma] = true;
                }

                if (i == 'e' || i == 'E')
                {
                    entry.TokenTypes[(int)KIFTokenType.Exp] = true;
                }

                //if (i == '/' || i == '#') // are these used in GDL too?
                if (i == ';')
                {
                    entry.TokenTypes[(int)KIFTokenType.Comment] = true;
                }

                if (i == '+' || i == '-')
                {
                    entry.TokenTypes[(int)KIFTokenType.Sign] = true;
                }

                if (i == '(')
                {
                    entry.TokenTypes[(int)KIFTokenType.NonParenNonWhiteSpace] = false;

                    entry.TokenTypes[(int)KIFTokenType.LeftParen] = true;
                }

                if (i == ')')
                {
                    entry.TokenTypes[(int)KIFTokenType.NonParenNonWhiteSpace] = false;

                    entry.TokenTypes[(int)KIFTokenType.RightParen] = true;
                }

                if (i == '=')
                {
                    entry.TokenTypes[(int)KIFTokenType.NonParenNonWhiteSpace] = false;

                    entry.TokenTypes[(int)KIFTokenType.Equal] = true;
                }

                if (i == '<')
                {
                    entry.TokenTypes[(int)KIFTokenType.NonParenNonWhiteSpace] = false;

                    entry.TokenTypes[(int)KIFTokenType.LessThan] = true;
                }

                if (i == '>')
                {
                    entry.TokenTypes[(int)KIFTokenType.NonParenNonWhiteSpace] = false;

                    entry.TokenTypes[(int)KIFTokenType.GreaterThan] = true;
                }

                if (entry.TokenTypes[(int)KIFTokenType.Upper] ||
                    entry.TokenTypes[(int)KIFTokenType.Lower]
                    )
                {
                    entry.TokenTypes[(int)KIFTokenType.InitialChar] = true;
                }


                if (entry.TokenTypes[(int)KIFTokenType.Upper] ||
                    entry.TokenTypes[(int)KIFTokenType.Lower] ||
                    entry.TokenTypes[(int)KIFTokenType.Digit] ||
                    entry.TokenTypes[(int)KIFTokenType.Special]
                    )
                {
                    entry.TokenTypes[(int)KIFTokenType.WordChar] = true;
                }

                if (entry.TokenTypes[(int)KIFTokenType.Upper] ||
                    entry.TokenTypes[(int)KIFTokenType.Lower] ||
                    entry.TokenTypes[(int)KIFTokenType.Digit] ||
                    entry.TokenTypes[(int)KIFTokenType.Special] ||
                    entry.TokenTypes[(int)KIFTokenType.WhiteSpace]
                    )
                {
                    entry.TokenTypes[(int)KIFTokenType.Character] = true;
                }


                _characterTable[i] = entry;
            }
        }

        protected sealed override void InitializeKIFStateTable()
        {
            _stateTable = new List<StateTableEntry>()
                            {
                                new StateTableEntry(KIFStateType.Start),
                                new StateTableEntry(KIFStateType.Digit1, AcceptNumber),
                                new StateTableEntry(KIFStateType.Digit2, AcceptNumber),
                                new StateTableEntry(KIFStateType.Digit3, AcceptNumber),
                                new StateTableEntry(KIFStateType.DigitN, AcceptNumber),
                                new StateTableEntry(KIFStateType.DigitComma1, AcceptString),
                                new StateTableEntry(KIFStateType.DigitComma2, AcceptString),
                                new StateTableEntry(KIFStateType.DigitComma3, AcceptString),
                                new StateTableEntry(KIFStateType.DigitComma4, AcceptNumber),
                                new StateTableEntry(KIFStateType.Float, AcceptNumber),
                                new StateTableEntry(KIFStateType.Sign, AcceptString),
                                new StateTableEntry(KIFStateType.Exp, AcceptString),
                                new StateTableEntry(KIFStateType.ExpSign, AcceptString),
                                new StateTableEntry(KIFStateType.ExpDigit, AcceptNumber),
                                new StateTableEntry(KIFStateType.Word, AcceptWord),
                                new StateTableEntry(KIFStateType.String, AcceptString),
                                new StateTableEntry(KIFStateType.LeftParen, AcceptLeftParen),
                                new StateTableEntry(KIFStateType.RightParen, AcceptRightParen),
                                new StateTableEntry(KIFStateType.QuotedStringStart, AcceptString),
                                new StateTableEntry(KIFStateType.QuotedStringEnd, AcceptQuotedString),
                                new StateTableEntry(KIFStateType.Equal, AcceptEqual),
                                new StateTableEntry(KIFStateType.LessThan),
                                new StateTableEntry(KIFStateType.LessThanOrEqual, AcceptImplicationToLeft),
                                new StateTableEntry(KIFStateType.EqualOrGreaterThan, AcceptImplicationToRight),
                                new StateTableEntry(KIFStateType.Equivalence, AcceptEquivalence),
                                new StateTableEntry(KIFStateType.QuestionMark, AcceptQuestionMark),
                                new StateTableEntry(KIFStateType.Ampersand, AcceptAmpersand),
                                new StateTableEntry(KIFStateType.Variable, AcceptVariable),
                                new StateTableEntry(KIFStateType.Comment),
                            };
        }

        protected sealed override void InitializeKIFStateTransitionTable()
        {
            _stateTransitionTable = new List<StateTransitionTableEntry>()
                                    {
                                        // Done
                                        /*************************************** Start ************************************************/
                                        new StateTransitionTableEntry(KIFStateType.Start, KIFTokenType.InitialChar,
                                                                        KIFStateType.Word),
                                        new StateTransitionTableEntry(KIFStateType.Start, KIFTokenType.LeftParen, 
                                                                        KIFStateType.LeftParen),
                                        new StateTransitionTableEntry(KIFStateType.Start, KIFTokenType.RightParen, 
                                                                        KIFStateType.RightParen),
                                        new StateTransitionTableEntry(KIFStateType.Start, KIFTokenType.LessThan, 
                                                                        KIFStateType.LessThan),
                                        new StateTransitionTableEntry(KIFStateType.Start, KIFTokenType.Equal, 
                                                                        KIFStateType.Equal),
                                        new StateTransitionTableEntry(KIFStateType.Start, KIFTokenType.QuestionMark, 
                                                                        KIFStateType.QuestionMark),
                                        new StateTransitionTableEntry(KIFStateType.Start, KIFTokenType.Ampersand, 
                                                                        KIFStateType.Ampersand),
                                        new StateTransitionTableEntry(KIFStateType.Start, KIFTokenType.Digit,
                                                                        KIFStateType.Digit1),
                                        new StateTransitionTableEntry(KIFStateType.Start, KIFTokenType.Dot,
                                                                        KIFStateType.Float),
                                        new StateTransitionTableEntry(KIFStateType.Start, KIFTokenType.Sign,
                                                                        KIFStateType.Sign),
                                        new StateTransitionTableEntry(KIFStateType.Start, KIFTokenType.DoubleQuote,
                                                                        KIFStateType.QuotedStringStart),
                                        new StateTransitionTableEntry(KIFStateType.Start, KIFTokenType.Comment,
                                                                        KIFStateType.Comment),
                                        new StateTransitionTableEntry(KIFStateType.Start, KIFTokenType.NonWhiteSpace, 
                                                                        KIFStateType.String),

                                        // Done
                                        /*************************************** LeftParen ************************************************/
                                        new StateTransitionTableEntry(KIFStateType.LeftParen, KIFTokenType.Comment,
                                                                        KIFStateType.Comment),


                                        // Done
                                        /*************************************** RightParen ************************************************/
                                        new StateTransitionTableEntry(KIFStateType.RightParen, KIFTokenType.Comment,
                                                                        KIFStateType.Comment),


                                        /*************************************** Equal ************************************************/
                                        new StateTransitionTableEntry(KIFStateType.Equal, KIFTokenType.GreaterThan, 
                                                                        KIFStateType.EqualOrGreaterThan),
                                        new StateTransitionTableEntry(KIFStateType.Equal, KIFTokenType.Comment,
                                                                        KIFStateType.Comment),


                                        /*************************************** LessThan ************************************************/
                                        new StateTransitionTableEntry(KIFStateType.LessThan, KIFTokenType.Equal, 
                                                                        KIFStateType.LessThanOrEqual),

                                        /*************************************** LessThanOrEqual ************************************************/
                                        new StateTransitionTableEntry(KIFStateType.LessThanOrEqual, KIFTokenType.GreaterThan, 
                                                                        KIFStateType.Equivalence),
                                        new StateTransitionTableEntry(KIFStateType.LessThanOrEqual, KIFTokenType.Comment,
                                                                        KIFStateType.Comment),


                                        /*************************************** EqualOrGreaterThan ************************************************/
                                        new StateTransitionTableEntry(KIFStateType.EqualOrGreaterThan, KIFTokenType.Comment,
                                                                        KIFStateType.Comment),


                                        /*************************************** LessThanOrEqual ************************************************/
                                        new StateTransitionTableEntry(KIFStateType.LessThanOrEqual, KIFTokenType.Comment,
                                                                        KIFStateType.Comment),


                                        /*************************************** Equivalence ************************************************/
                                        new StateTransitionTableEntry(KIFStateType.Equivalence, KIFTokenType.Comment,
                                                                        KIFStateType.Comment),


                                        /*************************************** QuestionMark ************************************************/
                                        new StateTransitionTableEntry(KIFStateType.QuestionMark, KIFTokenType.InitialChar, 
                                                                        KIFStateType.Variable),
                                        new StateTransitionTableEntry(KIFStateType.QuestionMark, KIFTokenType.Comment,
                                                                        KIFStateType.Comment),


                                        /*************************************** Ampersand ************************************************/
                                        new StateTransitionTableEntry(KIFStateType.Ampersand, KIFTokenType.InitialChar, 
                                                                        KIFStateType.Variable),
                                        new StateTransitionTableEntry(KIFStateType.Ampersand, KIFTokenType.Comment,
                                                                        KIFStateType.Comment),


                                        /*************************************** Variable ************************************************/
                                        new StateTransitionTableEntry(KIFStateType.Variable, KIFTokenType.WordChar, 
                                                                        KIFStateType.Variable),
                                        new StateTransitionTableEntry(KIFStateType.Variable, KIFTokenType.Comment,
                                                                        KIFStateType.Comment),

                                        // Done
                                        /*************************************** QuotedStringStart ************************************************/
                                        new StateTransitionTableEntry(KIFStateType.QuotedStringStart, KIFTokenType.Character, 
                                                                        KIFStateType.QuotedStringStart),
                                        new StateTransitionTableEntry(KIFStateType.QuotedStringStart, KIFTokenType.DoubleQuote, 
                                                                        KIFStateType.QuotedStringEnd),
                                        new StateTransitionTableEntry(KIFStateType.QuotedStringStart, KIFTokenType.NonParenNonWhiteSpace, 
                                                                        KIFStateType.String),

                                        // Done
                                        /*************************************** QuotedStringEnd ************************************************/
                                        new StateTransitionTableEntry(KIFStateType.QuotedStringEnd, KIFTokenType.Comment,
                                                                        KIFStateType.Comment),


                                        // Done
                                        /*************************************** String ************************************************/
                                        new StateTransitionTableEntry(KIFStateType.String, KIFTokenType.Comment,
                                                                        KIFStateType.Comment),
                                        new StateTransitionTableEntry(KIFStateType.String, KIFTokenType.NonParenNonWhiteSpace, 
                                                                        KIFStateType.String),

                                        // Done
                                        /*************************************** Sign ************************************************/
                                        new StateTransitionTableEntry(KIFStateType.Sign, KIFTokenType.Digit,
                                                                        KIFStateType.Digit1),
                                        new StateTransitionTableEntry(KIFStateType.Sign, KIFTokenType.Dot, 
                                                                        KIFStateType.Float),
                                        new StateTransitionTableEntry(KIFStateType.Sign, KIFTokenType.Comment,
                                                                        KIFStateType.Comment),
                                        new StateTransitionTableEntry(KIFStateType.Sign, KIFTokenType.NonParenNonWhiteSpace, 
                                                                        KIFStateType.String),

                                        // Done
                                        /*************************************** Word ************************************************/
                                        new StateTransitionTableEntry(KIFStateType.Word, KIFTokenType.WordChar, 
                                                                        KIFStateType.Word),
                                        new StateTransitionTableEntry(KIFStateType.Word, KIFTokenType.Comment,
                                                                        KIFStateType.Comment),
                                        new StateTransitionTableEntry(KIFStateType.Word, KIFTokenType.NonParenNonWhiteSpace, 
                                                                        KIFStateType.String),

                                        // Done
                                        /*************************************** Digit1 ************************************************/
                                        new StateTransitionTableEntry(KIFStateType.Digit1, KIFTokenType.Digit,
                                                                        KIFStateType.Digit2),
                                        new StateTransitionTableEntry(KIFStateType.Digit1, KIFTokenType.Dot,
                                                                        KIFStateType.Float),
                                        new StateTransitionTableEntry(KIFStateType.Digit1, KIFTokenType.Comma,
                                                                        KIFStateType.DigitComma1),
                                        new StateTransitionTableEntry(KIFStateType.Digit1, KIFTokenType.Comment,
                                                                        KIFStateType.Comment),
                                        new StateTransitionTableEntry(KIFStateType.Digit1, KIFTokenType.NonParenNonWhiteSpace,
                                                                        KIFStateType.String),

                                        // Done
                                        /*************************************** Digit2 ************************************************/
                                        new StateTransitionTableEntry(KIFStateType.Digit2, KIFTokenType.Digit,
                                                                        KIFStateType.Digit3),
                                        new StateTransitionTableEntry(KIFStateType.Digit2, KIFTokenType.Dot,
                                                                        KIFStateType.Float),
                                        new StateTransitionTableEntry(KIFStateType.Digit2, KIFTokenType.Comma,
                                                                        KIFStateType.DigitComma1),
                                        new StateTransitionTableEntry(KIFStateType.Digit2, KIFTokenType.Comment,
                                                                        KIFStateType.Comment),
                                        new StateTransitionTableEntry(KIFStateType.Digit2, KIFTokenType.NonParenNonWhiteSpace,
                                                                        KIFStateType.String),

                                        // Done
                                        /*************************************** Digit3 ************************************************/
                                        new StateTransitionTableEntry(KIFStateType.Digit3, KIFTokenType.Digit,
                                                                        KIFStateType.DigitN),
                                        new StateTransitionTableEntry(KIFStateType.Digit3, KIFTokenType.Dot,
                                                                        KIFStateType.Float),
                                        new StateTransitionTableEntry(KIFStateType.Digit3, KIFTokenType.Comma,
                                                                        KIFStateType.DigitComma1),
                                        new StateTransitionTableEntry(KIFStateType.Digit3, KIFTokenType.Comment,
                                                                        KIFStateType.Comment),
                                        new StateTransitionTableEntry(KIFStateType.Digit3, KIFTokenType.NonParenNonWhiteSpace,
                                                                        KIFStateType.String),

                                        // ToDo:  Modify this if this is acceptable:  integer exp
                                        // Done
                                        /*************************************** DigitN ************************************************/
                                        new StateTransitionTableEntry(KIFStateType.DigitN, KIFTokenType.Digit,
                                                                        KIFStateType.DigitN),
                                        new StateTransitionTableEntry(KIFStateType.DigitN, KIFTokenType.Dot,
                                                                        KIFStateType.Float),
                                        new StateTransitionTableEntry(KIFStateType.DigitN, KIFTokenType.Comment,
                                                                        KIFStateType.Comment),
                                        new StateTransitionTableEntry(KIFStateType.Digit3, KIFTokenType.NonParenNonWhiteSpace,
                                                                        KIFStateType.String),
                                        // Done
                                        /*************************************** DigitComma1 ************************************************/
                                        new StateTransitionTableEntry(KIFStateType.DigitComma1, KIFTokenType.Digit,
                                                                        KIFStateType.DigitComma2),
                                        new StateTransitionTableEntry(KIFStateType.DigitComma1, KIFTokenType.Comment,
                                                                        KIFStateType.Comment),
                                        new StateTransitionTableEntry(KIFStateType.DigitComma1, KIFTokenType.NonParenNonWhiteSpace,
                                                                        KIFStateType.String),

                                        // Done
                                        /*************************************** DigitComma2 ************************************************/
                                        new StateTransitionTableEntry(KIFStateType.DigitComma2, KIFTokenType.Digit,
                                                                        KIFStateType.DigitComma3),
                                        new StateTransitionTableEntry(KIFStateType.DigitComma2, KIFTokenType.Comment,
                                                                        KIFStateType.Comment),
                                        new StateTransitionTableEntry(KIFStateType.DigitComma2, KIFTokenType.NonParenNonWhiteSpace,
                                                                        KIFStateType.String),

                                        // Done
                                        /*************************************** DigitComma3 ************************************************/
                                        new StateTransitionTableEntry(KIFStateType.DigitComma3, KIFTokenType.Digit,
                                                                        KIFStateType.DigitComma4),
                                        new StateTransitionTableEntry(KIFStateType.DigitComma3, KIFTokenType.Comment,
                                                                        KIFStateType.Comment),
                                        new StateTransitionTableEntry(KIFStateType.DigitComma3, KIFTokenType.NonParenNonWhiteSpace,
                                                                        KIFStateType.String),

                                        // Done
                                        /*************************************** DigitComma4 ************************************************/
                                        new StateTransitionTableEntry(KIFStateType.DigitComma4, KIFTokenType.Dot,
                                                                        KIFStateType.Float),
                                        new StateTransitionTableEntry(KIFStateType.DigitComma4, KIFTokenType.Comma,
                                                                        KIFStateType.DigitComma1),
                                        new StateTransitionTableEntry(KIFStateType.DigitComma4, KIFTokenType.Comment,
                                                                        KIFStateType.Comment),
                                        new StateTransitionTableEntry(KIFStateType.DigitComma4, KIFTokenType.NonParenNonWhiteSpace,
                                                                        KIFStateType.String),

                                        // Done
                                        /*************************************** Float ************************************************/
                                        new StateTransitionTableEntry(KIFStateType.Float, KIFTokenType.Digit,
                                                                        KIFStateType.Float),
                                        new StateTransitionTableEntry(KIFStateType.Float, KIFTokenType.Exp, 
                                                                        KIFStateType.Exp),
                                        new StateTransitionTableEntry(KIFStateType.Float, KIFTokenType.Comment,
                                                                        KIFStateType.Comment),
                                        new StateTransitionTableEntry(KIFStateType.Float, KIFTokenType.NonParenNonWhiteSpace, 
                                                                        KIFStateType.String),

                                        // Done
                                        /*************************************** Exp ************************************************/
                                        new StateTransitionTableEntry(KIFStateType.Exp, KIFTokenType.Sign,
                                                                        KIFStateType.ExpSign),
                                        new StateTransitionTableEntry(KIFStateType.Exp, KIFTokenType.Digit,
                                                                        KIFStateType.ExpDigit),
                                        new StateTransitionTableEntry(KIFStateType.Exp, KIFTokenType.Comment,
                                                                        KIFStateType.Comment),
                                        new StateTransitionTableEntry(KIFStateType.Exp, KIFTokenType.NonParenNonWhiteSpace, 
                                                                        KIFStateType.String),

                                        // Done
                                        /*************************************** ExpSign ************************************************/
                                        new StateTransitionTableEntry(KIFStateType.ExpSign, KIFTokenType.Digit,
                                                                        KIFStateType.ExpDigit),
                                        new StateTransitionTableEntry(KIFStateType.ExpSign, KIFTokenType.NonParenNonWhiteSpace, 
                                                                        KIFStateType.String),

                                        // Done
                                        /*************************************** ExpDigit ************************************************/
                                        new StateTransitionTableEntry(KIFStateType.ExpDigit, KIFTokenType.Digit,
                                                                        KIFStateType.ExpDigit),
                                        new StateTransitionTableEntry(KIFStateType.ExpDigit, KIFTokenType.Comment,
                                                                        KIFStateType.Comment),
                                        new StateTransitionTableEntry(KIFStateType.ExpDigit, KIFTokenType.NonParenNonWhiteSpace, 
                                                                        KIFStateType.String),

                                        // Done
                                        /*************************************** Comment ************************************************/
                                        new StateTransitionTableEntry(KIFStateType.Comment, KIFTokenType.NonEndOfLine,
                                                                        KIFStateType.Comment),
                                    };
        }

        private Token AcceptWord()
        {
            Lexeme = _workingTokenBuffer.ToString().Substring(0, _mostRecentStateAcceptPos.Value);
            _workingTokenBuffer.Clear();
            _mostRecentStateAccept = null;
            _currStateType = StateType_Start;
            return new Token() { TheTokenType = KIFTokenType.Word, Lexeme = Lexeme, NVal = null };
        }

        private Token AcceptString()
        {
            Lexeme = _workingTokenBuffer.ToString().Substring(0, _mostRecentStateAcceptPos.Value);
            _workingTokenBuffer.Clear();
            _mostRecentStateAccept = null;
            _currStateType = StateType_Start;
            return new Token() { TheTokenType = KIFTokenType.String, Lexeme = Lexeme, NVal = null };
        }

        private Token AcceptNumber()
        {
            Lexeme = _workingTokenBuffer.ToString().Substring(0, _mostRecentStateAcceptPos.Value);
            NVal = Convert.ToDouble(Lexeme);
            _workingTokenBuffer.Clear();
            _mostRecentStateAccept = null;
            _currStateType = StateType_Start;
            return new Token() { TheTokenType = KIFTokenType.Number, Lexeme = Lexeme, NVal = NVal };
        }

        private Token AcceptLeftParen()
        {
            Lexeme = _workingTokenBuffer.ToString().Substring(0, _mostRecentStateAcceptPos.Value);
            _workingTokenBuffer.Clear();
            _mostRecentStateAccept = null;
            _currStateType = StateType_Start;
            return new Token() { TheTokenType = KIFTokenType.LeftParen, Lexeme = Lexeme, NVal = null };
        }

        private Token AcceptEqual()
        {
            Lexeme = _workingTokenBuffer.ToString().Substring(0, _mostRecentStateAcceptPos.Value);
            _workingTokenBuffer.Clear();
            _mostRecentStateAccept = null;
            _currStateType = StateType_Start;
            return new Token() { TheTokenType = KIFTokenType.Equal, Lexeme = Lexeme, NVal = null };
        }

        private Token AcceptImplicationToLeft()
        {
            Lexeme = _workingTokenBuffer.ToString().Substring(0, _mostRecentStateAcceptPos.Value);
            _workingTokenBuffer.Clear();
            _mostRecentStateAccept = null;
            _currStateType = StateType_Start;
            return new Token() { TheTokenType = KIFTokenType.ImplicationToLeft, Lexeme = Lexeme, NVal = null };
        }

        private Token AcceptImplicationToRight()
        {
            Lexeme = _workingTokenBuffer.ToString().Substring(0, _mostRecentStateAcceptPos.Value);
            _workingTokenBuffer.Clear();
            _mostRecentStateAccept = null;
            _currStateType = StateType_Start;
            return new Token() { TheTokenType = KIFTokenType.ImplicationToRight, Lexeme = Lexeme, NVal = null };
        }

        private Token AcceptEquivalence()
        {
            Lexeme = _workingTokenBuffer.ToString().Substring(0, _mostRecentStateAcceptPos.Value);
            _workingTokenBuffer.Clear();
            _mostRecentStateAccept = null;
            _currStateType = StateType_Start;
            return new Token() { TheTokenType = KIFTokenType.Equivalence, Lexeme = Lexeme, NVal = null };
        }

        private Token AcceptQuestionMark()
        {
            Lexeme = _workingTokenBuffer.ToString().Substring(0, _mostRecentStateAcceptPos.Value);
            _workingTokenBuffer.Clear();
            _mostRecentStateAccept = null;
            _currStateType = StateType_Start;
            return new Token() { TheTokenType = KIFTokenType.QuestionMark, Lexeme = Lexeme, NVal = null };
        }

        private Token AcceptAmpersand()
        {
            Lexeme = _workingTokenBuffer.ToString().Substring(0, _mostRecentStateAcceptPos.Value);
            _workingTokenBuffer.Clear();
            _mostRecentStateAccept = null;
            _currStateType = StateType_Start;
            return new Token() { TheTokenType = KIFTokenType.Ampersand, Lexeme = Lexeme, NVal = null };
        }

        private Token AcceptVariable()
        {
            Lexeme = _workingTokenBuffer.ToString().Substring(0, _mostRecentStateAcceptPos.Value);
            _workingTokenBuffer.Clear();
            _mostRecentStateAccept = null;
            _currStateType = StateType_Start;
            return new Token() { TheTokenType = KIFTokenType.Variable, Lexeme = Lexeme, NVal = null };
        }

        private Token AcceptRightParen()
        {
            Lexeme = _workingTokenBuffer.ToString().Substring(0, _mostRecentStateAcceptPos.Value);
            _workingTokenBuffer.Clear();
            _mostRecentStateAccept = null;
            _currStateType = StateType_Start;
            return new Token() { TheTokenType = KIFTokenType.RightParen, Lexeme = Lexeme, NVal = null };
        }

        private Token AcceptQuotedString()
        {
            Lexeme = _workingTokenBuffer.ToString().Substring(0, _mostRecentStateAcceptPos.Value);
            _workingTokenBuffer.Clear();
            _mostRecentStateAccept = null;
            _currStateType = StateType_Start;
            return new Token() { TheTokenType = KIFTokenType.QuotedString, Lexeme = Lexeme, NVal = null };
        }
    }
}



