using System;
using TFStreamTokenizer;

namespace API.Parsing.KIFParserUngerParallel
{
    public class Symbol
    {
        public string TheSymbol { get; private set; }
        public bool IsTerminal { get; private set; }
        public TokenType TheTokenType { get; private set; }

        public Symbol(object anObject, bool? isTerminal = null)
        {
            if (anObject is String)
            {
                TheSymbol = anObject as string;
                if (isTerminal == null)
                {
                    if (Char.IsLetter(TheSymbol[0]))
                    {
                        IsTerminal = Char.IsLower(TheSymbol[0]);
                    }
                    else
                    {
                        IsTerminal = true;
                    }                    
                }
                else
                {
                    IsTerminal = isTerminal.Value;
                }

                TheTokenType = TokenType.None;
            }
            else if (anObject is TokenType)
            {
                TheTokenType = (TokenType)anObject;
                IsTerminal = true;
                TheSymbol = Enum.GetName(typeof(TokenType), TheTokenType); 
            }
        }

        public override string ToString()
        {
            return TheSymbol;
        }
    }
}
