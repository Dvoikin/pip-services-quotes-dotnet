using PipServices.Commons.Data;
using PipServices.Commons.Random;

namespace PipServices.Quotes.Data.Version1
{
    public static class RandomQuoteV1
    {
        public static MultiString MultiString()
        {
            var value = new MultiString();
            value.Put("en", RandomText.Text(15, 50));
            return value;
        }

        public static string Status()
        {
            return RandomArray.Pick(
                new string[] {
                    QuoteStatusV1.New, QuoteStatusV1.Writing, QuoteStatusV1.Translating,
                    QuoteStatusV1.Verifying, QuoteStatusV1.Completed
                }
            );
        }

        public static string[] Tags()
        {
            var count = RandomInteger.NextInteger(0, 5);
            var tags = new string[count];

            for (var index = 0; index < count; index++)
                tags[index] = RandomText.Word().ToLower();

            return tags;
        }

        public static QuoteV1 Quote()
        {
            var tags = Tags();

            return new QuoteV1
            {
                Id = IdGenerator.NextLong(),
                Status = Status(),
                Author = MultiString(),
                Text = MultiString(),
                Tags = tags,
                AllTags = tags
            };
        }
    }
}
