using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TFStreamTokenizer
{
    public delegate Token StateAccept();

    public enum StateType
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

    // ToDo:  Make this bits so that a state transition can be 
    // described with a disjuntion or conjuction of these bits!!!
    public enum TokenType
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

    public class Token
    {
        public TokenType TheTokenType { get; set; }
        public string Lexeme { get; set; }
        public double? NVal { get; set; }

        public Token()
        {
            TheTokenType = TokenType.None;
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
        public BitArray TokenTypes = new BitArray((int)TokenType.Any + 1);

        public CharacterTableEntry(char theChar)
        {
            TheChar = theChar;
        }
    }

    public class StateTableEntry
    {
        public StateType StateType { get; set; }
        public StateAccept StateAccept { get; set; }

        public StateTableEntry(StateType stateType, StateAccept stateAccept = null)
        {
            StateType = stateType;
            StateAccept = stateAccept;
        }
    }

    public class StateTransitionTableEntry
    {
        public StateType CurrentStateType { get; set; }
        public TokenType InputTokenType { get; set; }
        public StateType NextStateType { get; set; }

        public StateTransitionTableEntry(StateType stateType, TokenType inputTokenType, StateType nextStateType)
        {
            CurrentStateType = stateType;
            InputTokenType = inputTokenType;
            NextStateType = nextStateType;
        }
    }

    public class StreamTokenizer
    {
        public string Lexeme { get; set; }
        public double? NVal { get; set; }
        public bool EOLIsSignificant { get; set; }

        private StreamReader _sr;
        private int _peekedInputChar;
        private CharacterTableEntry _peekedCharTableEntry;
        private readonly StringBuilder _workingTokenBuffer = new StringBuilder();
        //private int _currInputPos;  // position of input character to process
        //private int _tokenStartPos; // first position of this next token we're parsing
        private StateType _currStateType;
        private StateAccept _mostRecentStateAccept;
        private int? _mostRecentStateAcceptPos;
        private CharacterTableEntry[] _characterTable;
        private List<StateTransitionTableEntry> _stateTransitionTable;
        private List<StateTableEntry> _stateTable;

        public StreamTokenizer(StreamReader sr)
        {
            _sr = sr;
            //_currInputPos = 0;
            //_tokenStartPos = 0;
            EOLIsSignificant = false;
            _currStateType = StateType.Start;
            //_mostRecentStateAccept = null;
            //_mostRecentStateAcceptPos = null;

            InitializeCharacterTable();
            InitializeKIFStateTable();
            InitializeKIFStateTransitionTable();
        }

        private void InitializeCharacterTable()
        {
            _characterTable = new CharacterTableEntry[256];

            for (int i = 0; i < 256; i++)
            {
                var entry = new CharacterTableEntry((char)i);

                entry.TokenTypes[(int)TokenType.Any] = true;
                entry.TokenTypes[(int)TokenType.NonWhiteSpace] = true;
                entry.TokenTypes[(int)TokenType.NonDigitNonWhiteSpace] = true;
                entry.TokenTypes[(int)TokenType.NonParenNonWhiteSpace] = true;
                entry.TokenTypes[(int)TokenType.NonEndOfLine] = true;

                if ((0x00 <= i && i <= 0x20) || i == 0x7F)
                {
                    entry.TokenTypes[(int)TokenType.NonWhiteSpace] = false;
                    entry.TokenTypes[(int)TokenType.NonDigitNonWhiteSpace] = false;
                    entry.TokenTypes[(int)TokenType.NonParenNonWhiteSpace] = false;

                    entry.TokenTypes[(int)TokenType.WhiteSpace] = true;
                    if (i == 0x0A)
                    {
                        entry.TokenTypes[(int)TokenType.EndOfLine] = true;
                        entry.TokenTypes[(int)TokenType.NonEndOfLine] = false;
                    }
                }

                if ((0x41 <= i && i <= 0x5A) || (0x61 <= i && i <= 0x7A))     // A-Z, a-z
                {
                    entry.TokenTypes[(int)TokenType.Letter] = true;
                    entry.TokenTypes[(int)TokenType.Alphabetic] = true;

                    if (0x41 <= i && i <= 0x5A)
                    {
                        entry.TokenTypes[(int)TokenType.Upper] = true;
                    }
                    else
                    {
                        entry.TokenTypes[(int)TokenType.Lower] = true;
                    }
                }

                if (0x30 <= i && i <= 0x39)  // 0-9
                {
                    entry.TokenTypes[(int)TokenType.NonDigitNonWhiteSpace] = false;
                    entry.TokenTypes[(int)TokenType.Digit] = true;
                }

                if (i == '!' || i == '$' || i == '%' || i == '&' || i == '*' || i == '+' || i == '-' || i == '.' || i == '/' || i == '<' || i == '=' || i == '>' || i == '?' || i == '@' || i == '_' || i == '~')
                {
                    entry.TokenTypes[(int)TokenType.Special] = true;
                }

                if (i == 0x27) // single quote
                {
                    entry.TokenTypes[(int)TokenType.SingleQuote] = true;
                }

                if (i == 0x22) // double quote
                {
                    entry.TokenTypes[(int)TokenType.DoubleQuote] = true;
                }

                if (i == '?')
                {
                    entry.TokenTypes[(int)TokenType.QuestionMark] = true;
                }

                if (i == '@')
                {
                    entry.TokenTypes[(int)TokenType.Ampersand] = true;
                }

                if (i == '.')
                {
                    entry.TokenTypes[(int)TokenType.Dot] = true;
                }

                if (i == ',')
                {
                    entry.TokenTypes[(int)TokenType.Comma] = true;
                }

                if (i == 'e' || i == 'E')
                {
                    entry.TokenTypes[(int)TokenType.Exp] = true;
                }

                //if (i == '/' || i == '#') // are these used in GDL too?
                if (i == ';')
                {
                    entry.TokenTypes[(int)TokenType.Comment] = true;
                }

                if (i == '+' || i == '-')
                {
                    entry.TokenTypes[(int)TokenType.Sign] = true;
                }

                if (i == '(')
                {
                    entry.TokenTypes[(int)TokenType.NonParenNonWhiteSpace] = false;

                    entry.TokenTypes[(int)TokenType.LeftParen] = true;
                }

                if (i == ')')
                {
                    entry.TokenTypes[(int)TokenType.NonParenNonWhiteSpace] = false;

                    entry.TokenTypes[(int)TokenType.RightParen] = true;
                }

                if (i == '=')
                {
                    entry.TokenTypes[(int)TokenType.NonParenNonWhiteSpace] = false;

                    entry.TokenTypes[(int)TokenType.Equal] = true;
                }

                if (i == '<')                
                {
                    entry.TokenTypes[(int)TokenType.NonParenNonWhiteSpace] = false;

                    entry.TokenTypes[(int)TokenType.LessThan] = true;
                }

                if (i == '>')
                {
                    entry.TokenTypes[(int)TokenType.NonParenNonWhiteSpace] = false;

                    entry.TokenTypes[(int)TokenType.GreaterThan] = true;
                }

                if (entry.TokenTypes[(int)TokenType.Upper] ||
                    entry.TokenTypes[(int)TokenType.Lower]
                    )
                {
                    entry.TokenTypes[(int)TokenType.InitialChar] = true;
                }


                if (entry.TokenTypes[(int)TokenType.Upper] ||
                    entry.TokenTypes[(int)TokenType.Lower] ||
                    entry.TokenTypes[(int)TokenType.Digit] ||
                    entry.TokenTypes[(int)TokenType.Special]
                    )
                {
                    entry.TokenTypes[(int)TokenType.WordChar] = true;
                }

                if (entry.TokenTypes[(int)TokenType.Upper] ||
                    entry.TokenTypes[(int)TokenType.Lower] ||
                    entry.TokenTypes[(int)TokenType.Digit] ||
                    entry.TokenTypes[(int)TokenType.Special] ||
                    entry.TokenTypes[(int)TokenType.WhiteSpace]
                    )
                {
                    entry.TokenTypes[(int)TokenType.Character] = true;
                }


                _characterTable[i] = entry;
            }
        }


        private void InitializeKIFStateTable()
        {
            _stateTable = new List<StateTableEntry>()
                            {
                                new StateTableEntry(StateType.Start),
                                new StateTableEntry(StateType.Digit1, AcceptNumber),
                                new StateTableEntry(StateType.Digit2, AcceptNumber),
                                new StateTableEntry(StateType.Digit3, AcceptNumber),
                                new StateTableEntry(StateType.DigitN, AcceptNumber),
                                new StateTableEntry(StateType.DigitComma1, AcceptString),
                                new StateTableEntry(StateType.DigitComma2, AcceptString),
                                new StateTableEntry(StateType.DigitComma3, AcceptString),
                                new StateTableEntry(StateType.DigitComma4, AcceptNumber),
                                new StateTableEntry(StateType.Float, AcceptNumber),
                                new StateTableEntry(StateType.Sign, AcceptString),
                                new StateTableEntry(StateType.Exp, AcceptString),
                                new StateTableEntry(StateType.ExpSign, AcceptString),
                                new StateTableEntry(StateType.ExpDigit, AcceptNumber),
                                new StateTableEntry(StateType.Word, AcceptWord),
                                new StateTableEntry(StateType.String, AcceptString),
                                new StateTableEntry(StateType.LeftParen, AcceptLeftParen),
                                new StateTableEntry(StateType.RightParen, AcceptRightParen),
                                new StateTableEntry(StateType.QuotedStringStart, AcceptString),
                                new StateTableEntry(StateType.QuotedStringEnd, AcceptQuotedString),
                                new StateTableEntry(StateType.Equal, AcceptEqual),
                                new StateTableEntry(StateType.LessThan),
                                new StateTableEntry(StateType.LessThanOrEqual, AcceptImplicationToLeft),
                                new StateTableEntry(StateType.EqualOrGreaterThan, AcceptImplicationToRight),
                                new StateTableEntry(StateType.Equivalence, AcceptEquivalence),
                                new StateTableEntry(StateType.QuestionMark, AcceptQuestionMark),
                                new StateTableEntry(StateType.Ampersand, AcceptAmpersand),
                                new StateTableEntry(StateType.Variable, AcceptVariable),
                                new StateTableEntry(StateType.Comment),
                            };
        }

        private void InitializeKIFStateTransitionTable()
        {
            _stateTransitionTable = new List<StateTransitionTableEntry>()
                                    {
                                        // Done
                                        /*************************************** Start ************************************************/
                                        new StateTransitionTableEntry(StateType.Start, TokenType.InitialChar,
                                                                        StateType.Word),
                                        new StateTransitionTableEntry(StateType.Start, TokenType.LeftParen, 
                                                                        StateType.LeftParen),
                                        new StateTransitionTableEntry(StateType.Start, TokenType.RightParen, 
                                                                        StateType.RightParen),
                                        new StateTransitionTableEntry(StateType.Start, TokenType.LessThan, 
                                                                        StateType.LessThan),
                                        new StateTransitionTableEntry(StateType.Start, TokenType.Equal, 
                                                                        StateType.Equal),
                                        new StateTransitionTableEntry(StateType.Start, TokenType.QuestionMark, 
                                                                        StateType.QuestionMark),
                                        new StateTransitionTableEntry(StateType.Start, TokenType.Ampersand, 
                                                                        StateType.Ampersand),
                                        new StateTransitionTableEntry(StateType.Start, TokenType.Digit,
                                                                        StateType.Digit1),
                                        new StateTransitionTableEntry(StateType.Start, TokenType.Dot,
                                                                        StateType.Float),
                                        new StateTransitionTableEntry(StateType.Start, TokenType.Sign,
                                                                        StateType.Sign),
                                        new StateTransitionTableEntry(StateType.Start, TokenType.DoubleQuote,
                                                                        StateType.QuotedStringStart),
                                        new StateTransitionTableEntry(StateType.Start, TokenType.Comment,
                                                                        StateType.Comment),
                                        new StateTransitionTableEntry(StateType.Start, TokenType.NonWhiteSpace, 
                                                                        StateType.String),

                                        // Done
                                        /*************************************** LeftParen ************************************************/
                                        new StateTransitionTableEntry(StateType.LeftParen, TokenType.Comment,
                                                                        StateType.Comment),


                                        // Done
                                        /*************************************** RightParen ************************************************/
                                        new StateTransitionTableEntry(StateType.RightParen, TokenType.Comment,
                                                                        StateType.Comment),


                                        /*************************************** Equal ************************************************/
                                        new StateTransitionTableEntry(StateType.Equal, TokenType.GreaterThan, 
                                                                        StateType.EqualOrGreaterThan),
                                        new StateTransitionTableEntry(StateType.Equal, TokenType.Comment,
                                                                        StateType.Comment),


                                        /*************************************** LessThan ************************************************/
                                        new StateTransitionTableEntry(StateType.LessThan, TokenType.Equal, 
                                                                        StateType.LessThanOrEqual),

                                        /*************************************** LessThanOrEqual ************************************************/
                                        new StateTransitionTableEntry(StateType.LessThanOrEqual, TokenType.GreaterThan, 
                                                                        StateType.Equivalence),
                                        new StateTransitionTableEntry(StateType.LessThanOrEqual, TokenType.Comment,
                                                                        StateType.Comment),


                                        /*************************************** EqualOrGreaterThan ************************************************/
                                        new StateTransitionTableEntry(StateType.EqualOrGreaterThan, TokenType.Comment,
                                                                        StateType.Comment),


                                        /*************************************** LessThanOrEqual ************************************************/
                                        new StateTransitionTableEntry(StateType.LessThanOrEqual, TokenType.Comment,
                                                                        StateType.Comment),


                                        /*************************************** Equivalence ************************************************/
                                        new StateTransitionTableEntry(StateType.Equivalence, TokenType.Comment,
                                                                        StateType.Comment),


                                        /*************************************** QuestionMark ************************************************/
                                        new StateTransitionTableEntry(StateType.QuestionMark, TokenType.InitialChar, 
                                                                        StateType.Variable),
                                        new StateTransitionTableEntry(StateType.QuestionMark, TokenType.Comment,
                                                                        StateType.Comment),


                                        /*************************************** Ampersand ************************************************/
                                        new StateTransitionTableEntry(StateType.Ampersand, TokenType.InitialChar, 
                                                                        StateType.Variable),
                                        new StateTransitionTableEntry(StateType.Ampersand, TokenType.Comment,
                                                                        StateType.Comment),


                                        /*************************************** Variable ************************************************/
                                        new StateTransitionTableEntry(StateType.Variable, TokenType.WordChar, 
                                                                        StateType.Variable),
                                        new StateTransitionTableEntry(StateType.Variable, TokenType.Comment,
                                                                        StateType.Comment),

                                        // Done
                                        /*************************************** QuotedStringStart ************************************************/
                                        new StateTransitionTableEntry(StateType.QuotedStringStart, TokenType.Character, 
                                                                        StateType.QuotedStringStart),
                                        new StateTransitionTableEntry(StateType.QuotedStringStart, TokenType.DoubleQuote, 
                                                                        StateType.QuotedStringEnd),
                                        new StateTransitionTableEntry(StateType.QuotedStringStart, TokenType.NonParenNonWhiteSpace, 
                                                                        StateType.String),

                                        // Done
                                        /*************************************** QuotedStringEnd ************************************************/
                                        new StateTransitionTableEntry(StateType.QuotedStringEnd, TokenType.Comment,
                                                                        StateType.Comment),


                                        // Done
                                        /*************************************** String ************************************************/
                                        new StateTransitionTableEntry(StateType.String, TokenType.Comment,
                                                                        StateType.Comment),
                                        new StateTransitionTableEntry(StateType.String, TokenType.NonParenNonWhiteSpace, 
                                                                        StateType.String),

                                        // Done
                                        /*************************************** Sign ************************************************/
                                        new StateTransitionTableEntry(StateType.Sign, TokenType.Digit,
                                                                        StateType.Digit1),
                                        new StateTransitionTableEntry(StateType.Sign, TokenType.Dot, 
                                                                        StateType.Float),
                                        new StateTransitionTableEntry(StateType.Sign, TokenType.Comment,
                                                                        StateType.Comment),
                                        new StateTransitionTableEntry(StateType.Sign, TokenType.NonParenNonWhiteSpace, 
                                                                        StateType.String),

                                        // Done
                                        /*************************************** Word ************************************************/
                                        new StateTransitionTableEntry(StateType.Word, TokenType.WordChar, 
                                                                        StateType.Word),
                                        new StateTransitionTableEntry(StateType.Word, TokenType.Comment,
                                                                        StateType.Comment),
                                        new StateTransitionTableEntry(StateType.Word, TokenType.NonParenNonWhiteSpace, 
                                                                        StateType.String),

                                        // Done
                                        /*************************************** Digit1 ************************************************/
                                        new StateTransitionTableEntry(StateType.Digit1, TokenType.Digit,
                                                                        StateType.Digit2),
                                        new StateTransitionTableEntry(StateType.Digit1, TokenType.Dot,
                                                                        StateType.Float),
                                        new StateTransitionTableEntry(StateType.Digit1, TokenType.Comma,
                                                                        StateType.DigitComma1),
                                        new StateTransitionTableEntry(StateType.Digit1, TokenType.Comment,
                                                                        StateType.Comment),
                                        new StateTransitionTableEntry(StateType.Digit1, TokenType.NonParenNonWhiteSpace,
                                                                        StateType.String),

                                        // Done
                                        /*************************************** Digit2 ************************************************/
                                        new StateTransitionTableEntry(StateType.Digit2, TokenType.Digit,
                                                                        StateType.Digit3),
                                        new StateTransitionTableEntry(StateType.Digit2, TokenType.Dot,
                                                                        StateType.Float),
                                        new StateTransitionTableEntry(StateType.Digit2, TokenType.Comma,
                                                                        StateType.DigitComma1),
                                        new StateTransitionTableEntry(StateType.Digit2, TokenType.Comment,
                                                                        StateType.Comment),
                                        new StateTransitionTableEntry(StateType.Digit2, TokenType.NonParenNonWhiteSpace,
                                                                        StateType.String),

                                        // Done
                                        /*************************************** Digit3 ************************************************/
                                        new StateTransitionTableEntry(StateType.Digit3, TokenType.Digit,
                                                                        StateType.DigitN),
                                        new StateTransitionTableEntry(StateType.Digit3, TokenType.Dot,
                                                                        StateType.Float),
                                        new StateTransitionTableEntry(StateType.Digit3, TokenType.Comma,
                                                                        StateType.DigitComma1),
                                        new StateTransitionTableEntry(StateType.Digit3, TokenType.Comment,
                                                                        StateType.Comment),
                                        new StateTransitionTableEntry(StateType.Digit3, TokenType.NonParenNonWhiteSpace,
                                                                        StateType.String),

                                        // ToDo:  Modify this if this is acceptable:  integer exp
                                        // Done
                                        /*************************************** DigitN ************************************************/
                                        new StateTransitionTableEntry(StateType.DigitN, TokenType.Digit,
                                                                        StateType.DigitN),
                                        new StateTransitionTableEntry(StateType.DigitN, TokenType.Dot,
                                                                        StateType.Float),
                                        new StateTransitionTableEntry(StateType.DigitN, TokenType.Comment,
                                                                        StateType.Comment),
                                        new StateTransitionTableEntry(StateType.Digit3, TokenType.NonParenNonWhiteSpace,
                                                                        StateType.String),
                                        // Done
                                        /*************************************** DigitComma1 ************************************************/
                                        new StateTransitionTableEntry(StateType.DigitComma1, TokenType.Digit,
                                                                        StateType.DigitComma2),
                                        new StateTransitionTableEntry(StateType.DigitComma1, TokenType.Comment,
                                                                        StateType.Comment),
                                        new StateTransitionTableEntry(StateType.DigitComma1, TokenType.NonParenNonWhiteSpace,
                                                                        StateType.String),

                                        // Done
                                        /*************************************** DigitComma2 ************************************************/
                                        new StateTransitionTableEntry(StateType.DigitComma2, TokenType.Digit,
                                                                        StateType.DigitComma3),
                                        new StateTransitionTableEntry(StateType.DigitComma2, TokenType.Comment,
                                                                        StateType.Comment),
                                        new StateTransitionTableEntry(StateType.DigitComma2, TokenType.NonParenNonWhiteSpace,
                                                                        StateType.String),

                                        // Done
                                        /*************************************** DigitComma3 ************************************************/
                                        new StateTransitionTableEntry(StateType.DigitComma3, TokenType.Digit,
                                                                        StateType.DigitComma4),
                                        new StateTransitionTableEntry(StateType.DigitComma3, TokenType.Comment,
                                                                        StateType.Comment),
                                        new StateTransitionTableEntry(StateType.DigitComma3, TokenType.NonParenNonWhiteSpace,
                                                                        StateType.String),

                                        // Done
                                        /*************************************** DigitComma4 ************************************************/
                                        new StateTransitionTableEntry(StateType.DigitComma4, TokenType.Dot,
                                                                        StateType.Float),
                                        new StateTransitionTableEntry(StateType.DigitComma4, TokenType.Comma,
                                                                        StateType.DigitComma1),
                                        new StateTransitionTableEntry(StateType.DigitComma4, TokenType.Comment,
                                                                        StateType.Comment),
                                        new StateTransitionTableEntry(StateType.DigitComma4, TokenType.NonParenNonWhiteSpace,
                                                                        StateType.String),

                                        // Done
                                        /*************************************** Float ************************************************/
                                        new StateTransitionTableEntry(StateType.Float, TokenType.Digit,
                                                                        StateType.Float),
                                        new StateTransitionTableEntry(StateType.Float, TokenType.Exp, 
                                                                        StateType.Exp),
                                        new StateTransitionTableEntry(StateType.Float, TokenType.Comment,
                                                                        StateType.Comment),
                                        new StateTransitionTableEntry(StateType.Float, TokenType.NonParenNonWhiteSpace, 
                                                                        StateType.String),

                                        // Done
                                        /*************************************** Exp ************************************************/
                                        new StateTransitionTableEntry(StateType.Exp, TokenType.Sign,
                                                                        StateType.ExpSign),
                                        new StateTransitionTableEntry(StateType.Exp, TokenType.Digit,
                                                                        StateType.ExpDigit),
                                        new StateTransitionTableEntry(StateType.Exp, TokenType.Comment,
                                                                        StateType.Comment),
                                        new StateTransitionTableEntry(StateType.Exp, TokenType.NonParenNonWhiteSpace, 
                                                                        StateType.String),

                                        // Done
                                        /*************************************** ExpSign ************************************************/
                                        new StateTransitionTableEntry(StateType.ExpSign, TokenType.Digit,
                                                                        StateType.ExpDigit),
                                        new StateTransitionTableEntry(StateType.ExpSign, TokenType.NonParenNonWhiteSpace, 
                                                                        StateType.String),

                                        // Done
                                        /*************************************** ExpDigit ************************************************/
                                        new StateTransitionTableEntry(StateType.ExpDigit, TokenType.Digit,
                                                                        StateType.ExpDigit),
                                        new StateTransitionTableEntry(StateType.ExpDigit, TokenType.Comment,
                                                                        StateType.Comment),
                                        new StateTransitionTableEntry(StateType.ExpDigit, TokenType.NonParenNonWhiteSpace, 
                                                                        StateType.String),

                                        // Done
                                        /*************************************** Comment ************************************************/
                                        new StateTransitionTableEntry(StateType.Comment, TokenType.NonEndOfLine,
                                                                        StateType.Comment),
                                    };
        }


#if FIRST_STATE_MACHINE
    // this was my first state machine
    private void InitializeStateTable()
    {
        _stateTable = new List<StateTableEntry>()
        {
            new StateTableEntry(StateType.Start, ConsumeWhiteSpace),
            new StateTableEntry(StateType.Sign, ReturnWordOnWhiteSpace),
            new StateTableEntry(StateType.Word, ReturnWordOnWhiteSpace),
            new StateTableEntry(StateType.Digit1, ReturnNumberOnWhiteSpace),
            new StateTableEntry(StateType.Digit2, ReturnNumberOnWhiteSpace),
            new StateTableEntry(StateType.Digit3, ReturnNumberOnWhiteSpace),
            new StateTableEntry(StateType.DigitN, ReturnNumberOnWhiteSpace),
            new StateTableEntry(StateType.DigitComma1, ReturnWordOnWhiteSpace),
            new StateTableEntry(StateType.DigitComma2, ReturnWordOnWhiteSpace),
            new StateTableEntry(StateType.DigitComma3, ReturnWordOnWhiteSpace),
            new StateTableEntry(StateType.DigitComma4, ReturnNumberOnWhiteSpace),
            new StateTableEntry(StateType.Float, ReturnNumberOnWhiteSpace),
            new StateTableEntry(StateType.Exp, ReturnWordOnWhiteSpace),
            new StateTableEntry(StateType.ExpSign, ReturnWordOnWhiteSpace),
            new StateTableEntry(StateType.ExpDigit, ReturnNumberOnWhiteSpace),
        };
    }

    // this was my first state machine
    private void InitializeStateTransitionTable()
    {
        _stateTransitionTable = new List<StateTransitionTableEntry>()
                                    {
                                        /*************************************** Start ************************************************/
                                        new StateTransitionTableEntry(StateType.Start, TokenType.WhiteSpace,
                                                                        StateType.Start),
                                        new StateTransitionTableEntry(StateType.Start, TokenType.Digit,
                                                                        StateType.Digit1),
                                        new StateTransitionTableEntry(StateType.Start, TokenType.Sign,
                                                                        StateType.Sign),
                                        new StateTransitionTableEntry(StateType.Start, TokenType.Dot,
                                                                        StateType.Float),
                                        new StateTransitionTableEntry(StateType.Start, TokenType.Any, StateType.Word),
                                        /*************************************** Sign ************************************************/
                                        new StateTransitionTableEntry(StateType.Sign, TokenType.WhiteSpace,
                                                                        StateType.Start),
                                        new StateTransitionTableEntry(StateType.Sign, TokenType.Digit,
                                                                        StateType.Digit1),
                                        new StateTransitionTableEntry(StateType.Sign, TokenType.Dot, StateType.Float),
                                        new StateTransitionTableEntry(StateType.Sign, TokenType.Any, StateType.Word),
                                        /*************************************** Word ************************************************/
                                        new StateTransitionTableEntry(StateType.Word, TokenType.WhiteSpace,
                                                                        StateType.Start),
                                        new StateTransitionTableEntry(StateType.Word, TokenType.Any, StateType.Word),
                                        /*************************************** Digit1 ************************************************/
                                        new StateTransitionTableEntry(StateType.Digit1, TokenType.WhiteSpace,
                                                                        StateType.Start),
                                        new StateTransitionTableEntry(StateType.Digit1, TokenType.Digit,
                                                                        StateType.Digit2),
                                        new StateTransitionTableEntry(StateType.Digit1, TokenType.Dot,
                                                                        StateType.Float),
                                        new StateTransitionTableEntry(StateType.Digit1, TokenType.Comma,
                                                                        StateType.DigitComma1),
                                        new StateTransitionTableEntry(StateType.Digit1, TokenType.Any,
                                                                        StateType.Word),
                                        /*************************************** Digit2 ************************************************/
                                        new StateTransitionTableEntry(StateType.Digit2, TokenType.WhiteSpace,
                                                                        StateType.Start),
                                        new StateTransitionTableEntry(StateType.Digit2, TokenType.Digit,
                                                                        StateType.Digit3),
                                        new StateTransitionTableEntry(StateType.Digit2, TokenType.Dot,
                                                                        StateType.Float),
                                        new StateTransitionTableEntry(StateType.Digit2, TokenType.Comma,
                                                                        StateType.DigitComma1),
                                        new StateTransitionTableEntry(StateType.Digit2, TokenType.Any,
                                                                        StateType.Word),
                                        /*************************************** Digit3 ************************************************/
                                        new StateTransitionTableEntry(StateType.Digit3, TokenType.WhiteSpace,
                                                                        StateType.Start),
                                        new StateTransitionTableEntry(StateType.Digit3, TokenType.Digit,
                                                                        StateType.DigitN),
                                        new StateTransitionTableEntry(StateType.Digit3, TokenType.Dot,
                                                                        StateType.Float),
                                        new StateTransitionTableEntry(StateType.Digit3, TokenType.Comma,
                                                                        StateType.DigitComma1),
                                        new StateTransitionTableEntry(StateType.Digit3, TokenType.Any,
                                                                        StateType.Word),
                                        /*************************************** DigitN ************************************************/
                                        new StateTransitionTableEntry(StateType.DigitN, TokenType.WhiteSpace,
                                                                        StateType.Start),
                                        new StateTransitionTableEntry(StateType.DigitN, TokenType.Digit,
                                                                        StateType.DigitN),
                                        new StateTransitionTableEntry(StateType.DigitN, TokenType.Dot,
                                                                        StateType.Float),
                                        new StateTransitionTableEntry(StateType.Digit3, TokenType.Any,
                                                                        StateType.Word),
                                        /*************************************** DigitComma1 ************************************************/
                                        new StateTransitionTableEntry(StateType.DigitComma1, TokenType.WhiteSpace,
                                                                        StateType.Start),
                                        new StateTransitionTableEntry(StateType.DigitComma1, TokenType.Digit,
                                                                        StateType.DigitComma2),
                                        new StateTransitionTableEntry(StateType.DigitComma1, TokenType.Any,
                                                                        StateType.Word),
                                        /*************************************** DigitComma2 ************************************************/
                                        new StateTransitionTableEntry(StateType.DigitComma2, TokenType.WhiteSpace,
                                                                        StateType.Start),
                                        new StateTransitionTableEntry(StateType.DigitComma2, TokenType.Digit,
                                                                        StateType.DigitComma3),
                                        new StateTransitionTableEntry(StateType.DigitComma2, TokenType.Any,
                                                                        StateType.Word),
                                        /*************************************** DigitComma3 ************************************************/
                                        new StateTransitionTableEntry(StateType.DigitComma3, TokenType.WhiteSpace,
                                                                        StateType.Start),
                                        new StateTransitionTableEntry(StateType.DigitComma3, TokenType.Digit,
                                                                        StateType.DigitComma4),
                                        new StateTransitionTableEntry(StateType.DigitComma3, TokenType.Any,
                                                                        StateType.Word),
                                        /*************************************** DigitComma4 ************************************************/
                                        new StateTransitionTableEntry(StateType.DigitComma4, TokenType.WhiteSpace,
                                                                        StateType.Start),
                                        new StateTransitionTableEntry(StateType.DigitComma4, TokenType.Dot,
                                                                        StateType.Float),
                                        new StateTransitionTableEntry(StateType.DigitComma4, TokenType.Comma,
                                                                        StateType.DigitComma1),
                                        new StateTransitionTableEntry(StateType.DigitComma4, TokenType.Any,
                                                                        StateType.Word),
                                        /*************************************** Float ************************************************/
                                        new StateTransitionTableEntry(StateType.Float, TokenType.WhiteSpace,
                                                                        StateType.Start),
                                        new StateTransitionTableEntry(StateType.Float, TokenType.Digit,
                                                                        StateType.Float),
                                        new StateTransitionTableEntry(StateType.Float, TokenType.Exp, StateType.Exp),
                                        new StateTransitionTableEntry(StateType.Float, TokenType.Any, StateType.Word),
                                        /*************************************** Exp ************************************************/
                                        new StateTransitionTableEntry(StateType.Exp, TokenType.WhiteSpace,
                                                                        StateType.Start),
                                        new StateTransitionTableEntry(StateType.Exp, TokenType.Sign,
                                                                        StateType.ExpSign),
                                        new StateTransitionTableEntry(StateType.Exp, TokenType.Digit,
                                                                        StateType.ExpDigit),
                                        new StateTransitionTableEntry(StateType.Exp, TokenType.Any, StateType.Word),
                                        /*************************************** ExpSign ************************************************/
                                        new StateTransitionTableEntry(StateType.ExpSign, TokenType.WhiteSpace,
                                                                        StateType.Start),
                                        new StateTransitionTableEntry(StateType.ExpSign, TokenType.Digit,
                                                                        StateType.ExpDigit),
                                        new StateTransitionTableEntry(StateType.ExpSign, TokenType.Any, StateType.Word),
                                        /*************************************** ExpDigit ************************************************/
                                        new StateTransitionTableEntry(StateType.ExpDigit, TokenType.WhiteSpace,
                                                                        StateType.Start),
                                        new StateTransitionTableEntry(StateType.ExpDigit, TokenType.Digit,
                                                                        StateType.ExpDigit),
                                        new StateTransitionTableEntry(StateType.ExpDigit, TokenType.Any, StateType.Word)
                                    };
    }
#endif

        private Token AcceptWord()
        {
            Lexeme = _workingTokenBuffer.ToString().Substring(0, _mostRecentStateAcceptPos.Value);
            _workingTokenBuffer.Clear();
            _mostRecentStateAccept = null;
            _currStateType = StateType.Start;
            return new Token() { TheTokenType = TokenType.Word, Lexeme = Lexeme, NVal = null };
        }

        private Token AcceptString()
        {
            Lexeme = _workingTokenBuffer.ToString().Substring(0, _mostRecentStateAcceptPos.Value);
            _workingTokenBuffer.Clear();
            _mostRecentStateAccept = null;
            _currStateType = StateType.Start;
            return new Token() { TheTokenType = TokenType.String, Lexeme = Lexeme, NVal = null };
        }

        private Token AcceptNumber()
        {
            Lexeme = _workingTokenBuffer.ToString().Substring(0, _mostRecentStateAcceptPos.Value);
            NVal = Convert.ToDouble(Lexeme);
            _workingTokenBuffer.Clear();
            _mostRecentStateAccept = null;
            _currStateType = StateType.Start;
            return new Token() { TheTokenType = TokenType.Number, Lexeme = Lexeme, NVal = NVal };
        }

        private Token AcceptLeftParen()
        {
            Lexeme = _workingTokenBuffer.ToString().Substring(0, _mostRecentStateAcceptPos.Value);
            _workingTokenBuffer.Clear();
            _mostRecentStateAccept = null;
            _currStateType = StateType.Start;
            return new Token() { TheTokenType = TokenType.LeftParen, Lexeme = Lexeme, NVal = null };
        }

        private Token AcceptEqual()
        {
            Lexeme = _workingTokenBuffer.ToString().Substring(0, _mostRecentStateAcceptPos.Value);
            _workingTokenBuffer.Clear();
            _mostRecentStateAccept = null;
            _currStateType = StateType.Start;
            return new Token() { TheTokenType = TokenType.Equal, Lexeme = Lexeme, NVal = null };
        }

        private Token AcceptImplicationToLeft()
        {
            Lexeme = _workingTokenBuffer.ToString().Substring(0, _mostRecentStateAcceptPos.Value);
            _workingTokenBuffer.Clear();
            _mostRecentStateAccept = null;
            _currStateType = StateType.Start;
            return new Token() { TheTokenType = TokenType.ImplicationToLeft, Lexeme = Lexeme, NVal = null };
        }

        private Token AcceptImplicationToRight()
        {
            Lexeme = _workingTokenBuffer.ToString().Substring(0, _mostRecentStateAcceptPos.Value);
            _workingTokenBuffer.Clear();
            _mostRecentStateAccept = null;
            _currStateType = StateType.Start;
            return new Token() { TheTokenType = TokenType.ImplicationToRight, Lexeme = Lexeme, NVal = null };
        }

        private Token AcceptEquivalence()
        {
            Lexeme = _workingTokenBuffer.ToString().Substring(0, _mostRecentStateAcceptPos.Value);
            _workingTokenBuffer.Clear();
            _mostRecentStateAccept = null;
            _currStateType = StateType.Start;
            return new Token() { TheTokenType = TokenType.Equivalence, Lexeme = Lexeme, NVal = null };
        }

        private Token AcceptQuestionMark()
        {
            Lexeme = _workingTokenBuffer.ToString().Substring(0, _mostRecentStateAcceptPos.Value);
            _workingTokenBuffer.Clear();
            _mostRecentStateAccept = null;
            _currStateType = StateType.Start;
            return new Token() { TheTokenType = TokenType.QuestionMark, Lexeme = Lexeme, NVal = null };
        }

        private Token AcceptAmpersand()
        {
            Lexeme = _workingTokenBuffer.ToString().Substring(0, _mostRecentStateAcceptPos.Value);
            _workingTokenBuffer.Clear();
            _mostRecentStateAccept = null;
            _currStateType = StateType.Start;
            return new Token() { TheTokenType = TokenType.Ampersand, Lexeme = Lexeme, NVal = null };
        }

        private Token AcceptVariable()
        {
            Lexeme = _workingTokenBuffer.ToString().Substring(0, _mostRecentStateAcceptPos.Value);
            _workingTokenBuffer.Clear();
            _mostRecentStateAccept = null;
            _currStateType = StateType.Start;
            return new Token() { TheTokenType = TokenType.Variable, Lexeme = Lexeme, NVal = null };
        }

        private Token AcceptRightParen()
        {
            Lexeme = _workingTokenBuffer.ToString().Substring(0, _mostRecentStateAcceptPos.Value);
            _workingTokenBuffer.Clear();
            _mostRecentStateAccept = null;
            _currStateType = StateType.Start;
            return new Token() { TheTokenType = TokenType.RightParen, Lexeme = Lexeme, NVal = null };
        }

        private Token AcceptQuotedString()
        {
            Lexeme = _workingTokenBuffer.ToString().Substring(0, _mostRecentStateAcceptPos.Value);
            _workingTokenBuffer.Clear();
            _mostRecentStateAccept = null;
            _currStateType = StateType.Start;
            return new Token() { TheTokenType = TokenType.QuotedString, Lexeme = Lexeme, NVal = null };
        }

        public Token NextToken()
        {
            while (!_sr.EndOfStream)
            {
                _peekedInputChar = _sr.Peek();
                _peekedCharTableEntry = _characterTable.Where(n => n.TheChar == _peekedInputChar).First();

                var query =
                    _stateTransitionTable.Where(
                        n => n.CurrentStateType == _currStateType);
                bool foundATransition = false;
                if (query.Any())
                {

                    foreach (StateTransitionTableEntry stateTransitionTableEntry in query)
                    {
                        if (_peekedCharTableEntry.TokenTypes[(int) stateTransitionTableEntry.InputTokenType])
                        {
                            foundATransition = true;

                            _currStateType = stateTransitionTableEntry.NextStateType;
                            var currStateTableEntry = _stateTable.Where(n => n.StateType == _currStateType).First();
                            _workingTokenBuffer.Append((char) _sr.Read());


                            if (currStateTableEntry.StateAccept != null)
                            {
                                _mostRecentStateAccept = currStateTableEntry.StateAccept;
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
                        _currStateType = StateType.Start;
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
                return new Token() { TheTokenType = TokenType.EndOfFile, Lexeme = String.Empty };
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
}
