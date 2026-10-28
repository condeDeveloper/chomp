namespace Conde.Chomp;

/// <summary>
/// Uma posição de Chomp: a barra de chocolate que ainda está na mesa, descrita
/// pela altura de cada coluna. As alturas são sempre não crescentes, porque
/// morder um quadrado leva tudo que está em cima e à direita dele, e isso nunca
/// deixa uma coluna mais alta que a anterior.
/// </summary>
public sealed class Posicao : IEquatable<Posicao>
{
    private readonly int[] _alturas;

    public Posicao(params int[] alturas)
    {
        ArgumentNullException.ThrowIfNull(alturas);
        for (var i = 1; i < alturas.Length; i++)
            if (alturas[i] > alturas[i - 1])
                throw new ArgumentException("as alturas têm de ser não crescentes", nameof(alturas));
        foreach (var altura in alturas)
            if (altura < 0)
                throw new ArgumentException("altura negativa", nameof(alturas));

        // Colunas vazias no fim não mudam a posição; tirá-las faz posições iguais
        // terem a mesma chave.
        var fim = alturas.Length;
        while (fim > 0 && alturas[fim - 1] == 0) fim--;
        _alturas = alturas[..fim];
    }

    /// <summary>A barra inteira: <paramref name="colunas"/> colunas de <paramref name="linhas"/> quadrados.</summary>
    public static Posicao Barra(int colunas, int linhas)
    {
        if (colunas < 0 || linhas < 0) throw new ArgumentOutOfRangeException(nameof(colunas));
        var alturas = new int[colunas];
        Array.Fill(alturas, linhas);
        return new Posicao(alturas);
    }

    public IReadOnlyList<int> Alturas => _alturas;

    public int Colunas => _alturas.Length;

    public int Quadrados => _alturas.Sum();

    /// <summary>Só sobrou o quadrado envenenado: quem está na vez é obrigado a comê-lo.</summary>
    public bool SoOVeneno => _alturas.Length == 1 && _alturas[0] == 1;

    public bool Vazia => _alturas.Length == 0;

    /// <summary>
    /// A posição depois de morder o quadrado da coluna c, linha r: some ele e
    /// tudo que está em cima e à direita.
    /// </summary>
    public Posicao Morde(int coluna, int linha)
    {
        if (coluna < 0 || coluna >= Colunas) throw new ArgumentOutOfRangeException(nameof(coluna));
        if (linha < 0 || linha >= _alturas[coluna]) throw new ArgumentOutOfRangeException(nameof(linha));

        var novas = (int[])_alturas.Clone();
        for (var c = coluna; c < novas.Length; c++)
            novas[c] = Math.Min(novas[c], linha);
        return new Posicao(novas);
    }

    /// <summary>Todas as mordidas possíveis, e o que cada uma deixa na mesa.</summary>
    public IEnumerable<(int Coluna, int Linha, Posicao Resultado)> Mordidas()
    {
        for (var c = 0; c < Colunas; c++)
            for (var r = 0; r < _alturas[c]; r++)
                yield return (c, r, Morde(c, r));
    }

    /// <summary>
    /// Uma chave que cabe num long, para a memória da busca. Cada altura usa um
    /// dígito em base 16, então vale até barras de 15 por 15.
    /// </summary>
    public long Chave()
    {
        long chave = 0;
        foreach (var altura in _alturas)
        {
            if (altura > 15) throw new InvalidOperationException("altura acima de 15 não cabe na chave");
            chave = chave * 16 + altura + 1;
        }
        return chave;
    }

    public bool Equals(Posicao? outra) => outra is not null && _alturas.AsSpan().SequenceEqual(outra._alturas);

    public override bool Equals(object? obj) => Equals(obj as Posicao);

    public override int GetHashCode() => Chave().GetHashCode();

    public override string ToString() => _alturas.Length == 0 ? "(vazia)" : string.Join(",", _alturas);
}
