using PipServices.Commons.Data;
using PipServices.Quotes.Data.Version1;
using System.Threading.Tasks;
using Xunit;

namespace PipServices.Quotes.Persistence
{
    public class QuotesPersistenceFixture
    {
        private static QuoteV1 QUOTE1 = CreateQuote("1", QuoteStatusV1.New);
        private static QuoteV1 QUOTE2 = CreateQuote("2", QuoteStatusV1.Completed);
        private static QuoteV1 QUOTE3 = CreateQuote("3", QuoteStatusV1.Completed);

        private IQuotesPersistence _persistence;

        public QuotesPersistenceFixture(IQuotesPersistence persistence)
        {
            _persistence = persistence;
        }

        private static QuoteV1 CreateQuote(string id, string status)
        {
            var quote = RandomQuoteV1.Quote();
            quote.Id = id;
            quote.Status = status;
            return quote;
        }

        public async Task TestCrudOperationsAsync()
        {
            // Create one quote
            QuoteV1 quote1 = await _persistence.CreateAsync(null, QUOTE1);

            Assert.NotNull(quote1);
            Assert.Equal(QUOTE1.Id, quote1.Id);
            Assert.Equal(QUOTE1.Tags.Length, quote1.Tags.Length);
            Assert.Equal(QUOTE1.Status, quote1.Status);

            // Create another quote
            QuoteV1 quote2 = await _persistence.CreateAsync(null, QUOTE2);

            Assert.NotNull(quote2);
            Assert.Equal(QUOTE2.Id, quote2.Id);
            Assert.Equal(QUOTE2.Tags.Length, quote2.Tags.Length);
            Assert.Equal(QUOTE2.Status, quote2.Status);

            // Create yet another quote
            QuoteV1 quote3 = await _persistence.CreateAsync(null, QUOTE3);

            Assert.NotNull(quote3);
            Assert.Equal(QUOTE3.Id, quote3.Id);
            Assert.Equal(QUOTE3.Tags.Length, quote3.Tags.Length);
            Assert.Equal(QUOTE3.Status, quote3.Status);

            // Get all quotes
            DataPage<QuoteV1> page = await _persistence.GetPageByFilterAsync(null, null, null);
            Assert.NotNull(page);
            Assert.NotNull(page.Data);
            Assert.Equal(3, page.Data.Count);

            // Update the quote
            quote1.Status = QuoteStatusV1.Writing;
            QuoteV1 quote = await _persistence.UpdateAsync(
                null,
                quote1
            );

            Assert.NotNull(quote);
            Assert.Equal(quote1.Id, quote.Id);
            Assert.Equal(quote1.Tags.Length, quote.Tags.Length);
            Assert.Equal(QuoteStatusV1.Writing, quote.Status);

            // Delete the quote
            await _persistence.DeleteByIdAsync(null, quote1.Id);

            // Try to get deleted quote
            quote = await _persistence.GetOneByIdAsync(null, quote1.Id);
            Assert.Null(quote);
        }

        public async Task TestGetByFilterAsync()
        {
            // Create items
            await _persistence.CreateAsync(null, QUOTE1);
            await _persistence.CreateAsync(null, QUOTE2);
            await _persistence.CreateAsync(null, QUOTE3);

            // Get by id
            FilterParams filter = FilterParams.FromTuples("id", "1");
            DataPage<QuoteV1> page = await _persistence.GetPageByFilterAsync(null, filter, null);
            Assert.Single(page.Data);

            // Get by status
            filter = FilterParams.FromTuples("status", QuoteStatusV1.Completed);
            page = await _persistence.GetPageByFilterAsync(null, filter, null);
            Assert.Equal(2, page.Data.Count);

            // Get by search
            filter = FilterParams.FromTuples("search", QUOTE1.Author["en"]);
            page = await _persistence.GetPageByFilterAsync(null, filter, null);
            Assert.True(page.Data.Count >= 1);
        }
    }
}
