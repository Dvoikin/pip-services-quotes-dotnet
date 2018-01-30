using MongoDB.Driver;
using PipServices.Commons.Data;
using PipServices.Data.MongoDb;
using PipServices.Quotes.Data.Version1;
using System.Linq;
using System.Threading.Tasks;

namespace PipServices.Quotes.Persistence
{
    public class QuotesMongoDbPersistence : IdentifiableMongoDbPersistence<QuoteV1, string>, IQuotesPersistence
    {
        public QuotesMongoDbPersistence()
            : base("quotes")
        { }

        private FilterDefinition<QuoteV1> ComposeFilter(FilterParams filterParams)
        {
            filterParams = filterParams ?? new FilterParams();
            var search = filterParams.GetAsNullableString("search");
            var tags = filterParams.GetAsObject("tags");

            var id = filterParams.GetAsNullableString("id");
            var status = filterParams.GetAsNullableString("status");
            var author = filterParams.GetAsNullableString("author");

            var builder = Builders<QuoteV1>.Filter;
            var filter = builder.Empty;

            if (id != null) filter &= builder.Eq(q => q.Id, id);

            if (status != null) filter &= builder.Eq(q => q.Status, status);

            if (author != null)
            {
                filter &= builder.Eq(q => q.Author["en"], author)
                    | builder.Eq(q => q.Author["fr"], author)
                    | builder.Eq(q => q.Author["sp"], author)
                    | builder.Eq(q => q.Author["de"], author)
                    | builder.Eq(q => q.Author["pt"], author)
                    | builder.Eq(q => q.Author["ru"], author);
            }

            if (!string.IsNullOrEmpty(search))
            {
                var searchFilter = builder.Where(q => q.Text.Any(l => l.Value.ToLower().Contains(search)));
                searchFilter |= builder.Where(q => q.Author.Any(l => l.Value.ToLower().Contains(search)));
                searchFilter |= builder.Where(q => q.Status.ToLower().Contains(search));
                filter &= searchFilter;
            }

            return filter;
        }

        public Task<DataPage<QuoteV1>> GetPageByFilterAsync(string correlationId, FilterParams filterParams, PagingParams paging)
        {
            return base.GetPageByFilterAsync(correlationId, ComposeFilter(filterParams), paging);
        }

        public Task<QuoteV1> GetOneRandomAsync(string correlationId, FilterParams filterParams)
        {
            return base.GetOneRandomAsync(correlationId, ComposeFilter(filterParams));
        }
    }
}
