using PipServices.Commons.Config;
using System.Threading.Tasks;
using Xunit;

namespace PipServices.Quotes.Persistence
{
    public class QuotesFilePersistenceTest
    {
        private QuotesFilePersistence _persistence;
        private QuotesPersistenceFixture _fixture;

        public QuotesFilePersistenceTest()
        {
            ConfigParams config = ConfigParams.FromTuples(
                "path", "quotes.json"
            );
            _persistence = new QuotesFilePersistence();
            _persistence.Configure(config);
            _persistence.OpenAsync(null).Wait();
            _persistence.ClearAsync(null).Wait();

            _fixture = new QuotesPersistenceFixture(_persistence);
        }

        [Fact]
        public async Task TestFileCrudOperationsAsync()
        {
            await _fixture.TestCrudOperationsAsync();
        }

        [Fact]
        public async Task TestFileGetByFilterAsync()
        {
            await _fixture.TestGetByFilterAsync();
        }
    }
}
