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
    internal class RegularExpressionFactory
    {
        private readonly IParserAlphabet _alphabet;
        private readonly IRandomGenerator _random;
        private readonly IDictionary<RegexTokenType, TryGetExpression> _regexTokenToExpressionMapper;
        private readonly int _maxCharClassInverseLength;

        public RegularExpressionFactory(IParserAlphabet alphabet, IRandomGenerator random, int maxCharClassInverseLength)
        {
            _random = random;
            _alphabet = alphabet;
            _maxCharClassInverseLength = maxCharClassInverseLength;
            
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

        public IParseResult<ExpressionMetaData> TryGetRegularExpression(string pattern, out IExpression expression)
        {
            expression = null;
            var lexer = new Lexer(_alphabet);
            var inputStream = new StringStream(pattern);

            var parseResult = lexer.TryCreateTokenStream(inputStream, out var tokenStream);

            if (!parseResult.IsSuccess)
                return new FailureParseResult<ExpressionMetaData>(parseResult.Position, parseResult.ErrorType);

            tokenStream.Append(new EndOfStreamToken(pattern.Length));

            var unionResult = TryGetUnionExpression(tokenStream, out var regex);

            if (!unionResult.IsSuccess)
                return unionResult;

            if (tokenStream.LookAhead(0).GetType<RegexTokenType>() != RegexTokenType.EndOfStream)
                return new FailureParseResult<ExpressionMetaData>(tokenStream.CurrentPosition, RegSeedErrorType.UnionExpressionExpected);

            expression = regex;
            return unionResult;
        }

        public IParseResult<ExpressionMetaData> TryGetUnionExpression(ITokenStream tokenStream, out IExpression unionExpression)
        {
            unionExpression = new EmptyExpression();
            if (tokenStream.LookAhead(0).GetType<RegexTokenType>() == RegexTokenType.EndOfStream)
                return new SuccessParseResult<ExpressionMetaData>(tokenStream.CurrentPosition, new ExpressionMetaData());

            var intersectExpressionResult = TryGetIntersectionExpression(tokenStream, out var concatExpression);

            if (!intersectExpressionResult.IsSuccess)
                return intersectExpressionResult;

            var intersectExpressions = new List<IExpression> {concatExpression};

            var nextToken = tokenStream.LookAhead(0).GetType<RegexTokenType>();
            var metaData = intersectExpressionResult.Value;
            
            while (nextToken == RegexTokenType.Union)
            {
                tokenStream.Pop();

                var nextConcatExpressionResult = TryGetIntersectionExpression(tokenStream, out var nextConcatExpression);

                if (!nextConcatExpressionResult.IsSuccess)
                    return nextConcatExpressionResult;

                metaData.UpdateWith(nextConcatExpressionResult.Value);
                intersectExpressions.Add(nextConcatExpression);
                nextToken = tokenStream.LookAhead(0).GetType<RegexTokenType>();
            }

            unionExpression = new UnionExpression(intersectExpressions, _random);
            return new SuccessParseResult<ExpressionMetaData>(tokenStream.CurrentPosition, metaData);
        }

        public IParseResult<ExpressionMetaData> TryGetIntersectionExpression(ITokenStream tokenStream, out IExpression intersectionExpression)
        {
            intersectionExpression = new EmptyExpression();
            if (tokenStream.LookAhead(0).GetType<RegexTokenType>() == RegexTokenType.EndOfStream)
                return new SuccessParseResult<ExpressionMetaData>(tokenStream.CurrentPosition, new ExpressionMetaData());

            var concatExpressionResult = TryGetConcatenationExpression(tokenStream, out var concatExpression);

            if (!concatExpressionResult.IsSuccess)
                return concatExpressionResult;

            var metaData = concatExpressionResult.Value;
            var concatExpressions = new List<IExpression> {concatExpression};

            var nextToken = tokenStream.LookAhead(0).GetType<RegexTokenType>();

            while (nextToken == RegexTokenType.Intersection)
            {
                metaData.HasIntersection = true;
                tokenStream.Pop();

                var nextConcatExpressionResult = TryGetConcatenationExpression(tokenStream, out var nextConcatExpression);

                if (!nextConcatExpressionResult.IsSuccess)
                    return nextConcatExpressionResult;

                metaData.UpdateWith(nextConcatExpressionResult.Value);
                concatExpressions.Add(nextConcatExpression);
                nextToken = tokenStream.LookAhead(0).GetType<RegexTokenType>();
            }

            intersectionExpression = new IntersectionExpression(concatExpressions, _random);
            return new SuccessParseResult<ExpressionMetaData>(tokenStream.CurrentPosition, metaData);
        }

        public IParseResult<ExpressionMetaData> TryGetConcatenationExpression(ITokenStream tokenStream, out IExpression expression)
        {
            expression = null;
            var concatExpression = new ConcatenationExpression(_random);

            var elementaryExpressionResult = TryGetElementaryExpression(tokenStream, out var elementaryExpression);

            if (!elementaryExpressionResult.IsSuccess)
                return elementaryExpressionResult;

            var metaData = elementaryExpressionResult.Value;
            elementaryExpression.RepeatRange = GetRepeatConcatenationInterval(tokenStream);
            concatExpression.Append(elementaryExpression);

            var result = TryGetElementaryExpression(tokenStream, out elementaryExpression);
            while (result.IsSuccess)
            {
                metaData.UpdateWith(result.Value);
                elementaryExpression.RepeatRange = GetRepeatConcatenationInterval(tokenStream);
                concatExpression.Append(elementaryExpression);
                result = TryGetElementaryExpression(tokenStream, out elementaryExpression);
            }

            expression = concatExpression;
            return new SuccessParseResult<ExpressionMetaData>(tokenStream.CurrentPosition, metaData);
        }

        public IParseResult<ExpressionMetaData> TryGetElementaryExpression(ITokenStream tokenStream, out IExpression expression)
        {
            expression = null;
            var nextToken = tokenStream.LookAhead(0).GetType<RegexTokenType>();

            if (!_regexTokenToExpressionMapper.TryGetValue(nextToken, out var tryGetElementaryExpression))
                return new FailureParseResult<ExpressionMetaData>(tokenStream.CurrentPosition, RegSeedErrorType.CharacterTypeExpressionExpected);

            return tryGetElementaryExpression(tokenStream, out expression);
        }

        public IParseResult<ExpressionMetaData> TryGetComplementElementaryExpression(ITokenStream tokenStream, out IExpression expression)
        {
            expression = null;
            tokenStream.Pop();
            var elementaryResult = TryGetElementaryExpression(tokenStream, out var toInvertExpression);

            if (!elementaryResult.IsSuccess)
                return elementaryResult;

            expression = toInvertExpression.GetInverse();

            return new SuccessParseResult<ExpressionMetaData>(tokenStream.CurrentPosition, elementaryResult.Value);
        }

        public IParseResult<ExpressionMetaData> TryGetAnyCharacterExpression(ITokenStream tokenStream, out IExpression expression)
        {
            tokenStream.Pop();
            var characterExpression = new CharacterClassExpression(_alphabet, _random, _maxCharClassInverseLength);
            characterExpression.AddCharacters(_alphabet.GetAllCharacters());
            expression = characterExpression;
            return new SuccessParseResult<ExpressionMetaData>(tokenStream.CurrentPosition, new ExpressionMetaData());
        }

        public IParseResult<ExpressionMetaData> TryGetGroupExpression(ITokenStream tokenStream, out IExpression expression)
        {
            tokenStream.Pop();

            var closePrecedenceToken = tokenStream.LookAhead(0).GetType<RegexTokenType>();

            if (closePrecedenceToken == RegexTokenType.ClosePrecedence)
            {
                tokenStream.Pop();
                expression = new EmptyExpression();
                return new SuccessParseResult<ExpressionMetaData>(tokenStream.CurrentPosition, new ExpressionMetaData());
            }

            var unionExpressionResult = TryGetUnionExpression(tokenStream, out expression);

            if (!unionExpressionResult.IsSuccess)
                return unionExpressionResult;

            closePrecedenceToken = tokenStream.LookAhead(0).GetType<RegexTokenType>();

            if (closePrecedenceToken != RegexTokenType.ClosePrecedence)
                return new FailureParseResult<ExpressionMetaData>(tokenStream.CurrentPosition, RegSeedErrorType.ClosePrecedenceExpected);

            tokenStream.Pop();
            return new SuccessParseResult<ExpressionMetaData>(tokenStream.CurrentPosition, unionExpressionResult.Value);
        }

        public IParseResult<ExpressionMetaData> TryGetCharacterClassExpression(ITokenStream tokenStream, out IExpression expression)
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
                    characters.AddRange(nextToken.GetValue<CharacterRange>().Characters);

                nextTokenType = tokenStream.LookAhead(0).GetType<RegexTokenType>();
            }

            if (nextTokenType != RegexTokenType.CloseCharacterClass)
                return new FailureParseResult<ExpressionMetaData>(tokenStream.CurrentPosition, RegSeedErrorType.CloseCharacterClassExpected);

            tokenStream.Pop();

            var characterClassExpression = new CharacterClassExpression(_alphabet, _random, _maxCharClassInverseLength);

            characterClassExpression.AddCharacters(characters);

            expression = getComplement ? characterClassExpression.GetInverse() : characterClassExpression;
            return new SuccessParseResult<ExpressionMetaData>(tokenStream.CurrentPosition, new ExpressionMetaData());
        }

        public IParseResult<ExpressionMetaData> TryGetSingleCharacterExpression(ITokenStream tokenStream, out IExpression expression)
        {
            expression = null;
            var token = tokenStream.Pop();
            var value = token.GetValue<string>();
            var characterClassExpression = new CharacterClassExpression(_alphabet, _random, _maxCharClassInverseLength);

            characterClassExpression.AddCharacters(new List<string> {value});
                
            expression = characterClassExpression;

            return new SuccessParseResult<ExpressionMetaData>(tokenStream.CurrentPosition, new ExpressionMetaData());
        }

        private static IntegerInterval GetRepeatConcatenationInterval(ITokenStream tokenStream)
        {
            var nextToken = tokenStream.LookAhead(0).GetType<RegexTokenType>();

            return nextToken != RegexTokenType.IntegerRange
                ? new IntegerInterval(1)
                : tokenStream.Pop().GetValue<IntegerInterval>();
        }

        private delegate IParseResult<ExpressionMetaData> TryGetExpression(ITokenStream stream, out IExpression expression);
    }
}