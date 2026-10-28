using Conde.Chomp;
using Xunit;

namespace Conde.Chomp.Testes;

public class PosicaoTestes
{
    [Fact]
    public void ABarraTemTodasAsColunasDaMesmaAltura()
    {
        var barra = Posicao.Barra(4, 3);
        Assert.Equal(4, barra.Colunas);
        Assert.Equal(12, barra.Quadrados);
        Assert.Equal("3,3,3,3", barra.ToString());
    }

    [Fact]
    public void AlturaCrescenteNaoEhPosicaoDeChomp()
    {
        Assert.Throws<ArgumentException>(() => new Posicao(1, 2));
        Assert.Throws<ArgumentException>(() => new Posicao(2, -1));
    }

    [Fact]
    public void ColunaVaziaNoFimNaoMudaAPosicao()
    {
        Assert.Equal(new Posicao(3, 2), new Posicao(3, 2, 0, 0));
        Assert.Equal(new Posicao(3, 2).Chave(), new Posicao(3, 2, 0).Chave());
    }

    [Fact]
    public void MorderLevaTudoQueEstaEmCimaEADireita()
    {
        var barra = Posicao.Barra(3, 3);
        Assert.Equal("3,1,1", barra.Morde(1, 1).ToString());
        Assert.Equal("3,3,2", barra.Morde(2, 2).ToString());
    }

    [Fact]
    public void MorderOVenenoDeixaAMesaVazia()
    {
        Assert.True(Posicao.Barra(3, 3).Morde(0, 0).Vazia);
    }

    [Fact]
    public void MorderForaDaBarraNaoEhLance()
    {
        var barra = Posicao.Barra(2, 2);
        Assert.Throws<ArgumentOutOfRangeException>(() => barra.Morde(2, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => barra.Morde(0, 2));
    }

    [Fact]
    public void AsMordidasSaoUmaPorQuadradoQueSobrou()
    {
        Assert.Equal(12, Posicao.Barra(4, 3).Mordidas().Count());
        Assert.Equal(5, new Posicao(3, 2).Mordidas().Count());
    }

    [Fact]
    public void TodaMordidaTiraPeloMenosUmQuadrado()
    {
        var barra = Posicao.Barra(4, 4);
        foreach (var (_, _, resultado) in barra.Mordidas())
            Assert.True(resultado.Quadrados < barra.Quadrados);
    }

    [Fact]
    public void SoOVenenoEhUmaColunaDeUm()
    {
        Assert.True(new Posicao(1).SoOVeneno);
        Assert.False(new Posicao(2).SoOVeneno);
        Assert.False(new Posicao(1, 1).SoOVeneno);
    }

    [Fact]
    public void PosicoesIguaisTemAMesmaChave()
    {
        Assert.Equal(new Posicao(3, 2, 1).Chave(), new Posicao(3, 2, 1).Chave());
        Assert.NotEqual(new Posicao(3, 2, 1).Chave(), new Posicao(3, 2, 2).Chave());
    }

    [Fact]
    public void AlturaAcimaDeQuinzeNaoCabeNaChave()
    {
        Assert.Throws<InvalidOperationException>(() => Posicao.Barra(2, 16).Chave());
    }
}
