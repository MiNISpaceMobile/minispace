using Domain.DataModel;
using System.Text.Json.Serialization;

namespace Api.DTO.Posts;

public class ReactionsSummary
{
    [JsonExtensionData]
    public IDictionary<string, object> ReactionCounts { get; set; }

    public ReactionsSummary(IEnumerable<Reaction> reactions)
    {
        ReactionCounts = new Dictionary<string, object>();
        foreach (var reaction in reactions)
        {
            int count = 0;
            string key = reaction.Type.ToString();
            if (ReactionCounts.TryGetValue(key, out object? val))
                count = (int)val;
            ReactionCounts[key] = count + 1;
        }
    }
}
