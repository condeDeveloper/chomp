# Chomp

Chomp do zero em C# e .NET 8. Uma barra de chocolate em grade, o quadrado do canto
de baixo à esquerda está envenenado, e cada jogador morde um quadrado levando
junto tudo que está em cima e à direita dele. Quem come o veneno perde.

O jogo tem uma propriedade estranha: **sabe-se que quem começa ganha em qualquer
barra maior que um quadrado único, e não se sabe como.** A prova disso, de 1952,
cabe em cinco linhas e não constrói lance nenhum. Este repositório varre as
posições de verdade para ver o que a prova não conta.

```bash
dotnet test testes/Chomp.Testes/Chomp.Testes.csproj -c Release    # 49 testes
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release
```

## Quem ganha

```
    lado  lances que vencem  o primeiro   posicoes
       1                  0           -          0
       2                  1         1,1          4
       3                  1         1,1         17
       4                  1         1,1         47
       5                  1         1,1        138
       6                  1         1,1        378
       7                  1         1,1      1,453
```

Nas 49 barras até 7 por 7, o primeiro ganha em 48 — a exceção é o quadrado de um,
que é só o veneno. E, em todas as 48, **existe exatamente um lance vencedor**. Nem
dois, nem nenhum: um. Eu tinha escrito o medidor para imprimir "os lances que
vencem", no plural, esperando que barras grandes tivessem vários.

Na barra quadrada o lance é sempre o mesmo e é fácil de explicar: morder o vizinho
do veneno na diagonal, deixando um L de braços iguais. Depois disso, é só espelhar
o que o adversário fizer. **Quadrado é o único formato com estratégia conhecida.**

## O argumento que ganha sem dizer como

```
  colunas  linhas  o canto ja ganha  lance roubado   confere
        5       1               nao            1,0       sim
        5       2               sim            4,1       sim
        5       3               nao            3,2       sim
        5       4               nao            2,2       sim
        5       5               nao            1,1       sim
        6       6               nao            1,1       sim
```

O argumento é este. Morder *só* o canto mais longe do veneno deixa uma posição A.
Se A for perdida para o adversário, acabou: o primeiro ganha jogando isso. Se A
for ganha, então o adversário tem uma resposta M que vence; mas aí o primeiro
podia ter jogado M desde o começo, porque morder o canto antes não muda nada do
que M tira. Nos dois casos o primeiro ganha, e **em nenhum dos dois se descobre
qual é o lance**.

A tabela confere o argumento nas 36 barras até 6 por 6, uma por uma: nas que o
canto já ganha, o canto é lance vencedor; nas outras, o lance do adversário
roubado também vence na barra inteira. Confere em 36 de 36.

E o argumento é mesmo necessário: morder só o canto basta em **11 das 36 barras**,
e essas onze são exatamente as que têm um lado igual a 2. Nas outras 25, a
estratégia vencedora é o lance que o adversário usaria, e ele só aparece
calculando.

## Duas linhas, e só duas

```
   altura 2  altura 1
          0         1
          1         1
          2         1
          ...
          9         1
```

Com duas linhas a estratégia cabe numa frase: **perde quem recebe a posição em que
a linha de baixo tem exatamente um quadrado a mais que a de cima.** A escada.

```
  posicoes conferidas contra a regra: 99
  desobedeceram:                      0
```

A frase não é citada: ela é conferida nas 99 posições de duas linhas que existem
até nove colunas, contra o minimax. Nenhuma desobedece.

## O preço de lembrar

```
   colunas  linhas  nos com memoria  nos sem memoria     razao   bate
         1       1                1                1      1.0x    sim
         2       2               12               16      1.3x    sim
         3       3               44              125      2.8x    sim
         4       4              136            2,132     15.7x    sim
```

O mesmo minimax roda duas vezes: uma guardando o que já calculou, outra
recalculando tudo sempre. As duas sempre dão a mesma resposta, e é essa coluna que
faz a memória ser confiável: a versão lenta não compartilha tabela com ninguém.

A vantagem cresce rápido. Na barra de 4 por 4 já é 15,7 vezes, e na de 12 por 12 a
versão com memória guarda **577.688 posições e termina em dois segundos**, enquanto
a sem memória não terminaria nunca.

## O que as medidas me corrigiram

**Escrevi a regra das duas linhas de um jeito e a programei de outro.** A frase é
"a linha de baixo tem um quadrado a mais que a de cima". No código, isso virou
"colunas de altura 1 são as de altura 2 mais uma", que é outra coisa: a linha de
baixo tem *todas* as colunas, não só as baixinhas. Dezessete das 99 posições
desobedeceram, e eu quase culpei o minimax. A regra certa é "existe exatamente uma
coluna de altura 1".

**Esperava vários lances vencedores nas barras grandes.** São sempre um só, nas 48
barras que ganham. Deixei o plural no nome do método porque é o que a pergunta
pede, mas a resposta não.

**Chutei os nós do juiz em vez de medir.** No teste que prende a tabela da memória
pus 10 e 14 para a barra de 2 por 2, de cabeça. São 12 e 16. É literalmente o tipo
de número que existe para ser medido, e eu o inventei dentro do arquivo cuja
função é impedir que números sejam inventados.

**O argumento do roubo ficou mais estranho depois de medido, não menos.** Eu
esperava que o lance do canto fosse vencedor quase sempre, e que o roubo fosse uma
formalidade para fechar os casos raros. É o contrário: ele só ganha em 11 das 36
barras, e todas elas têm um lado igual a 2. Na maioria dos casos a prova usa um
lance que ela mesma não sabe calcular.

## Os arquivos

| arquivo | o que faz |
|---|---|
| `src/Chomp/Posicao.cs` | a barra pela altura de cada coluna, as mordidas, e a chave da memória |
| `src/Chomp/Resolvedor.cs` | o minimax com memória, os lances vencedores, e o mesmo sem memória |
| `src/Chomp/Roubo.cs` | o argumento de 1952 conferido passo a passo, e a regra das duas linhas |
| `src/Chomp/Medidas.cs` | as seis tabelas deste README |
| `ferramentas/Medidor` | imprime as tabelas, uma por comando |

## Rodando

```bash
dotnet test testes/Chomp.Testes/Chomp.Testes.csproj -c Release

dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- barras
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- quadrados
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- roubo
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- duaslinhas
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- juiz
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- maior
```

C# 12 e .NET 8. Zero dependências fora do xUnit, e só nos testes.

49 testes. Os números deste README estão presos em `MedidasTestes`.
