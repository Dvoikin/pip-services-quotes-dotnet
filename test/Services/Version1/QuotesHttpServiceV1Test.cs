using PipServices.Commons.Config;
using PipServices.Commons.Convert;
using PipServices.Commons.Data;
using PipServices.Commons.Refer;
using PipServices.Quotes.Data.Version1;
using PipServices.Quotes.Logic;
using PipServices.Quotes.Persistence;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PipServices.Quotes.Services.Version1
{
    public class QuotesHttpServiceV1Test: IDisposable
    {
        private static QuoteV1 QUOTE1 = CreateQuote("1", QuoteStatusV1.New);
        private static QuoteV1 QUOTE2 = CreateQuote("2", QuoteStatusV1.Completed);

        private QuotesMemoryPersistence _persistence;
        private QuotesController _controller;
        private QuotesHttpServiceV1 _service;

        public QuotesHttpServiceV1Test()
        {
            _persistence = new QuotesMemoryPersistence();
            _controller = new QuotesController();

            var config = ConfigParams.FromTuples(
                "connection.protocol", "http",
                "connection.host", "localhost",
                "connection.port", "3000"
            );
            _service = new QuotesHttpServiceV1();
            _service.Configure(config);

            var references = References.FromTuples(
                new Descriptor("pip-services-quotes", "persistence", "memory", "default", "1.0"), _persistence,
                new Descriptor("pip-services-quotes", "controller", "default", "default", "1.0"), _controller,
                new Descriptor("pip-services-quotes", "service", "http", "default", "1.0"), _service
            );

            _controller.SetReferences(references);

            _service.SetReferences(references);
            //_service.OpenAsync(null).Wait();

            // Todo: This is defect! Open shall not block the tread
            Task.Run(() => _service.OpenAsync(null));
            Thread.Sleep(1000); // Just let service a sec to be initialized
        }

        public void Dispose()
        {
            _service.CloseAsync(null).Wait();
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
            QuoteV1 quote1 = await Invoke<QuoteV1>("/quotes/create_quote", new { quote = QUOTE1 });

            Assert.NotNull(quote1);
            Assert.Equal(QUOTE1.Id, quote1.Id);
            Assert.Equal(QUOTE1.Tags.Length, quote1.Tags.Length);
            Assert.Equal(QUOTE1.Status, quote1.Status);

            // Create another quote
            QuoteV1 quote2 = await Invoke<QuoteV1>("/quotes/create_quote", new { quote = QUOTE2 });

            Assert.NotNull(quote2);
            Assert.Equal(QUOTE2.Id, quote2.Id);
            Assert.Equal(QUOTE2.Tags.Length, quote2.Tags.Length);
            Assert.Equal(QUOTE2.Status, quote2.Status);

            // Get all quotes
            DataPage<QuoteV1> page = await Invoke<DataPage<QuoteV1>>("/quotes/get_quotes", new { });
            Assert.NotNull(page);
            Assert.NotNull(page.Data);
            Assert.Equal(2, page.Data.Count);

            // Update the quote
            quote1.Status = QuoteStatusV1.Writing;
            QuoteV1 quote = await Invoke<QuoteV1>("/quotes/update_quote", new { quote = quote1 });

            Assert.NotNull(quote);
            Assert.Equal(quote1.Id, quote.Id);
            Assert.Equal(quote1.Tags.Length, quote.Tags.Length);
            Assert.Equal(QuoteStatusV1.Writing, quote.Status);

            // Delete the quote
            await Invoke<QuoteV1>("/quotes/delete_quote_by_id", new { quote_id = quote1.Id });

            // Try to get deleted quote
            quote = await Invoke<QuoteV1>("/quotes/get_quote_by_id", new { quote_id = quote1.Id });
            Assert.Null(quote);
        }

        private static async Task<T> Invoke<T>(string route, dynamic request)
        {
            using (var httpClient = new HttpClient())
            {
                var requestValue = JsonConverter.ToJson(request);
                using (var content = new StringContent(requestValue, Encoding.UTF8, "application/json"))
                {
                    var response = await httpClient.PostAsync("http://localhost:3000" + route, content);
                    var responseValue = response.Content.ReadAsStringAsync().Result;
                    return JsonConverter.FromJson<T>(responseValue);
                }
            }
        }

    }
}
