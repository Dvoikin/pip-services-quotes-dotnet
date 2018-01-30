using System.Threading.Tasks;
using Xunit;

namespace PipServices.Quotes.Persistence
{
    public class QuotesMemoryPersistenceTest
    {
        private QuotesMemoryPersistence _persistence;
        private QuotesPersistenceFixture _fixture;

        public QuotesMemoryPersistenceTest()
        {
            _persistence = new QuotesMemoryPersistence();
            _fixture = new QuotesPersistenceFixture(_persistence);
        }

        [Fact]
        public async Task TestMemoryCrudOperationsAsync()
        {
            await _fixture.TestCrudOperationsAsync();
        }

        [Fact]
        public async Task TestMemoryGetByFilterAsync()
        {
            await _fixture.TestGetByFilterAsync();
        }
    }
}
