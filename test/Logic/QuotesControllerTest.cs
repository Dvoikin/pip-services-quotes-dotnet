using PipServices.Commons.Data;
using PipServices.Commons.Refer;
using PipServices.Quotes.Data.Version1;
using PipServices.Quotes.Persistence;
using System.Threading.Tasks;
using Xunit;

namespace PipServices.Quotes.Logic
{
    public class QuotesControllerTest
    {
        private static QuoteV1 QUOTE1 = CreateQuote("1", QuoteStatusV1.New);
        private static QuoteV1 QUOTE2 = CreateQuote("2", QuoteStatusV1.Completed);

        private QuotesMemoryPersistence _persistence;
        private QuotesController _controller;

        public QuotesControllerTest()
        {
            _persistence = new QuotesMemoryPersistence();
            _controller = new QuotesController();

            var references = References.FromTuples(
                new Descriptor("pip-services-quotes", "persistence", "memory", "default", "1.0"), _persistence
            );
            _controller.SetReferences(references);
        }

        private static QuoteV1 CreateQuote(string id, string status)
        {
            var quote = RandomQuoteV1.Quote();
            quote.Id = id;
            quote.Status = status;
            return quote;
        }

        [Fact]
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

            // Get all quotes
            DataPage<QuoteV1> page = await _persistence.GetPageByFilterAsync(null, null, null);
            Assert.NotNull(page);
            Assert.NotNull(page.Data);
            Assert.Equal(2, page.Data.Count);

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
    }
}
