# Verificação técnica e pendências formais

## Escopo e rastreabilidade

Entregável FIAP: Nexo-Challenge/s1-dotnet-poc. Base remota inspecionada:
`c145738a79859344b76a08ae878fb1ec451491f7`.

Correção local solicitada por Alisson. Não altera Java, banco, mobile ou produto Nexo.
Arquivos autorizados: domínio/serviços/console .NET, testes e documentação deste
repositório. Nenhuma API, Oracle, migration ou backend adicional foi criado.
Decisões educativas estão em [fontes e limitações](fontes-e-limitacoes.md).

## Execução realizada nesta correção

Ambiente Windows, .NET SDK 10.0.402. Comandos:

```bash
dotnet build src/NexoNuclear.Console --configuration Release --warnaserror
dotnet run --project tests/NexoNuclear.Regression --configuration Release
```

Resultado observado: build sem avisos/erros e 29/29 regressões aprovadas.
Os testes são código do repositório, não somente um relato de programa auxiliar.
Foram observadas falhas antes das correções para números não finitos/overflow,
JSON inválido, catálogo vazio, cadastro fantasma em falha de IO, falta de backup,
exposição de referências mutáveis e execução do console em EOF.

Os testes de console iniciam processos reais com entrada/saída redirecionada; não
substituem uma demonstração visual interativa ou avaliação científica especializada.
A validação foi local; CI remota só poderá ser declarada se houver checks executados.
Este registro não comprova nota, publicação na main, submissão acadêmica nem
participação de outras pessoas. O PR e seus checks são a referência de publicação/revisão.

## O que permanece pendente

A consulta à API GitHub do histórico remoto mostrou apenas nomes de autor
correspondentes a Alisson. Nomes na tabela do README não resolvem esse requisito.
Não fabricar commits, alterar autores nem adicionar coautorias sem contribuição real.

Possíveis contribuições futuras, a combinar com cada pessoa, sem atribuição automática:

| Dono proposto | Tarefa real e limite | Dependência | Aceite observável |
| --- | --- | --- | --- |
| Alisson | Revisar e integrar a correção .NET | PR e revisão | Aprovação e merge realizados por pessoa autorizada |
| Kenji | Ampliar testes de integridade do JSON | Combinar casos ainda não cobertos; somente testes | Testes novos reproduzíveis e contribuição própria |
| Erick | Validar instruções em ambiente limpo | SDK 10 e versão revisada; documentação | Registrar comandos, ambiente e resultado reais em contribuição própria |
| Eduardo | Avaliar a usabilidade do console | Versão revisada; roteiro/documentação, sem mobile | Registrar achados reais e melhoria de texto com escopo combinado |
| Marcos | Revisar rubrica e evidências | PDF oficial e versão revisada; docs | Checklist fundamentado com pendências honestas e contribuição própria |

São propostas de um dono por tarefa; não representam trabalho já realizado nem
aprovação dos integrantes. Cada pessoa deve produzir sua contribuição autêntica,
revisada e identificada corretamente no histórico exigido. Apenas executar um teste
ou aparecer neste plano não significa ter feito commit. Não há prazo individual confirmado.

Conferir também prazo efetivo, acesso do professor e submissão no portal. O PDF
apresenta 01/11/2026 numa tabela e até 02/11 no cronograma; confirmar a fonte vigente.

## Limitações de dados

Catálogos existentes permanecem intactos: não há migração automática dos exemplos
antigos. Faça revisão manual das descrições antigas, sem perder seus registros.
O backup é rotativo; o armazenamento é local, para uma instância por arquivo.
Não é um sistema clínico, multiusuário ou de dosimetria ocupacional.
