﻿using PipServices.Commons.Data;
using PipServices.Data;
using PipServices.Quotes.Data;

using System.Threading.Tasks;

namespace PipServices.Quotes.Persistence
{
    public interface IQuotesPersistence : IGetter<QuoteV1, string>, IWriter<QuoteV1, string>
    {
        Task<QuoteV1[]> GetPageByFilterAsync(string correlationId, FilterParams filter, PagingParams paging);

        Task<QuoteV1> GetOneRandomAsync(string correlationId, FilterParams filter);
    }
}
