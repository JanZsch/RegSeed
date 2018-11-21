using System;
using Regseed.Common.Results;
using Regseed.Parser.ParserFactories;
using Regseed.Resources;
using Regseed.Streams;

namespace Regseed.Parser
{
    internal class Lexer : ILexer
    {
        private readonly IParserFactory _parserFactory;
               
        public Lexer(IParserFactory factory)
        {
             _parserFactory = factory ?? throw new ArgumentNullException();
        }

        public IParseResult TryConvertToTokenStream(IStringStream inputStream, out ITokenStream tokenStream)
        {
            if(inputStream == null)
                throw new ArgumentNullException();
            
            tokenStream = new TokenStream();
            
            while (!inputStream.IsEmpty())
            {
                var character = inputStream.LookAhead(0);
                
                if(!_parserFactory.TryGetTokenParser(character, out var tokenParser))
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