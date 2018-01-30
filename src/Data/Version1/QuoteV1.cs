using PipServices.Commons.Data;
using System.Runtime.Serialization;

namespace PipServices.Quotes.Data.Version1
{
    [DataContract]
    public class QuoteV1 : IStringIdentifiable
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "text")]
        public MultiString Text { get; set; }

        [DataMember(Name = "author")]
        public MultiString Author { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "tags")]
        public string[] Tags { get; set; }

        [DataMember(Name = "all_tags")]
        public string[] AllTags { get; set; }
    }
}
