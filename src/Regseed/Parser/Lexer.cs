using System;
using Regseed.Common.Results;
using Regseed.Parser.ParserFactories;
using Regseed.Parser.RegexTokens;
using Regseed.Resources;
using Regseed.Streams;

namespace Regseed.Parser
{
    internal class Lexer : ILexer
    {
        private readonly IParserFactory _parserFactory;

        public Lexer(IParserAlphabet alphabet)
        {
            _parserFactory = new StatefulParserFactory(alphabet ?? throw new ArgumentNullException());
        }

        public Lexer(IParserFactory parserFactory)
        {
            _parserFactory = parserFactory ?? throw new ArgumentNullException();
        }
        
        public IParseResult TryConvertToTokenStream(string pattern, out ITokenStream tokenStream)
        {
            var inputStream = new StringStream(pattern);

            var parseResult = TryConvertToTokenStream(inputStream, out tokenStream);

            if (!parseResult.IsSuccess)
                return new FailureParseResult(parseResult.Position, parseResult.ErrorType);

            tokenStream.Append(new EndOfStreamToken(pattern.Length));

            return parseResult;
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