using System.Text;

namespace Krat;

public struct HintGenerator
{
    private static readonly string[] Primary = ["t", "n", "s", "e", "r", "i", "a", "o", "g", "m"];
    private static readonly char[] Secondary = ['p', 'l', 'f', 'u', 'w', 'y', 'b', 'j', 'd', 'h', 'c', 'k', 'x', 'z'];

    // TODO: Melhorar a eficiência dos atalhos - Começar com os menores pelos cantos e mover para o centro de cada tela
    public static IEnumerable<string> New()
    {
        var sb = new StringBuilder();

        // foreach (var s in Secondary)
        // {
        //     foreach (var c in Primary)
        //     {
        //         sb.Clear();
        //         sb.Append(s);
        //         sb.Append(c);
        //         yield return sb.ToString();
        //     }
        // }

        foreach (var s1 in Secondary)
        {
            foreach (var s2 in Secondary)
            {
                if (s1 == s2)
                    continue;

                foreach (var c in Primary)
                {
                    sb.Clear();
                    sb.Append(s1);
                    sb.Append(s2);
                    sb.Append(c);
                    yield return sb.ToString();
                }
            }
        }

        // foreach (var s1 in _secondary)
        // {
        //     foreach (var s2 in _secondary)
        //     {
        //         if (s1 == s2)
        //             continue;
        //
        //         foreach (var s3 in _secondary)
        //         {
        //             foreach (var c in _primary)
        //             {
        //                 sb.Clear();
        //                 sb.Append(s1);
        //                 sb.Append(s2);
        //                 sb.Append(s3);
        //                 sb.Append(c);
        //                 yield return sb.ToString();
        //             }
        //         }
        //     }
        // }
    }
}