using Conde.Chomp;
using Xunit;

namespace Conde.Chomp.Testes;

/// <summary>Todo número do README está preso aqui.</summary>
public class MedidasTestes
{
    [Theory]
    [InlineData(1, 0, 0L)]
    [InlineData(2, 1, 4L)]
    [InlineData(3, 1, 17L)]
    [InlineData(4, 1, 47L)]
    [InlineData(5, 1, 138L)]
    [InlineData(6, 1, 378L)]
    [InlineData(7, 1, 1_453L)]
    public void CadaLinhaDosQuadrados(int lado, int lancesVencedores, long posicoes)
    {
        var linha = Medidas.Quadrados().Single(l => l.Colunas == lado);
        Assert.Equal(lancesVencedores, linha.LancesVencedores);
        Assert.Equal(posicoes, linha.Posicoes);
        Assert.Equal(lado > 1, linha.PrimeiroGanha);
        if (lado > 1) Assert.Equal("1,1", linha.Primeiro);
    }

    [Fact]
    public void OPrimeiroGanhaEmTodaBarraMenosNoQuadradoDeUm()
    {
        foreach (var linha in Medidas.Barras())
            Assert.Equal(linha.Colunas != 1 || linha.Linhas != 1, linha.PrimeiroGanha);
    }

    [Fact]
    public void EmTodaBarraOLanceVencedorEhUnico()
    {
        var (barras, semLance, comUm, comMais) = Medidas.LanceUnico();
        Assert.Equal(49, barras);
        Assert.Equal(1, semLance);
        Assert.Equal(48, comUm);
        Assert.Equal(0, comMais);
    }

    [Fact]
    public void OArgumentoDoRouboConfereEmTodasAsTrintaESeisBarras()
    {
        var (barras, cantoGanha, conferem) = Medidas.OCantoBasta();
        Assert.Equal(36, barras);
        Assert.Equal(36, conferem);
        Assert.Equal(11, cantoGanha);
    }

    [Fact]
    public void MorderSoOCantoBastaEmOnzeDasTrintaESeis()
    {
        // E as onze são exatamente as que têm um lado igual a 2.
        foreach (var linha in Medidas.Roubos().Where(l => l.CantoJaGanha))
            Assert.True(linha.Colunas == 2 || linha.Linhas == 2, $"{linha.Colunas} por {linha.Linhas}");
    }

    [Fact]
    public void AsPerdedorasDeDuasLinhasSaoSempreAEscada()
    {
        var perdedoras = Medidas.PerdedorasDeDuasLinhas();
        Assert.Equal(10, perdedoras.Count);
        foreach (var linha in perdedoras)
            Assert.Equal(1, linha.Baixas);
    }

    [Theory]
    [InlineData(1, 1, 1L, 1L)]
    [InlineData(2, 2, 12L, 16L)]
    [InlineData(3, 3, 44L, 125L)]
    [InlineData(4, 4, 136L, 2_132L)]
    public void CadaLinhaDoJuiz(int colunas, int linhas, long comMemoria, long semMemoria)
    {
        var linha = Medidas.ContraOJuiz().Single(l => l.Colunas == colunas && l.Linhas == linhas);
        Assert.Equal(comMemoria, linha.NosComMemoria);
        Assert.Equal(semMemoria, linha.NosSemMemoria);
        Assert.True(linha.Bate);
    }

    [Fact]
    public void AForcaBrutaEAMemoriaNuncaDiscordam()
    {
        foreach (var linha in Medidas.ContraOJuiz())
            Assert.True(linha.Bate, $"{linha.Colunas} por {linha.Linhas}");
    }

    [Fact]
    public void AVantagemDaMemoriaCresceComABarra()
    {
        var linhas = Medidas.ContraOJuiz();
        Assert.Equal(1.0, linhas.Single(l => l is { Colunas: 1, Linhas: 1 }).Vezes, 1);
        Assert.Equal(15.7, linhas.Single(l => l is { Colunas: 4, Linhas: 4 }).Vezes, 1);
    }

    [Fact]
    public void ABarraDeDozePorDozeCabeNaMemoria()
    {
        var (colunas, linhas, posicoes, _) = Medidas.AMaiorQueCabe();
        Assert.Equal(12, colunas);
        Assert.Equal(12, linhas);
        Assert.Equal(577_688L, posicoes);
    }

    [Fact]
    public void OPrimeiroLanceDaBarraDeDuasColunasComeOTopoDaSegunda()
    {
        foreach (var linha in Medidas.Barras().Where(l => l.Colunas == 2 && l.Linhas >= 2))
            Assert.Equal($"1,{linha.Linhas - 1}", linha.Primeiro);
    }
}
