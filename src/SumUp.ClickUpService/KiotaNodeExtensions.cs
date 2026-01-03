using Microsoft.Kiota.Abstractions.Serialization;

namespace ClickUp.Api;

public static class KiotaNodeExtensions
{
    public static string AsString(this UntypedNode node) => 
        node is UntypedString s ? s.GetValue() : null;

    public static IDictionary<string, UntypedNode> AsDictionary(this UntypedNode node) => 
        node is UntypedObject o ? o.GetValue() : null;

    public static IEnumerable<UntypedNode> AsArray(this UntypedNode node) => 
        node is UntypedArray a ? a.GetValue() : null;
}