using System.Collections.Generic;
using Regseed.Common.Random;
using Regseed.Common.Ranges;
using Regseed.Common.Results;
using Regseed.Expressions;
using Regseed.Parser;
using Regseed.Parser.RegexTokens;
using Regseed.Resources;
using Regseed.Streams;

namespace Regseed.Factories
{
    internal class RegularExpressionFactory : IRegularExpressionFactory
    {
        private readonly IParserAlphabet _alphabet;
        private readonly IRandomGenerator _random;
        private readonly IDictionary<RegexTokenType, TryGetExpression> _regexTokenToExpressionMapper;


        public RegularExpressionFactory(IParserAlphabet alphabet, IRandomGenerator random)
        {
            _random = random;
            _alphabet = alphabet;

            _regexTokenToExpressionMapper = new Dictionary<RegexTokenType, TryGetExpression>
            {
                {RegexTokenType.Complement, TryGetComplementElementaryExpression},
                {RegexTokenType.OpenPrecedence, TryGetGroupExpression},
                {RegexTokenType.OpenCharacterClass, TryGetCharacterClassExpression},
                {RegexTokenType.OpenNegateCharacterClass, TryGetCharacterClassExpression},
                {RegexTokenType.AnyCharacter, TryGetAnyCharacterExpression},
                {RegexTokenType.Character, TryGetSingleCharacterExpression}
            };
        }

        public IParseResult TryGetRegularExpression(string pattern, out IExpression expression)
        {
            expression = null;
            var lexer = new Lexer(_alphabet);
            var inputStream = new StringStream(pattern);

            var parseResult = lexer.TryCreateTokenStream(inputStream, out var tokenStream);

            if (!parseResult.IsSuccess)
                return parseResult;
            
            tokenStream.Append(new EndOfStreamToken(pattern.Length));

            var unionResult = TryGetUnionExpression(tokenStream, out var regex);

            if (!unionResult.IsSuccess)
                return unionResult;

            if (tokenStream.LookAhead(0).GetType<RegexTokenType>() != RegexTokenType.EndOfStream)
                return new FailureParseResult(tokenStream.CurrentPosition, RegSeedErrorType.UnionExpressionExpected);

            expression = regex;
            return new SuccessParseResult();
        }

        public IParseResult TryGetUnionExpression(ITokenStream tokenStream, out IExpression unionExpression)
        {
            unionExpression = new EmptyExpression();
            if (tokenStream.LookAhead(0).GetType<RegexTokenType>() == RegexTokenType.EndOfStream)
                return new SuccessParseResult();

            var concatExpressionResult = TryGetConcatenationExpression(tokenStream, out var concatExpression);

            if (!concatExpressionResult.IsSuccess)
                return concatExpressionResult;

            var simpleExpressions = new List<IExpression> {concatExpression};

            var nextToken = tokenStream.LookAhead(0).GetType<RegexTokenType>();

            while (nextToken == RegexTokenType.Union)
            {
                tokenStream.Pop();

                var nextConcatExpressionResult = TryGetConcatenationExpression(tokenStream, out var nextConcatExpression);

                if (!nextConcatExpressionResult.IsSuccess)
                    return nextConcatExpressionResult;

                simpleExpressions.Add(nextConcatExpression);
                nextToken = tokenStream.LookAhead(0).GetType<RegexTokenType>();
            }

            unionExpression = new UnionExpression(simpleExpressions, _random);
            return new SuccessParseResult();
        }

        public IParseResult TryGetConcatenationExpression(ITokenStream tokenStream, out IExpression expression)
        {
            expression = null;
            var concatExpression = new ConcatenationExpression(_random);

            var elementaryExpressionResult = TryGetElementaryExpression(tokenStream, out var elementaryExpression);
            
            if (!elementaryExpressionResult.IsSuccess)
                return elementaryExpressionResult;

            elementaryExpression.RepeatRange = GetRepeatConcatenationInterval(tokenStream);
            concatExpression.Append(elementaryExpression);

            while (TryGetElementaryExpression(tokenStream, out elementaryExpression).IsSuccess)
            {
                elementaryExpression.RepeatRange = GetRepeatConcatenationInterval(tokenStream);
                concatExpression.Append(elementaryExpression);
            }

            expression = concatExpression;
            return new SuccessParseResult();
        }

        public IParseResult TryGetElementaryExpression(ITokenStream tokenStream, out IExpression expression)
        {
            expression = null;
            var nextToken = tokenStream.LookAhead(0).GetType<RegexTokenType>();

            if (!_regexTokenToExpressionMapper.TryGetValue(nextToken, out var tryGetElementaryExpression))
                return new FailureParseResult(tokenStream.CurrentPosition, RegSeedErrorType.CharacterTypeExpressionExpected);

            return tryGetElementaryExpression(tokenStream, out expression);
        }

        public IParseResult TryGetComplementElementaryExpression(ITokenStream tokenStream, out IExpression expression)
        {
            expression = null;
            tokenStream.Pop();
            var elementaryResult = TryGetElementaryExpression(tokenStream, out var toInvertExpression);

            if (!elementaryResult.IsSuccess)
                return elementaryResult;

            expression = toInvertExpression.GetComplement();
            
            return new SuccessParseResult();
        }

        public IParseResult TryGetAnyCharacterExpression(ITokenStream tokenStream, out IExpression expression)
        {
            tokenStream.Pop();
            expression = new CharacterClassExpression(_alphabet.GetAllCharacters(), _alphabet, _random);
            return new SuccessParseResult();
        }

        public IParseResult TryGetGroupExpression(ITokenStream tokenStream, out IExpression expression)
        {
            tokenStream.Pop();

            var closePrecedenceToken = tokenStream.LookAhead(0).GetType<RegexTokenType>();

            if (closePrecedenceToken == RegexTokenType.ClosePrecedence)
            {
                tokenStream.Pop();
                expression = new EmptyExpression();
                return new SuccessParseResult();
            }

            var unionExpressionResult = TryGetUnionExpression(tokenStream, out expression);

            if (!unionExpressionResult.IsSuccess)
                return unionExpressionResult;

            closePrecedenceToken = tokenStream.LookAhead(0).GetType<RegexTokenType>();

            if (closePrecedenceToken != RegexTokenType.ClosePrecedence)
                return new FailureParseResult(tokenStream.CurrentPosition, RegSeedErrorType.ClosePrecedenceExpected);

            tokenStream.Pop();
            return new SuccessParseResult();
        }

        public IParseResult TryGetCharacterClassExpression(ITokenStream tokenStream, out IExpression expression)
        {
            expression = null;

            var nextTokenType = tokenStream.Pop().GetType<RegexTokenType>();

            var getComplement = nextTokenType == RegexTokenType.OpenNegateCharacterClass;

            nextTokenType = tokenStream.LookAhead(0).GetType<RegexTokenType>();

            var characters = new List<string>();

            while (nextTokenType == RegexTokenType.Character || nextTokenType == RegexTokenType.CharacterRange)
            {
                var nextToken = tokenStream.Pop();

                if (nextToken.GetType<RegexTokenType>() == RegexTokenType.Character)
                    characters.Add(nextToken.GetValue<string>());
                else
                    characters.AddRange(nextToken.GetValue<CharacterRange>().Letters);

                nextTokenType = tokenStream.LookAhead(0).GetType<RegexTokenType>();
            }

            if (nextTokenType != RegexTokenType.CloseCharacterClass)
                return new FailureParseResult(tokenStream.CurrentPosition, RegSeedErrorType.CloseCharacterClassExpected);

            tokenStream.Pop();

            var characterClassExpression = new CharacterClassExpression(characters, _alphabet, _random);

            expression = getComplement ? characterClassExpression.GetComplement() : characterClassExpression;
            return new SuccessParseResult();
        }

        public IParseResult TryGetSingleCharacterExpression(ITokenStream tokenStream, out IExpression expression)
        {
            var token = tokenStream.Pop();
            var value = token.GetValue<string>();
            expression = new CharacterClassExpression(new List<string> {value}, _alphabet, _random);
            return new SuccessParseResult();
        }

        private static IntegerInterval GetRepeatConcatenationInterval(ITokenStream tokenStream)
        {
            var nextToken = tokenStream.LookAhead(0).GetType<RegexTokenType>();

            return nextToken != RegexTokenType.IntegerRange
                ? new IntegerInterval(1, 1)
                : tokenStream.Pop().GetValue<IntegerInterval>();
        }

        private delegate IParseResult TryGetExpression(ITokenStream stream, out IExpression expression);
    }
}