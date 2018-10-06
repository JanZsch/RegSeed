using Regseed.Common.Results;
using Regseed.Expressions;
using Regseed.Streams;

namespace Regseed.Factories
{
    public interface IRegularExpressionFactory
    {
        IParseResult TryGetRegularExpression(string pattern, out IExpression expression);

        IParseResult TryGetUnionExpression(ITokenStream tokenStream, out IExpression expression);

        IParseResult TryGetConcatenationExpression(ITokenStream tokenStream, out IExpression expression);

        IParseResult TryGetElementaryExpression(ITokenStream tokenStream, out IExpression expression);

        IParseResult TryGetGroupExpression(ITokenStream tokenStream, out IExpression expression);
        
        IParseResult TryGetCharacterClassExpression(ITokenStream tokenStream, out IExpression expression);
        
        IParseResult TryGetSingleCharacterExpression(ITokenStream tokenStream, out IExpression expression);

        IParseResult TryGetAnyCharacterExpression(ITokenStream tokenStream, out IExpression expression);

        IParseResult TryGetComplementElementaryExpression(ITokenStream tokenStream, out IExpression expression);
    }
}