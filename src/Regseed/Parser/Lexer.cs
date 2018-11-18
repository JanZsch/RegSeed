using System;
using Regseed.Common.Results;
using Regseed.Resources;
using Regseed.Streams;

namespace Regseed.Parser
{
    internal class Lexer : ILexer
    {
        public IParserAlphabet ParserAlphabet { get; }
        
        public Lexer(IParserAlphabet alphabet)
        {
            ParserAlphabet = alphabet ?? throw new ArgumentNullException();
        }

        public IParseResult TryConvertToTokenStream(IStringStream inputStream, out ITokenStream tokenStream)
        {
            if(inputStream == null)
                throw new ArgumentNullException();
            
            tokenStream = new TokenStream();
            
            while (!inputStream.IsEmpty())
            {
                var letter = inputStream.LookAhead(0);
                
                if(!ParserAlphabet.TryGetTokenParser(letter, out var tokenParser) || !ParserAlphabet.IsValid(letter))
                    return new FailureParseResult(inputStream.CurrentPosition, RegSeedErrorType.InvalidInput);
                    
                var parseResult = tokenParser.TryGetToken(inputStream, out var token);

                if (!parseResult.IsSuccess)
                    return parseResult;

                tokenStream.Append(token);
            }
            
            return new SuccessParseResult(inputStream.CurrentPosition);
        }
    }
}